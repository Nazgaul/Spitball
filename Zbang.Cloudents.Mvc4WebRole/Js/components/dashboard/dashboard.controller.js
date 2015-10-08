(function () {
    angular.module('app.dashboard').controller('Dashboard', dashboard);
    dashboard.$inject = ['dashboardService'];

    function dashboard(dashboardService) {
        var d = this;
        d.boxes = [];
        dashboardService.getBoxes(0).then(function (response) {
            d.boxes = response;
        });

      
    }
})();






