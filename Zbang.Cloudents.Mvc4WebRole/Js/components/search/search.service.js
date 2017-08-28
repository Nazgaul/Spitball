"use strict";
var app;
(function (app) {
    "use strict";
    var SearchService = (function () {
        function SearchService(ajaxService2) {
            this.ajaxService2 = ajaxService2;
        }
        SearchService.prototype.searchBox = function (term, page) {
            return this.ajaxService2.get("/search/boxes/", { q: term, page: page }, "searchBox", "search");
        };
        SearchService.prototype.searchItems = function (term, page) {
            return this.ajaxService2.get("/search/items/", { q: term, page: page }, "searchItem", "search");
        };
        SearchService.prototype.searchQuizzes = function (term, page) {
            return this.ajaxService2.get("/search/quizzes/", { q: term, page: page }, "searchQuiz", "search");
        };
        SearchService.prototype.searchFlashcards = function (term, page) {
            return this.ajaxService2.get("/search/flashcards/", { q: term, page: page });
        };
        SearchService.$inject = ["ajaxService2"];
        return SearchService;
    }());
    angular.module("app.search").service("searchService", SearchService);
})(app || (app = {}));
//# sourceMappingURL=search.service.js.map