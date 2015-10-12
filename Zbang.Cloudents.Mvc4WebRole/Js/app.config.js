(function () {
    "use strict";

    angular.module('app').config(config);

    config.$inject = ['$controllerProvider', '$locationProvider', '$angularCacheFactoryProvider'];

    function config($controllerProvider, $locationProvider, $angularCacheFactoryProvider) {
        //$locationProvider.html5Mode(true).hashPrefix('!');
        $controllerProvider.allowGlobals();

        $angularCacheFactoryProvider.setCacheDefaults({
            maxAge: 45000, //45 seconds
            deleteOnExpire: 'aggressive',
            recycleFreq: 45000,
            cacheFlushInterval: 45000,
            storageMode: 'sessionStorage'
        });
    }
    
})();