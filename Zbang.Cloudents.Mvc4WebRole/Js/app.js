/// <reference path="../Scripts/angular.js" />
(function () {
    angular.module('app', [
        'ui.router',
        'angulartics',
        
        'angulartics.google.analytics',
        'app.userdetails',
        'app.dashboard'
    ]).config(config);

    config.$inject = ['$controllerProvider', '$locationProvider'];

    function config($controllerProvider, $locationProvider) {
        $locationProvider.html5Mode(true).hashPrefix('!');
        $controllerProvider.allowGlobals();
    }
})();