var app;
(function (app) {
    "use strict";
    var searchStateName = "searchinfo";
    var SearchTriggerController = (function () {
        function SearchTriggerController($scope, $state, $mdMedia, resManager) {
            var _this = this;
            this.$scope = $scope;
            this.$state = $state;
            this.$mdMedia = $mdMedia;
            this.resManager = resManager;
            this.term = $state.params["q"];
            if ($mdMedia('gt-xs')) {
                this.placeholder = resManager.get("Search");
            }
            else {
                this.placeholder = resManager.get("SearchMobile");
            }
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
        SearchTriggerController.$inject = ["$scope", "$state", "$mdMedia", "resManager"];
        return SearchTriggerController;
    }());
    angular.module("app.search").controller("SearchTriggerController", SearchTriggerController);
})(app || (app = {}));
