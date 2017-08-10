var app;
(function (app) {
    "use strict";
    var FlipClock = (function () {
        function FlipClock() {
            this.restrict = "E";
            this.link = function (scope, element) {
                var d = new Date(2016, 11, 16);
                element["FlipClock"]((d.getTime() - new Date().getTime()) / 1000, {
                    clockFace: 'DailyCounter',
                    countdown: true
                });
            };
        }
        FlipClock.factory = function () {
            var directive = function () {
                return new FlipClock();
            };
            return directive;
        };
        return FlipClock;
    }());
    angular
        .module("app.dashboard")
        .directive("flipClock", FlipClock.factory());
})(app || (app = {}));
//# sourceMappingURL=flipclock.directive.js.map