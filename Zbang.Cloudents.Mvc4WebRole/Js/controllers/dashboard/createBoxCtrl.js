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
                 sBox.createPrivate($scope.formData).then(function (response) {
                     if (response.success) {
                         var data =  response.payload || {};
                         $scope.box.url = data.url;
                         WizardHandler.wizard().finish();
                         return;
                     }
                     $scope.formData.error = response.payload[0].value[0];
                     //$modalInstance.close(box.payload || box.Payload);
                 });
                
                
             };

             $scope.cancel = function () {
                 // WizardHandler.wizard().finish();
                 WizardHandler.wizard().finish();
             };
         }
        ]);
