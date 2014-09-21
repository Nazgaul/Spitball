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
                     if (response.success) {
                         var data =  response.payload || {};
                         $scope.box.url = data.url;
                     $scope.box.id = data.id;
                     $scope.next();
                     }
                     $scope.formData.error = response.payload[0].value[0];
                     //$modalInstance.close(box.payload || box.Payload);
                 });
                
                
             }; 
         }
        ]);
