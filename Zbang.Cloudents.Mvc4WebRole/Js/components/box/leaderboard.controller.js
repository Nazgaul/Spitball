var app;
(function (app) {
    "use strict";
    var Leaderboard = (function () {
        function Leaderboard(boxService, $stateParams) {
            var _this = this;
            this.boxService = boxService;
            this.$stateParams = $stateParams;
            boxService.leaderBoard($stateParams.boxId)
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
        }
        Leaderboard.$inject = ["boxService", "$stateParams"];
        return Leaderboard;
    }());
    angular.module("app.box").controller("Leaderboard", Leaderboard);
})(app || (app = {}));
