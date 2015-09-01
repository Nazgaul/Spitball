(function () {
    angular.module('app.dashboard').controller('Dashboard', dashboard);
    dashboard.$inject = ['dashboardService'];

    function dashboard(dashboardService) {
        var d = this;
        d.boxes = [];
        dashboardService.getDetails().then(function (response) {
            d.boxes = response;
        });
    }
})();


(function () {
    angular.module('app.dashboard').service('dashboardService', dashboard);
    dashboard.$inject = ['ajaxService'];

    function dashboard(ajaxservice) {
        var d = this;
        d.isLoggedIn = false;
        d.getDetails = function () {
            return ajaxservice.get('/dashboard/boxlist/', null, 1800000);
        }
    }
})();