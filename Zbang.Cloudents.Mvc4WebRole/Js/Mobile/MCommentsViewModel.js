(function (cd, ko, dataContext, ZboxResources) {   
    "use strict";
    if (window.scriptLoaded.isLoaded('mC')) {
        return;
    }

    cd.loadModel('box', 'BoxContext', registerKoBoxComment);

    function registerKoBoxComment() {
        ko.applyBindings(new CommentViewModel(), $('#box_Comment')[0]);
    }
    function CommentViewModel() {
        function Comment(data) {
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
                data.UserImg = '/images/user-pic.png';
            }

            if (data.Replies !== undefined) {
                that.replies = ko.observableArray([]);
                if (data.Replies.length) {
                    var mappedReplies = $.map(data.Replies, function (c) { return new Comment(c); });
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
        self.commentCount = ko.computed(function () {
            var x = self.comments().length;
            $('#box_comment_count').text(x);
            return x;
        });
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

        function getComments() {
            dataContext.boxComments({
                data: { BoxUid: boxid },
                success: function (data) {
                    generateModel(data);
                }
            });

        }
        function generateModel(data) {
            self.comments([]);
            var mappedComments = $.map(data, function (c) { return new Comment(c); });

            self.comments.push.apply(self.comments, mappedComments);
        }

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
                    self.comments.unshift(new Comment(data));
                }
            });

        };

        $('textarea[data-id="CommentText"]')//.elastic()
           .focus(function () {
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
               $(this).addClass('commentFocused');
               var btn = $(this).next().removeClass('rFloat').show();
               window.setTimeout(function () {
                   btn.addClass('rFloat');
               },1);//.addClass('rFloat');//.slideDown(100);
               //btn.parent().css(overflow, 'auto');
               //$('body').on('click', function (e) {
               //    btn.hide();
               //});

           }).click(function (e) {
               e.stopPropagation();
           });
    }
})(cd, ko, cd.data, ZboxResources);