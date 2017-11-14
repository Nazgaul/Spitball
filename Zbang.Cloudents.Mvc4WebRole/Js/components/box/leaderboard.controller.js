"use strict";
var app;
(function (app) {
    "use strict";
    var Leaderboard = (function () {
        //leaderboardMyself = true;
        function Leaderboard(boxService, $stateParams, $mdDialog, userDetailsFactory, $scope) {
            var _this = this;
            this.boxService = boxService;
            this.$stateParams = $stateParams;
            this.$mdDialog = $mdDialog;
            this.userDetailsFactory = userDetailsFactory;
            this.$scope = $scope;
            this.leaderBoard();
            $scope.$on("$destroy", function () {
                _this.close();
            });
        }
        Leaderboard.prototype.leaderBoard = function () {
            var _this = this;
            var user = this.userDetailsFactory.get();
            this.leaderboardUser = {
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
                _this.leaderboardUser.rank = response.rank;
                _this.leaderboard = leaderBoard;
            });
        };
        Leaderboard.prototype.close = function () {
            this.$mdDialog.hide();
        };
        return Leaderboard;
    }());
    Leaderboard.$inject = ["boxService", "$stateParams", "$mdDialog", "userDetailsFactory", "$scope"];
    angular.module("app.box").controller("BoxLeaderboard", Leaderboard);
})(app || (app = {}));
//# sourceMappingURL=leaderboard.controller.js.map