mWizardBoxCreate.controller('createAcademicBoxCtrl',
        ['$scope',
         'sBox',
         'WizardHandler',
         function ($scope, sBox, WizardHandler) {
             $scope.create = function (isValid) {
                 console.log($scope.formData);
                
                 //TODO: add disabled state
                 if (!isValid) {
                     return;
                 }
                 sBox.createAcademic($scope.formData.academicBox).then(function (response) {
                     var data = response.success ? response.payload : [];
                     $scope.box.url = data.url;
                     WizardHandler.wizard().finish();
                     //WizardHandler.wizard().next();
                 //    //$modalInstance.close(box.payload || box.Payload);
                 });
             };

             $scope.cancel = function () {
                 // WizardHandler.wizard().finish();
                 WizardHandler.wizard().finish();
             };
         }
        ]);
