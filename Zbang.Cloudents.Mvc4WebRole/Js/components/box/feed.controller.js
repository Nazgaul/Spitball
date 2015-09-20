(function () {
    angular.module('app.box.feed').controller('FeedController', feed);
    feed.$inject = ['boxService', '$stateParams'];

    function feed(boxService, $stateParams) {
        var f = this;
        boxService.getFeed($stateParams.boxId).then(function (response) {
            f.data = response;

            for (var i = 0; i < f.data.length; i++) {
                for (var j = 0; j < f.data[i].files.length; j++) {
                    f.data[i].files[j].thumbnail = 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(f.data[i].files[j].source) + '.jpg?width=100&height=125&mode=crop';
                }
                for (var k = 0; k < f.data[i].answers.length; k++) {
                    for (var l = 0; l < f.data[i].answers[k].files; l++) {
                        f.data[i].answers[k].files[l].thumbnail = 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(f.data[i].answers[k].files[l].source) + '.jpg?width=100&height=125&mode=crop';
                    }
                }
            }
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
