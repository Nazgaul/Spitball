mAccount.controller('LoginCtrl',
    ['$scope', '$window', 'sFacebook', 'sAccount', '$modalInstance', '$analytics', '$angularCacheFactory', 'data',
        function ($scope, $window, sFacebook, sAccount, $modalInstance, $analytics, $angularCacheFactory, data) {
            "use strict";

            $scope.params.currentState = data.state;

            $scope.params.states = {
                registerFirst: 0,
                register: 1,
                login: 2
            };

            data.formData = data.formData || {};

            $scope.formData = {
                login: data.formData.login || {},
                register: data.formData.register || {}
            };

            $scope.params.language = $scope.params.currentLanague;

            $scope.changeState = function (state) {
                $scope.params.currentState = state;
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
            };

            $scope.facebookLogin = function () {
                sFacebook.registerFacebook().then(function () {
                    $window.location.reload();
                }, function () {
                    $modalInstance.dismiss();
                });
            };

            $scope.login = function () {
                sAccount.login($scope.formData.login).then(function () {
                    $window.location.reload();
                });
            };

            $scope.register = function () {                
                sAccount.register($scope.formData.register).then(function () {
                    var cache = $angularCacheFactory('points', {
                        maxAge: 60000
                    });

                    cache.put('newUser', true);

                    $window.location.reload();
                });
            };

            $scope.changeLanguage = function () {

                $angularCacheFactory.get('htmlCache').removeAll();

                $analytics.eventTrack('Account settings', {
                    category: 'Language Change',
                    label: 'User changed language to ' + $scope.params.language
                });

                sAccount.changeLanguage({ language: $scope.params.language }).then(function () {

                    var cache = $angularCacheFactory('changeLanguage', {
                        maxAge: 60000
                    });

                    cache.put('formData', JSON.stringify({
                        formData: {
                            login: $scope.formData.login,
                            register: $scope.formData.register,
                        },
                        currentStae: $scope.parmas.currentState
                    }));
                    $window.location.reload();
                });
            };
        }
    ]);
