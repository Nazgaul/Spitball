var app;
(function (app) {
    "use strict";
    var ClassChoose = (function () {
        function ClassChoose(searchService, $scope) {
            this.searchService = searchService;
            this.$scope = $scope;
            this.stepTitle = 1;
        }
        ClassChoose.prototype.classSearch = function () {
            var formElement = this.$scope["classChooseFrom"];
            if (formElement.$valid) {
                console.log('ss');
            }
            console.log('her');
            if (this.term) {
                this.stepTitle = 2;
            }
            else {
                this.stepTitle = 1;
            }
        };
        ClassChoose.$inject = ["searchService", "$scope"];
        return ClassChoose;
    }());
    angular.module("app.library").controller("ClassChoose", ClassChoose);
})(app || (app = {}));
