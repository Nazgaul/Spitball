mDashboard.controller('ChallengeCtrl',
        ['$scope',
         '$modalInstance',
         'data',
         function ($scope, $modalInstance,data) {
             $scope.users = data.users;
             for (var i = $scope.users.length; i < 4; i++) {
                 $scope.users.splice(1, 0, { image: '', name: '' });
                 //$scope.users.push({ });
             }

             $scope.afraidTry = function () {
                 $modalInstance.close();
             };

             $scope.takeChance = function () {
                 $modalInstance.dismiss();
             };
         }
        ]);
