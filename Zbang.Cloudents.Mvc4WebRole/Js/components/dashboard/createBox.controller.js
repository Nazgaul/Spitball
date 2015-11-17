(function () {
    angular.module('app.dashboard').controller('CreateBoxController', controller);

    controller.$inject = ['dashboardService', '$location', '$scope'];
    function controller(dashboardService, $location, $scope) {
        var self = this;
        self.create = create;


        function create() {
           
            dashboardService.createPrivateBox(self.boxName).then(function (response) {
                $scope.$parent.d.createBoxOn = false;
                self.boxName = '';
                $location.url(response.url);
            }, function (response) {
                self.error = response;
            });
        };
    }

})()