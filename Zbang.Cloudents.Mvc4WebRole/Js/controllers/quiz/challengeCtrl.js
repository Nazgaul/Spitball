
mDashboard.controller('ChallengeCtrl',
        ['$scope',
         '$modalInstance',
         'data',
         function ($scope, $modalInstance, data) {
             "use strict";
             $scope.popupUsers = data.users;
             for (var i = data.users.length; i < 4; i++) {
                 $scope.popupUsers.splice(1, 0, { image: '', name: '' });
             }

             $scope.afraidTry = function () {
                 $modalInstance.close();
                 //TODO analytics

             };

             $scope.takeChance = function () {
                 $modalInstance.dismiss();
                 //TODO analytics

             };
         }
        ]);
