var app = angular.module('app', ['ngRoute', 'ngSanitize', 'infinite-scroll', 'custom_scrollbar', 'monospaced.elastic', 'ngDragDrop', 'displayTime', 'textDirection',
    'pasvaz.bindonce', 'ui.bootstrap', 'ngAnimate', 'mDashboard', 'mBox', 'mItem', 'mLibrary', 'mQuiz', 'mUser', 'mSearch', 'debounce', 'angulartics', 'angulartics.google.analytics', 'angular-appinsights']);

app.config([
    '$routeProvider',
    '$locationProvider',
    '$httpProvider',
    '$tooltipProvider',
    '$provide',
    'insightsProvider',
    function ($routeProvider, $locationProvider, $httpProvider, $tooltipProvider, $provide, insightsProvider) {

        insightsProvider.start('25195c1a-be80-4b61-a3f8-00d10e2efa62');
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';



        $provide.factory('requestinterceptor', ['$location', '$q', function ($location, $q) {
            return {
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
                    //switch (response.status) {
                    //    case 401:
                    //    case 403:
                    //        window.open('/account', '_self');
                    //        break;
                    //    case 404:
                    //        window.open('/error', '_self');
                    //    case 500:
                    //        window.open('/error', '_self');
                    //    default:
                    //        window.open('/error', '_self');
                    //        break;

                    //}
                }
            };
        }]);



        $httpProvider.interceptors.push('requestinterceptor');

        $tooltipProvider.options({
            placement: 'bottom',
            animation: true,
            popupDelay: 500,
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
            templateUrl: function (params) {
                return '/quiz/IndexPartial/';
            }
        }).
        when('/department/:uniName/:libraryId/:libraryName/', {
            params: {
                type: 'library'
            },
            templateUrl: '/department/'
        }).
        when('/library/choose/', {
            params: {
                type: 'libraryChoose'
            },
            templateUrl: '/library/ChoosePartial/'
        }).
        when('/department/', {
            params: {
                type: 'library'
            },
            templateUrl: '/department/'
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
        //#endregion
        //#region store
             when('/store/', {
                 templateUrl: function (params) {
                     var url = '/Store/';
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
                    var url = '/Store/';
                    return buildQueryString(url, params);
                },
                controller: 'CategoryCtrl',
                params: {
                    type: 'products'
                }
            }).
            when('/store/product/:productId/:productName/', {
                templateUrl: function (params) {
                    var url = '/store/product/';
                    return buildQueryString(url, params);
                },
                controller: 'ProductCtrl',
                params: {
                    type: 'product'
                }
            }).
            when('/store/about/', {
                templateUrl: function (params) {
                    var url = '/Store/About/';
                    return buildQueryString(url, params);

                },
                controller: 'ViewCtrl',
                params: {
                    type: 'about'
                }
            }).
            when('/store/contact/', {
                //templateUrl: '/Store/Contact/',
                templateUrl: function (params) {
                    var url = '/Store/Contact/';
                    return buildQueryString(url, params);

                },
                controller: 'ContactCtrl',
                params: {
                    type: 'contact'
                }
            }).
            when('/store/checkout/:productId/', {
                templateUrl: function (params) {
                    var url = '/Store/Checkout/';
                    return buildQueryString(url, params);
                },

                controller: 'CheckoutCtrl',
                params: {
                    type: 'checkout'
                }
            }).
            when('/store/terms/', {
                templateUrl: function (params) {
                    var url = '/store/Terms/';
                    return buildQueryString(url, params);

                },
                controller: 'ViewCtrl',
                params: {
                    type: 'terms'
                }
            }).
            when('/store/thankyou/', {
                templateUrl: '/store/Thankyou/',
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

app.run(['$rootScope', '$window', '$location', 'sUserDetails', 'sNewUpdates', function ($rootScope, $window, $location, sUserDetails, sNewUpdates) {
    $rootScope.initDetails = function (userData) {
        sUserDetails.setDetails(userData);

    };

    $rootScope.$on('$routeChangeStart', function (event, next, current) {
        $window.scrollTo(0, 0);

        if (!next.$$route) {
            return;
        }
        if (!next.$$route.params) {
            return;
        }
        if (sUserDetails.isAuthenticated() && !sUserDetails.getDepartment() && next.$$route.params.type !== 'libraryChoose') {
            $location.path('/library/choose/');
        }
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

        switch (previous.$$route.params.type) {
            case 'item':
                cd.pubsub.publish('itemclear');
                break;
            case 'quiz':
                cd.pubsub.publish('quizclear');
                break;
        };

        if (!isCurrentRoute(current)) {
            return;
        }

        if (current.$$route.params.type === 'box') {
            if (sUserDetails.isAuthenticated()) {
                sNewUpdates.removeUpdates(current.params.boxId);
            }

            switch (previous.$$route.params.type) {
                case 'library':
                    if (current.$$route.params.isPrivate) {
                        setBackDashboard();
                    }
                    else {
                        $rootScope.back.title = previous.pathParams.libraryName;
                        $rootScope.back.url = '/department/' + previous.pathParams.libraryName + '/' + previous.pathParams.libraryId + '/' + previous.pathParams.libraryName;
                    }                    
                    break;
                case 'user':                    
                    $rootScope.back.title = previous.pathParams.userName;
                    $rootScope.back.url = previous.loadedTemplateUrl;
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
