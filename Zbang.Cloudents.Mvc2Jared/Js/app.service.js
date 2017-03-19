var app;
(function (app) {
    var SearchService = (function () {
        function SearchService($http) {
            this.$http = $http;
        }
        SearchService.prototype.searchItems = function (term) {
            return $.post('/home/items', { model: term });
        };
        SearchService.prototype.saveItem = function (itemId, boxId, newName, newType, newTags, removedTags) {
            var model = {
                itemId: itemId,
                boxId: boxId,
                itemName: newName,
                tabId: newType,
                newTags: newTags,
                removeTags: removedTags
            };
            return $.post('/home/save', { model: model });
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
    angular.module("app").service("searchService", SearchService);
})(app || (app = {}));
//# sourceMappingURL=app.service.js.map