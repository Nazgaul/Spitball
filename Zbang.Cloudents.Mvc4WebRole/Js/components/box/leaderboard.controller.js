var app;
(function (app) {
    "use strict";
    var Leaderboard = (function () {
        function Leaderboard(boxService, $stateParams) {
            this.boxService = boxService;
            this.$stateParams = $stateParams;
            this.leaderboardMyself = true;
            this.leaderBoard();
        }
        Leaderboard.prototype.leaderBoard = function () {
            var _this = this;
            this.boxService.leaderBoard(this.$stateParams.boxId, this.leaderboardMyself)
                .then(function (response) {
                for (var i = 0; i < response.length; i++) {
                    var elem = response[i];
                    if (i === 0) {
                        elem.progress = 100;
                        continue;
                    }
                    elem.progress = elem.score / response[0].score * 100;
                }
                _this.leaderboard = response;
            });
        };
        Leaderboard.prototype.goToSelf = function () {
            this.leaderboardMyself = !this.leaderboardMyself;
            this.leaderBoard();
        };
        Leaderboard.$inject = ["boxService", "$stateParams"];
        return Leaderboard;
    }());
    angular.module("app.box").controller("Leaderboard", Leaderboard);
})(app || (app = {}));
//# sourceMappingURL=leaderboard.controller.js.map