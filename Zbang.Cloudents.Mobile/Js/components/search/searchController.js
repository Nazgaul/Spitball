angular.module('search', ['ajax']).
    controller('SearchController',
    ['searchService', function (searchService) {
        var search = this;

        var coursesPage = 0,
            itemsPage = 0,          
            coursesEndResult,
            itemsEndResult;

        search.goBack = function () {
            searchService.goBack();
        };

        search.setCurrentTab = function (tab) {
            page = 0;
            search.currentTab = tab;
        };


        search.query = function (isAppend) {
            var term = search.formData.query;

            if (search.isSearching) {
                return;
            }

            if (term.length < 2) {
                return;
            }

            search.isSearching = true;

            if (!isAppend) {
                coursesPage = itemsPage = 0;

                searchService.queryAll(term).then(function (data) {
                    search.courses = data[0];
                    search.items = data[1];
                    coursesPage++;
                    itemsPage++;
                }).finally(function () {
                    search.isSearching = false;
                });

                return;
            }

            switch (search.currentTab) {
                case 'courses':
                    getCourses();
                    break;
                case 'items':
                    getItems();
                    break;
            }
        };

        //TODO: merge functions
        function getCourses() {
            var term = search.formData.query;
            searchService.queryCourses(term, coursesPage).then(function (courses) {
                if (!courses.length) {
                    coursesEndResult = true;
                    return;
                }

                coursesPage++;
                search.courses = search.courses.concat(courses);
                
            }).finally(function () {
                search.isSearching = false;
            });
        }

        search.setCurrentTab('courses');

        function getItems() {
            var term = search.formData.query;
            searchService.queryItems(term, itemsPage).then(function (items) {
                if (!items.length) {
                    itemsEndResult = true;
                    return;
                }

                itemsPage++;
                search.items = search.items.concat(items);
            }).finally(function () {
                search.isSearching = false;
            });
        }

        
    }]
);