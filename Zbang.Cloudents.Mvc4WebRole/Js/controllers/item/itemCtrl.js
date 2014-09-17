var mItem = angular.module('mItem', []);
mItem.controller('ItemCtrl',
        ['$scope', '$routeParams', 'sItem', '$timeout', '$rootScope',
function ($scope, $routeParams, sItem, $timeout, $rootScope) {
    // cd.pubsub.publish('initItem');
    var index = 0;
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
            imageNumber: index,
            id: $scope.item.id,
            boxId: $routeParams.boxId


        }).then(function (response) {
            var data = response.success ? response.payload : '';
            $scope.preview += data.preview;
        });
    }

    cd.pubsub.publish('item', $routeParams.itemId); //statistics
    //todo proper return;
}
        ]);
