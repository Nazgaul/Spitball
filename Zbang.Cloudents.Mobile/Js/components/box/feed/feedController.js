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
                content: extractUrls(feed.questionFormData.content),
            },
            qText = feed.questionFormData.content;

            setPostDetails(question);
            feed.questionFormData.content = null;

            feed.list.unshift(question);
            feed.empty = false;
            feedService.addQuestion(boxId, qText).then(function (questionId) {
                $scope.$emit('followBox');
                question.id = questionId;
            });
        };

        feed.toggleComment = function (question) {
            question.displayComment = true;// !question.displayComment;
        };


        feed.addAnswer = function (question) {
            var answer = {
                content: extractUrls(question.aFormData.content),
            },
            aText = question.aFormData.content;

            setPostDetails(answer);

            feed.empty = false;
            question.answers = question.answers || [];
            question.answers.unshift(answer);
            question.displayComment = false;
            question.aFormData.content = null;


            feedService.addAnswer(boxId, question.id, aText).then(function (answerId) {
                $scope.$emit('followBox');
                answer.id = answerId;
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
                    if (!isAppend) {
                        feed.empty = true;
                        return;
                    }
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

        function extractUrls(d) {
            if (!d) {
                return;
            }
            var urlex = /\b((?:https?:\/\/|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}\/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))/i;

            var array = d.match(urlex) || [];
            var matches = [];
            for (var i = 0, j = array.length; i < j; i++) {
                if (matches.indexOf(array[i]) < 0)
                    matches.push(array[i]);
            }
            if (!matches.length) {
                return d;
            }
            for (var i = 0; i < matches.length; i++) {
                var url = matches[i];
                if (!url) {
                    continue;
                }
                if (url.indexOf('http') !== 0) {
                    url = 'http://' + url;
                }
                d = d.replace(matches[i], "<a target=\"_blank\" href=\"" + url + "\">" + matches[i] + "</a>");
            }

            return d;
        }

    }]);
