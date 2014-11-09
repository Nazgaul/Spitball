mBox.controller('QnACtrl',
['$scope', 'sModal', 'sUserDetails', 'sNewUpdates', 'sQnA', '$rootScope', '$analytics',

        function ($scope, sModal, sUserDetails, sNewUpdates, sQnA, $rootScope, $analytics) {
            "use strict";
            var jsResources = window.JsResources;
            function Question(data) {
                var that = this;
                data = data || {};
                that.id = data.id;
                that.userName = data.userName;
                that.userImage = data.userImage;
                that.userId = data.userId;
                that.content = data.content ? data.content.replace(/\n/g, '<br/>') : '';
                that.createTime = data.creationTime;
                sNewUpdates.isNew($scope.boxId, 'questions', that.id, function (isNew) {
                    that.isNew = isNew;
                });
                that.answers = data.answers.map(function (answer) {
                    var answerObj = new Answer(answer);
                    if (answerObj.isNew) {
                        that.isNew = true;
                    }
                    return answerObj;
                });
                that.files = data.files.map(function (file) { return new File(file); });
                that.aFormData = {};
            }

            function Answer(data) {
                var that = this;
                data = data || {};
                that.id = data.id;
                that.userName = data.userName;
                that.userImage = data.userImage;
                that.userId = data.userId;
                that.content = data.content ? data.content.replace(/\n/g, '<br/>') : '';
                that.rating = data.rating;
                that.iRate = data.iRate;
                that.isAnswer = data.answer;
                that.createTime = data.creationTime;
                that.files = data.files.map(function (file) { return new File(file); });
                sNewUpdates.isNew($scope.boxId, 'answers', that.id, function (isNew) {
                    that.isNew = isNew;
                });
            }

            function File(data) {
                var that = this;
                data = data || {};
                that.id = data.id;
                that.name = data.name;
                that.thumbnail = data.thumbnail;


                var userId = sUserDetails.getDetails().id;
                that.isOwner = data.ownerId === userId;
                that.isVisible = that.isOwner;

                that.itemUrl = data.url || data.itemUrl;
                that.download = that.itemUrl + 'download/';
            }



            $scope.info = {
                //$scope.boxId = we get this from parent scope no info
                userName: sUserDetails.getDetails().name,
                userImage: sUserDetails.getDetails().image
            };

            $scope.qFormData = {};


            sQnA.list({ boxId: $scope.boxId }).then(function (questions) {

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


            $scope.postQuestion = function () {
                if ($scope.$parent.info.userType === 'none' || $scope.$parent.info.userType === 'invite') {
                    alert(jsResources.NeedToFollowBox);
                    return;
                }

                $scope.qFormData.boxId = $scope.boxId;

                var fileDisplay = $scope.qFormData.files;
                if ($scope.qFormData.files && $scope.qFormData.files.length) {
                    $scope.qFormData.files = $scope.qFormData.files.map(function (file) {
                        return file.id;
                    });
                }
                $analytics.eventTrack('Feed', {
                    category: 'Add a question',
                    label: 'User post a question in the feed'
                });

                sQnA.post.question($scope.qFormData).then(function (questionId) {


                    var obj = {
                        id: questionId,
                        userName: sUserDetails.getDetails().name,
                        userImage: sUserDetails.getDetails().image,
                        userId: sUserDetails.getDetails().id,
                        userUrl: sUserDetails.getDetails().url,
                        content: extractUrls($scope.qFormData.content),
                        creationTime: new Date().toISOString(),
                        answers: [],
                        files: fileDisplay || []
                    }

                    $scope.info.questions.unshift(new Question(obj));
                    $scope.$broadcast('update-scroll');
                    $scope.qFormData = {};

                    cd.pubsub.publish('addPoints', { type: 'question' });



                    //cd.analytics.trackEvent('Question', 'Add a question', 'The number of question added by users');

                }, function () {
                    alert('Error');
                });
            };

            $scope.displayComment = function (question) {
                question.displayComment = !question.displayComment;
            };

            $scope.postAnswer = function (question) {

                if ($scope.$parent.info.userType === 'none' || $scope.$parent.info.userType === 'invite') { //parent is box controller
                    alert(jsResources.NeedToFollowBox);
                    return;
                }

                $analytics.eventTrack('Feed', {
                    category: 'Add an answer',
                    label: 'User post an answer in the feed'
                });
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
                    var answerId = response;


                    var obj = {
                        id: answerId,
                        userName: sUserDetails.getDetails().name,
                        userImage: sUserDetails.getDetails().image,
                        userId: sUserDetails.getDetails().id,
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


                    //updatetime
                    //notify
                    question.aFormData = {};

                }, function () {
                    alert('Error');
                });
            };

            $scope.deleteQuestion = function (question) {
                var index = $scope.info.questions.indexOf(question);
                $scope.info.questions.splice(index, 1);

                sQnA.delete.question({ questionId: question.id });

                //TODO analytics 
            };

            $scope.deleteAnswer = function (question, answer) {
                var index = question.answers.indexOf(answer);
                question.answers.splice(index, 1);
                sQnA.delete.answer({ answerId: answer.id });

                //TODO analytics 
            };

            $scope.removeAttachment = function (obj, item) {
                sQnA.delete.attachment({ itemId: item.id }).then(function () {                
                    var index = obj.files.indexOf(item);
                    obj.files.splice(index, 1);
                }, function (response) {
                        alert(response);
                });

                //TODO analytics 
            };

            $scope.downloadItem = function (event/*, item*/) {
                if (!sUserDetails.isAuthenticated()) {
                    event.preventDefault();
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                //TODO analytics 
            };

            var qAttach, aAttach, questionAttach;
            $scope.$on('ItemUploaded', function (e, data) {
                if (data.boxId !== $scope.boxId) {
                    return;
                }

                data.itemDto.uid = data.itemDto.id;

                if (data.newQuestion) {
                    if ($scope.qFormData.files && $scope.qFormData.files.length) {
                        $scope.qFormData.files.push(data.itemDto);
                        return;
                    }

                    $scope.qFormData.files = [data.itemDto];

                    return;
                }

                if (data.questionId) {
                    var question = _.find($scope.info.questions, function (q) {
                        return data.questionId === q.id;
                    });

                    if (!question) {
                        return;
                    }

                    if (question.aFormData.files && question.aFormData.files.length) {
                        question.aFormData.files.push(data.itemDto);
                        return;
                    }

                    question.aFormData.files = [data.itemDto];
                }
            });

            $scope.addQuestionAttachment = function () {

                if (!sUserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                var data = {
                    boxId: $scope.boxId,
                    newQuestion: true
                };

                openUpload(data);

                //TODO analytics 
            };

            $scope.removeQuestionAttachment = function (file) {
                var index = $scope.qFormData.files.indexOf(file);
                if (index !== -1) {
                    $scope.qFormData.files.splice(index, 1);
                }

                //TODO analytics 
            }

            $scope.addAnswerAttachment = function (question) {
                if (!sUserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                var data = {
                    boxId: $scope.boxId,
                    tabId: null,
                    questionId: question.id
                };

                openUpload(data);

                //TODO analytics 
            };
            $scope.removeAnswerAttachment = function (question, file) {
                var index = question.aFormData.files.indexOf(file);
                if (index !== -1) {
                    question.aFormData.files.splice(index, 1);
                }

                //TODO analytics 
            }

            $scope.checkAuth = function () {
                if (!sUserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return false;
                }

                return true;
            };

            function openUpload(data) {

                sModal.open('upload', {
                    data: data,
                    callback: {
                        close: function (response) {
                            $scope.followBox(true);
                        }
                    }
                });
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
            }

        }
]);
