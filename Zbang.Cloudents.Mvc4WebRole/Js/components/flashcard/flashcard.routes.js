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
                            url: "/{boxtype:box|course}/{universityType:encodeStr}/{boxId:int}/{boxName:encodeStr}/flashcardcreate/?{id:int}&name",
                            controller: "flashcardCreate as f",
                            data: { animateClass: "full-screen flashcard" },
                            reloadOnSearch: false
                        },
                        templateUrl: "/flashcard/createpartial/"
                    }
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
