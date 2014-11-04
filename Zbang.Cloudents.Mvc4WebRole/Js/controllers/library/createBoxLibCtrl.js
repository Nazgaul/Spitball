
mLibrary.controller('CreateBoxLibCtrl',
        ['$scope',
         '$modalInstance',
         'sBox',
         'parentId',

         function ($scope, $modalInstance, sBox, parentId) {
             "use strict";
             $scope.formData = {
                 parentId: parentId
             };

             $scope.create = function (isValid) {
                 if (!isValid) {
                     return;
                 }

                 sBox.createAcademic($scope.formData).then(function (response) {
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
