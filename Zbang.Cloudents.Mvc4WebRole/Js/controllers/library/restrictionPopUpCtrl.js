libChoose.controller('restrictionPopUpCtrl',
[
    '$scope',
    '$modalInstance',
    'data',
    'sLibrary',
    function ($scope, $modalInstance, data, sLibrary) {
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
                if (!response.success) {
                    alert(response.payload[0].value[0]);
                    return;
                }

                $modalInstance.close();
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
            //$scope.submitted = true;
        };

    }
]);
