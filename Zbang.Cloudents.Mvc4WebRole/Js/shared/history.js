var app;
(function (app) {
    'use strict';
    var SbHistory = (function () {
        function SbHistory($rootScope, $location) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$location = $location;
            this.skipState = false;
            this.arr = [];
            this.popElement = function () {
                if (_this.arr.length === 1) {
                    return;
                }
                return _this.arr.pop();
            };
            this.firstState = function () {
                return _this.arr.length === 0;
            };
            this.$rootScope.$on('$stateChangeStart', function () {
                _this.url = _this.$location.url();
            });
            this.$rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
                if (fromState.name === toState.name) {
                    return;
                }
                if (toParams.fromBack) {
                    return;
                }
                if (_this.skipState) {
                    _this.skipState = false;
                    return;
                }
                // to be used for back button //won't work when page is reloaded.
                _this.arr.push({
                    name: fromState.name,
                    params: fromParams
                });
            });
            this.$rootScope.$on('from-back', function () {
                _this.skipState = true;
            });
        }
        SbHistory.$inject = ['$rootScope', '$location'];
        return SbHistory;
    }());
    angular.module('app').service('sbHistory', SbHistory);
})(app || (app = {}));
//(function () {
//    angular.module('app').service('history', h);
//    h.$inject = ['$rootScope', '$location'];
//    function h($rootScope, $location) {
//        var self = this, arr = [], skipState;
//        //self.arr = [];
//        var url;
//        $rootScope.$on('$stateChangeStart', () => {
//            url = $location.url();
//        });
//        $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
//            if (fromState.name === toState.name) {
//                return;
//            }
//            if (toParams.fromBack) {
//                return;
//            }
//            if (skipState) {
//                skipState = false;
//                return;
//            }
//            // to be used for back button //won't work when page is reloaded.
//            arr.push({
//                name: fromState.name,
//                params: fromParams
//            });
//            //arr.push($location.url());
//        });
//        $rootScope.$on('from-back', function () {
//            skipState = true;
//        });
//        //self.pushState = function () {
//        //    arr.push($location.url());
//        //}
//        self.popElement = function () {
//            if (arr.length === 1) {
//                return;
//            }
//            return arr.pop();
//        }
//        self.firstState = function () {
//            return arr.length === 0;
//        }
//    }
//})(); 
//# sourceMappingURL=history.js.map