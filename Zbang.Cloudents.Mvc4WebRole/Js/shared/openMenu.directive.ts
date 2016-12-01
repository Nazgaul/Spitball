module app {
    "use strict";
    class OpenMenu implements angular.IDirective {
        restrict = "A";
        //scope = true;
        //scope: { [boundProperty: string]: string } = {
        //    openMenu: '='
        //}
        constructor(private $rootScope: angular.IRootScopeService,
            private userDetailsFactory: IUserDetailsFactory, private $parse) {
        }
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
        compile = ($element, attr) => {
            var fn = this.$parse(attr["openMenu"], /* interceptorFn */ null, /* expensiveChecks */ true);
            return function ngEventHandler(scope, element) {
                element.on("click", function (event) {
                    var callback = function () {
                        fn(scope, { $event: event });
                    };
                        scope.$evalAsync(callback);
                });
            };
        }

        static factory(): angular.IDirectiveFactory {
            const directive = ($rootScope: angular.IRootScopeService, userDetailsFactory: IUserDetailsFactory, $parse) => {
                return new OpenMenu($rootScope, userDetailsFactory, $parse);
            };

            directive["$inject"] = ['$rootScope', 'userDetailsFactory', "$parse"];

            return directive;
        }
    }

    angular
        .module("app")
        .directive("openMenu", OpenMenu.factory());
}