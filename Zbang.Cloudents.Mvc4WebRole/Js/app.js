/// <reference path="../Scripts/angular.js" />
(function () {
    "use strict";
    //TODO: download ui bootstap only what needed
    angular.module('app', [
        'ui.router',
        'ngSanitize',
        'angulartics',
        'ui.bootstrap',
        'ngAnimate',
        'ngMaterial',
        'textAngular',
        'angulartics.google.analytics',
        'angular-plupload',
        'countUpModule',
        'infinite-scroll',
        'displayTime',
        'app.dashboard',
        'app.library',
        'app.user',
        'app.account',
        'app.box',
        'app.item',
        'app.quiz',
        'app.upload',
        'app.search'
    ]);
    //]).config(config);

    //config.$inject = ['$controllerProvider', '$locationProvider'];

    //function config($controllerProvider, $locationProvider) {
    //    $locationProvider.html5Mode(true).hashPrefix('#');
    //    //$controllerProvider.allowGlobals();
    //}


})();