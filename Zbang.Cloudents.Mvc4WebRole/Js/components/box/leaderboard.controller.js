var app;
(function (app) {
    "use strict";
    var Leaderboard = (function () {
        function Leaderboard(boxService, $stateParams, $mdDialog, userDetailsFactory) {
            this.boxService = boxService;
            this.$stateParams = $stateParams;
            this.$mdDialog = $mdDialog;
            this.userDetailsFactory = userDetailsFactory;
            this.leaderBoard();
        }
        Leaderboard.prototype.leaderBoard = function () {
            var _this = this;
            var user = this.userDetailsFactory.get();
            this.user = {
                name: user.name,
                image: user.image,
                levelName: user.levelName,
                progress: user.score / user.nextLevel * 100,
                points: user.score
            };
            this.boxService.leaderBoard(this.$stateParams.boxId)
                .then(function (response) {
                var leaderBoard = response.model;
                for (var i = 0; i < leaderBoard.length; i++) {
                    var elem = leaderBoard[i];
                    if (i === 0) {
                        elem.progress = 100;
                        continue;
                    }
                    elem.progress = elem.score / leaderBoard[0].score * 100;
                }
                _this.user.rank = response.rank;
                _this.leaderboard = leaderBoard;
            });
        };
        Leaderboard.prototype.close = function () {
            this.$mdDialog.hide();
        };
        Leaderboard.$inject = ["boxService", "$stateParams", "$mdDialog", "userDetailsFactory"];
        return Leaderboard;
    }());
    angular.module("app.box").controller("BoxLeaderboard", Leaderboard);
})(app || (app = {}));
//# sourceMappingURL=leaderboard.controller.js.map