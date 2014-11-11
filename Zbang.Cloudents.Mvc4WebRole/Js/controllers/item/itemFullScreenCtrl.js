
mItem.controller('itemFullScreenCtrl',
[
    '$scope', '$modalInstance','$sce','$analytics',
    function ($scope, $modalInstance, $sce, $analytics) {
        "use strict";
        $scope.preview2 = $scope.$parent.preview;
        
        $scope.$parent.fullScreen = true;
        $scope.closeFullScreen = function() {
            $modalInstance.close();
            $scope.$parent.fullScreen = false;
            $analytics.eventTrack('Item Fullscreen', {
                category: 'Close'
            });

        };

        $scope.$on('update', function (e, preview) {      
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