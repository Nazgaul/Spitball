app.controller('MainCtrl',
    ['$scope', '$rootScope', '$location', '$modal', 'sUser', 'sFacebook', 'sUserDetails', 'Store',
        function ($scope, $rootScope, $location, $modal, User, Facebook, sUserDetails, Store) {
            $scope.partials = {
                shareEmail: '/Share/MessagePartial/'
            }


            $scope.firstTime = $scope.viewBag;

            $rootScope.options = {
                quizOpen: false
            };
            $scope.params = {
                store: {}
            }

            $rootScope.params = {
                store: {}
            };

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
                    windowClass: "personalMsg",
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
                    windowClass: "personalMsg",
                    templateUrl: $scope.partials.shareEmail,
                    controller: 'ShareCtrl',
                    backdrop: 'static',
                    resolve: {
                        data: function () {
                            return {
                                singleMessage: true,
                                users: [user]
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
                
                $rootScope.params.isStore = current.$$route.originalPath.indexOf('store') > -1;       
                $rootScope.params.isQuiz = current.$$route.originalPath.indexOf('quiz') > -1 || current.$$route.originalPath.indexOf('item') > -1;



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

                $rootScope.params.store.currentTab = current.$$route.params.type;

                if (current.$$route.params.type === 'products' && current.params.categoryId === '646') {
                    $rootScope.params.store.currentTab = 'sales';
                }

                if (current.$$route.originalPath.toLowerCase().indexOf('store') > -1 && !sUserDetails.isAuthenticated() &&
                     !$scope.params.store.showRegisterPopup) {
                    $scope.params.store.showRegisterPopup = true;
                    cd.pubsub.publish('register', { action: true });
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

            $rootScope.isQuiz = function () {
                if (!$rootScope.params) {
                    return false;
                }
                if (!angular.isDefined($rootScope.params.isQuiz)) {
                    return false;
                }

                return $rootScope.params.isQuiz;
            };
            $rootScope.isItem = function () {
                if (!$rootScope.params) {
                    return false;
                }
                if (!angular.isDefined($rootScope.params.isItem)) {
                    return false;
                }

                return $rootScope.params.isQuiz;
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

           $rootScope.validateCoupon = function () {
                var invalidCouponMessage = 'קופון שגוי';
                if (!$rootScope.params.store.coupon.code) {
                    return;
                }

                var isNumber = /^\d+$/.test($rootScope.params.store.coupon.code);

                if (!isNumber) {
                    alert(invalidCouponMessage);
                    return;
                }

                $rootScope.params.store.coupon.buttonDisabled = true;

              

                
                Store.validateCoupon({ code: parseInt($rootScope.params.store.coupon.code, 10) }).then(function (response) {
                    $rootScope.params.store.coupon.buttonDisabled = false;
                    if (!response.success) {
                        return;
                    }
                    if (response.payload.isValid) {
                        $rootScope.params.store.coupon.valid = true;
                        $rootScope.params.store.coupon.code = $rootScope.params.store.coupon.code;
                        cd.pubsub.publish('resetLoginPopup');
                        return;
                    }
                    alert(invalidCouponMessage);
                }, function () {
                    $rootScope.params.store.coupon.buttonDisabled = false;
                });


            };


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
