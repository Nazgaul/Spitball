angular.module('search')
    .service('searchService',
    ['search', '$q', '$rootScope', '$analytics', function (search, $q, $rootScope, $analytics) {
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

        service.trackSearch = function (term, value) {
            $analytics.searchTrack(term, 'items');
        };

    }]
);