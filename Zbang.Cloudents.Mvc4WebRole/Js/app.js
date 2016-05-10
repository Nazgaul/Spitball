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
         'textAngular',
         'ang-drag-drop',
         'angular-plupload',
         'countUpModule',
         //'infinite-scroll',
         'srph.infinite-scroll',
         'jkuri.slimscroll',
         'yaru22.angular-timeago',
        // 'displayTime',
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
