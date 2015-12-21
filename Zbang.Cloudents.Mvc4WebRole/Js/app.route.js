﻿
(function () {
    angular.module('app').config(config);
    config.$inject = ['$stateProvider'];
    function config($stateProvider) {

        $stateProvider
            .state('root', {
                abstract: true,
                resolve: {
                    user: [
                        'userDetailsFactory', function (userDetails) {
                            return userDetails.init();
                        }
                    ]
                },
                template: '<div ui-view animation-class></div>'
            });
    }
})();


(function () {
    angular.module('app').run(appRun);

    appRun.$inject = ['routerHelper'];
    function appRun(routerHelper) {
        routerHelper.configureStates(getStates());


        function getStates() {
            return [
                {
                    state: 'jobs',
                    config: {
                        url: '/jobs/',
                        data: {
                            staticPage : true
                        }
                    },
                    templateUrl: '/home/jobs/'
                },
                 {
                     state: 'blog',
                     config: {
                         url: '/blog/',
                         data: {
                             staticPage: true
                         }
                     },
                     templateUrl: '/home/blog/'
                 },
                  {
                      state: 'help',
                      config: {
                          url: '/help/',
                          data: {
                              staticPage: true
                          }
                      },
                      templateUrl: '/home/helppartial/'
                  },
                   {
                       state: 'item',
                       config: {
                           url: '/item/{universityName}/{boxId}/{boxName}/{itemId}/{itemName}/',
                           controller: 'ItemController as i'
                       },
                       templateUrl: '/item/indexpartial/'
                   },
                    {
                        state: 'search',
                        config: {
                            url: '/search/?q',
                            controller: 'SearchController as s',
                            data: { animateClass: 'search' },
                            reloadOnSearch: false
                            //onEnter: routerHelper.universityRedirect
                        },
                        templateUrl: '/search/indexpartial/'
                    },
                     {
                         state: 'dashboard',
                         config: {
                             url: '/dashboard/',
                             controller: 'Dashboard as d',
                             data: { animateClass: 'dashboard' },
                             //onEnter: routerHelper.universityRedirect

                         },
                         templateUrl: '/dashboard/indexpartial/'
                     }
            ];
        }
    }
})();