﻿module app {
    export interface IHelpService {
        testService(): string;
        searchItems(term: any);
        getPreview(blobName: string, itemId: number);
        saveItem(itemId:number,newName:string,newType:string,newTags,removedTags)
    }

    class SearchService implements IHelpService {

        constructor(private $http: angular.IHttpService) { }
        testService() {
            return "yifat";
        }
        searchItems(term) {
            var aa;
            return $.post('/home/items', { model: term })
        }
        saveItem(itemId, newName, newType, newTags, removedTags) {
            return $.post('/home/save', {
                itemId: itemId,
                name: newName,
                docType: newType,
                newTags: newTags,
                removeTags: removedTags
            });
        }

        getPreview(blobName, itemId) {
            return $.get('/home/preview/',
                {
                    blobName: blobName,
                    id: itemId
                });
        }
        //    private ajaxService2: IAjaxService2) {
        //}
        //searchBox(term: string, page: number) {
        //    return this.ajaxService2.get("/search/boxes/", { q: term, page: page }, "searchBox", "search");
        //}

        //searchItems(term: string, page: number) {
        //    return this.ajaxService2.get("/search/items/", { q: term, page: page }, "searchItem", "search");
        //}
        //searchQuizzes(term: string, page: number) {
        //    return this.ajaxService2.get("/search/quizzes/", { q: term, page: page }, "searchQuiz", "search");
        //}
        //searchFlashcards(term: string, page: number) {
        //    return this.ajaxService2.get("/search/flashcards/", { q: term, page: page });
        //}

    }
    angular.module("app").service("searchService", SearchService);
}