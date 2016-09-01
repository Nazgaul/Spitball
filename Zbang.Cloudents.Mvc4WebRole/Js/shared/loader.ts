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
//'use strict';
//(function () {
//    angular.module('app').directive('ngSpinnerBar', [
//        '$rootScope', '$timeout',
//        function ($rootScope, $timeout) {
//            return {
//                //TODO: write this better
//                link: function (scope, element, attrs) {
//                    // by defult hide the spinner bar
//                    element.addClass('hide'); // hide spinner bar by default
//                    // display the spinner bar whenever the route changes(the content part started loading)
//                    $rootScope.$on('$stateChangeStart', function () {
//                        element.removeClass('hide'); // show spinner bar  
//                    });

//                    // hide the spinner bar on rounte change success(after the content loaded)
//                    $rootScope.$on('$stateChangeSuccess', function () {
//                        element.addClass('hide'); // hide spinner bar
//                    });

//                    // handle errors
//                    $rootScope.$on('$stateNotFound', function () {
//                        element.addClass('hide'); // hide spinner bar
//                    });

//                    // handle errors
//                    $rootScope.$on('$stateChangeError', function () {
//                        element.addClass('hide'); // hide spinner bar
//                    });

//                    $rootScope.$on('state-change-start-prevent', function () {
//                        $timeout(function () {
//                            element.addClass('hide'); // hide spinner bar
//                        }, 1);
//                    });
//                }
//            };
//        }
//    ]);
//})();

