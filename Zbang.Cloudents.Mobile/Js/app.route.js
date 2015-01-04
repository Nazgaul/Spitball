angular.module('app').config(
    ['$stateProvider', '$locationProvider', '$urlRouterProvider',
    function ($stateProvider, $locationProvider, $urlRouterProvider) {
        $locationProvider.html5Mode(true).hashPrefix('!');

        $stateProvider
            .state('root', {
                abstract: true,
                controller: ['user', function (user) {
                    var root = this;
                }],
                controllerAs: 'root',
                template: '<div ui-view></div>',
                resolve: {
                    user: function (account) {
                        return account.details();
                    }
                }
            })
            .state('root.account', {
                url: '/account/',
                templateUrl: '/account/indexpartial/',
                controller: 'AccountController as account'
            }).
            state('root.login', {
                url: '/account/login/',
                templateUrl: '/account/loginpartial/',
                controller: 'LoginController as login'
            }).
            state('root.register', {
                url: '/account/register/',
                templateUrl: '/account/registerpartial/',
                controller: 'RegisterController as register'
            }).
            state('root.libChoose', {
                url: '/library/choose/',
                templateUrl: '/library/choosepartial/',
                controller: 'LibChooseController as libChoose'
            }).
            state('root.dashboard', {
                url: '/dashboard/',
                templateUrl: '/dashboard/indexpartial/',
                controller: 'DashboardController as dashboard',
                onEnter: function (account) {
                    console.log(account);
                }
            }).


            state('root.box', {
                url: '/box/my/:boxId/:boxName/',
                templateUrl: '/box/indexpartial/',
                controller: 'BoxController as box'
            }).
            state('root.course', {
                url: '/course/:uniName/:boxId/:boxName/',
                templateUrl: '/box/indexpartial/',
                controller: 'BoxController as box'
            })
        //state('search', {
        //    url: '/search/',
        //    templateUrl: '/search/indexpartail/',
        //    controller: 'SearchController as search'
        //});

        $urlRouterProvider.otherwise('/dashboard/');
    }
    ]);