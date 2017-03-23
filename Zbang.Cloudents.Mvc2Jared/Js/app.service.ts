module app {
    export interface IHelpService {
        searchItems(term: any);
        getPreview(blobName: string, itemId: number);
        saveItem(itemId: number, boxId: number, newName: string, newType, newTags, removedTags)
        deleteDoc(itemId)
        autoUni(term: string);
        autoDepartment(term: string);
        autoTag(term: string);
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
                docType: newType,
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
        autoUni(term) {
            return $.get('/home/university/',
                {
                    term: term
                });
        }
        autoDepartment(term) {
            return $.get('/home/department/',
                {
                    term: term
                });
        }
        autoTag(term) {
            return $.get('/home/tag/',
                {
                    term: term
                });
        }
    }
    angular.module("app").service("searchService", SearchService);
}