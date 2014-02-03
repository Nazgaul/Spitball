(function (mmc, $) {
    "use strict";
    if (mmc.page.dashboard) {
        mmc.dashboardWall = function () {
            ko.applyBindings(new WallViewModel(), document.getElementById('wallSection'));
        };
    }
    function WallViewModel() {
        var self = this;
        function Activity(data) {
            var self = this;
            try {
                mmc.Comment.call(self, data);
                self.boxid = data.BoxUid;
            } catch (e) {
                //raise error
            }
            self.boxName = data.BoxName;
            self.boxurl = '/Box?BoxUid=' + data.BoxUid;
            if (self.item) {
                self.itemPic = data.ItemPic;
            }
        }
        WallData();
        self.activity = ko.observableArray([]);
        function WallData() {
            var request = new ZboxAjaxRequest({
                url: "/Dashboard/Wall",
                success: function (data) {
                    try {
                        var mappedactivity = $.map(data, function (activity) { return new Activity(activity); });
                        self.activity.push.apply(self.activity, mappedactivity);
                    } catch (e) {
                        //reaise error
                    }
                   
                }
            });
            request.Post();
        }
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
            var boxid = parentCommnet.boxid;

            var request = new ZboxAjaxRequest({
                url: "/Comment/Reply",
                data: { CommentToReplyId: id, CommentText: replyText, BoxUId: boxid },
                success: function (data) {
                    var location = self.activity.indexOf(parentCommnet);
                    parentCommnet.replies.push(new mmc.Comment(data));
                },
                complete: function () {
                    $form.parent().slideUp(200);
                    $reply.val('');
                }
            });
            request.Post();
        };
        $('#wallSection').on('click', 'a.commentAction', function () {
            var elem = $(this).parent().next();//.show()
            console.log(elem);
            elem.find('img').attr('src', $('#userDetails').find('img').attr('src'));
            elem.slideDown();
            elem.find('textarea').elastic().focus();
        });
    }
}(window.mmc = window.mmc || {}, jQuery));
