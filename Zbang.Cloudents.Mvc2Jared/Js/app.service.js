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
        SearchService.prototype.saveItem = function (itemId, newName, newType, newTags, removedTags) {
            return $.post('/home/save', {
                itemId: itemId,
                name: newName,
                docType: newType,
                newTags: newTags,
                removeTags: removedTags
            });
        };
        SearchService.prototype.getPreview = function (blobName, itemId) {
            return $.get('/home/preview/', {
                blobName: blobName,
                id: itemId
            });
        };
        return SearchService;
    }());
    angular.module("app").service("searchService", SearchService);
})(app || (app = {}));
