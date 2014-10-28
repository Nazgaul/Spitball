"use strict";
mItem.controller('itemFlagCtrl',
[
    '$scope', '$modalInstance', 'data', '$timeout', 'sItem',
function ($scope, $modalInstance, data, $timeout, sItem) {

    $scope.secondStep = false;
    $scope.disable = false;
    $scope.formdata = {
        id: data.id
    };

    $scope.flagItem = function () {
        $scope.disable = true;
        sItem.flag($scope.formdata).then(function (response) {
            if (!response.success) {
                alert(response.payload);
                return;
            }

            $scope.secondStep = true;

            $timeout(function () {
                $modalInstance.close();
            }, 3000);
        });

    }
    $scope.cancel = function () {
        if ($scope.secondStep) {
            $modalInstance.close();
        }
        $modalInstance.dismiss();
    };
}
]);