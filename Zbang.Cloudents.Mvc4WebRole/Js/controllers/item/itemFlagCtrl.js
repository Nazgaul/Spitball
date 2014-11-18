
mItem.controller('itemFlagCtrl',
[
    '$scope', '$modalInstance', 'data', '$timeout', '$analytics', 'sItem', 'sNotify',
function ($scope, $modalInstance, data, $timeout, $analytics, sItem, sNotify) {
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
            sNotify.alert(response);
        }).finally(function () {
            $scope.disable = false;
        });

        $analytics.eventTrack('Flag Item', {
            category: 'Item was flagged ' + $scope.formdata.id
        });
    }
    $scope.cancel = function () {
        if ($scope.secondStep) {
            $modalInstance.close();
        }
        $modalInstance.dismiss();
        $analytics.eventTrack('Flag Item', {
            category: 'Cancel'
        });
    };
}
]);