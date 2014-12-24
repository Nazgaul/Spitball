mBox.controller('LeavePromptCtrl',
    ['$scope', '$modalInstance',  'data',
function ($scope, $modalInstance, data) {
    "use strict";

    $scope.formData = {};

    $scope.name = data.name;

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