var app;
(function (app) {
    "use strict";
    var scoreToReach = 0;
    var badgesToReach = 0;
    var GamificationBoard = (function () {
        function GamificationBoard(userService, $interval, userDetailsFactory) {
            var _this = this;
            this.userService = userService;
            this.$interval = $interval;
            this.userDetailsFactory = userDetailsFactory;
            this.score = 0;
            this.badges = 0;
            this.userService.gamificationBoard()
                .then(function (response) {
                _this.data = response;
                var userData = userDetailsFactory.get();
                _this.data.userName = userData.name;
                _this.data.userImage = userData.image;
                scoreToReach = _this.data.score / _this.data.nextLevel * 100;
                var q = $interval(function () {
                    _this.score += 1;
                    _this.score = Math.min(_this.score, scoreToReach);
                    if (_this.score === scoreToReach) {
                        _this.$interval.cancel(q);
                    }
                }, 10);
                badgesToReach = _this.data.badgeCount / 5 * 100;
                var b = $interval(function () {
                    _this.badges += 1;
                    _this.badges = Math.min(_this.badges, badgesToReach);
                    if (_this.badges === badgesToReach) {
                        _this.$interval.cancel(b);
                    }
                }, 10);
            });
        }
        GamificationBoard.$inject = ["userService", "$interval", "userDetailsFactory"];
        return GamificationBoard;
    }());
    angular.module("app.dashboard").controller("gamificationBoard", GamificationBoard);
})(app || (app = {}));
//# sourceMappingURL=gamificationBoard.controller.js.map