angular.module('feed', ['ajax', 'monospaced.elastic', 'plupload', 'textDirection', 'displayTime']).
    controller('FeedController',
    ['$scope', 'feedService', '$stateParams', 'userDetails', function ($scope, feedService, $stateParams, userDetails) {
        var feed = this;

        var page = 0,
            endResult,
            isFetching,
            boxId = $stateParams.boxId;

        feed.questionFormData = {};

        getFeedPage();

        feed.getFeedPage = function () {
            getFeedPage(true);
        };

        feed.addQuestion = function () {     
            var question = {
                content: feed.questionFormData.content,
            };
            setPostDetails(question);

            feed.list.unshift(question);

            feedService.addQuestion(boxId, question.content).then(function (questionId) {
                $scope.emit('followBox');
                question.id = questionId;
                feed.questionFormData.content = null;
            });
        };

        feed.toggleComment = function (question) {
            question.displayComment = true;// !question.displayComment;
        };


        feed.addAnswer = function (question) {
            var answer = {
                content: question.aFormData.content,
            };

            setPostDetails(answer);


            question.answers.unshift(answer);
            question.aFormData = null;
            question.displayComment = false;

            feedService.addAnswer(boxId, question.id, answer.content).then(function (answerId) {
                $scope.emit('followBox');
                answer.id = answerId;
                question.aFormData.content = null;
            });
        };

        feed.checkLogin = function () {
            if (!userDetails.isAuthenticated()) {
                alert('Please register');
            }
        }

        function getFeedPage(isAppend) {
            if (isFetching) {
                return;
            }

            if (endResult) {
                return;
            }

            isFetching = true;
            feed.loading = true;

            feedService.getFeedPage(boxId, page).then(function (feedPage) {
                feedPage = feedPage || [];

                page++;


                if (!feedPage.length) {
                    endResult = true;
                    return;
                }

                if (!isAppend) {
                    feed.list = feedPage;
                    return;
                }

                feed.list = feed.list.concat(feedPage);



            }).finally(function () {
                isFetching = false;
                feed.loading = false;
            });
        }            

        function setPostDetails(post) {
            post.userName = userDetails.getName();
            post.userImage = userDetails.getImage();
            post.creationTime = new Date().toISOString();

            return post;
        }

    }]);
