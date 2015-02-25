mAccount.controller('UserDetailsCtrl',
        ['$scope', '$timeout',
          'sUserDetails',
            function ($scope, $timeout, sUserDetails) {
                "use strict";
                $scope.user = _.clone(sUserDetails.getDetails());

                $scope.$on('userDetailsChange', function (e) {
                    $scope.user.firstName = sUserDetails.getDetails().firstName;
                    $scope.user.image = null;                   
                    $timeout(function () {
                        $scope.user.image = sUserDetails.getDetails().image;
                    });

                });


            }
        ]);
