
mDashboard.controller('ChallengeCtrl',
        ['$scope',
         '$modalInstance',
         '$analytics',
         'data',
         function ($scope, $modalInstance, $analytics,data) {
             "use strict";
             $scope.popupUsers = data.users;
             for (var i = data.users.length; i < 4; i++) {
                 $scope.popupUsers.splice(1, 0, { image: '', name: null });
             }

             $scope.afraidTry = function () {
                 $modalInstance.close();
                 $analytics.eventTrack('Afraid to try', {
                     category: 'Quiz Chanllenge'
                 });

             };

             $scope.takeChance = function () {
                 $modalInstance.dismiss();
                 $analytics.eventTrack('Take Chance', {
                     category: 'Quiz Chanllenge'
                 });

             };
         }
        ]);
