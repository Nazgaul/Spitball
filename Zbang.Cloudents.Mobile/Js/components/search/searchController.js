angular.module('search', ['ajax']).
    controller('SearchController',
    ['searchService', function (searchService) {
        var search = this;

        search.goBack = function () {
            searchService.goBack();
        };
    }]
);