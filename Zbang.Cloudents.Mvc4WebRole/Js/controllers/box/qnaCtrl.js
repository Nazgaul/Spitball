//define('qnaCtrl', ['app'], function (app) {
mBox.controller('QnACtrl',
['$scope', 'sUserDetails', 'sNewUpdates', 'sQnA', 'sBox', 'sFocus',

    function ($scope, UserDetails, NewUpdates, QnA, Box, Focus) {
        function Question(data) {
            var that = this;
            data = data || {};
            that.id = data.id;
            that.userName = data.userName;
            that.userImage = data.userImage;
            that.userId = data.userUid; //uid\
            that.content = data.content.replace(/\n/g, '<br/>');
            that.createTime = data.creationTime;
            that.answers = data.answers.map(function (answer) { return new Answer(answer) });
            that.files = data.files.map(function (file) { return new File(file) });
            that.isNew = NewUpdates.isNew($scope.boxId, 'questions', that.id);
            that.bestAnswer = findBestAnswer(that.answers);
        }

        function Answer(data) {
            var that = this;
            data = data || {};
            that.id = data.id;
            that.userName = data.userName;
            that.userImage = data.userImage;
            that.userId = data.userUid; //uid
            that.content = data.content.replace(/\n/g, '<br/>');
            that.rating = data.rating;
            that.iRate = data.iRate;
            that.isAnswer = data.answer;
            that.createtime = data.creationtime;
            that.files = data.files.map(function (file) { return new File(file) });
            that.isNew = NewUpdates.isNew($scope.boxId, 'answers', that.id);
        }

        function File(data) {
            var that = this;
            data = data || {};
            that.id = data.uid; //uid
            that.thumbnail = data.thumbnail;
            that.download = "/d/" + $scope.boxId + "/" + that.id;

            var userId = UserDetails.getDetails().id;
            that.isOwner = data.ownerId === userId;
            that.isVisible = userId || $scope.info.ownerId === userId;
            //anserId ???

            that.itemUrl = data.url;
        }

        var states = {
            none: 0,
            empty: 1,
            questions: 2,
            answers: 3
        };

        $scope.info = {
            //$scope.boxId = we get this from parent scope no info
            userName: UserDetails.getDetails().name,
            userImage: UserDetails.getDetails().image,
            selectedQuestion: null,
        };


        $scope.$on('qna', function (e, questions) {
            $scope.info.questions = questions.map(function (question) {
                return new Question(question);
            });
            if (!questions) {
                return states.none;
            }
            if (!questions.length) {
                return states.empty;
            }

            $scope.info.state = states.questions;
        });

        $scope.qFormData = {
            boxId: $scope.boxId
        };

        $scope.aFormData = {
            boxId: $scope.boxId
        };        

        $scope.answersLength = function (question) {
            if (question.answers.length === 1) {
                return question.answers.length + ' ' + JsResources.Comment;
            }

            return question.answers.length + ' ' + JsResources.Comments;
        };

        $scope.deleteAnswer = function (question, answer) {
            var index = question.answers.indexOf(answer);
            question.answers.splice(index, 1);

            QnA.deleteAnswer({ answerId: answer.id }).then(function () {//TODO
                //SignalR notify
            });
        }

        $scope.canDelete = function (obj) { //question || answer
            var userId = UserDetails.getDetails().id;
            return obj.userId === userId || $scope.info.ownerId === userId;
        };

        $scope.showAllAnswers = function (question) {
            $scope.info.selectedQuestion = question;

            //TODO update time

            ////NewUpdates.remove upadte //not sure
            question.isNew = false;

            for (var i = 0, l = question.answers.length; i < l; i++) {
                question.answers[i].isNew = false;
            }

            //cleartooltip ?

            $scope.info.state = states.answers;

            Focus('qna:answer');


        };

        $scope.showAllQuestion = function () {

            $scope.info.selectedQuestion = null;
            //refresh sccrooll

            if ($scope.info.questions) {
                $scope.info.state = states.questions;
                return;
            }

            $scope.info.state = states.empty;
        };

        $scope.postQuestion = function () {
            if (UserDetails.isAuthenticated()) {
                //register popup
                cd.pubsub.publish('register', { action: true });
            }

            if (self.userType === 'none' || self.userType === 'owner') {
                alert(JsResources.NeedToFollowBox);
            }

            //add points;

            //analytics.trackEvent('Question', 'Add a question', 'The number of question added by users');
            //            cd.pubsub.publish('addPoints', { type: 'question' });


            QnA.post.question($scope.qFormData).then(function (questionId) {
                var obj = {
                    id: questionId,
                    userName: UserDetails.getDetails().name,
                    userImage: UserDetails.getDetails().image,
                    userUid: UserDetails.getDetails().id, //uid
                    userUrl: UserDetails.getDetails().url,
                    content: extractUrls($scope.qFormData.cotent),
                    creationTime: new Date(),
                    answers: [],
                    files: $scope.formData.files
                }

                $scope.questions.unshift(new Question(obj));
                $scope.qFormData.content = $scope.qFormData.files = null;

            });


            //update time
            //notify

        };

        $scope.postAnswer = function (question) {
            if (UserDetails.isAuthenticated()) {
                //register popup
                cd.pubsub.publish('register', { action: true });
            }

            if (self.userType === 'none' || self.userType === 'owner') {
                alert(JsResources.NeedToFollowBox);
            }

            //            analytics.trackEvent('Answer', 'Give answer', 'Providing answer ');
            //            cd.pubsub.publish('addPoints', { type: 'answer' });

            $scope.aFormData.questionId = question.id;

            QnA.post.answer($scope.aFormData).then(function (answerId) {
                var obj = {
                    id: answerId,
                    userName: UserDetails.getDetails().name,
                    userImage: UserDetails.getDetails().image,
                    userUid: UserDetails.getDetails().id, //uid
                    userUrl: UserDetails.getDetails().url,
                    content: extractUrls($scope.aFormData.content),
                    rating: 0,
                    creationTime: new Date(),
                    iRate: false,
                    answer: false,
                    files: $scope.aFormData.files
                };

                question.answers.push(new Answer(obj));

                //updatetime
                //notify
                $scope.aFormData.content = $scope.aFormData.files = null;

            });
        };

        $scope.deleteQuestion = function (question) {
            var index = $scope.questions.indexOf(question);
            $scope.questions.splice(index, 1);

            QnA.delete.question({ questionId: question.id }).then(function () {
                //notify
            });

            $scope.info.state = states.questions;
        };
        //question 

        //content: "asdsadas"
        //creationTime: "2014-03-20T09:54:27Z"
        //files: []
        //id: "59d599bb-34f5-4692-ac84-a2f400c43b74"
        //url: "/user/1/ram"
        //userImage: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg"
        //userName: "guy golan"
        //userUid: 18372

        //answer 

        //answer: false
        //content: "asdasd"
        //creationTime: "2014-03-20T09:57:20Z"
        //files: []
        //iRate: false
        //id: "233b4bdc-3fb3-4be7-8ec5-a2f400c5063e"
        //questionId: "59d599bb-34f5-4692-ac84-a2f400c43b74"
        //rating: 0
        //url: "/user/1/ram"
        //userId: 18372
        //userImage: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg"
        //userName: "guy golan"

        $scope.addAnswer = function (question) {

        };
        $scope.deleteAnswer = function (question, answer) {

        };


        $scope.addFiles = function (question, answer) {

        };

        $scope.viewItem = function (link) {

        };

        $scope.deleteItem = function (item) {
        };

        $scope.downloadItem = function (event, item) {
            if (!UserDetails.isAuthenticated()) {
                event.preventDefault();
                cd.pubsub.publish('register', { action: true });
                return;
            }
        };

        function sortAnswers(a, b) {
            if (a.isAnswer) {
                return -1;
            }
            if (b.isAnswer) {
                return 1;
            }
            if (a.createTime > b.createTime) {
                return 1;
            }
            return -1;
        }

        function extractUrls(d) {
            var urlex = /\b((?:https?:\/\/|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}\/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))/i;
            var matches = ko.utils.arrayGetDistinctValues(d.match(urlex));
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
        }

        function findBestAnswer(answers) {
            for (var i = 0, l = answers.length; i < l; i++) {
                if (answers[i].isAnswer) {
                    return answers[i];
                }
            }

            var lastRate = 0, answer;
            angular.forEach(answers, function (a) {
                if (a.rating > lastRate) {
                    answer = a;
                    lastRate = a.rating;
                }
            });

            if (answer) {
                return answer;
            }

            if (answers.length) {
                return answers[0];
            }

            return null;
        }
    }
]);
//});