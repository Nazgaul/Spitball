var app;
(function (app) {
    "use strict";
    var searchStateName = "searchinfo";
    var SearchTriggerController = (function () {
        function SearchTriggerController($scope, $state) {
            var _this = this;
            this.$scope = $scope;
            this.$state = $state;
            this.term = $state.params["q"];
            $scope.$on("$stateChangeStart", function (event, toState, toParams, fromState) {
                if (fromState.name === searchStateName && toState.name !== searchStateName) {
                    _this.term = "";
                }
            });
        }
        SearchTriggerController.prototype.change = function () {
            this.search();
        };
        SearchTriggerController.prototype.search = function () {
            var form = this.$scope["searchTrigger"];
            if (form.$valid) {
                this.$state.go(searchStateName, { q: this.term, t: this.$state.params["t"] });
            }
        };
        return SearchTriggerController;
    }());
    SearchTriggerController.$inject = ["$scope", "$state"];
    angular.module("app.search").controller("SearchTriggerController", SearchTriggerController);
})(app || (app = {}));
//# sourceMappingURL=searchTrigger.controller.js.map