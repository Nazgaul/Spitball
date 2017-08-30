mLibrary.controller('CreateDepartmentCtrl',
        ['$scope',
         '$modalInstance',         
'$analytics',
         function ($scope, $modalInstance, $analytics) {
             "use strict";
             $scope.formData = {};

             $scope.create = function (isValid) {
                 if (!isValid) {
                     return;
                 }

                 $analytics.eventTrack('Created', {
                     category: 'Create Department'
                 });

                $modalInstance.close($scope.formData);
             };

             $scope.cancel = function () {
                 $modalInstance.dismiss();

                 $analytics.eventTrack('Cancel', {
                     category: 'Create Department'
                 });

             };
         }
        ]);
