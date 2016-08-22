'use strict';
var app;
(function () {
    "use strict";
    app = angular.module('app', [
         'ui.router',
         'ngSanitize',
         'ui.bootstrap',
         'angular-google-analytics',
         'ngDfp',
         'ngAnimate',
         'ngMessages',
         'ngMaterial',
         'ang-drag-drop',
         'angular-plupload',
         'countUpModule',
         'srph.infinite-scroll',
         'ngScrollbars',
         'yaru22.angular-timeago',
         'app.dashboard',
         'app.library',
         'app.user',
         'app.account',
         'app.box',
         'app.item',
         'app.quiz',
         'app.quiz.create',
         'app.upload',
         'app.search',
         'app.chat'
    ]);
    //]).config(config);

    //config.$inject = ['$controllerProvider', '$locationProvider'];

    //function config($controllerProvider, $locationProvider) {
    //    $locationProvider.html5Mode(true).hashPrefix('#');
    //    //$controllerProvider.allowGlobals();
    //}


})();
