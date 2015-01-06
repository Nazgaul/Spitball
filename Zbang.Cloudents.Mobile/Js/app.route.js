﻿angular.module('app').config(
    ['$stateProvider', '$locationProvider', '$urlRouterProvider',
    function ($stateProvider, $locationProvider, $urlRouterProvider) {

        var accountRedirect = ['$state', 'userDetails', function ($state, userDetails) {
            if (userDetails.isAuthenticated()) {
                var uniId = userDetails.getUniversityId();
                if (!uniId) {
                    $state.go('root.libChoose');
                    return;
                }

                $state.go('root.dashboard');
            }
        }];

        var dashboardRedirect = ['$state', 'userDetails', function ($state, userDetails) {
            if (!userDetails.isAuthenticated()) {
                $state.go('root.account');
                return;
            }

            var uniId = userDetails.getUniversityId();
            if (!uniId) {
                $state.go('root.libChoose');
                return;
            }
        }];

        $locationProvider.html5Mode(true).hashPrefix('!');

        $stateProvider
            .state('root', {
                abstract: true,
                controller: ['userDetails', function (userDetails) {
                    var root = this;
                    root.user = {
                        name: userDetails.getName(),
                        image: userDetails.getImage(),
                        isAuthenticated: userDetails.isAuthenticated(),
                        isAdmin: userDetails.isAdmin()
                    };
                }],
                controllerAs: 'root',
                template: '<div ui-view"></div>',
                resolve: {
                    user: ['userDetails', function (userDetails) {
                        return userDetails.init();
                    }]
                }
            })
            .state('root.account', {
                url: '/account/',
                templateUrl: '/account/indexpartial/',
                controller: 'AccountController as account',
                onEnter: accountRedirect
            }).
            state('root.login', {
                url: '/account/login/',
                templateUrl: '/account/loginpartial/',
                controller: 'LoginController as login',
                onEnter: accountRedirect

            }).
            state('root.register', {
                url: '/account/register/',
                templateUrl: '/account/registerpartial/',
                controller: 'RegisterController as register',
                onEnter: accountRedirect
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
                onEnter: dashboardRedirect
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
            }).
            state('root.search', {
                url: '/search/',
                templateUrl: '/search/indexpartial/',
                controller: 'SearchController as search'
            });

        $urlRouterProvider.otherwise('/dashboard/').

        rule(function ($injector, $location) {

            var path = $location.path();
            var hasTrailingSlash = path[path.length - 1] === '/';

            if (!hasTrailingSlash) {
                //if last charcter is a slash, return the same url without the slash  
                var newPath = path + '/';
                return newPath;
            }
        });
    }
    ]);