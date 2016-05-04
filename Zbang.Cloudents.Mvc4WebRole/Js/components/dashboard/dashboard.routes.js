'use strict';
(function () {
    angular.module('app.box').run(appRun);
    appRun.$inject = ['routerHelper'];
    function appRun(routerHelper) {
        routerHelper.configureStates(getStates());

        function getStates() {
            return [
            {
                state: 'dashboard',
                config: {
                    url: '/dashboard/',
                    controller: 'Dashboard as d',
                    data: { animateClass: 'dashboard' },
                    resolve: {
                        boxes: ['dashboardService', function(dashboardService) {
                            return dashboardService.getBoxes();
                        }]
                    }
                },
                templateUrl: '/dashboard/indexpartial/'
            }];
        }
    }
})();