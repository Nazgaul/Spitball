(function () {
    'use strict';
    angular.module('app.quiz').controller('QuizController', quiz);

    quiz.$inject = ['data', '$timeout', '$stateParams', 'quizService', '$filter',
        'userDetailsFactory', '$rootScope', '$state'];

    function quiz(quizData, $timeout, $stateParams, quizService, $filter,
        userDetailsFactory, $rootScope, $state) {

        var q = this;
        q.states = {
            before: 1,
            solving: 2,
            complete: 3,
            solved: 4
        };
        q.state = q.states.before;
        q.timerControl = {};
        q.isSolved = false;
        q.answersCount = 0;

        q.checkAnswers = checkAnswers;
        q.start = start;
        q.selectAnswer = selectAnswer;
        q.isCorrect = isQuestionCorrect;
        q.isCorrectAnswer = isCorrectAnswer;
        q.retakeQuiz = retakeQuiz;
        q.postComment = postComment;
        q.removeComment = removeComment;
        q.next = next;
        q.back = back;
        q.swipeLeft = next;
        q.swipeRight = back;
        q.user = {
            id: userDetailsFactory.get().id,
            name: userDetailsFactory.get().name,
            image: userDetailsFactory.get().image
        };

        q.name = quizData.quiz.name;
        q.questions = quizData.quiz.questions;
        q.index = 0;
        q.sheet = quizData.sheet || {};
        q.topUsers = quizData.quiz.topUsers || [];

        for (var j = q.topUsers.length; j < 4; j++) {
            q.topUsers.push({
                image: '/images/site/user_' + j + '.png'
            });
        }

        q.boxUrl = $state.href("box.quiz", angular.extend({}, $stateParams)); //quizData.quiz.boxUrl;

        q.afraid = afraid;
        if (quizData.sheet) {
            setResults();
            getDiscussion();
            q.isSolved = true;
            q.state = q.states.solved;
            if (q.sheet.timeTaken.indexOf('.') > -1) { //00:00:00.12345
                q.sheet.timeTaken = q.sheet.timeTaken.split('.')[0];
            }

            q.sheet.correct = Math.round(q.sheet.score / 100 * q.questions.length);
            q.sheet.wrong = q.questions.length - q.sheet.correct;
            return;
        }
        quizService.getNumberOfSolvers({ quizId: $stateParams.quizId }).then(function (response) {
            q.topUsers = response.users;
            for (var jj = q.topUsers.length; jj < 4; jj++) {
                q.topUsers.push({ name: '', image: '/images/site/user_' + jj + '.png' });
            }
            q.classmatesCount = response.solversCount;
        });

        //q.backToBox = backToBox;

        //function backToBox() {
        //    if ($previousState.get()) {
        //        $previousState.go();
        //    }
        //    $location.url(q.boxUrl);
        //}


        function start() {
            if (!q.user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            if (q.timerControl.isRunning) {
                q.timerControl.pause();
                return;
            }
            if (q.isSolved) {
                reset();
                q.timerControl.reset();
                q.timerControl.start();
                return;
            }
            q.state = q.states.solving;
            q.timerControl.start();

        }

        function afraid() {
            if (!q.user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            q.isSolved = true;
            q.sheet.score = 0;
            q.sheet.correct = 0;
            q.sheet.wrong = q.questions.length;
            q.sheet.timeTaken = '00:00:00';
            q.state = q.states.solved;
            getDiscussion();
            solveQuiz();
        }

        function reset() {
            q.isSolved = false;
            q.state = q.states.solving;
            q.answersCount = 0;
            q.sheet = {};
            q.index = 0;
            angular.forEach(q.questions, function (v) {
                v.isCorrect = undefined;
                v.selectedAnswer = null;
            });
        }

        function solveQuiz() {
            angular.forEach(q.questions, function (question) {
                //var found = false;
                var answer;
                for (var i = 0; i < question.answers.length /*&& !found*/; i++) {
                    answer = question.answers[i];
                    if (question.correctAnswer === answer.id) {
                        question.selectedAnswer = answer;
                        question.isCorrect = true;
                    }
                }
            });
        }


        function setResults() {
            if (q.sheet == null) { //user answered
                return;
            }
            var question, answer;
            angular.forEach(q.sheet.questions, function (map) {
                for (var i = 0 ; i < q.questions.length; i++) {
                    if (q.questions[i].id !== map.questionId) {
                        continue;
                    }
                    question = q.questions[i];

                    for (var jj = 0; jj < question.answers.length; jj++) {
                        if (question.answers[jj].id !== map.answerId) {
                            continue;
                        }

                        answer = question.answers[jj];
                        assignAnswerToQuestion(question, answer);
                    }
                }
            });
        }

        function checkAnswers() {
            q.isSolved = true;
            q.timerControl.pause();
            q.state = q.states.solved;
            q.index = 0;
            sendData();
            getDiscussion();
        }

        function retakeQuiz() {
            reset();
            q.timerControl.reset();
            q.timerControl.toggle();
        }



        function isQuestionCorrect(question) {
            if (!q.isSolved) {
                return null;
            }
            if (angular.isUndefined(question.isCorrect)) {
                return 'no-answer';
            }
            return question.isCorrect ? 'correct' : 'wrong';


        }



        function isCorrectAnswer(question, answer) {
            if (!q.isSolved) {
                return false;
            }

            return question.correctAnswer === answer.id ? true : false;
        }

        function selectAnswer(question, answer) {
            if (q.isSolved) {
                return;
            }
            q.timerControl.start();
            assignAnswerToQuestion(question, answer);

            $timeout(function () {
                next();
            }, 20);
        }

        function assignAnswerToQuestion(question, answer) {
            var reAnswered = typeof question.selectedAnswer !== "undefined" && question.selectedAnswer !== null;
            question.selectedAnswer = answer;
            question.isCorrect = question.correctAnswer === answer.id;
            if (!reAnswered) {
                q.answersCount++;
            }
        }




        function next() {
            if (q.questions.length - 1 > q.index) {
                q.index++;
            } else {
                if (q.state === q.states.solving) {
                    q.state = q.states.complete;
                }
            }
        }

        function back() {
            if (q.state === q.states.complete) {
                q.state = q.states.solving;
            }
            if (q.index > 0) {
                q.index--;
            }


        }

        function sendData() {
           // cacheFactory.clearAll(); //autofollow issue
            var data = {
                boxId: $stateParams.boxId,
                quizId: $stateParams.quizId,
                numberOfMilliseconds: q.timerControl.getTime()
            };

            data.answers = [];
            var question;
            for (var i = 0; i < q.questions.length; i++) {
                question = q.questions[i];
                if (angular.isObject(question.selectedAnswer)) {
                    data.answers.push({ questionId: question.id, answerId: question.selectedAnswer.id });
                }
            }
            var timeTaken = $filter('stopwatch')(q.timerControl.getTime()),
                    correct = 0;
            angular.forEach(q.questions, function (c) {
                if (c.isCorrect === true) {
                    correct++;
                }
            });
            var wrong = q.questions.length - correct;
            q.sheet.score = Math.round(correct / q.questions.length * 100);
            q.sheet.correct = correct;
            q.sheet.wrong = wrong;
            q.sheet.timeTaken = timeTaken;
            quizService.saveAnswers(data);
        }

        function getDiscussion() {
            quizService.getDiscussion({ quizId: $stateParams.quizId }).then(function (data) {
                angular.forEach(q.questions, function (qq) {
                    qq.comments = [];
                });

                angular.forEach(data, function (comment) {
                    var question = $filter('filter')(q.questions, function (c) {
                        return comment.questionId === c.id;
                    });

                    question = question[0];

                    if (!question.comments) {
                        question.comments = [];
                    }


                    comment.isDelete = q.user.id === comment.userId;
                    question.comments.push(comment);
                    question.newComment = ''; // fix for disable send button                        

                });
            });
        }

        function postComment(question, myform) {

            var comment = {
                questionId: question.id,
                content: question.newComment,
                creationTime: new Date().toISOString(),
                userId: q.user.id,
                userName: q.user.name,
                userImage: q.user.image,
                isDelete: true
            };



            quizService.createDiscussion({ questionId: comment.questionId, text: comment.content }).then(function (id) {
                if (!question.comments) {
                    question.comments = [];
                }
                myform.$setPristine();
                myform.$setUntouched();
                question.newComment = '';
                comment.id = id;
                question.comments.unshift(comment);
            });
        }

        function removeComment(question, comment) {
            var index = question.comments.indexOf(comment);
            question.comments.splice(index, 1);

            quizService.removeDiscussion({ id: comment.id });

        }
    }


})();


