var mItem = angular.module('mItem', []);
mItem.controller('ItemCtrl',
        ['$scope', '$routeParams', 'sItem', '$timeout', '$rootScope', 'sModal', 'sUserDetails', '$location', 'resManager', 'sFacebook', '$sce', '$analytics',
function ($scope, $routeParams, sItem, $timeout, $rootScope, sModal, sUserDetails, $location, resManager, sFacebook, $sce, $analytics) {
    "use strict";
    var index = 0, loadMore = false;
    $scope.navigation = {};
    $scope.popup = {};
    $scope.fromReply = {};
    $scope.formData = {};
    $scope.preview = '';
    $scope.fullScreen = false;
    $scope.load = {
        contentLoading: false,
        contentLoadMore: false
    };
    $scope.canNavigate = false;
    $scope.flagText = resManager.get('Flag');

    var itemId = $routeParams.itemId, boxId = $routeParams.boxId;

    sItem.load({ itemId: itemId, boxId: boxId }).then(function (data) {
        $scope.item = data;
        $scope.item.url = $location.absUrl();
        $scope.item.downloadUrl = $location.url() + 'download/';
        $scope.item.printUrl = $location.url() + 'print/';
        getPreview();
        $timeout(function () {
            $rootScope.$broadcast('viewContentLoaded');
            $scope.$broadcast('update-scroll');
            $scope.canNavigate = true;
        });
    });

    function getPreview() {
        if (index > 0) {
            $scope.load.contentLoadMore = true;
        } else {
            $scope.load.contentLoading = true;
        }
        if (!sUserDetails.isAuthenticated() && index > 0) {
            cd.pubsub.publish('register', { action: true });
            return;
        }
        //string blobName, int imageNumber, long id, string boxId, int width = 0, int height = 0
        sItem.preview({
            blobName: $scope.item.blob,
            index: index,
            id: itemId,
            boxId: boxId
        }).then(function (data) {
            $scope.load.contentLoading = $scope.load.contentLoadMore = false;
            if (data.preview) {
                if (data.preview.indexOf('iframe') > 0
                    || data.preview.indexOf('audio') > 0
                    || data.preview.indexOf('video') > 0) {
                    $scope.preview = $sce.trustAsHtml(data.preview);
                } else {
                    $scope.preview += data.preview;
                    $analytics.eventTrack('Get Prview', {
                        category: 'Item'
                    });
                    loadMore = true;
                }
                $scope.$broadcast('update', data.preview); //for fullscreen
            }
        });
    }

    $scope.loadMore = function () {
        if (loadMore) {
            loadMore = false;
            ++index;
            getPreview();
        }
    };
    $scope.fullScreenWindow = function () {
        $analytics.eventTrack('Item', {
            category: 'Full Screen'
        });
        $location.hash('fullscreen');

        sModal.open('itemFullscreen', {
            scope: $scope,
            callback: {
                close: function () {
                    $location.hash('');
                }
            }
        });
    };

    if ($location.hash() === 'fullscreen') {
        $scope.fullScreenWindow();
    }

    $scope.flagItemWindow = function () {
        sModal.open('flagItem', {
            data: {
                id: itemId
            },
            callback: {
                close: function () {
                    $scope.flagged = true;
                    $scope.flagText = resManager.get('Flagged');
                }
            }
        });
        $analytics.eventTrack('Item', {
            category: 'Flag Item'
        });
    };

    $scope.renameWindow = function () {
        sModal.open('itemRename', {
            data: {
                name: $scope.item.name,
                id: itemId
            },
            callback: {
                close: function (d) {
                    $scope.item.name = d.name;
                    $location.path(d.url).replace();
                }
            }
        });

        $analytics.eventTrack('Item', {
            category: 'Rename Popup'
        });
    };
    $scope.create = function (isValid) {
        if (!isValid) {
            return;
        }
        $scope.commentp = true;
        $scope.formData.itemId = itemId;
        sItem.addComment($scope.formData).then(function (response) {
            $scope.item.comments.unshift({
                comment: $scope.formData.Comment,
                creationDate: new Date().toISOString(),
                id: response,
                replies: [],
                userId: sUserDetails.getDetails().id,
                userName: sUserDetails.getDetails().name
            });
            $scope.formData = {};
            $scope.showBtn = false;
            $scope.$broadcast('update-scroll');

        }, function (respoonse) {
            alert(respoonse);
        }).finally(function () {
            $scope.commentp = false;
        });

        $analytics.eventTrack('Item', {
            category: 'Add Comment'
        });
    };
    $scope.deleteComment = function (comment) {
        sItem.deleteComment({ CommentId: comment.id }).then(function () {
            var indexC = $scope.item.comments.indexOf(comment);
            $scope.item.comments.splice(indexC, 1);
            $scope.$broadcast('update-scroll');
        }, function (response) {
            alert(response);
        });

        $analytics.eventTrack('Item', {
            category: 'Delete Comment'
        });
    };
    $scope.deleteReply = function (reply, comment) {
        sItem.deleteReply({ ReplyId: reply.id }).then(function () {
            var indexC = comment.replies.indexOf(reply);
            comment.replies.splice(indexC, 1);
            $scope.$broadcast('update-scroll');
        }, function (response) {
            alert(response);
        });
        $analytics.eventTrack('Item', {
            category: 'Delete Reply'
        });
    };
    $scope.canDelete = function (id) {
        return id == sUserDetails.getDetails().id; //id is string
    };
    $scope.canFlag = function () {
        if ($scope.flagged) {
            return false;
        }
        return $scope.item && sUserDetails.getDetails().id !== $scope.item.ownerId;
    };
    $scope.addReply = function (comment, valid) {
        if (!valid) {
            return;
        }
        comment.replyp = true;
        $scope.fromReply.itemId = itemId;
        $scope.fromReply.commentId = comment.id;

        sItem.replyComment($scope.fromReply).then(function (response) {
            comment.replies.unshift({
                comment: $scope.fromReply.Comment,
                creationDate: new Date().toISOString(),
                id: response,
                userId: sUserDetails.getDetails().id,
                userName: sUserDetails.getDetails().name

            });
            $scope.fromReply = {};
            comment.showReplyF = false;

            $scope.$broadcast('update-scroll');
        }, function (response) {
            alert(response.error[0].value[0]);
        }).finally(function () {
            comment.replyp = false;
        });;

        $analytics.eventTrack('Item', {
            category: 'Add Reply'
        });
    };

    //#region share
    $scope.shareFacebook = function () {
        $scope.popup.share = false;
        sFacebook.share($location.absUrl(), //url
          $scope.item.name, //title
          $routeParams.uniName ? $scope.item.boxName + ' - ' + $routeParams.uniName : $scope.item.boxName, //caption          
          resManager.get('IShared') + ' ' + $scope.item.name + ' ' + resManager.get('OnCloudents') + '<center>&#160;</center><center></center>' + resManager.get('CloudentsJoin'),
            null //picture
       ).then(function () {
           cd.pubsub.publish('addPoints', { type: 'shareFb' });
       });

        $analytics.eventTrack('Item', {
            category: 'Share Facebook'
        });
    };

    $scope.shareEmail = function () {
        $scope.popup.share = false;

        sModal.open('shareEmail');
        $analytics.eventTrack('Item', {
            category: 'Share Email'
        });
    };
    $scope.rate = function (t) {
        sItem.rate({ ItemId: itemId, rate: t });

        $analytics.eventTrack('Item', {
            category: 'Rate Item'
        });
    };
}
        ]);
