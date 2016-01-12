
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
                template: '<div class="page-animation" ui-view animation-class></div>'
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
                           controller: 'ItemController as i'
                       },
                       templateUrl: '/item/indexpartial/'
                   },
                    {
                        state: 'searchWithQuery',
                        config: {
                            url: '/search/?q',
                            controller: 'SearchController as s',
                            data: { animateClass: 'search full-screen' },
                            reloadOnSearch: false
                            //onEnter: routerHelper.universityRedirect
                        },
                        templateUrl: '/search/indexpartial/'
                    },
                    {
                        state: 'search',
                        config: {
                            url: '/search/',
                            controller: 'SearchController as s',
                            data: { animateClass: 'search full-screen' },
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