﻿var mItem = angular.module('mItem', []);
mItem.controller('ItemCtrl',
        ['$scope', '$routeParams', 'sItem', '$timeout', '$rootScope', '$modal','sUserDetails',
function ($scope, $routeParams, sItem, $timeout, $rootScope, $modal, sUserDetails) {
    // cd.pubsub.publish('initItem');
    var index = 0, loadMore = false;

    $scope.navigation = {};
    $scope.fromReply = {};
    $scope.preview = '';
    $scope.fullScreen = false;
    //$scope.contentLoading = true;
    //$scope.contentLoadMore = false;

    sItem.load({ itemId: $routeParams.itemId, boxId: $routeParams.boxId }).then(function (response) {
        var data = response.success ? response.payload : [];
        $scope.item = data;
        getPreview();
        $timeout(function () {
            $rootScope.$broadcast('viewContentLoaded');
            $scope.$broadcast('update-scroll');
        });
    });


    function getPreview() {
        
        if (loadMore) {
            $scope.contentLoadMore = true
        } else {
            $scope.contentLoading = true;
        }
            
        //string blobName, int imageNumber, long id, string boxId, int width = 0, int height = 0
        sItem.preview({
            blobName: $scope.item.blob,
            index: index,
            id: $scope.item.id,
            boxId: $routeParams.boxId


        }).then(function (response) {
            $scope.contentLoading = false;
            $scope.contentLoadMore = false;

            var data = response.success ? response.payload : '';
            if (data.preview.length > 10) {
                loadMore = true;
            }
            $scope.preview += data.preview;
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
        var modalInstance = $modal.open({
            windowClass: 'fullscreen',
            templateUrl: '/Item/FullScreen/',
            controller: 'ItemFullScreenCtrl',
            backdrop: false,
            scope: $scope
            // resolve: {
            //friends: function () {
            //return data.payload.my;
            //}
            // }
        });
        //modalInstance.result.then(function (url) {
        //});
    }
    $scope.create = function(isValid) {
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
    $scope.addReply = function (comment) {
        comment.replyp = true;
        $scope.fromReply.itemId = $routeParams.itemId;
        $scope.fromReply.commentId = comment.id;
        
        sItem.replyComment($scope.fromReply).then(function (response) {
            if (!response.success) {
                alert(response.payload);
                return;
            }
            comment.replies.unshift({
                comment: $scope.fromReply.Comment,
                creationDate:  new Date().toISOString(),
                id: response.payload,
                userId:  sUserDetails.getDetails().id,
                userName: sUserDetails.getDetails().name
            
            });
            $scope.fromReply = {};
            comment.showReplyF = false;
            comment.replyp = false;
            $scope.$broadcast('update-scroll');
        });
    };
    cd.pubsub.publish('item', $routeParams.itemId); //statistics
    //todo proper return;
}
        ]);
