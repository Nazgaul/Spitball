var app;
(function (app) {
    "use strict";
    var SbHistory = (function () {
        function SbHistory($rootScope, $window) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$window = $window;
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
            this.$rootScope.$on('$stateChangeStart', function (event) {
                _this.pageYOffset = $window.pageYOffset;
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
                _this.arr.push({
                    name: fromState.name,
                    params: angular.extend({}, fromParams, { pageYOffset: _this.pageYOffset })
                });
                _this.pageYOffset = 0;
            });
            this.$rootScope.$on('from-back', function () {
                _this.skipState = true;
            });
        }
        SbHistory.$inject = ["$rootScope", "$window"];
        return SbHistory;
    }());
    angular.module('app').service('sbHistory', SbHistory);
})(app || (app = {}));
