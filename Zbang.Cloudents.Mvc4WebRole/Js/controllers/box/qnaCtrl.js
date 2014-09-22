//define('qnaCtrl', ['app'], function (app) {
mBox.controller('QnACtrl',
['$scope', 'sUserDetails', 'sNewUpdates', 'sQnA','$rootScope',

    function ($scope, sUserDetails, sNewUpdates, sQnA, $rootScope) {
        var jsResources = window.JsResources;
        function Question(data) {
            var that = this;
            data = data || {};
            that.id = data.id;
            that.userName = data.userName;
            that.userImage = data.userImage;
            that.userId = data.userUid; //uid\
            that.content = data.content.replace(/\n/g, '<br/>');
            that.createTime = data.creationTime;
            that.isNew = sNewUpdates.isNew($scope.boxId, 'questions', that.id);
            that.answers = data.answers.map(function (answer) {
                var answerObj = new Answer(answer);
                if (answerObj.isNew) {
                    that.isNew = true;
                }
                return answerObj;
            });
            that.files = data.files.map(function (file) { return new File(file); });            
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
            that.files = data.files.map(function (file) { return new File(file); });
            that.isNew = sNewUpdates.isNew($scope.boxId, 'answers', that.id);
        }

        function File(data) {
            var that = this;
            data = data || {};
            that.id = data.uid || data.id; //uid
            that.name = data.name;
            that.thumbnail = data.thumbnail;
            that.download = data.downloadUrl; //"/d/" + $scope.boxId + "/" + that.id;

            var userId = sUserDetails.getDetails().id;
            that.isOwner = data.ownerId === userId;
            that.isVisible = that.isOwner;
            //anserId ???

            that.itemUrl = data.url || data.itemUrl;
        }

        //var states = {
        //    none: 0,
        //    empty: 1,
        //    questions: 2,
        //    answers: 3
        //};

        $scope.focused = false;

        $scope.formFocus = function ($event) {
           $scope.focused = false;
           $event.stopPropagation();
           $scope.focused = true;
       }

        $scope.info = {
            //$scope.boxId = we get this from parent scope no info
            userName: sUserDetails.getDetails().name,
            userImage: sUserDetails.getDetails().image
        };

        $scope.qFormData = {};


        sQnA.list({ boxId: $scope.boxId, uniName: $scope.uniName, boxName: $scope.boxName }).then(function (response) {


            var questions = response.success ? response.payload : {}
            $scope.info.questions = questions.map(function (question) {
                return new Question(question);
            });

            $scope.options.loader = false;

            //if (!questions) {
            //  //  $scope.info.state = states.none;
            //    return;
            //}
            //if (!questions.length) {
            //    $scope.info.state = states.empty;
            //    return;
            //}

            //$scope.info.state = states.questions;
        });

        $scope.answersLength = function (question) {
            if (question.answers.length === 1) {
                return question.answers.length + ' ' + jsResources.Comment;
            }

            return question.answers.length + ' ' + jsResources.Comments;
        };

        $scope.canDelete = function (obj) { //question || answer
            var userId = sUserDetails.getDetails().id;
            return obj.userId === userId || $scope.info.ownerId === userId || sUserDetails.getDetails().score > 1000000;
        };

        //$scope.showAllAnswers = function (question) {
        //    $scope.qFormData = {};

        //    $scope.info.selectedQuestion = question;
            
            

            
        //    question.isNew = false;
        //    sNewUpdates.setOld($scope.boxId, 'questions', question.id);

        //    for (var i = 0, l = question.answers.length; i < l; i++) {
        //        question.answers[i].isNew = false;
        //        sNewUpdates.setOld($scope.boxId, 'answers', question.answers[i].id);
        //    }
            
        //    //cleartooltip ?

        //    //$scope.info.state = states.answers;
            
        //    sFocus('qna:answer');
        //    //TODO: temp solution
        //    window.setTimeout(cd.updateTimeActions, 1000);


        //};

        //$scope.showAllQuestion = function () {

        //    $scope.info.selectedQuestion = null;
        //    //refresh sccrooll
        //    //todo: temp solution
        //    window.setTimeout(cd.updateTimeActions, 1000);
        //    if ($scope.info.questions) {
        //        //$scope.info.state = states.questions;
        //        return;
        //    }

        //    //$scope.info.state = states.empty;
            
        //};

        $scope.postQuestion = function () {
            if ($scope.$parent.info.userType === 'none' || $scope.$parent.info.userType === 'invite') {
                alert(jsResources.NeedToFollowBox);
                return;
            }

            $scope.qFormData.boxId = $scope.boxId;

            cd.pubsub.publish('addPoints', { type: 'question' });

            cd.analytics.trackEvent('Question', 'Add a question', 'The number of question added by users');
            //            cd.pubsub.publish('addPoints', { type: 'question' });
            var fileDisplay = $scope.qFormData.files;
            if ($scope.qFormData.files && $scope.qFormData.files.length) {
                $scope.qFormData.files = $scope.qFormData.files.map(function (file) {
                    return file.id;
                });
            }

            sQnA.post.question($scope.qFormData).then(function (response) {
                var questionId;
                if (response.success) {
                    questionId = response.payload;
                }
                var obj = {
                    id: questionId,
                    userName: sUserDetails.getDetails().name,
                    userImage: sUserDetails.getDetails().image,
                    userUid: sUserDetails.getDetails().id, //uid
                    userUrl: sUserDetails.getDetails().url,
                    content: extractUrls($scope.qFormData.content),
                    creationTime: new Date().toISOString(),
                    answers: [],
                    files: fileDisplay || []
                }

                $scope.info.questions.unshift(new Question(obj));
                $scope.$broadcast('update-scroll');
                $scope.qFormData = {};

                //if ($scope.info.state === states.empty) {
                //    $scope.info.state = states.questions;
                //}
            });


            //update time
            //notify

        };

        $scope.postAnswer = function (question) {
            
            if ($scope.$parent.info.userType === 'none' || $scope.$parent.info.userType === 'invite') { //parent is box controller
                alert(jsResources.NeedToFollowBox);
                return;
            }

            cd.analytics.trackEvent('Answer', 'Give answer', 'Providing answer ');

            cd.pubsub.publish('addPoints', { type: 'answer' });

            question.aFormData.questionId = question.id;
            question.aFormData.boxId = $scope.boxId;
            var fileDisplay = question.aFormData.files;
            if (question.aFormData.files && question.aFormData.files.length) {
                question.aFormData.files = question.aFormData.files.map(function (file) {
                    return file.id;
                });
            }


            sQnA.post.answer(question.aFormData).then(function (response) {
                var answerId;
                if (response.success) {
                    answerId = response.payload;
                }

                var obj = {
                    id: answerId,
                    userName: sUserDetails.getDetails().name,
                    userImage: sUserDetails.getDetails().image,
                    userId: sUserDetails.getDetails().id, //uid
                    userUrl: sUserDetails.getDetails().url,
                    content: extractUrls(question.aFormData.content),
                    rating: 0,
                    creationTime: new Date().toISOString(),
                    iRate: false,
                    answer: false,
                    files: fileDisplay || []
                };

                question.answers.push(new Answer(obj));
                $scope.$broadcast('update-scroll');

                question.bestAnswer = findBestAnswer(question.answers);
                //updatetime
                //notify
                question.aFormData = {};

            });
        };

        $scope.deleteQuestion = function (question) {
            var index = $scope.info.questions.indexOf(question);
            $scope.info.questions.splice(index, 1);

            sQnA.delete.question({ questionId: question.id }).then(function () {
                //notify
            });

            //$scope.info.selectedQuestion = null;
            //if (!$scope.info.questions.length) {
            //    //$scope.info.state = states.empty;
            //    return;
            //}
           // $scope.info.state = states.questions;


        };

        $scope.deleteAnswer = function (question, answer) {
            var index = question.answers.indexOf(answer);
            question.answers.splice(index, 1);
            question.bestAnswer = findBestAnswer(question.answers);

            sQnA.delete.answer({ answerId: answer.id }).then(function () {//TODO SignalR notify
            });
        };

        $scope.removeAttachment = function (obj, item) {
            sQnA.delete.attachment({ itemId: item.id }).then(function (response) {
                if (!response.success) {
                    alert(response.payload);
                    return;
                }

                var index = obj.files.indexOf(item);
                obj.files.splice(index, 1);
            });
        };

        $scope.downloadItem = function (event/*, item*/) {
            if (!sUserDetails.isAuthenticated()) {
                event.preventDefault();
                cd.pubsub.publish('register', { action: true });
                return;
            }
        };

        var qAttach, aAttach, questionAttach;
        $scope.$on('FileAdded', function (event, data) {
           var file = data.item;
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
                        questionAttach.aFormData.files = [];
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

            $rootScope.$broadcast('openUpload', true);
            //$scope.openUploadPopup(true).then(function (files) {
            //    qAttach = false;

            //    var mapped = files.map(function (file) {
            //        file.uid = file.id;
            //        return file;
            //    });

            //    if (!$scope.qFormData.files) {
            //        $scope.qFormData.files = mapped;
            //        return;
            //    }

            //    $scope.qFormData.files = $scope.qFormData.files.concat(mapped);
            //});
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
                    return file;
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

        $scope.checkAuth = function () {
            if (!sUserDetails.isAuthenticated()) {
                cd.pubsub.publish('register', { action: true });
                return false;
            }
            
            return true;
        };

        //function sortAnswers(a, b) {
        //    if (a.isAnswer) {
        //        return -1;
        //    }
        //    if (b.isAnswer) {
        //        return 1;
        //    }
        //    if (a.createTime > b.createTime) {
        //        return 1;
        //    }
        //    return -1;
        //}

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
