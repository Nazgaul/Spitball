var mItem = angular.module('mItem', []);
mItem.controller('ItemCtrl',
        ['$scope', '$routeParams', 'sItem', '$timeout', '$rootScope', '$modal','sUserDetails','$location',
function ($scope, $routeParams, sItem, $timeout, $rootScope, $modal, sUserDetails,$location) {
    // cd.pubsub.publish('initItem');
    var index = 0, loadMore = false;

    $scope.navigation = {};
    $scope.fromReply = {};
    $scope.preview = '';
    $scope.fullScreen = false;
    $scope.load = {
        contentLoading: false,
        contentLoadMore: false
    };

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
        
        if (index > 0) {
            $scope.load.contentLoadMore = true;
        } else {
            $scope.load.contentLoading = true;
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
                loadMore = true;
                $scope.preview += data.preview;
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
        var modalInstance = $modal.open({
            windowClass: 'fullscreen',
            templateUrl: '/Item/FullScreen/',
            controller: 'itemFullScreenCtrl',
            backdrop: false,
            scope: $scope
            // resolve: {
            //friends: function () {
            //return data.payload.my;
            //}
            // }
        });
        $scope.$on('$destroy', function () {
            if (modalInstance) {
                modalInstance.close();
            }
        });
       
    }
    $scope.flagItemWindow = function () {
        var modalInstance = $modal.open({
            windowClass: 'flagItem',
            templateUrl: '/Item/Flag/',
            controller: 'itemFlagCtrl',
            backdrop: false,
            resolve : {
                data: function() {
                    return {
                        id: $routeParams.itemId
                    };
                }
            }
        });
        $scope.$on('$destroy', function () {
            if (modalInstance) {
                modalInstance.close();
            }
        });
        
    }
   
    $scope.renameWindow = function() {
        var modelInstance = $modal.open({
            windowClass: 'rename',
            templateUrl: '/Item/Rename/',
            controller: 'itemRenameCtrl',
            backdrop: false,
            resolve: {
                data: function () {
                    return {
                        name: $scope.item.name,
                        id: $routeParams.itemId
                    };
                }
            }
        });
        modelInstance.result.then(function (d) {
            $scope.item.name = d.name;
            modelInstance = null; //avoid exception on destroy
            $location.path(d.url).replace();
        });
        $scope.$on('$destroy', function () {
            if (modelInstance) {
                modelInstance.dismiss();
            }
        });
    };
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
    $scope.canFlag = function () {
        return $scope.item && sUserDetails.getDetails().id !== $scope.item.ownerId;
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
