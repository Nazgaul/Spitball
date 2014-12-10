mLibrary.controller('restrictionPopUpCtrl',
[
    '$scope',
    '$modalInstance',
    'data',
    'sLibrary',
    'sNotify',
    function ($scope, $modalInstance, data, sLibrary, sNotify) {
        "use strict";
        $scope.formData = {
            UniversityId: data.university.id
        };
        $scope.formData1 = {};
        $scope.params = {
            isStudent: true
        };
        $scope.russainStep2 = false;

        $scope.create = function (isValid) {
            if (!isValid) {
                return;
            }
            sLibrary.updateUniversity($scope.formData).then(function (response) {
                $modalInstance.close();
            }, function (response) {
                sNotify.alert(response[0].value[0]);
            });
        };

        $scope.cancel = function () {
            $modalInstance.dismiss();
        };

        $scope.russianStep1 = function (isValid) {
            if (!isValid) {
                return;
            }

            if ($scope.formData1.russianCode === "cloudvivt") {
                $scope.russainStep2 = true;
                return;
            }
            $scope.submitted = true;
        };

    }
]);
