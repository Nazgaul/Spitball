module app {
    export interface IHelpService {
        searchItems(term: any);
        getPreview(blobName: string, itemId: number);
        saveItem(itemId: number, boxId: number, newName: string, newType, newTags, removedTags)
        deleteDoc(itemId)
    }

    class SearchService implements IHelpService {

        constructor(private $http: angular.IHttpService) { }

        searchItems(term) {
            return $.post('/home/items', { model: term })
        }
        saveItem(itemId, boxId, newName, newType, newTags, removedTags) {
            var model = {
                itemId: itemId,
                boxId: boxId,
                itemName: newName,
                tabId: newType,
                newTags: newTags,
                removeTags: removedTags
            };
            return $.post('/home/save', { model: model });
        }

        getPreview(blobName, itemId) {
            return $.get('/home/preview/',
                {
                    blobName: blobName,
                    id: itemId
                });
        }
        
        deleteDoc(itemId) {
            return $.post('/home/delete/',
                {
                    itemId: itemId
                });
        }
    }
    angular.module("app").service("searchService", SearchService);
}