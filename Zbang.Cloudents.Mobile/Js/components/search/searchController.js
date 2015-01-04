angular.module('search', ['ajax']).
    controller('SearchController',
    ['searchService', function (searchService) {
        var search = this;

        var page = 0;

        search.goBack = function () {
            searchService.goBack();
        };



        search.query = function () {

        };

        search.setCurrentTab = function (tab) {            
            page = 0;
            search.currentTab = tab;
        };
    }]
);