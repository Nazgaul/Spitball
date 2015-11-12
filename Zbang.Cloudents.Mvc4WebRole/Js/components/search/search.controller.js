(function() {
    angular.module('app.search').controller('SearchController', search);

    search.$inject = ['searchService','$location'];
    function search(searchService, $location) {
        var self = this;

        if (typeof $location.search().q === 'string') {
            self.term = $location.search().q;
        }
       
       

        self.search = function () {
            $location.search('q', self.term);
            searchService.search(self.term, 0).then(function (response) {
                self.result = response;
                console.log(response)
                for (var i = 0; i < self.result.items.length; i++) {
                    self.result.items[i].thumbnail = buildThumbnailUrl(self.result.items[i].source);
                }


            });
        }

        self.search();
        function buildThumbnailUrl(name) {
            return 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(name) + '.jpg?width=368&height=520&mode=crop&scale=both';
        }
    }
})();