(function ($, dataContext, ko, cd, ZboxResources) {
    "use strict";

    cd.loadModel('Box', 'BoxContext', registerKoBoxComment);

    function registerKoBoxComment() {
        //ko.applyBindings(new CommentViewModel(), document.getElementById('box_Comment'));
    }

    function CommentViewModel() {
        function comment(data) {
            var that = this;
            that.userName = data.UserName;
            that.userImg = data.UserImg;
            that.userUid = data.UserUid;

            if (data.Comment) {
                data.Comment = data.Comment.replace(/\n/g, '<br/>');
            }
            that.comment = data.Comment;
            that.date = cd.dateToShow(new Date(parseInt(data.Date.replace("/Date(", "").replace(")/", ""), 10)));

            that.id = data.Id;
            that.isComment = data.ParentId === null && data.id !== null;
            that.parentId = data.ParentId;
            that.item = data.ItemUid !== null;
            if (that.item) {
                that.itemName = data.ItemName;
                that.itemurl = '/Item/' + data.BoxUid + '/' + data.ItemUid;
            }
            if (!data.UserImg) {
                data.UserImg = '/images/usrPic.png';
            }

            if (data.Replies !== undefined) {
                that.replies = ko.observableArray([]);
                if (data.Replies.length) {
                    var mappedReplies = $.map(data.Replies, function (c) { return new comment(c); });
                    that.replies.push.apply(that.replies, mappedReplies);
                }
            }

            that.deleteAllow = ko.computed(
                function () {
                    var permission = self.permission();
                    return permission > 1 && (permission === 3 ||
                        that.userUid === $('#userName').data('id'));
                    //sec.IsDeleteAllow(that.userUid);
                });

        }


        var self = this, boxid;//, itemid;

        self.comments = ko.observableArray([]);
        self.permission = ko.observable(0);

        
        cd.pubsub.subscribe('box', function (data) {
            self.comments([]);
            boxid = data.id;
            self.permission(0);
            getComments();
        });

        cd.pubsub.subscribe('perm', function (d) {
            self.permission(d);
        });

        //remove that due to animation - the clear trigger after the add
        cd.pubsub.subscribe('Boxclear', function () {
            $('.commentTextarea').val('');
        });
       

        function getComments() {
            dataContext.boxComments({
                data: { BoxUid: boxid/*, ItemUid: itemid*/ },
                success: function (data) {
                    generateModel(data);
                }
            });

        }
        function generateModel(data) {
            self.comments([]);
            var mappedComments = $.map(data, function (c) { return new comment(c); });

            self.comments.push.apply(self.comments, mappedComments);
        }

        //#region add
        self.addReply = function (form) {
            var $form = $(form);
            var $reply = $form.find('textarea');
            var replyText = $reply.val();
            if ($.trim(replyText) === '') {
                cd.notification(ZboxResources.NoTextEntered);
                return;
            }
            var parentCommnet = ko.dataFor(form);
            var id = parentCommnet.id;

            dataContext.replyAdd({
                data: { CommentToReplyId: id, CommentText: replyText, BoxUId: boxid },
                success: function (data) {
                    var location = self.comments.indexOf(parentCommnet);
                    parentCommnet.replies.push(new comment(data));
                    self.comments.sort(sort);
                },
                always: function () {
                    $form.parent().slideUp(200);
                    $reply.val('');
                }
            });
            //not using sort because we have recursion in here
            function sort(c1, c2) {
                c1.replies.sort(sort);
                if (c1.id > c2.id) return -1;
                return 1;
            }
        };
        self.addComment = function (form) {
            var $commentText = $(form).find('textarea'),
                text = $.trim($commentText.val());
            if (self.permission() < 2) {
                cd.notification(ZboxResources.NeedToFollowBox);
                $('.boxFollow').show();
                return false;
            }
            if (!text) {
                return;
            }
            $commentText.val('').removeClass('commentFocused').css('height', '32px');
            $(form).find('button:submit').hide();
            dataContext.commentAdd({
                data: { CommentText: text, BoxUid: boxid/*, ItemUid: itemid */ },
                success: function (data) {
                    self.comments.unshift(new comment(data));
                },
                always: function () {
                }

            });

        };
        //#endregion

        //#region remove
        self.removeComment = function (deletecomment) {
            if (!deletecomment.deleteAllow()) {
                cd.notification(ZboxResources.DontHavePermission);
                return false;
            }
            if (!confirm(ZboxResources.SureDeleteComment)) {
                return false;
            }
            var itemRemoved = self.comments.remove(deletecomment);
            if (!itemRemoved.length) {
                var pcomment = ko.utils.arrayFirst(self.comments(), function (comment) {
                    return comment.id === deletecomment.parentId;
                });
                pcomment.replies.remove(deletecomment);
            }
            dataContext.commentRemove({
                data: { CommentId: deletecomment.id, BoxUid: boxid },
                error: function () {
                    self.comments.push(deletecomment);
                    //self.comments.sort(sort);
                }

            });
        };


        //#endregion
        //if (cd.register()) {
        registerEvents();
        //}

        function registerEvents() {
           
            $('textarea[data-id="CommentText"]').elastic()
            .focus(function (e) {
                e.preventDefault();
                if (!cd.register()) {
                    cd.unregisterAction(this);
                    return;
                }
                if (self.permission() < 2) {
                    // ie issue bug 455
                    window.setTimeout(function () {
                        cd.notification(ZboxResources.NeedToFollowBox);
                    },5);
                    $('.boxFollow').show();
                    //window.setTimeout(function () {
                    $(this).blur();
                    //}, 50);
                    return;
                }
                $(this).addClass('commentFocused');
                var btn = $('.QForm').find('button').slideDown(100);
                $('body').on('click', function (e) {

                    btn.hide();
                });

            }).click(function (e) {
              
                e.stopPropagation();
            });
            $('[data-section="comments"]').on('click', 'a.commentAction', function () {
                if (!cd.register()) {
                    cd.unregisterAction(this);
                    return;
                }
                if (self.permission() < 2) {
                    cd.notification(ZboxResources.NeedToFollowBox);
                    $('.boxFollow').show();
                    $(this).blur();
                    return false;
                }
                //dont need that sec.isnotconnected as well????
                var elem = $(this).parent().next();
                elem.find('img').attr('src', $('#userDetails').find('img').attr('src'));

                elem.slideDown().
                find('textarea').elastic().focus();
            });
        }
    }
})(jQuery, cd.data, ko, cd, ZboxResources);