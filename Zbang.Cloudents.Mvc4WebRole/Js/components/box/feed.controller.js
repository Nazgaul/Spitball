(function () {
    angular.module('app.box.feed').controller('FeedController', feed);
    feed.$inject = ['boxService', '$stateParams'];

    function feed(boxService, $stateParams) {
        var f = this;
        boxService.getFeed($stateParams.boxId).then(function (response) {
            f.data = response;

            for (var i = 0; i < f.data.length; i++) {
                for (var j = 0; j < f.data[i].files.length; j++) {
                    f.data[i].files[j].thumbnail = buildThumbnailUrl(f.data[i].files[j].source);
                }
                for (var k = 0; k < f.data[i].answers.length; k++) {
                    for (var l = 0; l < f.data[i].answers[k].files; l++) {
                        f.data[i].answers[k].files[l].thumbnail = buildThumbnailUrl(f.data[i].answers[k].files[l].source);
                    }
                }
            }
        });

        function buildThumbnailUrl(name) {
            return 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(name) + '.jpg?width=100&height=125&mode=crop&scale=canvas';
        }
    }
})();

