(function () {
    'use strict';
    angular.module('app').controller('LeaderboardController', leaderboard);
    leaderboard.$inject = [ '$stateParams', 'boxService', '$scope','$filter'];

    function leaderboard($stateParams, boxService, $scope, $filter) {
        var self = this;
        //if ($stateParams.boxId) {
            if ($stateParams.boxtype.toLowerCase() === 'box') {
                self.hide = true;
            } else {
                boxService.leaderBoard($stateParams.boxId).then(successCallback);
            }
        //}
        //else {
        //    dashboardService.leaderboard().then(successCallback);
        //}

        function successCallback(response) {
            if (response.length < 3) {
                $scope.$emit('hide-leader-board');
            }
            for (var i = 0; i < response.length; i++) {
                response[i].score = $filter('megaNumber')(response[i].score);
            }
            self.leaderboard = response;
        }
    }
})();