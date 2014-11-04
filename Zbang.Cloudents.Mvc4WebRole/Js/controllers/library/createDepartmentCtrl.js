
mLibrary.controller('CreateDepartmentCtrl',
        ['$scope',
         '$modalInstance',         

         function ($scope, $modalInstance) {
             "use strict";
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
