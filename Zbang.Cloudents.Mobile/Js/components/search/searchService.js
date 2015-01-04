angular.module('search')
    .service('searchService',
    ['search', '$window', function (search, $window) {
        var service = this;

        service.goBack = function () {
            $window.history.back();
        };
    }]
);