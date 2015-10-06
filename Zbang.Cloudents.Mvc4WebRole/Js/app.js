﻿/// <reference path="../Scripts/angular.js" />
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
        'angulartics.google.analytics',
        'angular-plupload',
        'infinite-scroll',
        'app.dashboard',
        'app.library',
        'app.user',
        //'app.user.details',
        //'app.user.account',
        'app.box',
        // 'app.box.feed',
        //'app.box.items',
        //'app.box.members',
        'app.item',
        'app.quiz',
        'app.upload'
    ]);
    //]).config(config);

    //config.$inject = ['$controllerProvider', '$locationProvider'];

    //function config($controllerProvider, $locationProvider) {
    //    $locationProvider.html5Mode(true).hashPrefix('#');
    //    //$controllerProvider.allowGlobals();
    //}


})();