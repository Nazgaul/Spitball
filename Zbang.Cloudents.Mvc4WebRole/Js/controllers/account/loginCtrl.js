﻿mAccount.controller('LoginCtrl',
    ['$scope', '$window', '$location', '$routeParams', 'sFacebook', 'sAccount', '$modalInstance', '$analytics', '$angularCacheFactory', 'data',
        function ($scope, $window, $location, $routeParams, sFacebook, sAccount, $modalInstance, $analytics, $angularCacheFactory, data) {
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
                if ($routeParams.boxId) {
                    $scope.formData.register.boxId = $routeParams.boxId;
                }

                sAccount.register($scope.formData.register).then(function () {
                    var cache = $angularCacheFactory('points', {
                        maxAge: 600000
                    });

                    cache.put('newUser', true);

                    if ($routeParams.boxId) {
                        $window.location.reload();
                        return;
                    }

                    $location.path('/library/choose/');
                    

                });
            };

            $scope.changeLanguage = function () {

                $angularCacheFactory.get('htmlCache').removeAll();

                $analytics.eventTrack('Language Change', {
                    category: 'Register Popup',
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
