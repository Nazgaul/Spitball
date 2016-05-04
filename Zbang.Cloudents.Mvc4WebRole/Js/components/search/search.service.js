'use strict';
(function () {
    angular.module('app.search').service('searchService', search);

    search.$inject = ['ajaxService'];
    function search(ajaxService) {
        var self = this;
        //self.search = function (term, page) {
        //    return ajaxService.get('/search/data/', { q: term, page: page }, 30000);
        //}

        self.searchBox = function (term, page) {
            return ajaxService.get('/search/boxes/', { q: term, page: page }, 30000);
        }
        self.searchItems = function (term, page) {
            return ajaxService.get('/search/items/', { q: term, page: page }, 30000);
        }
        self.searchQuizzes = function (term, page) {
            return ajaxService.get('/search/quizzes/', { q: term, page: page }, 30000);
        }
    }
})()