(function() {
    angular.module('app.search').controller('SearchController', search);

    search.$inject = ['searchService','$location'];
    function search(searchService, $location) {
        var self = this, page = 0, loading = false, needToBringMore = true;
        

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



        self.myPagingFunction = function() {
            doQuery(true);
        }

        self.search();

        function doQuery(needToAppend) {
            if (!loading && needToBringMore) {
                loading = true;
                searchService.search(self.term, page).then(function(response) {
                    //self.result = response;
                    for (var i = 0; i < response.items.length; i++) {
                        response.items[i].thumbnail = buildThumbnailUrl(response.items[i].source);
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
                    loading = false;
                });
            }
        }
        function buildThumbnailUrl(name) {
            return 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(name) + '.jpg?width=368&height=520&mode=crop&scale=both';
        }
    }
})();