var app;
(function (app) {
    "use strict";
    var AppRun = (function () {
        function AppRun(routerHelper) {
            this.routerHelper = routerHelper;
            this.routerHelper.configureStates(getStates());
            function getStates() {
                return [
                    {
                        state: "dashboard",
                        config: {
                            url: "/dashboard/",
                            controller: "Dashboard as d",
                            data: { animateClass: "dashboard" },
                            resolve: {
                                boxes: ["dashboardService", function (dashboardService) { return dashboardService.getBoxes(); }]
                            }
                        },
                        templateUrl: "/dashboard/indexpartial/"
                    }];
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
    angular.module("app.dashboard").run(AppRun.factory());
})(app || (app = {}));
