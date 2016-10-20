var app;
(function (app) {
    "use strict";
    var ClassChoose = (function () {
        function ClassChoose(searchService) {
            this.searchService = searchService;
        }
        ClassChoose.$inject = ["searchService"];
        return ClassChoose;
    }());
    angular.module("app.library").controller("ClassChoose", ClassChoose);
})(app || (app = {}));
