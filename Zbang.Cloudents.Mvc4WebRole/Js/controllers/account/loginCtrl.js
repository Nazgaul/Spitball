mAccount.controller('LoginCtrl',
    ['$scope','$modalInstance',
        function ($scope, $modalInstance) {
            "use strict";

            $scope.params.currentState = 0;

            $scope.params.states = {
                registerFirst: 0,
                register: 1,
                login: 2
            };


            $scope.changeState = function (state) {
                $scope.params.currentState = state;
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
            };
        }
    ]);
