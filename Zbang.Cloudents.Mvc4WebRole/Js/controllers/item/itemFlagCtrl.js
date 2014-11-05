
mItem.controller('itemFlagCtrl',
[
    '$scope', '$modalInstance', 'data', '$timeout', 'sItem',
function ($scope, $modalInstance, data, $timeout, sItem) {
    "use strict";
    $scope.secondStep = false;
    $scope.disable = false;
    $scope.formdata = {
        id: data.id
    };

    $scope.flagItem = function () {
        $scope.disable = true;
        sItem.flag($scope.formdata).then(function (response) {
            $scope.secondStep = true;

            $timeout(function () {
                $modalInstance.close();
            }, 3000);
        }, function (response) {
            alert(response);
        }).finally(function () {
            $scope.disable = false;
        });

        //TODO analytics

    }
    $scope.cancel = function () {
        if ($scope.secondStep) {
            $modalInstance.close();
        }
        $modalInstance.dismiss();
        //TODO analytics

    };
}
]);