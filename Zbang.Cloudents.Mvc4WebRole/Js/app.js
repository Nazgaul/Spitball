'use strict';
var app;
(function () {
    "use strict";
    app = angular.module('app', [
         'ui.router',
         'ct.ui.router.extras',
         //'ct.ui.router.extras.previous',
         'ngSanitize',
         'ui.bootstrap',
         'angular-google-analytics',
         'ngDfp',
         'ngAnimate',
         'ngMessages',
         'ngMaterial',
         //'ngEmbed',
         'textAngular',
         'ang-drag-drop',
         'angular-plupload',
         'countUpModule',
         //'infinite-scroll',
         'srph.infinite-scroll',
         //'jkuri.slimscroll',
         'ngScrollbars',
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
