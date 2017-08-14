var app;
(function (app) {
    "use strict";
    var OpenMenu = (function () {
        function OpenMenu($rootScope, userDetailsFactory, $parse) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.userDetailsFactory = userDetailsFactory;
            this.$parse = $parse;
            this.restrict = "A";
            this.compile = function ($element, attr) {
                var fn = _this.$parse(attr["openMenu"], null, true);
                return function ngEventHandler(scope, element) {
                    element.on("click", function (event) {
                        var callback = function () {
                            fn(scope, { $event: event });
                        };
                        scope.$evalAsync(callback);
                    });
                };
            };
        }
        OpenMenu.factory = function () {
            var directive = function ($rootScope, userDetailsFactory, $parse) {
                return new OpenMenu($rootScope, userDetailsFactory, $parse);
            };
            directive["$inject"] = ['$rootScope', 'userDetailsFactory', "$parse"];
            return directive;
        };
        return OpenMenu;
    }());
    angular
        .module("app")
        .directive("openMenu", OpenMenu.factory());
})(app || (app = {}));
//# sourceMappingURL=openMenu.directive.js.map