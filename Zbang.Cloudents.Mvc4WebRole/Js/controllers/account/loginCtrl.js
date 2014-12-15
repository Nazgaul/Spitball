mAccount.controller('LoginCtrl',
    ['$scope', '$window', '$route', '$location', '$routeParams', 'sFacebook', 'sAccount', '$modalInstance', '$analytics', '$angularCacheFactory', 'data',
        function ($scope, $window, $route, $location, $routeParams, sFacebook, sAccount, $modalInstance, $analytics, $angularCacheFactory, data) {
            "use strict";
            var loginDisable, registerDisable;

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
                $scope.params.loginServerError = $scope.params.registerServerError = null;                
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
            };

            $scope.facebookLogin = function () {
                sFacebook.registerFacebook({ boxId: $routeParams.boxId }).then(function () {
                    $window.location.reload();
                }, function () {
                    $modalInstance.dismiss();
                });
            };

            $scope.login = function (isValid) {
                if (!isValid) {
                    return;
                }
                if (loginDisable) {
                    return;
                }
                loginDisable = true;

                $scope.params.loginServerError = null;

                sAccount.login($scope.formData.login).then(function () {
                    var routeName = $route.current.$$route.params.type;

                    if (routeName === 'account') {
                        window.location.href = '/dashboard/';
                        return;
                    }

                    $window.location.reload();


                }, function (response) {
                    $scope.params.loginServerError = response[0].value[0];
                }).finally(function () {
                    loginDisable = false;
                });
            };

            $scope.register = function (isValid) {
                if (!isValid) {
                    return;
                }
                if (registerDisable) {
                    return;
                }
                registerDisable = true;

                $scope.params.registerServerError = null;

                if ($routeParams.boxId) {
                    $scope.formData.register.boxId = $routeParams.boxId;
                }

                sAccount.register($scope.formData.register).then(function () {
                    var routeName = $route.current.$$route.params.type;
                    var cache = $angularCacheFactory('points', {
                        maxAge: 600000
                    });

                    cache.put('newUser', true);

                    if ($routeParams.boxId) {
                        $window.location.reload();
                        return;
                    }

                    if (routeName === 'account') {
                        window.location.href = '/library/choose/';
                    }
                    //for now we do postback all the time
                    //else {
                    //    $location.path('/library/choose/');

                    //}



                }, function (response) {
                    $scope.params.registerServerError = response[0].value[0];
                }).finally(function () {
                    registerDisable = false;
                });
            };

            $scope.changeLanguage = function () {

                $angularCacheFactory.get('htmlCache').removeAll();

                $analytics.eventTrack('Language Change', {
                    category: 'Register Popup',
                    label: 'User changed language to ' + $scope.params.language
                });

                sAccount.changeLocale({ language: $scope.params.language }).then(function () {

                    var cache = $angularCacheFactory.get('changeLanguage') || $angularCacheFactory('changeLanguage');

                    cache.put('formData', JSON.stringify({
                        formData: {
                            login: $scope.formData.login,
                            register: $scope.formData.register,
                        },
                        currentState: $scope.params.currentState
                    }));
                    $window.location.reload();
                });
            };
        }
    ]);
