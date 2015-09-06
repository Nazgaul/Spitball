(function () {
    angular.module('app.box.feed').controller('FeedController', feed);
    feed.$inject = ['boxService', '$stateParams'];

    function feed(boxService, $stateParams) {
        var f = this;
        //var boxData;
        boxService.getFeed($stateParams.boxId).then(function (response) {
            f.data = response;
            //    boxData = response;
            //    b.name = response.name;
            //    b.professorName = response.professorName;
            //    b.courseId = response.courseId;
        });
    }
})();

(function () {
    angular.module('app.box.feed').controller('Recommended', recommended);
    recommended.$inject = ['boxService', '$stateParams'];

    function recommended(boxService, $stateParams) {
        var r = this;
        r.boxes = [];
        //var boxData;
        boxService.getRecommended($stateParams.boxId).then(function (response) {
            console.log(response);
            r.boxes = response;
        });
    }
})();
