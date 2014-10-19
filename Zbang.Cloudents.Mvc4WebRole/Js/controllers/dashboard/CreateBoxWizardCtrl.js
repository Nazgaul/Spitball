mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope', '$modalInstance', 'WizardHandler', 'sLibrary', 'data',
        function ($scope, $modalInstance, WizardHandler, sLibrary, data) {

            var wizard;

            if (data.isAcademic) {
                $scope.isAcademic = true;
                $scope.department = data.department;
            }
            if (data.isPrivate) {
                $scope.isPrivate = true;
            }

            $scope.display = { createDep: false };
            $scope.formData = {};
            $scope.box = {};

            $scope.next = function () {
                if (!wizard) {
                    wizard = WizardHandler.wizard();
                }

                wizard.next();
            };

            $scope.cancel = function () {
                if (!$scope.box.url) {
                    $modalInstance.dismiss();
                    return;
                }

                wizard.finish();
            };

            $scope.completeWizard = function (items) {
                var url = $scope.box.url;                
                $modalInstance.close({
                    url: url,
                    isItems: items
                });
            };

            //$scope.createDepartmentSubmit = function (isValid) {
            //    if (!isValid) {
            //        return;
            //    }
            //    sLibrary.createDepartment($scope.formData.createDepartment).then(function (response) {
            //        if (response.success) {
            //            $scope.display.createDep = false;
            //            $scope.$broadcast('newDep', { id: response.payload.id, name: $scope.formData.createDepartment.name });
            //        }
            //    });
            //};
        }]
    );
