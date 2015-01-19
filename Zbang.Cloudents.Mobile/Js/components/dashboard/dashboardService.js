angular.module('dashboard')
    .service('dashboardService',
    ['$rootScope', 'dashboard','account', function ($rootScope ,dashboard,account) {
        var service = this;

        service.getBoxList = function (page) {
            return dashboard.boxList({ page: page });
        };

        service.getRecommendedBoxesList = function () {
            return dashboard.recommendedBoxes();
        };

        service.disableFirstTime = function () {
            account.disableFirstTime({ firstTime: 'Library' });
        };
        service.doneLoad = function () {
            $rootScope.$broadcast('$stateLoaded');
        };
    }]
);