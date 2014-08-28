mDashboard.controller('ChallengeCtrl',
        ['$scope',
         '$modalInstance',  

         function ($scope, $modalInstance) {

             $scope.afraidTry = function () {
                 $modalInstance.close(); 
             };

             $scope.takeChance = function () {
                 $modalInstance.dismiss();
             };             
         }
        ]);
