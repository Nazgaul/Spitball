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
    angular.module('app.dashboard').controller('SideMenu', dashboard);
    dashboard.$inject = ['dashboardService'];

    function dashboard(dashboardService) {
        var d = this;
        d.boxes = [];
        d.courses = [];
        d.privateBoxes = [];
        dashboardService.getBoxes().then(function (response) {
            d.boxes = response;
            d.courses = $.grep(d.boxes, function (b) {
                return b.boxType === 'academic';
            });
            d.privateBoxes = $.grep(d.boxes, function (b) {
                return b.boxType === 'box';
            });
        });
    }
})();


(function () {
    angular.module('app.dashboard').service('dashboardService', dashboard);
    dashboard.$inject = ['$q', 'ajaxService', '$timeout'];

    function dashboard($q, ajaxservice, $timeout) {
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
    }
})();