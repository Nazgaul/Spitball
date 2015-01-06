angular.module('feed')
    .service('feedService',
    ['feed', function (feed) {
        var service = this;

        service.addQuestion = function (boxId, content, files) {
            return feed.addQuestion({
                boxId: boxId,
                content: content,
                files:files
            });
        };

        service.addAnswer = function (boxId, questionId, content, files) {
            return feed.addQuestion({
                boxId: boxId,
                questionId: questionId,
                content: content,
                files: files
            });
        };

        service.getFeedPage = function (boxId, page) {
            return feed.list({ boxId: boxId, page: page });
        };

    }]
);