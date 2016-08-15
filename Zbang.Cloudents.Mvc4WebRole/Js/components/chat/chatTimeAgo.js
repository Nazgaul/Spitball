var app;
(function (app) {
    var ChatTimeAgo = (function () {
        function ChatTimeAgo(timeAgo, nowTime) {
            var _this = this;
            this.timeAgo = timeAgo;
            this.nowTime = nowTime;
            this.scope = {
                fromTime: '@',
                format: '@'
            };
            this.restrict = 'EA';
            this.link = function (scope, element, attrs) {
                var fromTime;
                scope.$watch('fromTime', function () {
                    fromTime = _this.timeAgo.parse(scope.fromTime);
                });
                scope.$watch(function () {
                    return _this.nowTime() - fromTime;
                }, function (value) {
                    var threeDaysInMilliseconds = 2.592e+8;
                    if (value > threeDaysInMilliseconds) {
                        angular.element(element).text('');
                        return;
                    }
                    angular.element(element).text(_this.timeAgo.inWords(value, fromTime, scope.format));
                });
            };
        }
        ChatTimeAgo.factory = function () {
            var directive = function (timeAgo, nowTime) {
                return new ChatTimeAgo(timeAgo, nowTime);
            };
            directive['$inject'] = ['timeAgo', 'nowTime'];
            return directive;
        };
        return ChatTimeAgo;
    }());
    angular
        .module("app.chat")
        .directive("chatTimeAgo", ChatTimeAgo.factory());
})(app || (app = {}));
;
(function () {
    angular.module('app.chat').directive('chatTimeAgo2', timeAgo);
    timeAgo.$inject = ['timeAgo', 'nowTime'];
    function timeAgo(timeAgo, nowTime) {
        return {
            scope: {
                fromTime: '@',
                format: '@'
            },
            restrict: 'EA',
            link: function (scope, elem) {
                var fromTime;
                scope.$watch('fromTime', function () {
                    fromTime = timeAgo.parse(scope.fromTime);
                });
                scope.$watch(function () {
                    return nowTime() - fromTime;
                }, function (value) {
                    var threeDaysInMilliseconds = 2.592e+8;
                    if (value > threeDaysInMilliseconds) {
                        angular.element(elem).text('');
                        return;
                    }
                    angular.element(elem).text(timeAgo.inWords(value, fromTime, scope.format));
                });
            }
        };
    }
})();
