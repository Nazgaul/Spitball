module app {
    "use strict";

    class ClassChoose {
        static $inject = ["searchService"];

        constructor(private searchService: ISearchService) {
            
        }
    }

    angular.module("app.library").controller("ClassChoose", ClassChoose);
}