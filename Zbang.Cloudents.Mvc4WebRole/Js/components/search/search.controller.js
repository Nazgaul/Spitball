(function () {
    angular.module('app.search').controller('SearchController', search);

    search.$inject = ['searchService', '$location', 'itemThumbnailService', '$q'];
    function search(searchService, $location, itemThumbnailService, $q) {
        var self = this, page = 0, needToBringMore = true;


        if (typeof $location.search().q === 'string') {
            self.term = $location.search().q;
        }

        self.tab = 'b';

        self.search = function () {
            $location.search('q', self.term);
            page = 0;
            needToBringMore = true;
            doQuery();
        }



        self.myPagingFunction = function () {
            //no paging in initial state
            if (!self.term) {
                var defer = $q.defer();
                defer.resolve();
                return defer.promise;
            }
            return doQuery(true);
        }

        self.search();

        function doQuery(needToAppend) {
            if (!needToBringMore) {
                var defer = $q.defer();
                defer.resolve();
                return defer.promise;
            }
            return searchService.search(self.term, page).then(function (response) {
                //self.result = response;
                response.items = itemThumbnailService.assignValues(response.items);

                for (var j = 0; j < response.quizzes.length; j++) {
                    response.quizzes[j].publish = true;
                }
                if (needToAppend) {
                    self.result.boxes = self.result.boxes.concat(response.boxes);
                    self.result.items = self.result.items.concat(response.items);

                    self.result.quizzes = self.result.quizzes.concat(response.quizzes);
                } else {
                    self.result = response;
                }
                if (!response.boxes.length && !response.items.length && !response.quizzes.length) {
                    needToBringMore = false;
                }
                page++;
            });
        }


    }
})();