angular.module('app').config(
    ['$stateProvider', '$locationProvider', '$urlRouterProvider',
    function ($stateProvider, $locationProvider, $urlRouterProvider) {
        $locationProvider.html5Mode(true).hashPrefix('!');

        $stateProvider
            .state('account', {
                url: '/account/',
                templateUrl: '/account/indexpartial/',
                controller: 'AccountController as account'
            }).
            state('login', {
                url: '/account/login/',
                templateUrl: '/account/loginpartial/',
                controller: 'LoginController as login'
            }).
            state('register', {
                url: '/account/register/',
                templateUrl: '/account/registerpartial/',
                controller: 'RegisterController as register'
            }).
            state('libChoose', {
                url: '/library/choose/',
                templateUrl: '/library/choosepartial/',
                controller: 'LibChooseController as libChoose'
            }).
            state('dashboard', {
                url: '/dashboard/',
                templateUrl: '/dashboard/indexpartial/',
                controller: 'DashboardController as dashboard'
            }).


            state('box', {
                url: '/box/my/:boxId/:boxName/',
                templateUrl: '/box/indexpartial/',
                controller: 'BoxController as box'
            }).
            state('course', {
                url: '/course/:uniName/:boxId/:boxName/',
                templateUrl: '/box/indexpartial/',
                controller: 'BoxController as box'
            }).
             state('search', {
                 url: '/search/',
                 templateUrl: '/search/indexpartail/',
                 controller: 'SearchController as search'
             });

        $urlRouterProvider.otherwise('/list/');
    }
    ]);