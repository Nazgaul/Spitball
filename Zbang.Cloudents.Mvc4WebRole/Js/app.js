﻿/// <reference path="../Scripts/angular.js" />
(function () {
    angular.module('app', [
        'ui.router',
        'ngSanitize',
        'angulartics',
        'ui.bootstrap',
        'ngMaterial',
        'angulartics.google.analytics',
        'angular-plupload',
        
        'app.dashboard',

        'app.library',
        
        'app.user',
        'app.user.details',
        'app.user.account',

        'app.box',
        'app.box.feed',
        'app.box.items',

        'app.item'
    ]).config(config);

    config.$inject = ['$controllerProvider', '$locationProvider'];

    function config($controllerProvider, $locationProvider) {
        $locationProvider.html5Mode(true).hashPrefix('!');
        $controllerProvider.allowGlobals();
    }


  

})();