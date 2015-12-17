(function () {
    angular.module('app.dashboard').controller('CreateBoxController', controller);

    controller.$inject = ['dashboardService', '$location', '$scope'];
    function controller(dashboardService, $location, $scope) {
        var self = this;
        self.create = create;
        self.cancel = cancel;

        function create(myform) {
           
            dashboardService.createPrivateBox(self.boxName).then(function (response) {
                $scope.$parent.d.createBoxOn = false;
                cancel(myform);
                $location.url(response.url);
            }, function (response) {
                myform.name.$setValidity('server', false);
                self.error = response;
            });
        };

        function cancel(myform) {
            self.boxName = '';
            $scope.app.resetForm(myform);
            $scope.d.createBoxOn  = false;
        }
    }

})()