
app.controller('MainCtrl',
    ['$scope', '$rootScope', '$location', '$modal', '$angularCacheFactory', 'sUser', 'sFacebook', 'sUserDetails', 'Store','$analytics',
        function ($scope, $rootScope, $location, $modal, $angularCacheFactory, sUser, sFacebook, sUserDetails, sStore, $analytics) {
            "use strict";
           
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
            
          $rootScope.checkReg = function (event) {
                if (!sUserDetails.isAuthenticated()) {
                    event.preventDefault();
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

            };

            $scope.$on('$routeChangeSuccess', function (event, current) {
                if (!current.$$route) {
                    return;
                }

                $rootScope.params.isStore = current.$$route.originalPath.indexOf('store') > -1;
                $rootScope.params.isDashboard = current.$$route.originalPath.indexOf('dashboard') > -1;
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

            $rootScope.showSearch = function () {
                if (!$rootScope.params) {
                    return false;
                }
                if (!angular.isDefined($rootScope.params.isDashboard) && !angular.isDefined($rootScope.params.isStore)) {
                    return false;
                }

                return !$rootScope.params.isDashboard && !$rootScope.params.isStore;
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

                sStore.validateCoupon({ code: parseInt($rootScope.params.store.coupon.code, 10) }).then(function (response) {
                    $rootScope.params.store.coupon.buttonDisabled = false;                    
                    if (response.isValid) {
                        $rootScope.params.store.coupon.valid = true;
                        $rootScope.params.store.coupon.code = $rootScope.params.store.coupon.code;
                        cd.pubsub.publish('resetLoginPopup');
                        return;
                    }
                    alert(invalidCouponMessage);
                }).finally(function() {
                    $rootScope.params.store.coupon.buttonDisabled = false;
                });


            };

            $rootScope.logout = function () {
                $angularCacheFactory.removeAll();
                window.location.href = '/account/logoff';

                $analytics.eventTrack('Site Header', {
                    category: 'Logout'
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
