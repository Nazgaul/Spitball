(function () {
    angular.module('app').controller('LeaderboardController', leaderboard);

    leaderboard.$inject = ['dashboardService', '$stateParams', 'boxService'];

    function leaderboard(dashboardService, $stateParams, boxService) {
        var self = this;

        self.loading = true;
        self.isPrivate = false;
        if ($stateParams.boxId) {
            if ($stateParams.boxtype.toLowerCase() === 'box') {
                self.isPrivate = true;
            } else {
                boxService.leaderBoard($stateParams.boxId).then(function(response) {
                    self.leaderboard = response;
                    self.loading = false;
                });
            }
        }
        else {
            dashboardService.leaderboard().then(function (response) {
                self.leaderboard = response;
                self.loading = false;
            });
        }

        self.flexWidth = 50;
        if (self.isPrivate) {
            self.flexWidth = 100;
        }
    }
})()