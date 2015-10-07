(function () {
    angular.module('app').controller('LeaderboardController', leaderboard);

    leaderboard.$inject = ['dashboardService', '$stateParams'];

    function leaderboard(dashboardService, $stateParams) {
        var self = this;


        if ($stateParams.boxid) {
        }
        else {
            dashboardService.leaderboard().then(function (response) {
                self.leaderboard = response;
            });
        }
    }
})()