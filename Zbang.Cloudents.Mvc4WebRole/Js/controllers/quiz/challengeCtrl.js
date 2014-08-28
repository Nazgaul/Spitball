mDashboard.controller('ChallengeCtrl',
        ['$scope',
         '$modalInstance',  

         function ($scope, $modalInstance) {

             $scope.takeChance = function () {
                 $modalInstance.close();
             };
             $scope.afraidTry = function () {
                 $modalInstance.cancel();
             };
         }
        ]);
