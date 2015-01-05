angular.module('search')
    .service('searchService',
    ['search', '$window', '$q', function (search, $window, $q) {
        var service = this;

        service.goBack = function () {
            $window.history.back();
        };

        service.queryItems = function (term, page) {
            return search.items({ term: term, page: page });
        };

        service.queryCourses = function (term, page) {
            return search.boxes({ term: term, page: page });
        };
    }]
);