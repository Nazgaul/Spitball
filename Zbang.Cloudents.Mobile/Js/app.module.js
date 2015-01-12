"use strict";
angular.module('app', ['ui.router', 'jmdobry.angular-cache', 'angulartics', 'angular-gestures', 'ngTouch', 'facebook', 'scroll', 'app.events','ngLocale',
    'account', 'register', 'login', 'box', 'dashboard', 'libChoose', 'search',
    'angulartics.google.analytics', 'ngAnimate', 'ngSanitize', 'stackTrace']).
config([
   '$httpProvider',
   '$provide',
   '$angularCacheFactoryProvider',
   'FacebookProvider',
   function ($httpProvider, $provide, $angularCacheFactoryProvider, FacebookProvider) {
       $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';

       $angularCacheFactoryProvider.setCacheDefaults({
           maxAge: 1500000, //25 minutes
           deleteOnExpire: 'aggressive',
           recycleFreq: 30000,
           storageMode: 'sessionStorage'
       });

       FacebookProvider.setAppId(450314258355338);
       FacebookProvider.setCookie(true);
       FacebookProvider.setXfbml(true);
       FacebookProvider.setStatus(true);
       FacebookProvider.setSdkVersion('v2.2');
       FacebookProvider.setLoadSDK(true);


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
]).
run(['$rootScope', '$window', 'sVerChecker', function ($rootScope, $window, sVerChecker) {


    sVerChecker.checkVersion();

    angular.element($window).on('beforeunload', function () {
        $window.scrollTo(0, 0);
    });

    $rootScope.$on('$stateChangeStart', function () {
        setTimeout(function () { window.scrollTo(0, 0); }, 0);
    });

    //analytics
    (function (i, s, o, g, r, a, m) {
        i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
            (i[r].q = i[r].q || []).push(arguments)
        }, i[r].l = 1 * new Date(); a = s.createElement(o),
        m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
    })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

}]);
