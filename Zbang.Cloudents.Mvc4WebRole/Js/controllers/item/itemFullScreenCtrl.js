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

        $scope.$on('update', function (e,preview) {
            console.log(preview);
            if (preview.indexOf('iframe') > 0) {
                $scope.preview2 = $sce.trustAsHtml(preview);
            } else {
                $scope.preview2 += preview;
            }
            // $scope.$apply(function() { //regular binding doesnt work
           
             //   $scope.preview2 += $scope.$parent.preview;

            //});
        });
    }
]);