module app {
    "use strict";

    class AppRun {

        constructor(private routerHelper: IRouterHelper) {
            routerHelper.configureStates(getStates());
            function getStates() {
                return [
                    {
                        state: "flashcardCreate",
                        config: {
                            url: "/{boxtype:box|course}/{universityType:encodeStr}/{boxId:int}/{boxName:encodeStr}/flashcardcreate/?{id:int}",
                            controller: "flashcardCreate as f",
                            resolve: {
                                flashcard: [
                                    "flashcardService", "$stateParams", (flashcardService: IFlashcardService, $stateParams: angular.ui.IStateParamsService) => {
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

                    }
                ];
            }
        }

        static factory() {
            const factory = (routerHelper) => {
                return new AppRun(routerHelper);
            };

            factory["$inject"] = ["routerHelper"];
            return factory;
        }

    }

    angular.module("app.flashcard").run(AppRun.factory());
}
