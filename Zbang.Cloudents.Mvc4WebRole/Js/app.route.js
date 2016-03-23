
(function () {
    angular.module('app').config(config);
    config.$inject = ['$stateProvider'];
    function config($stateProvider) {

        $stateProvider
            .state('root', {
                abstract: true,
                resolve: {
                    
                    user: [
                        '$q', 'userDetailsFactory', function ($q, userDetails) {
                            //$q https://github.com/angular-ui/ui-router/issues/105
                            return userDetails.init();
                        }
                    ],
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
                         url: '/aboutus/',
                         data: {
                             staticPage: true
                         }
                     },
                     templateUrl: '/home/aboutuspartial/'
                 },
                 {
                     state: 'privacy',
                     config: {
                         url: '/privacy/',
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
                  //{
                  //    state: 'help',
                  //    config: {
                  //        url: '/help/',
                  //        data: {
                  //            staticPage: true
                  //        }
                  //    },
                  //    templateUrl: '/home/helppartial/'
                  //},
                   {
                       state: 'item',
                       config: {
                           url: '/item/{universityName}/{boxId}/{boxName}/{itemId}/{itemName}/',
                           controller: 'ItemController as i',
                           resolve: {
                               itemData: ['itemService', '$stateParams', 'history', function (itemService, $stateParams, history2) {
                                   return itemService.getDetails($stateParams.boxId, $stateParams.itemId, history2.firstState());
                               }]
                           },
                           data: { animateClass: 'itemPage' }
                       },
                       templateUrl: '/item/indexpartial/'
                   },
                    {
                        state: 'searchinfo',
                        config: {
                            url: '/search/?q&t',
                            controller: 'SearchController as s',
                            data: { animateClass: 'search' },
                            reloadOnSearch: false,
                            //onEnter: routerHelper.universityRedirect
                        },
                        templateUrl: '/search/indexpartial/'
                    }
            ];
        }
    }
})();