mWizardBoxCreate.controller('CreateBoxCtrl',
        ['$scope',
         'sBox',
         'WizardHandler',
         function ($scope, sBox, WizardHandler) {
             $scope.create = function (isValid) {
                 //TODO: add disabled state
                 if (!isValid) {
                     return;
                 }
                 sBox.createPrivate($scope.formData).then(function (box) {
                     
                     //$modalInstance.close(box.payload || box.Payload);
                 });
             };

             $scope.cancel = function () {
                 // WizardHandler.wizard().finish();
                 WizardHandler.wizard().finish();
             };
         }
        ]);
