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
         
         'countUpModule',
         'srph.infinite-scroll',
         'ngScrollbars',
         'yaru22.angular-timeago',
         "oc.lazyLoad",
         'app.dashboard',
         'app.library',
         'app.user',
         'app.account',
         //'app.box',
         'app.item',
         'app.quiz',
         //'app.upload',
         'app.search',
         'app.chat'
    ]);
    
})();
