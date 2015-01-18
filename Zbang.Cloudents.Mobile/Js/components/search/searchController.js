angular.module('search', ['ajax']).
    controller('SearchController',
    ['$scope', 'searchService', function ($scope, searchService) {
        var search = this;

        var coursesPage = 0,
            itemsPage = 0,
            coursesEndResult,
            itemsEndResult,
            page;

        search.emptyResult = false;
        search.formData = {};

        search.setCurrentTab = function (tab) {
            page = 0;
            search.currentTab = tab;
            var term = search.formData.query;
            if (!term) {
                return;
            }
            switch (tab) {
                case 'courses':
                    getCourses();
                    break;
                case 'items':
                    getItems();
                    break;
            }

        };

        search.clearInput = function () {
            $scope.$broadcast('clearInput');
            search.formData.query = null;

        };


        search.query = function (isAppend) {
            var term = search.formData.query;

            if (search.isSearching) {
                return;
            }

            if (!term) {
                search.courses = search.items = [];
                return;
            }


            //if (!isAppend) {
            //    $location.search('q', term);
            //}

            search.loading = true;
            search.isSearching = true;

            switch (search.currentTab) {
                case 'courses':
                    getCourses(isAppend);
                    break;
                case 'items':
                    getItems(isAppend);
                    break;
            }
        };

        searchService.doneLoad();
        search.setCurrentTab('courses');

        //TODO: merge functions
        function getCourses(isAppend) {
            var term = search.formData.query;

            if (!isAppend) {
                coursesPage = 0;
            }
            search.loading = true;

            searchService.queryCourses(term, coursesPage).then(function (courses) {
                
                search.emptyResult = false;
                if (!courses.length && coursesPage === 0) {
                    search.emptyResult = true;
                }
                coursesPage++;
                if (!isAppend) {
                
                    search.courses = courses;
                    return;
                }

                search.courses = search.courses.concat(courses);

                if (!courses.length) {
                    coursesEndResult = true;
                }
                

            }).finally(function () {
                search.isSearching = false;
                search.loading = false;
            });
        }



        function getItems(isAppend) {
            var term = search.formData.query;

            if (!isAppend) {
                itemsPage = 0;
            }

            search.loading = true;

            searchService.queryItems(term, itemsPage).then(function (items) {
                search.emptyResult = false;
                if (!items.length && itemsPage === 0) {
                    search.emptyResult = true;

                }
                itemsPage++;
                if (!isAppend) {
                    
                    search.items = items;
                    return;
                }


                if (!items.length) {
                    itemsEndResult = true;
                    return;
                }

                search.items = search.items.concat(items);
                

            }).finally(function () {
                search.isSearching = false;
                search.loading = false;
            });
        }


    }]
);