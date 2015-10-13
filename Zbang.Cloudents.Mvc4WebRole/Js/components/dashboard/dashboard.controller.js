(function () {
    angular.module('app.dashboard').controller('Dashboard', dashboard);
    dashboard.$inject = ['dashboardService','$scope'];

    function dashboard(dashboardService, $scope) {
        var d = this;
        d.boxes = [];
        d.inviteOpen = false;
        d.inviteToSpitabll = function () {
            d.inviteOpen = true;
            $scope.$broadcast('open_invite');
        }





        dashboardService.getBoxes(0).then(function (response) {
            d.boxes = response;
        });

        $scope.$on("close_invite", function () {
            d.inviteOpen = false;
        });
      
    }
})();






