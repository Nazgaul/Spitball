mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope', '$modalInstance', 'WizardHandler',
        function ($scope, $modalInstance, WizardHandler) {

            var wizard;

            $scope.display = { createDep: false };

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

            $scope.completeWizard = function () {
                $modalInstance.close($scope.box.url);
            };

        }]
    );
