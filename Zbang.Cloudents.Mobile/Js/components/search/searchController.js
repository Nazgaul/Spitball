angular.module('search', ['ajax']).
    controller('SearchController',
    ['searchService', '$location', function (searchService, $location) {
        var search = this;

        var coursesPage = 0,
            itemsPage = 0,
            coursesEndResult,
            itemsEndResult;

        search.formData = {};

        search.goBack = function () {
            searchService.goBack();
        };

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


        search.query = function (isAppend) {
            var term = search.formData.query;

            if (search.isSearching) {
                return;
            }

            if (term.length < 2) {
                return;
            }

            //if (!isAppend) {
            //    $location.search('q', term);
            //}

            search.isSearching = true;

            switch (search.currentTab) {
                case 'courses':
                    getCourses();
                    break;
                case 'items':
                    getItems();
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
            searchService.queryCourses(term, coursesPage).then(function (courses) {
                if (!courses.length) {
                    coursesEndResult = true;
                    return;
                }

                coursesPage++;

                if (!isAppend) {
                    search.courses = courses;
                    return;
                }

                search.courses = search.courses.concat(courses);

            }).finally(function () {
                search.isSearching = false;
            });
        }



        function getItems(isAppend) {
            var term = search.formData.query;

            if (!isAppend) {
                itemsPage = 0;
            }

            searchService.queryItems(term, itemsPage).then(function (items) {
                if (!items.length) {
                    itemsEndResult = true;
                    return;
                }

                itemsPage++;

                if (!isAppend) {
                    search.items = items;
                    return;
                }

                search.items = search.items.concat(items);

            }).finally(function () {
                search.isSearching = false;
            });
        }


    }]
);