mDashboard.controller('ChallengeCtrl',
        ['$scope',
         '$modalInstance',
         'data',
         function ($scope, $modalInstance,data) {
             $scope.popupUsers = data.users;
             for (var i = data.users.length; i < 4; i++) {
                 $scope.users.push({ image: '', name: '' });                 
             }

             $scope.afraidTry = function () {
                 $modalInstance.close();
             };

             $scope.takeChance = function () {
                 $modalInstance.dismiss();
             };
         }
        ]);
