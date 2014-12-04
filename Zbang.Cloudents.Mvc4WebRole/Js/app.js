var app;
(function (angular) {
    "use strict";
    app = angular.module('app', ['ngRoute', 'ngSanitize', 'infinite-scroll', 'custom_scrollbar', 'monospaced.elastic', 'ngDragDrop', 'displayTime', 'textDirection', 'jmdobry.angular-cache',
       'pasvaz.bindonce', 'ui.bootstrap', 'ngAnimate', 'mAccount', 'mDashboard', 'mBox', 'mItem', 'mLibrary', 'mQuiz', 'mUser', 'mSearch', 'debounce', 'angulartics', 'angulartics.google.analytics']).
   config([
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
               recycleFreq: 30000,
               storageMode: 'sessionStorage'
           });

           $provide.factory('requestinterceptor', [function () {

               return {
                   'request': function (config) {

                       return config;
                   },
                   // optional method
                   'response': function (response) {
                       // do something on success
                       //switch (response.status) {
                       //    case 200:
                       //        return response;
                       //        break;
                       //}

                       return response;
                   },
                   'responseError': function (response) {
                       switch (response.status) {

                           case 400:
                               alert('Cloudents has updated, refreshing page');
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
                               alert('Cloudents has updated, refreshing page');
                               window.location.reload(true);
                               break;
                           case 500:
                               window.open('/error', '_self');
                               break;
                           default:
                               window.open('/error', '_self');
                               break;

                       }

                       return response;
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


           var originalWhen = $routeProvider.when;

           $routeProvider.when = function (path, route) {
               route.resolve = {
                   currentUser: ['$q', 'sUserDetails', 'sNewUpdates', function ($q, sUserDetails, sNewUpdates) {
                       return sUserDetails.initDetails().then(sNewUpdates.loadUpdates);
                   }]
               };

               return originalWhen.call($routeProvider, path, route);
           };
           //#region routes
           $routeProvider.
           //#region cloudents
           when('/dashboard/', {
               params: {
                   type: 'dashboard'
               },
               templateUrl: '/dashboard/indexpartial/'
           }).
           when('/box/my/:boxId/:boxName/', {
               params: {
                   type: 'box',
                   isPrivate: true
               },
               templateUrl: '/box/indexpartial/',
               reloadOnSearch: false
           }).
           when('/box/my/:boxId/:boxName/#:tab?', {
               params: {
                   type: 'box',
                   isPrivate: true
               },
               templateUrl: '/box/indexpartial/',
               reloadOnSearch: false
           }).
           when('/course/:uniName/:boxId/:boxName/#:tab?', {
               params: {
                   type: 'box',
                   isPrivate: false
               },
               templateUrl: '/box/indexpartial/',
               reloadOnSearch: false
           }).
           when('/course/:uniName/:boxId/:boxName/', {
               params: {
                   type: 'box',
                   isPrivate: false
               },
               templateUrl: '/box/indexpartial/',
               reloadOnSearch: false
           }).
           when('/item/:uniName/:boxId/:boxName/:itemId/:itemName/', {
               params: {
                   type: 'item'
               },
               templateUrl: '/item/indexpartial/',
               reloadOnSearch: false

           }).
           when('/item/:uniName/:boxId/:boxName/:itemId/:itemName/#fullscreen', {
               params: {
                   type: 'item'
               },
               templateUrl: '/item/indexpartial/',
               reloadOnSearch: false

           }).
           when('/quiz/:uniName/:boxId/:boxName/:quizId/:quizName/', {
               params: {
                   type: 'quiz'
               },
               templateUrl: '/quiz/indexpartial/'
           }).
           when('/library/:libraryId/:libraryName/', {
               params: {
                   type: 'library'
               },
               templateUrl: '/library/indexpartial/'
           }).
           when('/library/choose/', {
               params: {
                   type: 'libraryChoose'
               },
               templateUrl: '/library/choosepartial/'
           }).
           when('/library/', {
               params: {
                   type: 'library'
               },
               templateUrl: '/library/indexpartial/'
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
               templateUrl: '/user/indexpartial'
           }).
           when('/account/settings', {
               params: {
                   type: 'accountSettings'
               },
               templateUrl: '/account/settings'
           }).
           //#endregion
           //#region store
                when('/store/', {
                    templateUrl: function (params) {
                        var url = '/store/indexpartial/';
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
                       var url = '/store/indexpartial/';
                       return buildQueryString(url, params);
                   },
                   controller: 'CategoryCtrl',
                   params: {
                       type: 'products'
                   }
               }).
               when('/store/product/:productId/:productName/', {
                   templateUrl: function (params) {
                       var url = '/store/productpartial/';
                       return buildQueryString(url, params);
                   },
                   controller: 'ProductCtrl',
                   params: {
                       type: 'product'
                   }
               }).
               when('/store/about/', {
                   templateUrl: function (params) {
                       var url = '/store/aboutpartial/';
                       return buildQueryString(url, params);

                   },
                   controller: 'ViewCtrl',
                   params: {
                       type: 'about'
                   }
               }).
               when('/store/contact/', {
                   templateUrl: function (params) {
                       var url = '/store/contactpartial/';
                       return buildQueryString(url, params);

                   },
                   controller: 'ContactCtrl',
                   params: {
                       type: 'contact'
                   }
               }).
               when('/store/checkout/:productId/', {
                   templateUrl: function (params) {
                       var url = '/store/checkoutpartial/';
                       return buildQueryString(url, params);
                   },

                   controller: 'CheckoutCtrl',
                   params: {
                       type: 'checkout'
                   }
               }).
               when('/store/terms/', {
                   templateUrl: function (params) {
                       var url = '/store/termspartial/';
                       return buildQueryString(url, params);

                   },
                   controller: 'ViewCtrl',
                   params: {
                       type: 'terms'
                   }
               }).
               when('/store/thankyou/', {
                   templateUrl: '/store/thankyoupartial/',
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
                   }
               };
           }]);

           //#endregion
       }
   ]);

    app.run(['$rootScope', '$window', '$location', 'sUserDetails', 'sNewUpdates', 'sVerChecker', '$angularCacheFactory', '$http', '$templateCache', function ($rootScope, $window, $location, sUserDetails, sNewUpdates, sVerChecker, $angularCacheFactory, $http, $templateCache) {


        //analytics
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');



        //ga('require', 'displayfeatures');
        //if (location.pathname.indexOf('account')) {
        //    ga('send', 'pageview');
        //}
 
        sVerChecker.checkVersion();
        $rootScope.$on('$routeChangeStart', function (event, next) {
            $window.scrollTo(0, 0);
        });

        $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {

            //title 
            if (!previous) { //no previous firsttime load

                ga('create', 'UA-9850006-3', {
                    'userId': sUserDetails.getDetails().id,
                    'siteSpeedSampleRate': 70,
                    'cookieDomain': 'cloudents.com',
                    'alwaysSendReferrer': true
                });

                ga('set', 'dimension3', sUserDetails.getDetails().id);

                try {
                    if (current.$$route.params.type === 'box') {
                        if (sUserDetails.isAuthenticated()) {
                            sNewUpdates.removeUpdates(current.params.boxId);
                        }

                        setBackDashboard();
                    }
                }
                catch (ex) {

                }
                return;
            }

            try {
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
                        case 'search':
                            $rootScope.back.title = 'Search "' + previous.params.q + '"';
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
            }
            catch (ex) {

            }
        });



        var htmlCache = $angularCacheFactory('htmlCache', {
            maxAge: 2592000000,
            deleteOnExpire: 'aggressive',            
            storageMode: 'localStorage'
        });
                
        $http.get('/dashboard/indexpartial/', { cache: htmlCache }).then(function (res) { putInCache('/dashboard/indexpartial/', res); } );
        $http.get('/box/IndexPartial/', { cache: htmlCache }).then(function (res) { putInCache('/box/IndexPartial/', res); });
        $http.get('/item/IndexPartial/', { cache: htmlCache }).then(function (res) { putInCache('/item/IndexPartial/', res); });
        $http.get('/quiz/IndexPartial/', { cache: htmlCache }).then(function (res) { putInCache('/quiz/IndexPartial/', res); });
        $http.get('/library/IndexPartial/', { cache: htmlCache }).then(function (res) { putInCache('/library/IndexPartial/', res); });

        function setBackDashboard() {
            $rootScope.back.url = '/dashboard/';
            $rootScope.back.title = 'Dashboard';
        }

        function putInCache(key,res) {
            $templateCache.put(key, res.data);
        }
    }]);
}(window.angular));
