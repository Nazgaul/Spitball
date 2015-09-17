(function () {
    angular.module('app.dashboard').controller('Dashboard', dashboard);
    dashboard.$inject = ['dashboardService', '$scope'];

    function dashboard(dashboardService, $scope) {
        var d = this;
        d.boxes = [];
        dashboardService.getBoxes().then(function (response) {
            d.boxes = response;
        });

      
    }
})();

(function () {
    angular.module('app.dashboard').controller('UniversityMeta', universityMeta);
    universityMeta.$inject = ['dashboardService'];

    function universityMeta(dashboardService) {
        var um = this;
        dashboardService.getUniversityMeta().then(function (response) {
            um.leaderBoard = response.leaderBoard;
            um.info = response.info;
        });
    }
})();




