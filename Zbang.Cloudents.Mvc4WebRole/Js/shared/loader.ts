module app {
    "use strict";

    class NgSpinnerBar implements angular.IDirective {

        constructor(private $rootScope: angular.IRootScopeService,private $timeout: angular.ITimeoutService) {

        }
        link = (scope, element: ng.IAugmentedJQuery) => {
            const hide = "hide";
            
            //element.addClass('hide'); // hide spinner bar by default
            // display the spinner bar whenever the route changes(the content part started loading)
            this.$rootScope.$on("$stateChangeStart", () => {
                element.removeClass(hide); // show spinner bar  
            });

            // hide the spinner bar on rounte change success(after the content loaded)
            this.$rootScope.$on("$stateChangeSuccess", addClass);

            // handle errors
            this.$rootScope.$on("$stateNotFound", addClass);

            // handle errors
            this.$rootScope.$on("$stateChangeError", addClass);

            this.$rootScope.$on("state-change-start-prevent", () => {
                this.$timeout(addClass, 1);
            });

            function addClass() {
                element.addClass(hide);
            }
        }
        static factory(): angular.IDirectiveFactory {
            const directive = ($rootScope, $timeout) => {
                return new NgSpinnerBar($rootScope, $timeout);
            };
            directive["$inject"] = ["$rootScope", "$timeout"];
            return directive;
        }

    }
    angular
        .module("app")
        .directive("ngSpinnerBar", NgSpinnerBar.factory());
}


