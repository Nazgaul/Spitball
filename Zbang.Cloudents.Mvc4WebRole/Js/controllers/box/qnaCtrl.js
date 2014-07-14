﻿//define('qnaCtrl', ['app'], function (app) {
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
            that.userId = data.userId;
            that.content = data.content.replace(/\n/g, '<br/>');
            that.rating = data.rating;
            that.iRate = data.iRate;
            that.isAnswer = data.answer;
            that.createTime = data.creationTime;
            that.files = data.files.map(function (file) { return new File(file) });
            that.isNew = NewUpdates.isNew($scope.boxId, 'answers', that.id);
        }

        function File(data) {
            var that = this;
            data = data || {};
            that.id = data.uid; //uid
            that.name = data.name;
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

        $scope.qFormData = {};

        $scope.$on('qna', function (e, questions) {
            $scope.info.questions = questions.map(function (question) {
                return new Question(question);
            });
            if (!questions) {
                $scope.info.state = states.none;
                return;
            }
            if (!questions.length) {
                $scope.info.state = states.empty;
                return;
            }

            $scope.info.state = states.questions;
        });

        $scope.answersLength = function (question) {
            if (question.answers.length === 1) {
                return question.answers.length + ' ' + JsResources.Comment;
            }

            return question.answers.length + ' ' + JsResources.Comments;
        };

        $scope.canDelete = function (obj) { //question || answer
            var userId = UserDetails.getDetails().id;
            return obj.userId === userId || $scope.info.ownerId === userId;
        };

        $scope.showAllAnswers = function (question) {
            $scope.qFormData = {};

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
            if (!UserDetails.isAuthenticated()) {
                //register popup
                cd.pubsub.publish('register', { action: true });
                return;
            }

            if (self.userType === 'none' || self.userType === 'owner') {
                alert(JsResources.NeedToFollowBox);
                return;
            }

            $scope.qFormData.boxUid = $scope.boxId;

            //add points;

            cd.analytics.trackEvent('Question', 'Add a question', 'The number of question added by users');
            //            cd.pubsub.publish('addPoints', { type: 'question' });
            var fileDisplay = $scope.qFormData.files;
            if ($scope.qFormData.files && $scope.qFormData.files.length) {
                $scope.qFormData.files = $scope.qFormData.files.map(function (file) {
                    return file.id;
                });
            }            

            QnA.post.question($scope.qFormData).then(function (response) {
                var questionId;
                if (response.Success) {
                    questionId = response.Payload;
                }
                var obj = {
                    id: questionId,
                    userName: UserDetails.getDetails().name,
                    userImage: UserDetails.getDetails().image,
                    userUid: UserDetails.getDetails().id, //uid
                    userUrl: UserDetails.getDetails().url,
                    content: extractUrls($scope.qFormData.content),
                    creationTime: new Date(),
                    answers: [],
                    files: fileDisplay || []
                }

                $scope.info.questions.unshift(new Question(obj));
                $scope.qFormData = {};

                if ($scope.info.state === states.empty) {
                    $scope.info.state = states.questions;
                }
            });


            //update time
            //notify

        };

        $scope.postAnswer = function (question) {
            if (!UserDetails.isAuthenticated()) {
                //register popup
                cd.pubsub.publish('register', { action: true });
                return;
            }

            if ($scope.$parent.info.userType === 'none' || $scope.$parent.info.userType === 'owner') { //parent is box controller
                alert(JsResources.NeedToFollowBox);
                return;
            }

            cd.analytics.trackEvent('Answer', 'Give answer', 'Providing answer ');
            //            cd.pubsub.publish('addPoints', { type: 'answer' });

            question.aFormData.questionId = question.id;
            question.aFormData.boxUid = $scope.boxId;
            var fileDisplay = question.aFormData.files;
            if (question.aFormData.files && question.aFormData.files.length) {
                question.aFormData.files = question.aFormData.files.map(function (file) {
                    return file.id;
                });
            }
            

            QnA.post.answer(question.aFormData).then(function (answerId) {
                var obj = {
                    id: answerId,
                    userName: UserDetails.getDetails().name,
                    userImage: UserDetails.getDetails().image,
                    userUid: UserDetails.getDetails().id, //uid
                    userUrl: UserDetails.getDetails().url,
                    content: extractUrls(question.aFormData.content),
                    rating: 0,
                    creationTime: new Date(),
                    iRate: false,
                    answer: false,
                    files: fileDisplay || []
                };

                question.answers.push(new Answer(obj));
                question.bestAnswer = findBestAnswer(question.answers);
                //updatetime
                //notify
                question.aFormData = {};

            });
        };

        $scope.deleteQuestion = function (question) {
            var index = $scope.info.questions.indexOf(question);
            $scope.info.questions.splice(index, 1);

            QnA.delete.question({ questionId: question.id }).then(function () {
                //notify
            });

            $scope.info.selectedQuestion = null;
            if (!$scope.info.questions.length) {
                $scope.info.state = states.empty;
                return;
            }
            $scope.info.state = states.questions;


        };

        $scope.deleteAnswer = function (question, answer) {
            var index = question.answers.indexOf(answer);
            question.answers.splice(index, 1);
            question.bestAnswer = findBestAnswer(question.answers);

            QnA.delete.answer({ answerId: answer.id }).then(function () {//TODO
                //SignalR notify
            });
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

        var qAttach, aAttach, questionAttach;
        $scope.$on('FileAdded', function (event, data) {
            file = new File(data.item);
            $scope.$apply(function () {
                if (data.boxId !== $scope.boxId) {
                    return;
                }

                if (qAttach) {
                    if (!($scope.qFormData.files && $scope.qFormData.files.length)) {
                        $scope.qFormData.files = [];
                    }
                    $scope.qFormData.files.push(file);
                    qAttach = false;
                    questionAttach = null;

                    return;
                }

                if (aAttach) {
                    if (!(questionAttach.aFormData.files && questionAttach.aFormData.files.length)) {
                        $scope.qFormData.files = [];
                    }
                    questionAttach.aFormData.files.push(file);
                    aAttach = true;
                    questionAttach = null;
                    return;
                }

            });
        });

        $scope.addQuestionAttachment = function () {
            qAttach = true;
            $scope.openUploadPopup(true).then(function (files) {
                qAttach = false;

                var mapped = files.map(function (file) {
                    file.uid = file.id;
                    return new File(file)
                });

                if (!$scope.qFormData.files) {
                    $scope.qFormData.files = mapped;
                    return;
                }

                $scope.qFormData.files = $scope.qFormData.files.concat(mapped);
            });
        };
        $scope.removeQuestionAttachment = function (file) {
            var index = $scope.qFormData.files.indexOf(file);
            if (index !== -1) {
                $scope.qFormData.files.splice(index, 1);
            }
        }

        $scope.addAnswerAttachment = function (question) {
            aAttach = true;
            questionAttach = question;
            $scope.openUploadPopup(true).then(function (files) {
                aAttach = false;
                questionAttach = null;

                var mapped = files.map(function (file) {
                    file.uid = file.id;
                    return new File(file)
                });

                if (!question.aFormData.files) {
                    question.aFormData.files = mapped;
                    return;
                }
                question.aFormData.files = question.aFormData.files.concat(mapped);

            });
        };
        $scope.removeAnswerAttachment = function (question, file) {
            var index = question.aFormData.files.indexOf(file);
            if (index !== -1) {
                question.aFormData.files.splice(index, 1);
            }            
        }

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
