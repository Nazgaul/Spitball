var app;
(function (app) {
    "use strict";
    var AppRun = (function () {
        function AppRun(routerHelper) {
            this.routerHelper = routerHelper;
            routerHelper.configureStates(getStates());
            function getStates() {
                return [
                    {
                        state: "flashcardCreate",
                        config: {
                            url: "/course/{universityType:encodeStr}/{boxId:int}/{boxName:encodeStr}/flashcardcreate/?{id:int}",
                            controller: "flashcardCreate as f",
                            resolve: {
                                flashcard: [
                                    "flashcardService", "$stateParams", function (flashcardService, $stateParams) {
                                        if ($stateParams["id"]) {
                                            return flashcardService.draft($stateParams["id"]);
                                        }
                                    }
                                ]
                            },
                            data: { animateClass: "full-screen flashcard" },
                            reloadOnSearch: false
                        },
                        templateUrl: "/flashcard/createpartial/"
                    },
                    {
                        state: 'flashcard',
                        config: {
                            url: '/flashcard/{universityType:encodeStr}/{boxId:int}/{boxName:encodeStr}/{id:int}/{name:encodeStr}/',
                            controller: 'flashcard as f',
                            resolve: {
                                flashcard: ['flashcardService', '$stateParams', function (flashcardService, $stateParams) { return flashcardService.get($stateParams["id"], $stateParams.boxId); }]
                            },
                            data: { animateClass: 'flashcardPage' },
                            views: {
                                "menu@": {
                                    template: ''
                                }
                            }
                        },
                        templateUrl: '/flashcard/indexpartial/'
                    },
                ];
            }
        }
        AppRun.factory = function () {
            var factory = function (routerHelper) {
                return new AppRun(routerHelper);
            };
            factory["$inject"] = ["routerHelper"];
            return factory;
        };
        return AppRun;
    }());
    angular.module("app.flashcard").run(AppRun.factory());
})(app || (app = {}));
