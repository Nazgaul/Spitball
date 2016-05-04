'use strict';
(function () {
    angular.module('app').controller('LeaderboardController', leaderboard);

    leaderboard.$inject = ['dashboardService', '$stateParams', 'boxService', '$scope'];

    function leaderboard(dashboardService, $stateParams, boxService, $scope) {
        var self = this;

        self.loading = true;
        if ($stateParams.boxId) {
            if ($stateParams.boxtype.toLowerCase() === 'box') {
                self.hide = true;
            } else {
                boxService.leaderBoard($stateParams.boxId).then(successCallback);
            }
        }
        else {
            dashboardService.leaderboard().then(successCallback);
        }

        function successCallback(response) {
            self.leaderboard = response;
            if (response.length < 3) {
                $scope.$emit('hide-leader-board');
            }
            self.loading = false;
        }


    }
})()