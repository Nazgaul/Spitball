angular.module('search')
    .service('searchService',
    ['search', '$q', '$rootScope', '$analytics', '$state', function (search, $q, $rootScope, $analytics, $state) {
        var service = this;

        service.queryItems = function (term, page) {
            service.trackSearch(term,'item');
            return search.items({ term: term, page: page });
        };

        service.queryCourses = function (term, page) {
            service.trackSearch(term, 'box');
            return search.boxes({ term: term, page: page });
        };

        service.doneLoad = function () {
            $rootScope.$broadcast('$stateLoaded');
        };

        service.trackSearch = function (term, value) {
            $analytics.searchTrack($state.current.name, term, value);
        };

    }]
);