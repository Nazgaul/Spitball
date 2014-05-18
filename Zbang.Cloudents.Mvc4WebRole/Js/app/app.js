var appRoot = angular.module('main', ['mDashboard']);
cd.$scope = {};
//appRoot.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
//    //$locationProvider.html5Mode(true)
//    //                .hashPrefix('!');
   
//    //$routeProvider.when('/home', { templateUrl: '/home/main', controller: 'mainController' })
//    //.otherwise({ redirectTo: '/home' });

//}]);

appRoot.controller('MainController', ['$scope','$compile', function ($scope,$compile) {
    cd.$scope.main = $scope;
    cd.$compile = $compile;    
}]);