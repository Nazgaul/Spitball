module app {
    "use strict";

    class AppRun {

        constructor(private routerHelper: IRouterHelper) {
            routerHelper.configureStates(getStates());
            function getStates() {
                return [
                    {
                        state: "dashboard",
                        config: {
                            url: "/dashboard/",
                            controller: "Dashboard as d",
                            data: { animateClass: "dashboard" },
                            resolve: {
                                boxes: ["dashboardService", dashboardService => dashboardService.getBoxes()]
                            },
                            onEnter: [function () {
                                console.log('b');
                            }]
                        },
                        templateUrl: "/dashboard/indexpartial/"
                    }];
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

    angular.module("app.dashboard").run(AppRun.factory());
}
