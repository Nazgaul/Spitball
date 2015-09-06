(function () {
    angular.module('app.box.feed').controller('FeedController', feed);
    feed.$inject = ['feedService', '$stateParams'];

    function feed(feedService, $stateParams) {
        var f = this;
        //var boxData;
        feedService.getFeed($stateParams.boxId).then(function (response) {
            console.log(response);
            f.data = response;
            //    boxData = response;
            //    b.name = response.name;
            //    b.professorName = response.professorName;
            //    b.courseId = response.courseId;
        });
    }
})();


(function () {
    angular.module('app.box.feed').service('feedService', feed);
    feed.$inject = ['ajaxService'];

    function feed(ajaxservice) {
        var d = this;
        d.getFeed = function (boxid) {
            return ajaxservice.get('/qna/', { id: boxid });
        }

    }
})();