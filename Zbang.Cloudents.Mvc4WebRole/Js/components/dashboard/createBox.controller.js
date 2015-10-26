(function () {
    angular.module('app.dashboard').controller('CreateBoxController', controller);

    controller.$inject = ['dashboardService', '$location'];
    function controller(dashboardService, $location) {
        var self = this;


        self.createOn = false;

        self.createBoxClick = function () {
            self.createOn = true;
        };


        self.create = function () {
            dashboardService.createPrivateBox(self.boxName).then(function (response) {
                self.createOn = false;
                self.boxName = '';
                $location.url(response.url);
            }, function (response) {
                self.error = response;
            });
        };
    }

})()