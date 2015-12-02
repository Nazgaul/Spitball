(function () {
    angular.module('app.dashboard').controller('Dashboard', dashboard);
    dashboard.$inject = ['dashboardService', '$scope'];

    function dashboard(dashboardService, $scope) {
        var d = this;
        d.boxes = [];
        d.inviteOpen = false;
        d.inviteToSpitabll = function () {
            d.inviteOpen = true;
            $scope.$broadcast('open_invite');
        }


        d.boxes = [];
        dashboardService.getBoxes().then(function (response) {
            d.boxes = d.boxes.concat(response);
            dashboardService.recommended().then(function (response2) {
                for (var i = 0; i < response2.length; i++) {
                    response2[i].recommended = true;
                    response2[i].updates = 0;
                }
                d.boxes = d.boxes.concat(response2);
            });
        });



        $scope.$on("close_invite", function () {
            d.inviteOpen = false;
        });

    }
})();






