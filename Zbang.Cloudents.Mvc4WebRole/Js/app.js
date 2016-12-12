var app;
(function () {
    "use strict";
    app = angular.module('app', [
         'ui.router',
         'ngSanitize',
         'ui.bootstrap',
         'angular-google-analytics',
         "angular-google-adsense",
         //'ngDfp',
         'ngAnimate',
         'ngMessages',
         'ngMaterial',
          'ang-drag-drop',
         'angular-plupload',
         'countUpModule',
         'srph.infinite-scroll',
         'ngScrollbars',
         'yaru22.angular-timeago',
         "oc.lazyLoad",
         "vs-repeat",
         'app.dashboard',
         'app.library',
         'app.user',
         'app.account',
         'app.box',
         'app.item',
         'app.quiz',
         'app.upload',
         'app.search',
         'app.chat',
         "app.flashcard",
         "ngclipboard",
         "ApplicationInsightsModule"
    ]);

})();
