angular.module('main', ['ui.bootstrap', 'apiService', 'main.directives', 'main.filters', 'mDashboard', 'custom_scrollbar', 'monospaced.elastic', 'QuizCreate']).
    run(['$compile','NewUpdates', function ($compile,NewUpdates) {
        NewUpdates.loadUpdates();
        cd.$compile = $compile;
    }]).
//appRoot.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
//    //$locationProvider.html5Mode(true)
//    //                .hashPrefix('!');
   
//    //$routeProvider.when('/home', { templateUrl: '/home/main', controller: 'mainController' })
//    //.otherwise({ redirectTo: '/home' });

//}]);
    controller('MainController', ['$scope', '$compile', function ($scope, $compile) {
    //cd.$scope.main = $scope;
        
    }]);