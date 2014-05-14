var appRoot = angular.module('main', ['ngRoute', 'ngResource', 'main.directives', 'main.services', ]);

appRoot.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    $locationProvider.html5Mode(true)
                    .hashPrefix('!');
   
    $routeProvider.when('/home', { templateUrl: '/home/main', controller: 'mainController' })
    .otherwise({ redirectTo: '/home' });

}]);