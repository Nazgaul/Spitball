
(function () {
    angular.module('app').config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {

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
                template: '<div class="page-animation" ui-view animation-class></div>'
            });
        //$urlRouterProvider.otherwise('/dashboard/');
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
                            staticPage: true
                        }
                    },
                    templateUrl: '/home/jobs/'
                },
                 {
                     state: 'abousUs',
                     config: {
                         url: '/home/aboutus/',
                         data: {
                             staticPage: true
                         }
                     },
                     templateUrl: '/home/aboutuspartial/'
                 },
                 {
                     state: 'privacy',
                     config: {
                         url: '/home/privacy/',
                         data: {
                             staticPage: true
                         }
                     },
                     templateUrl: '/home/privacypartial/'
                 },
                 {
                     state: 'terms',
                     config: {
                         url: '/terms/',
                         data: {
                             staticPage: true
                         }
                     },
                     templateUrl: '/home/termspartial/'
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
                           controller: 'ItemController as i',
                           resolve: {
                               itemData: ['itemService', '$stateParams', '$rootScope', function (itemService, $stateParams) {
                                   return itemService.getDetails($stateParams.boxId, $stateParams.itemId);
                               }]
                           },
                       },
                       templateUrl: '/item/indexpartial/'
                   },
                    {
                        state: 'search',
                        config: {
                            url: '/search/',
                            controller: 'SearchController as s',
                            data: { animateClass: 'search' },
                            reloadOnSearch: false
                            //onEnter: routerHelper.universityRedirect
                        },
                        templateUrl: '/search/indexpartial/'
                    },
                    //{
                    //    state: 'search',
                    //    config: {
                    //        url: '/search/',
                    //        controller: 'SearchController as s',
                    //        data: { animateClass: 'search full-screen' },
                    //        reloadOnSearch: false
                    //        //onEnter: routerHelper.universityRedirect
                    //    },
                    //    templateUrl: '/search/indexpartial/'
                    //},
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