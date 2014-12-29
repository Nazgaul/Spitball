angular.module('app').config(
    ['$stateProvider', '$locationProvider', '$urlRouterProvider',
    function ($stateProvider, $locationProvider, $urlRouterProvider) {
        $locationProvider.html5Mode(true).hashPrefix('!');

        $stateProvider
            .state('account', {
                url: '/account/',
                template: '/account/index',
                //controller: 'AccountCtrl as account'
            }).
            state('login', {
                url: '/login/',
                templateUrl: '/account/login/',
                controller: 'LoginController as login'
            }).
            state('register', {
                url: '/account/register/',
                templateUrl: '/account/register',
                controller: 'RegisterController as register'
            }).
            state('libChoose', {
                url: '/register/',
                template: '/account/index',
                controller: 'LibChooseController as libChoose'
            }).
            state('dashboard', {
                url: '/dashboard/',
                template: '/dashboard/index',
                controller: 'DashboardController as dashboard'
            }).
            state('box', {
                url: '/box/',
                template: '/box/index',
                controller: 'BoxController as box'
            }).
             state('search', {
                 url: '/search/',
                 template: '/search/index',
                 controller: 'SearchController as search'
             });

        $urlRouterProvider.otherwise('/list/');
    }
    ]);