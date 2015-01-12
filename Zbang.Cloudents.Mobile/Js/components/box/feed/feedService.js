angular.module('feed')
    .service('feedService',
    ['$analytics', 'feed', function ($analytics, feed) {
        var service = this;

        service.addQuestion = function (boxId, content, files) {
            $analytics.eventTrack('Add question', {
                category: 'Feed page',
                label: 'User added a question'
            });

            return feed.addQuestion({
                boxId: boxId,
                content: content,
                files: files
            });
        };

        service.addAnswer = function (boxId, questionId, content, files) {
            $analytics.eventTrack('Add answer', {
                category: 'Feed page',
                label: 'User added a answer'
            });
            return feed.addAnswer({
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