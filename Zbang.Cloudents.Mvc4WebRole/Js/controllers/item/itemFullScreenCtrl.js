mItem.controller('itemFullScreenCtrl',
[
    '$scope', '$modalInstance','$sce',
    function ($scope, $modalInstance, $sce) {
        $scope.preview2 = '';
        $scope.$parent.fullScreen = true;
        $scope.closeFullScreen = function() {
            $modalInstance.close();
            $scope.$parent.fullScreen = false;
        };

        $scope.$on('update', function () {
            $scope.$apply(function () { //regular binding doesnt work
                $scope.preview2 += $scope.$parent.preview;    

            });
        });
       
       
    }
]);