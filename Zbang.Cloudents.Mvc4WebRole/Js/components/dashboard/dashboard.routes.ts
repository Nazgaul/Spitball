module app {
    'use strict';

    class AppRun {

        constructor(private routerHelper: IRouterHelper) {
            this.routerHelper.configureStates(getStates());
            function getStates() {
                return [
                    {
                        state: 'dashboard',
                        config: {
                            url: '/dashboard/',
                            controller: 'Dashboard as d',
                            data: { animateClass: 'dashboard' },
                            resolve: {
                                boxes: ['dashboardService', function (dashboardService) {
                                    return dashboardService.getBoxes();
                                }]
                            }
                        },
                        templateUrl: '/dashboard/indexpartial/'
                    }];
            }
        }

        public static factory() {
            const factory = (routerHelper) => {
                return new AppRun(routerHelper);
            };

            factory["$inject"] = ["routerHelper"];
            return factory;
        }

    }

    angular.module("app.dashboard").run(AppRun.factory());
}
//(function () {
//    'use strict';
//    angular.module('app.box').run(appRun);
//    appRun.$inject = ['routerHelper'];
//    function appRun(routerHelper) {
//        routerHelper.configureStates(getStates());

//        function getStates() {
//            return [
//                {
//                    state: 'dashboard',
//                    config: {
//                        url: '/dashboard/',
//                        controller: 'Dashboard as d',
//                        data: { animateClass: 'dashboard' },
//                        resolve: {
//                            boxes: ['dashboardService', function (dashboardService) {
//                                return dashboardService.getBoxes();
//                            }]
//                        }
//                    },
//                    templateUrl: '/dashboard/indexpartial/'
//                }];
//        }
//    }
//})();