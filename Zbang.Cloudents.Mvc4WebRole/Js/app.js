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


    MetronicApp.controller('AppController', ['$scope', '$rootScope', function ($scope, $rootScope) {
        $scope.$on('$viewContentLoaded', function () {
            Metronic.initComponents(); // init core components
            //Layout.init(); //  Init entire layout(header, footer, sidebar, etc) on page load if the partials included in server side instead of loading with ng-include directive 
        });
    }]);

})();