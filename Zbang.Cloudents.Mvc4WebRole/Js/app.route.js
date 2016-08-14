'use strict';
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
                       state: 'item',
                       config: {
                           url: '/item/{universityName}/{boxId}/{boxName}/{itemId}/{itemName}/',
                           controller: 'ItemController as i',
                           resolve: {
                               itemData: ['itemService', '$stateParams', 'sbHistory', function (itemService, $stateParams, sbHistory) {
                                   return itemService.getDetails($stateParams.boxId, $stateParams.itemId, sbHistory.firstState());
                               }]
                           },
                           reloadOnSearch: false,
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
                            reloadOnSearch: false
                        },
                        templateUrl: '/search/indexpartial/'
                    }
            ];
        }
    }
})();