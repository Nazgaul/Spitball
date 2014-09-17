var mItem = angular.module('mItem', []);
mItem.controller('ItemCtrl',
        ['$scope', '$routeParams', 'sItem', '$timeout', '$rootScope', '$modal','sUserDetails',
function ($scope, $routeParams, sItem, $timeout, $rootScope, $modal, sUserDetails) {
    // cd.pubsub.publish('initItem');
    var index = 0, loadMore = true;

    $scope.navigation = {};
    $scope.preview = '';

    sItem.load({ itemId: $routeParams.itemId, boxId: $routeParams.boxId }).then(function (response) {
        var data = response.success ? response.payload : [];
        $scope.item = data;
        console.log($scope.item);
        getPreview();
        $timeout(function () {
            $rootScope.$broadcast('viewContentLoaded');
        });
    });


    function getPreview() {

        //string blobName, int imageNumber, long id, string boxId, int width = 0, int height = 0
        sItem.preview({
            blobName: $scope.item.blob,
            index: index,
            id: $scope.item.id,
            boxId: $routeParams.boxId


        }).then(function (response) {
            loadMore = true;
            var data = response.success ? response.payload : '';
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
    $scope.fullScreen = function () {
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
    $scope.create = function (isValid) {
        if (!isValid) {
            return;
        }
        //TODO: add disable state
        $scope.formData.itemId = $routeParams.itemId;
        sItem.addComment($scope.formData).then(function(response) {
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
        });


    }
    cd.pubsub.publish('item', $routeParams.itemId); //statistics
    //todo proper return;
}
        ]);
