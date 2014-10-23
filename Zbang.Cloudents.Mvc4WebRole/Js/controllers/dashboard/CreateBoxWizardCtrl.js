mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope', '$modalInstance', 'WizardHandler', 'sLibrary', 'data',
        function ($scope, $modalInstance, WizardHandler, sLibrary, data) {

            var wizard;

            if (data.isAcademic) {
                $scope.isAcademic = true;
                $scope.boxType = 'academic';
                $scope.department = data.department;
            }

            if (data.isPrivate) {
                $scope.isPrivate = true;
                $scope.boxType = 'private';
            }

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
        }]
    );
