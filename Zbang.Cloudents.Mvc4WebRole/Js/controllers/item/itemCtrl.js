﻿var mItem = angular.module('mItem', []);
mItem.controller('ItemCtrl',
        ['$scope', '$routeParams', 'sItem', '$timeout', '$rootScope', '$modal', 'sUserDetails', '$location', '$filter', 'sFacebook', '$sce',
function ($scope, $routeParams, sItem, $timeout, $rootScope, $modal, sUserDetails, $location, $filter, sFacebook, $sce) {
    // cd.pubsub.publish('initItem');
    var index = 0, loadMore = false;
    $scope.navigation = {};
    $scope.popup = {};
    $scope.fromReply = {};
    $scope.preview = '';
    $scope.fullScreen = false;
    $scope.load = {
        contentLoading: false,
        contentLoadMore: false
    };
    $scope.canNavigate = false;
    $scope.flagText = JsResources.Flag;
    sFacebook.loginStatus(); //check if user is authenticated so user can use facebook properly


    sItem.load({ itemId: $routeParams.itemId, boxId: $routeParams.boxId }).then(function (response) {
        var data = response.success ? response.payload : [];
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
            id: $routeParams.itemId,
            boxId: $routeParams.boxId


        }).then(function (response) {
            $scope.load.contentLoading = $scope.load.contentLoadMore = false;

            var data = response.success ? response.payload || {} : {};
            if (data.preview) {
                if (data.preview.indexOf('iframe') > 0) {
                    $scope.preview = $sce.trustAsHtml(data.preview);
                } else {
                    $scope.preview += data.preview;

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
    }
    $scope.fullScreenWindow = function () {
        $location.hash('fullscreen');
        var modalInstance = $modal.open({
            windowClass: 'fullscreen',
            templateUrl: '/item/fullscreen/',
            controller: 'itemFullScreenCtrl',
            backdrop: false,
            scope: $scope
        });
        modalInstance.result.then(function () {
            $location.hash('');
        });
        $scope.$on('$destroy', function () {
            if (modalInstance) {
                modalInstance.dismiss();
            }
        });
    }
    if ($location.hash() === 'fullscreen') {
        $scope.fullScreenWindow();
    }

    $scope.flagItemWindow = function () {
        var modalInstance = $modal.open({
            windowClass: 'flagItem',
            templateUrl: '/Item/Flag/',
            controller: 'itemFlagCtrl',
            backdrop: false,
            resolve: {
                data: function () {
                    return {
                        id: $routeParams.itemId
                    };
                }
            }
        });
        modalInstance.result.then(function (d) {
            $scope.flagged = true;
            $scope.flagText = JsResources.Flagged;

        });
        $scope.$on('$destroy', function () {
            if (modalInstance) {
                modalInstance.close();
            }
        });

    }

    $scope.renameWindow = function () {
        var modalInstance = $modal.open({
            windowClass: 'rename',
            templateUrl: '/Item/Rename/',
            controller: 'itemRenameCtrl',
            resolve: {
                data: function () {
                    return {
                        name: $scope.item.name,
                        id: $routeParams.itemId
                    };
                }
            }
        });
        modalInstance.result.then(function (d) {
            $scope.item.name = d.name;
            modalInstance = null; //avoid exception on destroy
            $location.path(d.url).replace();
        });
        $scope.$on('$destroy', function () {
            if (modalInstance) {
                modalInstance.dismiss();
            }
        });
    };
    $scope.create = function (isValid) {
        if (!isValid) {
            return;
        }
        $scope.commentp = true;
        //TODO: add disable state
        $scope.formData.itemId = $routeParams.itemId;
        sItem.addComment($scope.formData).then(function (response) {
            $scope.commentp = false;

            if (!response.success) {
                alert(response.payload);
                return;
            }

            $scope.item.comments.unshift({
                comment: $scope.formData.Comment,
                creationDate: new Date().toISOString(),
                id: response.payload,
                replies: [],
                userId: sUserDetails.getDetails().id,
                userName: sUserDetails.getDetails().name
            });
            $scope.formData = {};
            $scope.showBtn = false;
            $scope.$broadcast('update-scroll');
        });
    };
    $scope.deleteComment = function (comment) {
        sItem.deleteComment({ CommentId: comment.id }).then(function (response) {
            if (!response.success) {
                alert(response.payload);
                return;
            }
            var indexC = $scope.item.comments.indexOf(comment);
            $scope.item.comments.splice(indexC, 1);
            $scope.$broadcast('update-scroll');
        });
    };
    $scope.deleteReply = function (reply, comment) {
        sItem.deleteReply({ ReplyId: reply.id }).then(function (response) {
            if (!response.success) {
                alert(response.payload);
                return;
            }
            var indexC = comment.replies.indexOf(reply);
            comment.replies.splice(indexC, 1);
            $scope.$broadcast('update-scroll');
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
        $scope.fromReply.itemId = $routeParams.itemId;
        $scope.fromReply.commentId = comment.id;

        sItem.replyComment($scope.fromReply).then(function (response) {
            if (!response.success) {
                alert(response.payload.error[0].value[0]);
                return;
            }
            comment.replies.unshift({
                comment: $scope.fromReply.Comment,
                creationDate: new Date().toISOString(),
                id: response.payload,
                userId: sUserDetails.getDetails().id,
                userName: sUserDetails.getDetails().name

            });
            $scope.fromReply = {};
            comment.showReplyF = false;
            comment.replyp = false;
            $scope.$broadcast('update-scroll');
        });
    };
    //todo proper return;

    //#region share
    $scope.shareFacebook = function () {
        var jsResources = window.JsResources;
        $scope.popup.share = false;
        sFacebook.share($location.absUrl(), //url
          $scope.item.name, //title
          $routeParams.uniName ? $scope.item.boxName + ' - ' + $routeParams.uniName : $scope.item.boxName, //caption          
          $filter('stringFormat')(jsResources.IShared + ' {0} ' + jsResources.OnCloudents + '<center>&#160;</center><center></center>' + jsResources.CloudentsJoin, [$scope.item.name]),
            null //picture
       ).then(function () {
           cd.pubsub.publish('addPoints', { type: 'shareFb' });
       });
    };

    $scope.shareEmail = function () {
        $scope.popup.share = false;

        var modalInstance = $modal.open({
            windowClass: "invite",
            templateUrl: $scope.partials.shareEmail,
            controller: 'ShareCtrl',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return null;
                }
            }
        });

        modalInstance.result.then(function () {
        }, function () {
            //dismiss
        });

        $scope.$on('$destroy', function () {
            if (modalInstance) {
                modalInstance.close();
            }
        });
    };
    $scope.rate = function (t) {
        sItem.rate({ ItemId: $routeParams.itemId, rate: t });
    }
}
        ]);
