/// <reference path="../Scripts/angular.js" />
(function () {
    angular.module('app', [
        'ui.router',
        'ngSanitize',
        'angulartics',
        'ui.bootstrap',
        
        'angulartics.google.analytics',
        'app.userdetails',
        'app.dashboard',
        'app.box',
        'app.user',
        'app.box.feed',
        'app.box.items'
    ]).config(config);

    config.$inject = ['$controllerProvider', '$locationProvider'];

    function config($controllerProvider, $locationProvider) {
        $locationProvider.html5Mode(true).hashPrefix('!');
        $controllerProvider.allowGlobals();
    }


  

})();