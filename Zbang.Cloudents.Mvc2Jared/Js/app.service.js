var app;
(function (app) {
    var SearchService = (function () {
        function SearchService() {
        }
        SearchService.prototype.testService = function () {
            return "yifat";
        };
        SearchService.prototype.searchItems = function (term) {
            var aa;
            return $.post('/home/items', { model: term });
        };
        return SearchService;
    }());
    angular.module("app").service("searchService", SearchService);
})(app || (app = {}));
//# sourceMappingURL=app.service.js.map