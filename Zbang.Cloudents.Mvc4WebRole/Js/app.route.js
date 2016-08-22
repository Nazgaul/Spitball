'use strict';
(function () {
    angular.module('app').config(config);
    config.$inject = ['$stateProvider'];
    function config($stateProvider) {

        $stateProvider
            .state("root", {
                abstract: true,
                resolve: {
                    user: [
                        "$q", "userDetailsFactory", function ($q, userDetails) {
                            //$q https://github.com/angular-ui/ui-router/issues/105
                            return userDetails.init();
                        }
                    ]
                },
                views: {
                    "": { template: "<div class=\"page-animation\" ui-view animation-class></div>" },
                    "user-profile": {
                        controller: "UserDetailsController as ud",
                        templateProvider: ['user', 'ajaxService2', function (user, ajaxService2) {
                            if (user.id) {
                                return ajaxService2.getHtml("account/userdetails");// '<div>hello ram</div>';
                            } else {
                                return ajaxService2.getHtml("account/unregisterview");
                            }
                        }]
                    },
                    "chat": {
                        controller: "ChatController as c",
                        templateProvider: [
                            'user', 'ajaxService2', function (user, ajaxService2) {
                                if (user.id && user.university.id > 0) {
                                    return ajaxService2.getHtml("chat/index");
                                }
                            }
                        ]
                    },
                    "search-bar": {
                        controller: "SearchTriggerController as st",
                        //template: '<div>hi</div>'
                        templateUrl: "search-bar.html"
                    },
                    "menu": {
                        controller: "SideMenu as d",
                        templateUrl: "menu.html"
                    }
                }
                //template: '<div class="page-animation" ui-view animation-class></div>'
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
                           url: '/item/{universityName}/{boxId:int}/{boxName}/{itemId:int}/{itemName}/',
                           controller: 'ItemController as i',
                           resolve: {
                               itemData: ['itemService', '$stateParams', 'sbHistory', function (itemService, $stateParams, sbHistory) {
                                   return itemService.getDetails($stateParams.boxId, $stateParams.itemId, sbHistory.firstState());
                               }]
                           },
                           reloadOnSearch: false,
                           data: { animateClass: 'itemPage' },
                           views: {
                               "menu@": {
                                   template: ''
                               }

                           }
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