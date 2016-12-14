var app;
(function (app) {
    "use strict";
    var ChatTimeAgo = (function () {
        function ChatTimeAgo(timeAgo, nowTime) {
            var _this = this;
            this.timeAgo = timeAgo;
            this.nowTime = nowTime;
            this.scope = {
                fromTime: "@",
                format: "@"
            };
            this.restrict = "EA";
            this.link = function (scope, element) {
                var threeDaysInMilliseconds = 2.592e+8;
                var fromTime;
                scope.$watch('fromTime', function () {
                    fromTime = _this.timeAgo.parse(scope.fromTime);
                });
                var unregister = scope.$watch(function () {
                    return _this.nowTime() - fromTime;
                }, function (value) {
                    if (value > threeDaysInMilliseconds) {
                        element.text('');
                        unregister();
                        return;
                    }
                    element.text(_this.timeAgo.inWords(value, fromTime, scope.format));
                });
            };
        }
        ChatTimeAgo.factory = function () {
            var directive = function (timeAgo, nowTime) {
                return new ChatTimeAgo(timeAgo, nowTime);
            };
            directive["$inject"] = ["timeAgo", "nowTime"];
            return directive;
        };
        return ChatTimeAgo;
    }());
    angular
        .module("app.chat")
        .directive("chatTimeAgo", ChatTimeAgo.factory());
})(app || (app = {}));
;
//# sourceMappingURL=chatTimeAgo.js.map