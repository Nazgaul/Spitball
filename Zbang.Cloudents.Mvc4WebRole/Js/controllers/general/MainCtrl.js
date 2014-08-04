app.controller('MainCtrl',
    ['$scope', '$rootScope', '$location', '$modal', 'sUser', 'sFacebook', 'sUserDetails',
        function ($scope, $rootScope, $location, $routeParams, $modal, User, Facebook, sUserDetails) {
            $scope.partials = {
                shareEmail: '/Share/MessagePartial/'
            }


            $rootScope.options = {
                quizOpen: false
            };
            $scope.params = {
                store: {}
            }

            $rootScope.back = {};

            $rootScope.$back = function (url) {
                if (url && url.length) {
                    $location.path(url);
                }
            };
            $rootScope.setUrl = function (url) {
                url = url.replace(location.origin, '');
                $location.url(url, '', url).replace();
            }


            $scope.$on('message', function (e, user) {
                if (user) {
                    $scope.sendUserMessage(user);
                    return;
                }

                var modalInstance = $modal.open({
                    templateUrl: $scope.partials.shareEmail,
                    controller: 'ShareCtrl',
                    backdrop: 'static',
                    resolve: {
                        data: function () {
                            return null;
                        }
                    }
                });

                modalInstance.result.then(function () {
                }, function () {
                    //dismiss
                });
            }); //temp

            $scope.$on('messageFB', function (e, obj) {
                Facebook.share(obj.url, obj.name, obj.caption, obj.description, obj.picture).then(function () {
                    cd.pubsub.publish('addPoints', { type: 'shareFb' });
                });
            });

            $scope.sendUserMessage = function (user) {
                if (!sUserDetails.isAuthenticated()) {
                    event.preventDefault();
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                var modalInstance = $modal.open({
                    templateUrl: $scope.partials.shareEmail,
                    controller: 'ShareCtrl',
                    backdrop: 'static',
                    resolve: {
                        data: function () {
                            return {
                                singleMessage: true,
                                user: user
                            };
                        }
                    }
                });

                modalInstance.result.then(function () {
                }, function () {
                    //dismiss
                });
            };

            $rootScope.checkReg = function (event) {
                if (!sUserDetails.isAuthenticated()) {
                    event.preventDefault();
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

            };

            $scope.$on('$routeChangeSuccess', function (event, current, previous) {
                if (!current.$$route) {
                    return;
                }
                $rootScope.params = {
                    isStore: current.$$route.originalPath.indexOf('store') > -1
                };

                if (!current) {
                    return;
                }
                if (!current.$$route) {
                    return;
                }
                if (!current.$$route.params) {
                    return;
                }
                if (!current.$$route.params.type) {
                    return;
                }

                $scope.params.store.currentTab = current.$$route.params.type;

                if (current.$$route.type === 'products' && current.params.categoryId === '646') {
                    $scope.params.currentTab = 'sales';
                    return;
                }



            });

            $rootScope.isStore = function () {
                if (!$rootScope.params) {
                    return false;
                }
                if (!angular.isDefined($rootScope.params.isStore)) {
                    return false;
                }

                return $rootScope.params.isStore;
            }

            $rootScope.isSearch = function () {
                if (!$rootScope.params) {
                    return false;
                }
                if (!angular.isDefined($rootScope.params.isStore)) {
                    return false;
                }


                return !$rootScope.params.isStore;
            }




            //$scope.info = {
            //    currentLanguage: (function () {
            //        var language = cookieService('lang');
            //        if (language) {
            //            return language;
            //        }

            //        cookieService('lang', 'he-IL', { path: '/' });
            //        return 'he-IL'

            //    })()
            //};

            //$scope.setLanguage = function (val) {
            //    if ($scope.info.currentLanguage === val) {
            //        return;
            //    }
            //    cookieService('lang', val, { path: '/' });
            //    $window.location.reload();
            //};



        }
    ]);
