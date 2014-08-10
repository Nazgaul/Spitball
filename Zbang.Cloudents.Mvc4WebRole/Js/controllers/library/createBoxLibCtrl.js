mLibrary.controller('CreateBoxLibCtrl',
        ['$scope',
         '$modalInstance',
         'sLibrary',
         'parentId',

         function ($scope, $modalInstance, sLibrary, parentId) {
             $scope.formData = {
                 parentId: parentId
             };

             $scope.create = function (isValid) {
                 if (!isValid) {
                     return;
                 }

                 sLibrary.box.create($scope.formData).then(function (response) {
                     if (!response.success) {
                         alert(response.payload || response.Payload);
                         return;
                     }
                     $modalInstance.close(response.payload || response.Payload);
                 },
                 function () {
                     alert('error creating box');
                 }
                 );
             };

             $scope.cancel = function () {
                 $modalInstance.dismiss();
             };
         }
        ]);
