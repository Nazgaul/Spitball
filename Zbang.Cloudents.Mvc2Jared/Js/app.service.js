var app;
(function (app) {
    var SearchService = (function () {
        function SearchService($http) {
            this.$http = $http;
        }
        SearchService.prototype.searchItems = function (term) {
            return $.post('/home/items', { model: term });
        };
        SearchService.prototype.getPreview = function (blobName, itemId) {
            return $.get('/home/preview/', {
                blobName: blobName,
                id: itemId
            });
        };
        SearchService.prototype.getTabs = function (boxId) {
            return $.get('/home/tabs/', {
                id: boxId
            });
        };
        return SearchService;
    }());
    SearchService.$inject = ["$http"];
    angular.module("app").service("searchService", SearchService);
})(app || (app = {}));
