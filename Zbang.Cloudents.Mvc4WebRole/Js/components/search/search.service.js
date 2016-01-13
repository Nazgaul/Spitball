(function () {
    angular.module('app.search').service('searchService', search);

    search.$inject = ['ajaxService'];
    function search(ajaxService) {
        var self = this;
        self.search = function (term, page) {
            return ajaxService.get('/search/data/', { q: term, page: page }, 30000);
        }
    }
})()