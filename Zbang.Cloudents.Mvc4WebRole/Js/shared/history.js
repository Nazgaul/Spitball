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
                _this.arr.push({
                    name: fromState.name,
                    params: fromParams
                });
            });
            this.$rootScope.$on('from-back', function () {
                _this.skipState = true;
            });
        }
        SbHistory.prototype.popElement = function () {
            if (this.arr.length === 1) {
                return;
            }
            return this.arr.pop();
        };
        SbHistory.prototype.firstState = function () {
            return this.arr.length === 0;
        };
        SbHistory.$inject = ['$rootScope', '$location'];
        return SbHistory;
    }());
})(app || (app = {}));
