﻿module app {
    "use strict";

    export interface ISearchService {
        searchBox(term: string, page: number): angular.IPromise<any>;
        searchItems(term: string, page: number): angular.IPromise<any>;
        searchQuizzes(term: string, page: number): angular.IPromise<any>;
        searchFlashcards(term: string, page: number): angular.IPromise<any>;
    }

    class SearchService implements ISearchService {
        static $inject = ["ajaxService2"];

        constructor(
            private ajaxService2: IAjaxService2) {
        }
        searchBox(term: string, page: number) {
            return this.ajaxService2.get("/search/boxes/", { q: term, page: page }, "searchBox", "search");
        }
       
        searchItems(term: string, page: number) {
            return this.ajaxService2.get("/search/items/", { q: term, page: page }, "searchItem", "search");
        }
        searchQuizzes(term: string, page: number) {
            return this.ajaxService2.get("/search/quizzes/", { q: term, page: page }, "searchQuiz", "search");
        }
        searchFlashcards(term: string, page: number) {
            return this.ajaxService2.get("/search/flashcards/", { q: term, page: page });
        }

    }
    angular.module("app.search").service("searchService", SearchService);
}