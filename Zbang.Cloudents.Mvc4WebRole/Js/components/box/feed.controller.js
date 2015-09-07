(function () {
    angular.module('app.box.feed').controller('FeedController', feed);
    feed.$inject = ['boxService', '$stateParams'];

    function feed(boxService, $stateParams) {
        var f = this;
        boxService.getFeed($stateParams.boxId).then(function (response) {
            f.data = response;
        });
    }
})();

(function () {
    angular.module('app.box.feed').controller('Recommended', recommended);
    recommended.$inject = ['boxService', '$stateParams'];

    function recommended(boxService, $stateParams) {
        var r = this;
        r.boxes = [];
        boxService.getRecommended($stateParams.boxId).then(function (response) {
            console.log(response);
            r.boxes = response;
        });
    }
})();
