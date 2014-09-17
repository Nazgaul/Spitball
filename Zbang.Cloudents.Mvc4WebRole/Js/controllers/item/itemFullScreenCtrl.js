mItem.controller('ItemFullScreenCtrl',
[
    '$scope', '$modalInstance', 
    function ($scope, $modalInstance) {
        console.log($scope);

        $scope.closeFullScreen = function() {
            $modalInstance.close();
        };
    }
]);