﻿mItem.controller('ItemRenameCtrl',
[
    '$scope', '$modalInstance', '$analytics','data', 'sItem','sNotify',
function ($scope, $modalInstance,$analytics, data, sItem,sNotify) {
    "use strict";
    $scope.formdata = {}
    $scope.formdata.newName = data.name;
    $scope.formdata.id = data.id;

    $scope.disable = false;

    $scope.renameItem = function () {
        $scope.disable = true;

        $analytics.eventTrack('Rename Item', {
            category: 'Item'
        });

        sItem.rename($scope.formdata).then(function (response) {
            $modalInstance.close(response);
        }, function (response) {
            sNotify.alert(response);
        }).finally(function () {
            $scope.disable = false;
        });

    };

    $scope.cancel = function () {
        $analytics.eventTrack('Cancel Rename', {
            category: 'Item'
        });

        $modalInstance.dismiss();
    };
}

]);