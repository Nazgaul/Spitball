var app;
(function (app) {
    var SearchService = (function () {
        function SearchService() {
        }
        SearchService.prototype.testService = function () {
            return "yifat";
        };
        SearchService.prototype.searchItems = function (term, page) {
            $.get('/home/items', { term: term, page: 0 }).done(function (data) {
                console.log(data);
            });
            //this.ajaxService.get("/home/items/", { q: term, page: page }).then(response => {
            //    console.log(response);
            //});
            return "Hello";
        };
        return SearchService;
    }());
    angular.module("app").service("searchService", SearchService);
})(app || (app = {}));
//# sourceMappingURL=app.service.js.map