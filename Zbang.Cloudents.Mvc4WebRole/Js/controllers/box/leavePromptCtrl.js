mBox.controller('LeavePromptCtrl',
    ['$scope', '$modalInstance', '$location', 'sBox',
function ($scope, $modalInstance, $location, sBox) {
    "use strict";

    $scope.delete = function () {
        $modalInstance.close({ 'delete': true });
    };

    $scope.leave = function () {
        $modalInstance.close({ leave: true });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };
}]);