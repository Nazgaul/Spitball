angular.module('dashboard')
    .service('dashboardService',
    ['$rootScope', 'dashboard', function ($rootScope ,dashboard) {
        var service = this;

        service.getBoxList = function (page) {
            return dashboard.boxList({ page: page });
        };

        service.getRecommendedBoxesList = function () {
            return dashboard.recommendedBoxes();
        };

        service.doneLoad = function () {
            $rootScope.$broadcast('$stateLoaded');
        };
    }]
);