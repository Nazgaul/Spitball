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
                template: '/account/login',
                controller: 'LoginCtrl as login'
            }).
            state('register', {
                url: '/account/register/',
                templateUrl: '/account/register',
                controller: 'RegisterController as register'
            }).
            state('libChoose', {
                url: '/register/',
                template: '/account/index',
                controller: 'LibChooseCtrl as libChoose'
            }).
            state('dashboard', {
                url: '/dashboard/',
                template: '/dashboard/index',
                controller: 'DashboardCtrl as dashboard'
            }).
            state('box', {
                url: '/box/',
                template: '/box/index',
                controller: 'BoxCtrl as box'
            }).
             state('search', {
                 url: '/search/',
                 template: '/search/index',
                 controller: 'SearchCtrl as search'
             });

        $urlRouterProvider.otherwise('/list/');
    }
    ]);