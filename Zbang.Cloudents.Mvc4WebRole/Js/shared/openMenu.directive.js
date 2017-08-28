"use strict";
var app;
(function (app) {
    "use strict";
    var OpenMenu = (function () {
        //scope = true;
        //scope: { [boundProperty: string]: string } = {
        //    openMenu: '='
        //}
        function OpenMenu($rootScope, userDetailsFactory, $parse) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.userDetailsFactory = userDetailsFactory;
            this.$parse = $parse;
            this.restrict = "A";
            //link = (scope: angular.IScope, element: ng.IAugmentedJQuery,attr) => {
            //    element.on('click', (ev) => {
            //        console.log(ev);
            //        if (!this.userDetailsFactory.isAuthenticated()) {
            //            this.$rootScope.$broadcast("show-unregisterd-box");
            //            return;
            //        }
            //        console.log(attr["$mdOpenMenu"]);
            //        //scope["openMenu"](ev);
            //        //scope['$mdOpenMenu'](ev);
            //    });
            //};
            this.compile = function ($element, attr) {
                var fn = _this.$parse(attr["openMenu"], /* interceptorFn */ null, /* expensiveChecks */ true);
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