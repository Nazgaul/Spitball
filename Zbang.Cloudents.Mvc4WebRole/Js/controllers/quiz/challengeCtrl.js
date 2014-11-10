
mDashboard.controller('ChallengeCtrl',
        ['$scope',
         '$modalInstance',
         '$analytics',
         'data',
         function ($scope, $modalInstance, $analytics,data) {
             "use strict";
             $scope.popupUsers = data.users;
             for (var i = data.users.length; i < 4; i++) {
                 $scope.popupUsers.splice(1, 0, { image: '', name: '' });
             }

             $scope.afraidTry = function () {
                 $modalInstance.close();
                 $analytics.eventTrack('Quiz Chanllenge', {
                     category: 'Afraid to try'
                 });

             };

             $scope.takeChance = function () {
                 $modalInstance.dismiss();
                 $analytics.eventTrack('Quiz Chanllenge', {
                     category: 'Take Chance'
                 });

             };
         }
        ]);
