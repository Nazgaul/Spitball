var app;
(function (app) {
    "use strict";
    var scoreToReach = 0;
    var GamificationBoard = (function () {
        function GamificationBoard(userService, $interval) {
            var _this = this;
            this.userService = userService;
            this.$interval = $interval;
            this.score = 0;
            this.userService.gamificationBoard()
                .then(function (response) {
                _this.data = response;
                scoreToReach = _this.data.score / _this.data.nextLevel * 100;
                var q = $interval(function () {
                    _this.score += 1;
                    _this.score = Math.min(_this.score, scoreToReach);
                    if (_this.score === scoreToReach) {
                        _this.$interval.cancel(q);
                    }
                }, 10);
            });
        }
        GamificationBoard.$inject = ["userService", "$interval"];
        return GamificationBoard;
    }());
    angular.module("app.dashboard").controller("gamificationBoard", GamificationBoard);
})(app || (app = {}));
