module app {
    export interface IHelpService {
        testService(): string;
        searchItems(term: any): string;
        //searchBox(term: string, page: number): angular.IPromise<any>;
        //searchItems(term: string, page: number): angular.IPromise<any>;
        //searchQuizzes(term: string, page: number): angular.IPromise<any>;
        //searchFlashcards(term: string, page: number): angular.IPromise<any>;
    }

    class SearchService implements IHelpService {
      

        constructor() { }
        testService() {
            return "yifat";
        }
        searchItems(term) {
          
            $.post('/home/items', {model:term}).done(function (data) {
                console.log(data);})
            //this.ajaxService.get("/home/items/", { q: term, page: page }).then(response => {
            //    console.log(response);
            //});
            return "Hello";
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