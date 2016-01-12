(function () {
    angular.module('app.item').controller('ItemController', item);
    item.$inject = ['$stateParams', 'itemService', '$sce', '$location', '$q', 'user'];

    function item($stateParams, itemService, $sce, $location, $q, user) {
        var i = this, boxid = $stateParams.boxId, itemId = $stateParams.itemId;
        var index = 0, needLoadMore = false;

        i.state = {
            regular: 0,
            rename: 1,
            flag: 2
        };

        i.preview = '';
        itemService.getDetails(boxid, itemId).then(function (response) {
            i.details = response;
            console.log("response", response);
            i.details.downloadUrl = $location.url() + 'download/';
            i.details.printUrl = $location.url() + 'print/';
            getPreview();
        });




        //i.renameOn = true;
        i.loadMore = loadMore;
        i.selectedState = i.state.regular;


        i.renameItem = renameItem;
        i.flagItem = flagItem;
        i.cancelFlag = cancelFlag;
        i.like = like;

        i.showRename = showRename;

        i.swipeLeft = swipeLeft;
        i.swipeRight = swipeRight;
        i.addComment = addComment;
        i.addCommentReply = addCommentReply;
        i.openReply = openReply;

        function getPreview() {
            i.loader = true;
            return itemService.getPreview(i.details.blob, index, itemId, boxid).then(function (data) {
                data = data || {};
                i.loader = false;
                if (data.preview) {
                    if (data.preview.indexOf('iframe') > 0
                        || data.preview.indexOf('audio') > 0
                        || data.preview.indexOf('video') > 0) {
                        i.preview = $sce.trustAsHtml(data.preview);
                    } else {
                        i.preview += data.preview;
                        needLoadMore = true;
                    }

                }
            });
        }
        function swipeLeft() {
            if (i.details.next) {
                $location.url(i.details.next);
            }
        }


        function swipeRight() {
            if (i.details.previous) {
                $location.url(i.details.previous);
            }
        }

        function showRename() {
            i.selectedState = i.state.rename;
            i.renameText = i.details.name;
        }

        function loadMore() {
            var defer = $q.defer();

            if (needLoadMore && user.id) {
                needLoadMore = false;
                ++index;
                return getPreview();
            }
            defer.resolve();
            return defer.promise;
        };
        function renameItem() {
            i.submitDisabled = true;
            itemService.renameItem(i.renameText, itemId).then(function (response) {
                i.details.name = response.name;
                $location.path(response.url).replace();
            }).finally(function () {
                i.submitDisabled = false;
            });
        }
        function addComment() {
            itemService.addComment(i.newCommentText, boxid, itemId).then(function (response) {
                console.log(response);
            });

        }

        function addCommentReply(comment) {
            console.log("comment.id", comment.id);
            itemService.replycomment(i.newCommentReplyText, itemId, boxid, comment.id).then(function (response) {
                console.log(response);
            });
        }

        function openReply(comment) {
            angular.forEach(i.details.comments, function (elem) {
                elem.showFrom = false;
            });
            comment.showFrom = true;
            i.newCommentReplyText = '';
        }

        function flagItem() {
            itemService.flag(i.flag, i.customFlag, itemId);
            cancelFlag();
        }

        function cancelFlag() {
            i.flag = '';
            i.selectedState = i.state.regular;
        }


        function like() {
            itemService.like(itemId, boxid);
            if (i.details.like) {
                i.details.likes--;
            } else {
                i.details.likes++;
            }
            i.details.like = !i.details.like;

        }
    }


})();


