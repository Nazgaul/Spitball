var app;
(function (app) {
    "use strict";
    var AppController = (function () {
        function AppController($scope, searchService) {
            this.$scope = $scope;
            this.searchService = searchService;
            this.noResults = false;
            this.result = [];
            this.class = "this is Class";
            this.yifat = 5;
        }
        AppController.prototype.search = function () {
            var _this = this;
            if (this.formData) {
                Object.keys(this.formData).forEach(function (k) { return (!_this.formData[k] && _this.formData[k] !== undefined) && delete _this.formData[k]; });
                if (!Object.keys(this.formData).length)
                    console.log("empty");
                console.log(this.formData);
                var promise = this.searchService.searchItems(this.formData);
                promise.then(function (response) { return _this.result = response; });
            }
        };
        return AppController;
    }());
    AppController.$inject = ["$scope", "searchService"];
    angular.module("app").controller("AppController", AppController);
})(app || (app = {}));
//# sourceMappingURL=app.controller.js.map