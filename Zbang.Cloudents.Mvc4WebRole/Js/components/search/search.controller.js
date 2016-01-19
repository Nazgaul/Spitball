(function () {
    angular.module('app.search').controller('SearchController', search);

    search.$inject = ['searchService', '$location', 'itemThumbnailService', '$q'];
    function search(searchService, $location, itemThumbnailService, $q) {
        var self = this, page = 0, needToBringMore = true, term;
        self.state = {
            box: 'box',
            item: 'item',
            quiz: 'quiz',
        }

        if (typeof $location.search().q === 'string') {
            term = $location.search().q;
        }
        self.tab = self.state.box;
        self.tabIndex = 0;
        if (typeof $location.search().t === 'string') {
            var index = 0;
            for (var prop in self.state) {
                if (self.state.hasOwnProperty(prop)) {
                    if (self.state[prop] === $location.search().t) {
                        self.tab = self.state[prop];
                        self.tabIndex = index;
                    }
                }
                index++;
            }
        }
        self.boxes = [];
        self.items = [];
        self.quizzes = [];




        self.search = function () {
            //    $location.search({
            //        'q': self.term,
            //        't': self.tab
            //    });
            page = 0;
            needToBringMore = true;
            doQuery();


        }
        self.changeTab = changeTab;





        self.myPagingFunction = function () {
            //no paging in initial state
            //if (!self.term) {
            //    return createEmptyPromise();
            //}
            return doQuery(true);
        }

        self.search();

        function changeTab(tab) {
            self.tab = tab;
            self.search();
        }

        function createEmptyPromise() {
            var defer = $q.defer();
            defer.resolve();
            return defer.promise;
        }

        function doQuery(needToAppend) {
            if (!needToBringMore) {
                return createEmptyPromise();
            }
            switch (self.tab) {
                case self.state.item:
                    return getItems(needToAppend);
                case self.state.quiz:
                    return getQuizzes(needToAppend);
                default:
                    return getBoxes(needToAppend);
            }

        }

        function getBoxes(needToAppend) {
            return searchService.searchBox(term, page).then(function (response) {
                self.noResults = false;
                if (needToAppend) {
                    self.result = self.result.concat(response);
                } else {
                    self.result = response;
                }
                if (!response.length) {
                    needToBringMore = false;
                    self.noResults = true;
                }
                page++;
            });
        }
        function getItems(needToAppend) {
            return searchService.searchItems(term, page).then(function (response) {
                self.noResults = false;
                angular.forEach(response, function (value) {
                    var retVal = itemThumbnailService.assignValue(value.source);
                    value.thumbnail = retVal.thumbnail;
                    value.nameExtension = value.name.replace(/\.[^/.]+$/, "");
                });
                if (needToAppend) {
                    self.result = self.result.concat(response);
                } else {
                    self.result = response;
                }
                if (!response.length) {
                    needToBringMore = false;
                    self.noResults = true;
                }
                page++;
            });
        }
        function getQuizzes(needToAppend) {
            return searchService.searchQuizzes(term, page).then(function (response) {
                self.noResults = false;
                for (var j = 0; j < response.length; j++) {
                    response[j].publish = true;
                }
                if (needToAppend) {
                    self.result = self.result.concat(response);
                } else {
                    self.result = response;
                }
                if (!response.length) {
                    needToBringMore = false;
                    self.noResults = true;
                }
                page++;
            });
        }


    }
})();

(function () {
    angular.module('app.search').controller('SearchTriggerController', searchTriggerController);
    searchTriggerController.$inject = ['$state', '$rootScope'];

    function searchTriggerController($state, $rootScope) {
        var st = this;

        st.search = search;
        st.change = search;


        function search(isValid) {
            console.log($state);
            if (isValid) {
                $state.go('search', { q: st.term });
                if ($state.current.name === 'search') {
                    $rootScope.$broadcast('search-query');
                }
            }
        }
    }
})()