
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
                     $modalInstance.close(response);
                 },
                 function () {
                     alert(response);
                 });
             };

             $scope.cancel = function () {
                 $modalInstance.dismiss();
             };
         }
        ]);
