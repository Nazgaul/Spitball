angular.module('dashboard')
    .service('dashboardService',
    ['dashboard', function (dashboard) {
        var service = this;

        service.getBoxList = function (page) {
            return dashboard.boxList({ page: page });
        };

        service.getRecommendedBoxesList = function () {
            return dashboard.recommendedBoxes();
        };
    }]
);