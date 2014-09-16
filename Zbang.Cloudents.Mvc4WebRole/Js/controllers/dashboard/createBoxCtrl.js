mDashboard.controller('CreateBoxCtrl',
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
                     var data = response.success ? response.payload : [];
                     $scope.box.url = data.url;
                     $scope.box.id = data.id;
                     $scope.next();
                     //$modalInstance.close(box.payload || box.Payload);
                 });
                
                
             }; 
         }
        ]);
