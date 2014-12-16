var mQuiz = angular.module('mQuiz', ['timer']);
mQuiz.controller('QuizCtrl',
        ['$scope', '$window', '$timeout', '$filter', '$routeParams', 'sModal', 'sQuiz', 'sUserDetails', '$analytics', 'resManager', 'sLogin',
        function ($scope, $window, $timeout, $fliter, $routeParams, sModal, sQuiz, sUserDetails, $analytics, resManager, sLogin) {
            "use strict";
            var questions,
                challengeTimeout;

            $scope.profile = {
                userImage: sUserDetails.getDetails().image
            };

            $scope.formData = {
                quizId: $routeParams.quizId,
                answerSheet: []
            };


            sQuiz.data({ quizId: $routeParams.quizId, quizName: $routeParams.quizName, boxId: $routeParams.boxId }).then(function (response) {
                $scope.quiz = response.quiz;
                questions = angular.copy(response.quiz.questions, questions);
                if (sUserDetails.isAuthenticated()) {
                    var savedSheet = $window.localStorage.getItem($scope.quiz.id);
                    if (savedSheet) {
                        $scope.formData = JSON.parse(savedSheet);
                        submitResult();
                        setResults();
                        $window.localStorage.removeItem($scope.quiz.id);
                        $timeout(function () {
                            $scope.$emit('viewContentLoaded');
                        });
                        return;
                    }
                }

                populateTopUsers();
                if (response.sheet) {
                    response.sheet.answerSheet = response.sheet.questions;
                    $scope.formData = response.sheet;
                    $scope.formData.quizId = $routeParams.quizId;

                    if ($scope.formData) {
                        $scope.formData.answerSheet = _.map($scope.formData.answerSheet, function (answerSheetObj) {
                            var qObj = _.find($scope.quiz.questions, function (question) {
                                return question.id === answerSheetObj.questionId;
                            });

                            var aObj = _.find(qObj.answers, function (answer) {
                                return answer.id === answerSheetObj.answerId;
                            });

                            return { question: qObj, answer: aObj };
                        });
                        setResults();
                    }
                }
                function populateTopUsers() {
                    var topUsers = $scope.quiz.topUsers || [];
                    for (var i = topUsers.length; i < 3; i++) {
                        topUsers.push({ name: '', image: '' });
                    }
                }

                $timeout(function () {
                    $scope.$emit('viewContentLoaded');
                });

                if (!sUserDetails.iAuthenticated()) {
                    sLogin.connect();
                } else {
                    challengeTimeout = $timeout(function () {



                        if ($scope.quiz.testInProgress) {
                            return;
                        }
                        if ($scope.quiz.userDone) {
                            return;
                        }

                        sModal.open('quizChallenge', {
                            data: {
                                users: $scope.quiz.topUsers, // data
                                quizId: $scope.quiz.id
                            },
                            callback: {
                                close: function () {
                                    solveQuiz();
                                    getDiscussion();
                                    $scope.quiz.afraid = true;
                                },
                                dismiss: function () {
                                    startTimer();
                                }
                            }

                        });
                    }, 1000);

                }

            });

            $scope.timer = {
                state: resManager.get('Play')
            };

            //#region quiz
            $scope.takeQuiz = function () {
                if ($scope.quiz.afraid) {
                    $scope.quiz.afraid = false;
                    $scope.quiz.questions = _.clone(questions); //reset the data
                }
                $timeout.cancel(challengeTimeout);


                startResumeQuiz();
            };

            $scope.checkAnswers = function () {
                checkAnswers();
                $analytics.eventTrack('Quiz', {
                    category: 'Check Answers'
                });
                $scope.$broadcast('timer-stop');
            };

            $scope.retakeQuiz = function () {
                $timeout.cancel(challengeTimeout);
                resetQuiz();
                $analytics.eventTrack('Quiz', {
                    category: 'Retake'
                });
            };

            $scope.markCorrect = function (question, answer) {
                //add the answer delete the old one if exists and start/resume test if needed
                var oldAnswer = _.find($scope.formData.answerSheet, function (item) {
                    return item.question.id === question.id;
                });

                if (oldAnswer) {

                    if (oldAnswer.answer.id === answer.id) {
                        return;
                    }

                    var index = $scope.formData.answerSheet.indexOf(oldAnswer);
                    $scope.formData.answerSheet.splice(index, 1);
                }

                $scope.formData.answerSheet.push({ question: question, answer: answer });

                if ($scope.quiz.testInProgress) {
                    return;
                }


                $timeout.cancel(challengeTimeout);

                if ($scope.quiz.afraid) {
                    $scope.quiz.afraid = false;
                    $scope.quiz.questions = _.clone(questions); //reset the data
                }


                if ($scope.quiz.paused) {
                    resumeTimer();
                    return;
                }

                startTimer();

            };

            $scope.$on('timer-stopped', function (event, data) {
                if ($scope.quiz.paused) {
                    return;
                }

                $scope.formData.startTime = new Date(data.startTime),
                $scope.formData.endTime = new Date(data.endTime);
                $scope.formData.score = Math.round($scope.quiz.correctAnswers / $scope.quiz.questions.length * 100);
                $scope.formData.timeTaken = data.hours + ':' + data.minutes + ':' + data.seconds;



                if (!sUserDetails.isAuthenticated()) {
                    $window.localStorage.setItem($scope.quiz.id, JSON.stringify($scope.formData));
                    sLogin.registerAction();
                    return;

                }

                $scope.quiz.timeTaken = $scope.formData.timeTaken;
                $scope.quiz.userDone = true;
                setResults();
                //getDiscussion();
                submitResult();
            });

            $scope.closeQuiz = function () {
                $timeout.cancel(challengeTimeout);
            };

            function solveQuiz() {
                var question;
                for (var i = 0; i < $scope.quiz.questions.length; i++) {
                    question = $scope.quiz.questions[i];
                    question.correct = true;
                    var correctAnswer = _.find(question.answers, function (answer) {
                        return question.correctAnswer === answer.id;
                    });

                    correctAnswer.correct = true;
                    correctAnswer.isChecked = true;
                }
            }

            function checkAnswers() {
                $scope.quiz.correctAnswers = $scope.quiz.wrongAnswers = 0;

                var answerSheet = $scope.formData.answerSheet,
                    userAnswer, question;
                for (var i = 0; i < $scope.quiz.questions.length; i++) {
                    question = $scope.quiz.questions[i];
                    userAnswer = _.find(answerSheet, function (item) {
                        return item.question.id === question.id;
                    });
                    if (userAnswer) {
                        if (question.correctAnswer === userAnswer.answer.id) {
                            question.correct = true;
                            userAnswer.answer.correct = true;
                            $scope.quiz.correctAnswers++;
                            continue;
                        }

                        question.wrong = true;
                        userAnswer.answer.wrong = true;
                        $scope.quiz.wrongAnswers++;
                        markCorrect();
                        continue;
                    }

                    question.wrong = true;
                    question.noAnswer = true;
                    markCorrect();
                }


                function markCorrect() {
                    var correctAnswer = _.find(question.answers, function (answer) {
                        return question.correctAnswer === answer.id;
                    });

                    correctAnswer.correct = true;
                }
            }

            function setResults() {
                $scope.quiz.userDone = true;
                checkAnswers();
                $scope.quiz.timeTaken = $scope.formData.timeTaken.split('.')[0]; //remove millis
                $scope.quiz.result = $scope.formData.score;
                getDiscussion();
            }

            function submitResult() {
                $scope.formData.answers = _.map($scope.formData.answerSheet, function (answer) {
                    return { questionId: answer.question.id, answerId: answer.answer.id }
                });

                $scope.formData.boxId = $routeParams.boxId;
                sQuiz.saveAnswers($scope.formData);
            }

            function resetQuiz() {
                $scope.quiz.questions = _.clone(questions);
                $scope.formData.answerSheet = [];
                $scope.formData.startTime = null;
                $scope.quiz.testInProgress = false;
                $scope.quiz.userDone = false;
                $scope.timer.state = resManager.get('Play');
                $scope.$broadcast('timer-clear');

                angular.copy(questions, $scope.quiz.questions);

            }

            //#endregion

            //#region timer
            function startResumeQuiz() {
                if ($scope.quiz.testInProgress) {
                    pauseTimer();

                    $analytics.eventTrack('Quiz', {
                        category: 'Pause Quiz'
                    }); return;
                }


                if ($scope.quiz.paused) {
                    $analytics.eventTrack('Quiz', {
                        category: 'Resume Quiz'
                    });
                    resumeTimer();
                    return;
                }

                startTimer();
                $analytics.eventTrack('Quiz', {
                    category: 'Start Quiz'
                });
            };


            function startTimer() {
                $scope.$broadcast('timer-start');
                $scope.timer.state = resManager.get('Pause');
                $scope.quiz.testInProgress = true;
                $scope.formData.startTime = new Date();
            }

            function pauseTimer() {
                $scope.quiz.paused = true;
                $scope.$broadcast('timer-stop');
                $scope.quiz.testInProgress = false;
                $scope.timer.state = resManager.get('Play');
            }
            function resumeTimer() {
                $scope.$broadcast('timer-resume');
                $scope.quiz.paused = false;
                $scope.quiz.testInProgress = true;
                $scope.timer.state = resManager.get('Pause');
            }
            //#endregion

            //#region comments 


            $scope.getCommentsLength = function (comments) {
                if (!comments) {
                    return resManager.get('AddComment');
                }

                if (comments.length === 1) {
                    return comments.length + ' ' + resManager.get('Comment');
                }

                return comments.length + ' ' + resManager.get('Comments');
            };

            $scope.createComment = function (question) {
                var comment = {
                    date: new Date().toISOString(),
                    questionId: question.id,
                    text: question.newComment,
                    userId: sUserDetails.getDetails().id,
                    userName: sUserDetails.getDetails().name,
                    userPicture: sUserDetails.getDetails().image,
                    isDelete: true
                }
                if (!question.comments) {
                    question.comments = [];
                }

                question.comments.push(comment);
                question.newComment = '';
                sQuiz.discussion.createDiscussion({ questionId: question.id, text: comment.text }).then(function (response) {
                    comment.id = response;
                });

                $analytics.eventTrack('Quiz', {
                    category: 'Create Comment'
                });
            };

            $scope.deleteComment = function (question, comment) {
                var index = question.comments.indexOf(comment);
                question.comments.splice(index, 1);

                sQuiz.discussion.deleteDiscussion({ id: comment.id });
                $analytics.eventTrack('Quiz', {
                    category: 'Delete Comment'
                });
            };
            function getDiscussion() {
                sQuiz.discussion.getDiscussion({ quizId: $scope.quiz.id }).then(function (data) {
                    _.forEach(data, function (comment) {
                        var question = _.find($scope.quiz.questions, function (question2) {
                            return comment.questionId === question2.id;
                        });


                        if (!question.comments) {
                            question.comments = [];
                        }


                        comment.isDelete = sUserDetails.getDetails().id === comment.userId;
                        question.comments.push(comment);
                        question.newComment = ''; // fix for disable send button                        

                    });

                });
            }

            //#endregion




            //#region share

            $scope.shareFB = function () {
                //Facebook.share($scope.info.url, //url
                //      $scope.info.name, //title
                //       $scope.info.boxType === 'academic' ? $scope.info.name + ' - ' + $scope.info.ownerName : $scope.info.name, //caption
                //       jsResources.IShared + ' ' + $scope.info.name + ' ' + jsResources.OnCloudents + '<center>&#160;</center><center></center>' + jsResources.CloudentsJoin,
                //        null //picture
                //   ).then(function () {
                //       cd.pubsub.publish('addPoints', { type: 'shareFb' });
                //   });
            };

            $scope.shareEmail = function () {

            };
            //#endregion

        }
        ]);
