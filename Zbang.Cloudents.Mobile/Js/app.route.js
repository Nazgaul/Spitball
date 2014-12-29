angular.module('app').config(
    ['$stateProvider', '$locationProvider', '$urlRouterProvider',
    function ($stateProvider, $locationProvider, $urlRouterProvider) {
        $locationProvider.html5Mode(true).hashPrefix('!');

        $stateProvider
            .state('account', {
                url: '/account/',
                template: '/account/index',
                controller: 'AccountController as account'
            }).
            state('login', {
                url: '/account/login/',
                templateUrl: '/account/loginpartial/',
                controller: 'LoginController as login'
            }).
            state('register', {
                url: '/account/register/',
                templateUrl: '/account/registerpartial',
                controller: 'RegisterController as register'
            }).
            state('libChoose', {
                url: '/library/choose',
                template: '/library/choosepartial',
                controller: 'LibChooseController as libChoose'
            }).
            state('dashboard', {
                url: '/dashboard/',
                template: '/dashboard/indexpartial',
                controller: 'DashboardController as dashboard'
            }).


            state('box', {
                url: '/box/my/:boxId/:boxName/',
                template: '/box/indexpartial',
                controller: 'BoxController as box'
            }).
            state('course', {
                url: '/course/:uniName/:boxId/:boxName/',
                template: '/box/indexpartial',
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