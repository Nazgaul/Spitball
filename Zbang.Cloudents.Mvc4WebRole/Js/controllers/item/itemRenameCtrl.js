
mItem.controller('itemRenameCtrl',
[
    '$scope', '$modalInstance', 'data', 'sItem',
function ($scope, $modalInstance, data, sItem) {
    "use strict";
    $scope.formdata = {}
    $scope.formdata.newName = data.name;
    $scope.formdata.id = data.id;

    $scope.disable = false;

    $scope.renameItem = function () {
        $scope.disable = true;

        $analytics.eventTrack('Rename Item', {
            category: 'Item Renamed'
        });

        sItem.rename($scope.formdata).then(function (response) {
            $modalInstance.close(response);
        }, function (response) {
            alert(response);
        }).finally(function () {
            $scope.disable = false;
        });

    };

    $scope.cancel = function () {
        $analytics.eventTrack('Rename Item', {
            category: 'Cancel Rename'
        });

        $modalInstance.dismiss();
    };
}

]);