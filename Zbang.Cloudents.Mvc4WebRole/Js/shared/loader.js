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
                //element.addClass('hide'); // hide spinner bar by default
                // display the spinner bar whenever the route changes(the content part started loading)
                _this.$rootScope.$on("$stateChangeStart", function () {
                    element.removeClass(hide); // show spinner bar  
                });
                // hide the spinner bar on rounte change success(after the content loaded)
                _this.$rootScope.$on("$stateChangeSuccess", addClass);
                // handle errors
                _this.$rootScope.$on("$stateNotFound", addClass);
                // handle errors
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
//# sourceMappingURL=loader.js.map