var app;
(function (app) {
    var SearchService = (function () {
        function SearchService($http) {
            this.$http = $http;
        }
        SearchService.prototype.testService = function () {
            return "yifat";
        };
        SearchService.prototype.searchItems = function (term) {
            var aa;
            return $.post('/home/items', { model: term });
        };
        SearchService.prototype.getPreview = function (blobName, index, itemId) {
            return $.get('/home/preview/', {
                index: index,
                blobName: blobName,
                id: itemId
            });
        };
        return SearchService;
    }());
    SearchService.$inject = ["$http"];
    angular.module("app").service("searchService", SearchService);
})(app || (app = {}));
//# sourceMappingURL=app.service.js.map