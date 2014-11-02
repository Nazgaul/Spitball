var app = angular.module('app', ['ngRoute', 'ngSanitize', 'infinite-scroll', 'custom_scrollbar', 'monospaced.elastic', 'ngDragDrop', 'displayTime', 'textDirection', 'jmdobry.angular-cache',
    'pasvaz.bindonce', 'ui.bootstrap', 'ngAnimate', 'mAccount', 'mDashboard', 'mBox', 'mItem', 'mLibrary', 'mQuiz', 'mUser', 'mSearch', 'debounce', 'angulartics', 'angulartics.google.analytics']);

app.config([
    '$routeProvider',
    '$locationProvider',
    '$httpProvider',
    '$tooltipProvider',
    '$provide',
    '$angularCacheFactoryProvider',
    function ($routeProvider, $locationProvider, $httpProvider, $tooltipProvider, $provide, $angularCacheFactoryProvider) {
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';

        $angularCacheFactoryProvider.setCacheDefaults({
            maxAge: 1500000, //25 minutes
            deleteOnExpire: 'aggressive',
            recycleFreq: 600000,
            storageMode: 'sessionStorage'
        });

        $provide.factory('requestinterceptor', ['$location', '$q', '$angularCacheFactory', function ($location, $q, $angularCacheFactory) {
            return {
                'request': function (config) {

                    return config;
                },
                // optional method
                'response': function (response) {
                    // do something on success
                    switch (response.status) {
                        case 200:
                            return response;                        
                    }
                },
                'responseError': function (response) {
                    // do something on success
                    switch (response.status) {
                        case 400:
                            alert('Version mismatch, page will refresh');
                            window.location.reload(true);
                            break;
                        case 401:
                        case 403:
                            window.open('/account', '_self');
                            break;
                        case 404:
                            window.open('/error', '_self');
                            break;
                        case 412:
                            alert('Version mismatch, page will refresh');
                            window.location.reload(true);
                            break;
                        case 500:
                            window.open('/error', '_self');
                        default:
                            window.open('/error', '_self');
                            break;

                    }
                }
            };
        }]);



        $httpProvider.interceptors.push('requestinterceptor');

        $tooltipProvider.options({
            placement: 'bottom',
            animation: true,
            popupDelay: 500
            //appendToBody: true
        });

        $locationProvider.html5Mode(true).hashPrefix('!');

        //#region routes
        $routeProvider.
        //#region cloudents
        when('/dashboard/', {
            params: {
                type: 'dashboard'
            },
            templateUrl: '/dashboard/'
        }).
        when('/box/my/:boxId/:boxName/', {
            params: {
                type: 'box',
                isPrivate: true
            },
            templateUrl: '/box/IndexPartial/',
            reloadOnSearch: false
        }).
        when('/box/my/:boxId/:boxName/#:tab?', {
            params: {
                type: 'box',
                isPrivate: true
            },
            templateUrl: '/box/IndexPartial/',
            reloadOnSearch: false
        }).
        when('/course/:uniName/:boxId/:boxName/#:tab?', {
            params: {
                type: 'box',
                isPrivate: false
            },
            templateUrl: '/box/IndexPartial/',
            reloadOnSearch: false
        }).
        when('/course/:uniName/:boxId/:boxName/', {
            params: {
                type: 'box',
                isPrivate: false
            },
            templateUrl: '/box/IndexPartial/',
            reloadOnSearch: false
        }).
        when('/item/:uniName/:boxId/:boxName/:itemId/:itemName/', {
            params: {
                type: 'item'
            },
            templateUrl: '/item/IndexPartial/',
            reloadOnSearch: false

        }).
        when('/item/:uniName/:boxId/:boxName/:itemId/:itemName/#fullscreen', {
            params: {
                type: 'item'
            },
            templateUrl: '/item/IndexPartial/',
            reloadOnSearch: false

        }).
        when('/quiz/:uniName/:boxId/:boxName/:quizId/:quizName/', {
            params: {
                type: 'quiz'
            },
            templateUrl: '/quiz/IndexPartial/'
        }).
        when('/library/:libraryId/:libraryName/', {
            params: {
                type: 'library'
            },
            templateUrl: '/library/IndexPartial/'
        }).
        when('/library/choose/', {
            params: {
                type: 'libraryChoose'
            },
            templateUrl: '/library/ChoosePartial/'
        }).
        when('/library/', {
            params: {
                type: 'library'
            },
            templateUrl: '/library/IndexPartial/'
        }).
        when('/search/', {
            params: {
                type: 'search'
            },
            templateUrl: function (params) { return '/search/?q=' + params.q; },
            reloadOnSearch: false
        }).
        when('/user/:userId/:userName/', {
            params: {
                type: 'user'
            },
            templateUrl: function (params) { return '/user/' + params.userId + '/' + encodeURIComponent(params.userName) + '/'; }
        }).
        when('/account/settings', {
            params: {
                type: 'accountSettings'
            },
            templateUrl: '/Account/Settings'
        }).
        //#endregion
        //#region store
             when('/store/', {
                 templateUrl: function (params) {
                     var url = '/Store/IndexPartial/';
                     return buildQueryString(url, params);
                 },
                 controller: 'CategoryCtrl',
                 reloadOnSearch: false,
                 params: {
                     type: 'home'
                 }
             }).
            when('/store/category/:categoryId/', {
                templateUrl: function (params) {
                    var url = '/Store/IndexPartial/';
                    return buildQueryString(url, params);
                },
                controller: 'CategoryCtrl',
                params: {
                    type: 'products'
                }
            }).
            when('/store/product/:productId/:productName/', {
                templateUrl: function (params) {
                    var url = '/store/ProductPartial/';
                    return buildQueryString(url, params);
                },
                controller: 'ProductCtrl',
                params: {
                    type: 'product'
                }
            }).
            when('/store/about/', {
                templateUrl: function (params) {
                    var url = '/Store/AboutPartial/';
                    return buildQueryString(url, params);

                },
                controller: 'ViewCtrl',
                params: {
                    type: 'about'
                }
            }).
            when('/store/contact/', {
                templateUrl: function (params) {
                    var url = '/Store/ContactPartial/';
                    return buildQueryString(url, params);

                },
                controller: 'ContactCtrl',
                params: {
                    type: 'contact'
                }
            }).
            when('/store/checkout/:productId/', {
                templateUrl: function (params) {
                    var url = '/Store/CheckOutPartial/';
                    return buildQueryString(url, params);
                },

                controller: 'CheckoutCtrl',
                params: {
                    type: 'checkout'
                }
            }).
            when('/store/terms/', {
                templateUrl: function (params) {
                    var url = '/store/TermsPartial/';
                    return buildQueryString(url, params);

                },
                controller: 'ViewCtrl',
                params: {
                    type: 'terms'
                }
            }).
            when('/store/thankyou/', {
                templateUrl: '/store/ThankyouPartial/',
                controller: 'ViewCtrl',
                params: {
                    type: 'thankyou'
                }
            }).
          //#endregion
        otherwise({ redirectTo: '/dashboard/' });


        function buildQueryString(url, params) {
            var first = true;
            for (var key in params) {
                if (first) {
                    url += '?' + key.toLowerCase() + '=' + params[key];
                    first = false;
                    continue;
                }
                url += '&' + key.toLowerCase() + '=' + params[key];

            }
            return url;
        }

        //#endregion


        //#region log js errors 
        $provide.decorator('$exceptionHandler', ['$delegate', '$log', 'stackTraceService', function ($delegate, $log, stackTraceService) {
            return function (exception, cause) {
                $delegate(exception, cause);

                try {
                    var errorMessage = exception.toString(),
                        stackTrace = stackTraceService.print({ e: exception });

                    $.ajax({
                        type: 'POST',
                        url: '/Error/JsLog',
                        contentType: 'application/json',
                        data: angular.toJson({
                            errorUrl: window.location.href,
                            errorMessage: errorMessage,
                            stackTrace: stackTrace,
                            cause: cause || ''
                        })
                    });

                }
                catch (loggingError) {
                    $log.warn('Error logging failed');
                    $log.log(loggingError);
                };
            };
        }]);

        //#endregion
    }
]);

