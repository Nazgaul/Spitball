﻿mItem.controller('itemFullScreenCtrl',
[
    '$scope', '$modalInstance', 
    function ($scope, $modalInstance) {
        $scope.$parent.fullScreen = true;
        $scope.closeFullScreen = function() {
            $modalInstance.close();
            $scope.$parent.fullScreen = false;
        };
    }
]);