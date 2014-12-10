mAccount.controller('LoginCtrl',
    ['$scope', 'sFacebook', '$modalInstance', 'data',
        function ($scope, sFacebook, $modalInstance, data) {
            "use strict";

            $scope.params.currentState = data.state;

            $scope.params.states = {
                registerFirst: 0,
                register: 1,
                login: 2
            };

            $scope.formData = {
                login: {},
                register: {}
            };

            $scope.changeState = function (state) {
                $scope.params.currentState = state;
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
            };

            $scope.facebookLogin = function () {
                sFacebook.registerFacebook().then(function () {
                    $modalInstance.close();
                }, function () {
                    $modalInstance.dismiss();
                });
            };

            $scope.login = function () {

            };

            $scope.register = function () { };
        }
    ]);
