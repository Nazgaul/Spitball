var app;
(function (app) {
    "use strict";
    var NgSpinnerBar = (function () {
        function NgSpinnerBar($rootScope, $timeout) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$timeout = $timeout;
            this.link = function (scope, element) {
                var hide = "hide";
                _this.$rootScope.$on("$stateChangeStart", function () {
                    element.removeClass(hide);
                });
                _this.$rootScope.$on("$stateChangeSuccess", addClass);
                _this.$rootScope.$on("$stateNotFound", addClass);
                _this.$rootScope.$on("$stateChangeError", addClass);
                _this.$rootScope.$on("state-change-start-prevent", function () {
                    _this.$timeout(addClass, 1);
                });
                function addClass() {
                    element.addClass(hide);
                }
            };
        }
        NgSpinnerBar.factory = function () {
            var directive = function ($rootScope, $timeout) {
                return new NgSpinnerBar($rootScope, $timeout);
            };
            directive["$inject"] = ["$rootScope", "$timeout"];
            return directive;
        };
        return NgSpinnerBar;
    }());
    angular
        .module("app")
        .directive("ngSpinnerBar", NgSpinnerBar.factory());
})(app || (app = {}));
//# sourceMappingURL=loader.js.map