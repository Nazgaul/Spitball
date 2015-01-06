angular.module('box', ['ajax']).
    controller('FeedController',
    ['feedService', '$stateParams', function (feedService, $stateParams) {
        var feed = this;

        var page = 0,
            endResult,
            isFetching,
            boxId = $stateParams.boxId;


        getFeedPage();

        feed.getFeedPage = function () {
            getFeedPage(true);
        };

        feed.addQuestion = function () {
            feedService.addQuestion(boxId, content/*,files*/);
            //add question to list

        };

        feed.addAnswer = function (question) {
            feedService.addAnswer(boxId, question.id, content/*files*/).then(function () {
                //add answer to question list
            });
        }


        function getFeedPage(isAppend) {
            if (isFetching) {
                return;
            }

            isFetching = true;

            feedService.getFeedPage(boxId, page).then(function (feedPage) {
                feedPage = feedPage || [];

                if (!feedPage) {
                    endResult = true;
                    return;
                }

                feed.list = feedPage;
                page++;

            }).finally(function () {
                isFetching = false;
            });
        }

    }]);
