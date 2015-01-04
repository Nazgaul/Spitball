angular.module('search')
    .service('searchService',
    ['search', '$window', function (search, $window) {
        var service = this;

        service.goBack = function () {
            $window.history.back();
        };

        service.queryItems = function (term, page) {
            return search.items();
        };

        service.queryCourses = function (term, page) {
            return search.boxes();
        };
    }]
);