angular.module('search')
    .service('searchService',
    ['search', '$window', '$q', '$rootScope', function (search, $window, $q, $rootScope) {
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

        service.doneLoad = function () {
            $rootScope.$broadcast('$stateLoaded');
        };
    }]
);