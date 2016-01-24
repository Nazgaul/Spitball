(function () {
    angular.module('app.search').controller('SearchController', search);

    search.$inject = ['searchService', 'itemThumbnailService', '$q', '$rootScope', '$scope', 'Analytics',
        '$stateParams', '$location', '$state'];
    function search(searchService, itemThumbnailService, $q, $rootScope, $scope, analytics, $stateParams, $location, $state) {
        var self = this, page = 0, needToBringMore = true, term;
        self.state = {
            box: 'box',
            item: 'item',
            quiz: 'quiz',
        }
        self.boxes = [];
        self.items = [];
        self.quizzes = [];
        assignTab();

        $rootScope.$on('search-query', searchElements);
        self.changeTab = changeTab;

        self.back = function () {
            $rootScope.$broadcast('search-close');
            $scope.app.back('/dashboard/');
        }

        self.myPagingFunction = function () {
            //no paging in initial state
            //if (!self.term) {
            //    return createEmptyPromise();
            //}
            return doQuery(true);
        }

        //searchElements();
        function searchElements() {
            //    $location.search({
            //        'q': self.term,
            //        't': self.tab
            //    });
            term = $stateParams.q;
            //assignTab();
            page = 0;
            needToBringMore = true;
            doQuery();


        }
        function assignTab() {
            self.tab = self.state.box;
            self.tabIndex = 0;
            var index = 0;
            for (var prop in self.state) {
                if (self.state.hasOwnProperty(prop)) {
                    if (self.state[prop] === $stateParams.t) {
                        self.tab = self.state[prop];
                        self.tabIndex = index;
                    }
                }
                index++;
            }
        }

        function changeTab(tab) {
            self.tab = tab;
            $state.go('searchinfo', { q: $stateParams.q, t: self.tab });

            searchElements();
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
            analytics.trackPage($location.url(), 'Search');
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

