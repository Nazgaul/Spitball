(function (mmc, $) {
    "use strict";
    if (mmc.page.box || mmc.page.item || mmc.page.dashboard) {
        mmc.Comment = function (data) {
            var self = this;
            self.userName = data.UserName;
            if (data.Comment) {
                data.Comment = data.Comment.replace(/\n/g, '<br/>');
            }
            self.comment = data.Comment;
            self.date = mmc.dateToShow(new Date(parseInt(data.Date.replace("/Date(", "").replace(")/", ""), 10)));

            self.id = data.Id;
            self.isComment = data.ParentId === null && data.id !== null;
            self.parentId = data.ParentId;
            self.item = data.ItemUid !== null;
            if (self.item) {
                self.itemName = data.ItemName;
                self.itemurl = '/Item?BoxUid=' + data.BoxUid + '&ItemUid=' + data.ItemUid;
            }
            if (!data.UserImg) {
                data.UserImg = '/images/usrPic.png';
            }
            self.userImg = data.UserImg;
            if (data.Replies !== undefined) {
                self.replies = ko.observableArray([]);
                if (data.Replies.length) {
                    var mappedReplies = $.map(data.Replies, function (c) { return new mmc.Comment(c); });
                    self.replies.push.apply(self.replies, mappedReplies);
                }
            }
            self.deleteAllow = false;
            if (window.sec) { // happen on search - checking existance of object sec
                self.deleteAllow = sec.IsDeleteAllow(self.authorName);
            }
        }
        mmc.comment = function () {
            ko.applyBindings(new CommentViewModel(), document.getElementById('commentsSection'));
        }
    }
    function CommentViewModel() {
        var self = this;

        var boxid = Zbox.getParameterByName('BoxUid'), itemid = Zbox.getParameterByName('ItemUid'), countofdata = 0, sync = true;
        if (boxid === '') {
            throw 'boxid cannot be null';
        }
        self.comments = ko.observableArray([]);
        Comments();

        mmc.commentItem = function (itemId) {
            itemid = itemId;
            self.comments([]);
            countofdata = 0;
            Comments();
        };

        registerEvents();
        self.removeComment = function (deletecomment) {
            if (!sec.IsDeleteAllow(deletecomment.authorName)) {
                mmc.notification(ZboxResources.DontHavePermission);
                return false;
            }
            var answer = confirm(ZboxResources.SureDeleteComment);
            if (!answer) {
                return false;
            }
            var itemRemoved = self.comments.remove(deletecomment);
            if (!itemRemoved.length) {
                var pcomment = ko.utils.arrayFirst(self.comments(), function (comment) {
                    return comment.id === deletecomment.parentId;
                });
                pcomment.replies.remove(deletecomment);
            }

            sync = false;

            var request = new ZboxAjaxRequest({
                url: "/Comment/Delete",
                data: { CommentId: deletecomment.id, BoxUid: boxid },
                success: function () {
                    countofdata--;
                },
                error: function () {
                    self.comments.push(deletecomment);
                    self.comments.sort(sort);
                },
                complete: function () {
                    sync = true;
                }
            });
            request.Post();
        };

        self.addReply = function (form) {
            var $form = $(form);
            var $reply = $form.find('textarea');
            var replyText = $reply.val();
            if ($.trim(replyText) === '') {
                mmc.notification(ZboxResources.NoTextEntered);
                return;
            }
            var parentCommnet = ko.dataFor(form);
            var id = parentCommnet.id;

            sync = false;
            var request = new ZboxAjaxRequest({
                url: "/Comment/Reply",
                data: { CommentToReplyId: id, CommentText: replyText, BoxUId: boxid },
                success: function (data) {
                    var location = self.comments.indexOf(parentCommnet);
                    parentCommnet.replies.push(new mmc.Comment(data));
                    self.comments.sort(sort);
                    countofdata++;
                },
                complete: function () {
                    $form.parent().slideUp(200);
                    $reply.val('');
                    sync = true;
                }
            });
            request.Post();
        };
        self.addComment = function (form) {
            var $commentText = $('#CommentText');
            if (sec.IsNotConnected()) {
                mmc.notification(ZboxResources.NeedToFollowBox);
                return false;
            }
            if (!$.trim($commentText.val())) {
                return;
            }
            sync = false;
            var request = new ZboxAjaxRequest({
                data: JSON.stringify({ CommentText: $commentText.val(), BoxUid: boxid, ItemUid: itemid }),
                contentType: 'application/json; charset=utf-8',
                url: "/Comment/Add",
                success: function (data) {
                    self.comments.unshift(new mmc.Comment(data));
                    countofdata++;
                },
                complete: function () {
                    $commentText.val('').removeClass('commentFocused').css('height', '32px');
                    $(form).find('button:submit').hide();
                    sync = true;
                }
            });
            request.Post();
        };

        function sort(c1, c2) {
            c1.replies.sort(sort);
            if (c1.id > c2.id) return -1;
            return 1;
        }

        function Comments() {
            if (!sync) {
                return;
            }
            var request = new ZboxAjaxRequest({
                data: { BoxUid: boxid, ItemUid: itemid },
                url: "/Comment",
                success: function (data) {
                    if (countofdata === data.length) {
                        return;
                    }
                    GenerateModel(data);
                }
            });
            request.Post();
        }
        function GenerateModel(data) {
            self.comments([]);

            var mappedComment = $.map(data, function (c) { return new mmc.Comment(c); });
            self.comments.push.apply(self.comments, mappedComment);
        }

        mmc.UpdateComments = function (data) {
            if (data.length === 0) {
                return;
            }
            for (var i = 0; i < data.length; i++) {
                if (data[i].ParentId !== null) {
                    var parent = ko.utils.arrayFirst(self.comments(), function (c) {
                        return c.id === data[i].ParentId;
                    });
                    if (parent === null) {
                        return;
                    }
                    var reply = ko.utils.arrayFirst(parent.replies(), function (c) {
                        return c.id === data[i].Id;
                    });
                    if (data[i].Deleted) {
                        parent.replies.remove(reply);
                        continue;
                    }
                    if (reply === null) {
                        var newComment = new mmc.Comment(data[i]);
                        parent.replies.push(newComment);
                        continue;
                    }

                }
                else {
                    var comment = ko.utils.arrayFirst(self.comments(), function (c) {
                        return c.id === data[i].Id;
                    });
                    if (data[i].Deleted) {
                        self.comments.remove(comment);
                        continue;
                    }
                    if (comment === null) {
                        var newComment = new mmc.Comment(data[i]);
                        self.comments.push(newComment);
                        continue;
                    }

                }
            }
        };
        function registerEvents() {
            if (mmc.register) {
                $('#CommentText').elastic()
                .focus(function () {
                    if (sec.IsNotConnected()) {
                        mmc.notification(ZboxResources.NeedToFollowBox);
                        $(this).blur();
                        return false;
                    }
                    $(this).addClass('commentFocused');
                    $('.newBoxComment').find('button').slideDown(100);
                });
                $('#commentsSection').on('click', 'a.commentAction', function () {
                    var elem = $(this).parent().next();//.show()
                    elem.find('img').attr('src', $('#userDetails').find('img').attr('src'));

                    elem.slideDown();
                    elem.find('textarea').elastic().focus();
                });
            }
        }
    }

}(window.mmc = window.mmc || {}, jQuery))