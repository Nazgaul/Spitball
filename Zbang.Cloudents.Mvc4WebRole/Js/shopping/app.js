var app = angular.module('app', ['ngRoute', 'ngSanitize', 'infinite-scroll', 'pasvaz.bindonce', 'ui.bootstrap', 'ngAnimate', 'Cookie', 'debounce']);

app.config([
    '$routeProvider',
    '$locationProvider',
    '$httpProvider',
    '$provide',

    function ($routeProvider, $locationProvider, $httpProvider, $provide) {


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
                    switch (response.status) {
                        case 401:
                        case 403:
                            window.open('/account', '_self');
                            break;
                        case 404:
                            window.open('/error', '_self');
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
            when('/store/', {
                templateUrl: '/Store/',
                controller: 'CategoryCtrl',
                type: 'home'
            }).
            when('/store/category/:categoryId/:categoryName/', {
                templateUrl: '/Store/',
                controller: 'CategoryCtrl',
                type: 'products'
            }).
            when('/store/product/:productId/:productName/', {
                templateUrl: function (params) { return '/store/product/?id=' + params.productId; },
                //controller: 'ProductCtrl',
                type: 'product'
            }).
            when('/store/about/', {
                templateUrl: '/Store/About',
                controller: 'AboutCtrl',
                type: 'about'
            }).
            when('/store/contact/', {
                templateUrl: '/Store/Contact',
                controller: 'ContactCtrl',
                type: 'contact'
            }).
            when('/store/checkout/:productId', {
                templateUrl: function (params) { return '/Store/Checkout/?id=' + params.productId; },
                controller: 'CheckoutCtrl',
                type: 'checkout'
            }).
        otherwise({ redirectTo: '/store/' });

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

app.run(['$rootScope', '$window', 'sUserDetails', function ($rootScope, $window, sUserDetails) {
    $rootScope.initDetails = function (id, name, image, score, url) {

        if (id) {
            sUserDetails.setDetails(id, name, image, score, url);
            return;
        }
        sUserDetails.setDetails(null, '', $('body').data('pic'), 0, null);

    };

    $rootScope.$on('$routeChangeStart', function () {
        $window.scrollTo(0, 0);
    });
}]);

