mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope', '$modalInstance', 'WizardHandler','sLibrary',
        function ($scope, $modalInstance, WizardHandler, sLibrary) {

            var wizard;

            $scope.display = { createDep: false };
            $scope.formData = {};
            $scope.box = {};

            $scope.next = function () {
                if (!wizard) {
                    wizard = WizardHandler.wizard();
                }

                wizard.next();
            };
            //window.setTimeout(function() {
            //    $scope.next();
            //}, 500);
            $scope.cancel = function () {
                if (!$scope.box.url) {
                    $modalInstance.dismiss();
                    return;
                }

                wizard.finish();                
            };

            $scope.completeWizard = function () {
                $modalInstance.close($scope.box.url);
            };

            $scope.createDepartmentSubmit = function (isValid) {
                if (!isValid) {
                    return;
                }
                sLibrary.createDepartment($scope.formData.createDepartment).then(function (response) {
                    if (response.success) {
                        $scope.display.createDep = false;
                        $scope.$broadcast('newDep', { id: response.payload.id, name: $scope.formData.createDepartment.name });
                    }
                });
            };
        }]
    );
