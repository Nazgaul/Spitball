angular.module('search')
    .service('searchService',
    ['search', '$q', '$rootScope', function (search, $q, $rootScope) {
        var service = this;

        service.queryItems = function (term, page) {
            return search.items({ term: term, page: page });
        };

        service.queryCourses = function (term, page) {
            return search.boxes({ term: term, page: page });
        };

        service.doneLoad = function () {
            $rootScope.$broadcast('$stateLoaded');
        };
    }]
);