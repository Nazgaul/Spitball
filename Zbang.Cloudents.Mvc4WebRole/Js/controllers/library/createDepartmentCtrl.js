"use strict";
mLibrary.controller('CreateDepartmentCtrl',
        ['$scope',
         '$modalInstance',         

         function ($scope, $modalInstance) {
             
             $scope.formData = {};

             $scope.create = function (isValid) {
                 if (!isValid) {
                     return;
                 }
                $modalInstance.close($scope.formData);
             };

             $scope.cancel = function () {
                 $modalInstance.dismiss();
             };
         }
        ]);