app.run(['$rootScope', '$window', '$location', 'sUserDetails', 'sNewUpdates', 'sVerChecker', function ($rootScope, $window, $location, sUserDetails, sNewUpdates, sVerChecker) {
    sVerChecker.checkVersion();

    $rootScope.initDetails = function (userData) {
        sUserDetails.setDetails(userData);

    };

    $rootScope.$on('$routeChangeStart', function (event, next) {
        $window.scrollTo(0, 0);

        if (!next.$$route) {
            return;
        }
        if (!next.$$route.params) {
            return;
        }
        //if (sUserDetails.isAuthenticated() && !sUserDetails.getDepartment() && next.$$route.params.type !== 'libraryChoose') {
        //    $location.path('/library/choose/');
        //}Start
        sNewUpdates.loadUpdates();
    });

    $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {

        //title 
        if (!previous) {

            if (!isCurrentRoute(current)) {
                return;
            }

            if (current.$$route.params.type === 'box') {
                if (sUserDetails.isAuthenticated()) {
                    sNewUpdates.removeUpdates(current.params.boxId);
                }

                setBackDashboard();
            }
            return;
        }
        if (!previous.$$route) {
            return;
        }
        if (!previous.$$route.params) {
            return;
        }
        if (!previous.$$route.params.type) {
            return;
        }

        if (!isCurrentRoute(current)) {
            return;
        }

        if (current.$$route.params.type === 'box') {
            if (sUserDetails.isAuthenticated()) {
                sNewUpdates.removeUpdates(current.params.boxId);
            }

            switch (previous.$$route.params.type) {
                case 'user':
                    $rootScope.back.title = previous.pathParams.userName;
                    $rootScope.back.url = previous.loadedTemplateUrl;
                    break;
                case 'library':
                    $rootScope.back.title = previous.params.libraryName;
                    $rootScope.back.url = '/library/' + previous.params.libraryId + '/' + previous.params.libraryName + '/';
                    break;
                default:
                    setBackDashboard();
                    break;
            }
        }

        if (current.$$route.params.type === 'library') {
            $rootScope.back.title = previous.pathParams.libraryName;
            $rootScope.back.url = previous.loadedTemplateUrl;
        }
    });

    function isCurrentRoute(current) {
        if (!current) {
            return false;
        }
        if (!current.$$route) {
            return false;
        }
        if (!current.$$route.params) {
            return false;
        }
        if (!current.$$route.params.type) {
            return false;
        }

        return true;
    }
    function setBackDashboard() {
        $rootScope.back.url = '/dashboard/';
        $rootScope.back.title = 'Dashboard';
    }
}]);
