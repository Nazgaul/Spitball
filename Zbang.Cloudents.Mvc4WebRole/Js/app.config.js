(function () {
    "use strict";

    angular.module('app').config(config);

    config.$inject = ['$controllerProvider', '$locationProvider', '$angularCacheFactoryProvider', '$provide'];

    function config($controllerProvider, $locationProvider, $angularCacheFactoryProvider, $provide) {
        //$locationProvider.html5Mode(true).hashPrefix('!');
        $controllerProvider.allowGlobals();

        $angularCacheFactoryProvider.setCacheDefaults({
            maxAge: 45000, //45 seconds
            deleteOnExpire: 'aggressive',
            recycleFreq: 45000,
            cacheFlushInterval: 45000,
            storageMode: 'sessionStorage'
        });


        $provide.factory('requestinterceptor', [function () {

            return {
                'request': function (c) {
                    return c;
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
                        case 412:
                            alert('Spitball has updated, refreshing page');
                            window.location.reload(true);
                            break;
                        case 401:
                        case 403:
                            window.open('/account/', '_self');
                            break;
                        case 404:
                            window.open('/error/', '_self');
                            break;
                        case 500:
                            window.open('/error/', '_self');
                            break;
                        default:
                            // somehow firefox in incognito crash and transfer to error page
                            //   window.open('/error/', '_self');
                            break;

                    }

                    return response;
                }
            };
        }]);
    }
    
})();