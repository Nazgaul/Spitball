(function () {
    'use strict';
    angular.module('app').config(config);
    config.$inject = ['$stateProvider'];
    function config($stateProvider) {

        $stateProvider
            .state("root", {
                //controller: 'ClassChoose as cc',
                //url: '^',
                abstract: true,
                resolve: {
                    user: [
                        "$q", "userDetailsFactory", function ($q, userDetails) {
                            //$q https://github.com/angular-ui/ui-router/issues/105
                            return userDetails.init();
                        }
                    ],
                    boxes: ["dashboardService", function (dashboardService) {
                        if (window.id > 0) {
                            return dashboardService.getBoxes();

                        }
                    }]
                },
                views: {
                    "": {
                        //controller: [
                        //'user', '$state', 'boxes', '$location', '$window', function (user, $state, boxes, $location, $window) {
                        //    if (window.id > 0) {
                        //        //if (!user.university.id && $state.current.name !== "universityChoose") {
                        //        //    $state.go("universityChoose");
                        //        //}
                        //        //console.log($state.params);
                        //        //console.log($state);
                        //        if (boxes.length < 3 && $state.href("classChoose") !== $location.url()) {
                        //            //console.log('c')
                        //            //$state.go("classChoose");
                        //            //$location.url($state.href("classChoose"));
                        //             location.href = $state.href("classChoose");
                        //        }
                        //        //console.log('d')
                        //    }

                        //}
                        //],
                        template: "<div class=\"page-animation\" ui-view animation-class></div>"
                    },
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
                        templateProvider: [
                            "user", "ajaxService2", "$mdMedia", function (user, ajaxService2, $mdMedia) {
                                if ($mdMedia('gt-xs') && user.id && user.university.id > 0) {
                                    return ajaxService2.getHtml("chat/indexpartial/");
                                }
                            }
                        ]
                    },
                    "search-bar": {
                        controller: "SearchTriggerController as st",
                        templateProvider: [
                       "user", "$templateCache", function (user, $templateCache) {
                           if (user.id) {
                               return $templateCache.get("search-bar.html");
                           }
                       }]
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
        routerHelper.configureStates([
            {
                state: 'chat',
                config: {
                    url: '/chat/',
                    onEnter: [
                        '$mdMedia', '$state', function ($mdMedia, $state) {
                            if ($mdMedia('gt-xs')) {
                                $state.go('dashboard');
                            }
                        }
                    ],
                    data: { animateClass: 'chatPage' },
                    views: {
                        "chat@": {
                            template: ''
                        }
                    },
                    params: {
                        conversationData: null
                    }

                },
                templateUrl: '/chat/indexmobilepartial/'
            }
        ]);

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