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
                docType: newType,
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
        SearchService.prototype.deleteDoc = function (itemId) {
            return $.post('/home/delete/', {
                itemId: itemId
            });
        };
        SearchService.prototype.autoUni = function (term) {
            return $.get('/home/university/', {
                term: term
            });
        };
        SearchService.prototype.autoDepartment = function (term) {
            return $.get('/home/department/', {
                term: term
            });
        };
        SearchService.prototype.autoTag = function (term) {
            return $.get('/home/tag/', {
                term: term
            });
        };
        return SearchService;
    }());
    angular.module("app").service("searchService", SearchService);
})(app || (app = {}));
