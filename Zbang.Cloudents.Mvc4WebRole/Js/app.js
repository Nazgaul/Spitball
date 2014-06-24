//angular.module('main', ['ngRoute','ui.bootstrap', 'apiService', 'main.directives', 'main.filters', 'mDashboard','mBox', 'custom_scrollbar', 'monospaced.elastic', 'QuizCreate']).
//    run(['$compile','NewUpdates', function ($compile,NewUpdates) {
//        NewUpdates.loadUpdates();
//        cd.$compile = $compile;
//    }]).
//    config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
//        $locationProvider.html5Mode(true).hashPrefix('!');   
//        $routeProvider
//            .when('/home', { templateUrl: '/home/main', controller: 'mainController' })

//            .otherwise({ redirectTo: '/dashboard' });

//    }]).
//    controller('MainController', ['$scope', '$compile', function ($scope, $compile) {
//    //cd.$scope.main = $scope;
        
//    }]);

define(['routes', 'services/dependencyResolverFor'], function (config, dependencyResolverFor) {
    var app = angular.module('app', ['ngRoute','ui.bootstrap']);

    app.config([
        '$routeProvider',
        '$locationProvider',
        '$controllerProvider',
        '$compileProvider',
        '$filterProvider',
        '$httpProvider',
        '$provide',

        function ($routeProvider, $locationProvider, $controllerProvider, $compileProvider, $filterProvider, $httpProvider, $provide) {
            app.controller = $controllerProvider.register;
            app.directive = $compileProvider.directive;
            app.filter = $filterProvider.register;
            app.factory = $provide.factory;
            app.service = $provide.service;

            if (window.history && history.pushState) {
                $locationProvider.html5Mode(true).hashPrefix('!');
            } else {
                $locationProvider.hashPrefix('!');
            }
            


            if (config.routes !== undefined) {
                angular.forEach(config.routes, function (route, path) {
                    $routeProvider.when(path, { templateUrl: route.templateUrl, params: route.params, resolve: dependencyResolverFor(route.dependencies) });
                });
            }

            if (config.defaultRoutePath !== undefined) {
                $routeProvider.otherwise({ redirectTo: config.defaultRoutePath });
            }

            $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';

        }
    ]);
    
    app.run(['$rootScope', '$window', function ($rootScope, $window) {
        $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {

            //title 
            if (!previous) {
                return;
            }
            switch (previous.$$route.params.type) {
                case 'dashboard':
                    $rootScope.previousTitle = 'Dashboard';
                    break;
            }            
            console.log(event, current, previous);
        });

        $rootScope.$back = function () {
            $window.history.back();
        };
    }]);

    return app;
});