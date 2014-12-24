mAccount.controller('UserDetailsCtrl',
        ['$scope',
          'sUserDetails',
            function ($scope, sUserDetails) {
                "use strict";
                $scope.user = sUserDetails.getDetails();

                $scope.$on('userDetailsChange', function (e) {
                    $scope.user = sUserDetails.getDetails();
                });
            }
        ]);
