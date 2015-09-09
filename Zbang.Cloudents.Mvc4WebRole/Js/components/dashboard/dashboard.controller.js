(function () {
    angular.module('app.dashboard').controller('Dashboard', dashboard);
    dashboard.$inject = ['dashboardService'];

    function dashboard(dashboardService) {
        var d = this;
        d.boxes = [];
        d.courses = [];
        d.privateBoxes = [];
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
            console.log(response);
            um.leaderBoard = response.leaderBoard;
            um.info = response.info;


        });
    }
})();




(function () {
    angular.module('app.dashboard').service('dashboardService', dashboard);
    dashboard.$inject = ['$q', 'ajaxService'];

    function dashboard($q, ajaxservice) {
        var d = this;
        var serverCall = false;
        var defer = $q.defer();

        d.getBoxes = function () {
            if (d.boxes) {
                defer.resolve(d.boxes);
                return defer.promise;
            }

            if (!serverCall) {
                serverCall = true;
                ajaxservice.get('/dashboard/boxlist/', null, 1800000).then(function (response) {
                    serverCall = false;
                    d.boxes = response;
                    defer.resolve(d.boxes);
                });
            }
            return defer.promise;
        }

        d.getUniversityMeta = function() {
            return ajaxservice.get('/dashboard/SideBar/', null, 1800000);
        }
    }
})();