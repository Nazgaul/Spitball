var app = angular.module('app', ['ngRoute', 'ngSanitize', 'infinite-scroll', 'pasvaz.bindonce', 'ui.bootstrap', 'ngAnimate']);

app.config([
    '$routeProvider',
    '$locationProvider',
    '$httpProvider',
    '$tooltipProvider',
    '$provide',

    function ($routeProvider, $locationProvider,$httpProvider, $provide) {

   
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';



        $provide.factory('requestinterceptor', ['$location', '$q', function ($location, $q) {
            return {
                // optional method
                'response': function (response) {
                    // do something on success
                    switch (response.status) {
                        case 200:
                            return response;
                        case 401:
                        case 403:
                            window.open('/account', '_self');
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
     
        $locationProvider.html5Mode(true).hashPrefix('!');

        //#region routes
        $routeProvider.
        when('/shopping/', {            
            templateUrl: '/shopping/',
            controller: 'homeCtrl'
        }).
        //when('/box/my/:boxId/:boxName/', {
        //    params: {
        //        type: 'box'
        //    },
        //    templateUrl: function (params) { return '/box/my/' + params.boxId + '/' + encodeURIComponent(params.boxName) + '/'; }
        //}).
        otherwise({ redirectTo: '/shopping/' });

        //#endregion


        //#region log js errors 
        $provide.decorator('$exceptionHandler', ['$delegate','$log', 'stackTraceService', function ($delegate,$log, stackTraceService) {
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
                    })

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

app.run(['$rootScope', '$window', 'sUserDetails', 'sNewUpdates', function ($rootScope, $window, sUserDetails, sNewUpdates) {
    $rootScope.initDetails = function (id, name, image, score, url) {

        if (id) {
            sUserDetails.setDetails(id, name, image, score, url);
            sNewUpdates.loadUpdates();
            return;
        }
        sUserDetails.setDetails(null, '', $('body').data('pic'), 0, null);

    };

    $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
    });

}]);

