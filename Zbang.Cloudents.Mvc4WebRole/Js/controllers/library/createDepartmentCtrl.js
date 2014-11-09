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

                 $analytics.eventTrack('Create Department', {
                     category: 'Department Created'
                 });

                $modalInstance.close($scope.formData);
             };

             $scope.cancel = function () {
                 $modalInstance.dismiss();

                 $analytics.eventTrack('Create Department', {
                     category: 'Cancel Create'
                 });

             };
         }
        ]);
