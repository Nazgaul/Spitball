(function () {
    angular.module('app.quiz').controller('QuizController', quiz);

    quiz.$inject = ['$scope', '$stateParams', 'quizService', '$sce', '$location', '$timeout', '$uibModal', '$modalStack', '$filter', 'accountService'];

    function quiz($scope, $stateParams, quizService, $sce, $location, $timeout, $uibModal, $modalStack, $filter, accountService) {
        var q = this;
        
        q.timerControl = {};
        q.isSolved = false;

        q.createId = getId;
        q.checkAnswers = checkAnswers;
        q.start = start;
        q.selectAnswer = selectAnswer;
        q.isCorrect = isQuestionCorrect;
        q.isCorrectAnswer = isCorrectAnswer;
        q.isSelectedAnswer = isSelectedAnswer;
        q.buttonText = getButtonText;

        q.postComment = postComment;
        q.removeComment = removeComment;

        accountService.getAccountDetails().then(function (data) {
            q.user = {
                id: data.id,
                name: data.name,
                image: data.image
            };
        });

        $scope.$on('$destroy', function () {
            $modalStack.dismissAll();
        });

        quizService.getQuiz($stateParams.boxId, $stateParams.quizId).then(function (data) {
            q.name = data.quiz.name
            q.questions = data.quiz.questions;
            q.sheet = data.sheet;
            q.topUsers = data.quiz.topUsers;
            q.boxUrl = data.quiz.boxUrl;


            if (q.sheet != null) {
                setResults();
                getDiscussion();
                q.isSolved = true;                
                q.timerControl.setTime(q.sheet.timeTaken);
                var score = q.sheet.score,
                      correct = Math.round(q.sheet.score / 100 * q.questions.length),
                      wrong = q.questions.length - correct,
                      timeTaken = q.sheet.timeTaken;

                openScorePopup({
                    score: score,
                    correct: correct,
                    wrong: wrong,
                    timeTaken: timeTaken
                });
                return;
            }

            $timeout(function () {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'quizChallenge.html',
                    controller: 'QuizChallengeController',
                    controllerAs: 'c',
                    backdrop: 'static',
                    windowClass: 'challenge-modal',
                    resolve: {
                        topUsers: function () {
                            return q.topUsers;
                        }
                    }
                });

                modalInstance.result.then(function () {
                    start();
                }, function () {
                    afraid();
                });
            }, 1000);

        });


        function start() {
            if (q.timerControl.isRunning) {
                q.timerControl.pause();
                return;
            }
            if (q.isSolved) {
                q.isSolved = false;
                reset();
                q.timerControl.reset();
                q.timerControl.start();
                return;
            }

            q.timerControl.start();

        }

        function afraid() {
            q.isSolved = true;
            solveQuiz();
        }

        function seeAnswers() {
            q.isSolved = true;
            //setResults();
        }

        function resume() {
            if (!q.timerControl.isRunning) {
                q.timerControl.toggle();
            }
        }


        //function pause() {

        //}

        function reset() {
            q.isSolved = false;
            q.sheet = null;
            angular.forEach(q.questions, function (q) {

                q.isCorrect = undefined;
                q.selectedAnswer = null;
            });
        }

        function solveQuiz() {
            angular.forEach(q.questions, function (question) {
                var found = false;
                var answer;
                for (var i = 0; i < question.answers.length && !found; i++) {
                    answer = question.answers[i];
                    if (question.correctAnswer == answer.id) {
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
                    if (q.questions[i].id != map.questionId) {
                        continue;
                    }
                    question = q.questions[i];

                    for (var j = 0; j < question.answers.length; j++) {
                        if (question.answers[j].id != map.answerId) {
                            continue;
                        }

                        answer = question.answers[j];
                        selectAnswer(question, answer);
                    }
                }
            });
        }

        function checkAnswers() {
            q.isSolved = true;
            q.timerControl.pause();
            sendData();
            getDiscussion();
        }

        function retakeQuiz() {
            reset();
            q.timerControl.reset();
            q.timerControl.toggle();
        }

        function getId(item) {
            return item.id.slice(0, 6);
        }

        function isSelectedAnswer(question, answer) {            
            if (question.selectedAnswer == null) {
                return false;
            }

            return question.selectedAnswer.id == answer.id;
        }

        function isQuestionCorrect(question) {
            if (!q.isSolved) {
                return null;
            }
            if (angular.isUndefined(question.isCorrect)) {
                return 'noAnswer';
            }
            return question.isCorrect ? 'correct' : 'wrong';


        }

        function getButtonText() {
            return q.timerControl.isRunning ? 'Pause' : 'Play';

        }

        function isCorrectAnswer(question, answer) {
            if (!q.isSolved) {
                return null;
            }

            return question.correctAnswer == answer.id ? 'correct' : null;
        }

        function selectAnswer(question, answer) {
            if (q.isSolved) {
                return;
            }
            question.selectedAnswer = answer;
            question.isCorrect = question.correctAnswer == answer.id;
        }

        function sendData() {
            var data = {
                boxId: $stateParams.boxId,
                quizId: $stateParams.quizId
            };

            data.startTime = new Date();
            var endTime = new Date(data.startTime);
            endTime.setSeconds(data.startTime.getSeconds() + 100);
            data.endTime = endTime;
            data.answers = [];
            var question;
            for (var i = 0; i < q.questions.length; i++) {
                question = q.questions[i];
                if (angular.isObject(question.selectedAnswer)) {
                    data.answers.push({ questionId: question.id, answerId: question.selectedAnswer.id });
                }
            }
            quizService.saveAnswers(data).then(function () {
                var timeTaken = $filter('stopwatch')(q.timerControl.getTime()),
                    correct = 0;
                angular.forEach(q.questions, function (question) {
                    if (question.isCorrect == true) {
                        correct++;
                    }
                });
                wrong = q.questions.length - correct;
                openScorePopup({
                    score: correct / q.questions.length * 100,
                    correct: correct,
                    wrong: wrong,
                    timeTaken: timeTaken
                });
            });
        }

        function openScorePopup(sheet) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'quizScore.html',
                controller: 'QuizScoreController',
                controllerAs: 's',
                backdrop: 'static',
                windowClass: 'score-modal',
                resolve: {
                    topUsers: function () {
                        return q.topUsers;
                    },
                    sheet: function () {
                        sheet.score = Math.round(sheet.score);
                        return sheet;

                    }
                }
            });

            modalInstance.result.then(function () {
                retakeQuiz();
            }, function () {
                seeAnswers();
            });

        }

        function getDiscussion() {
            quizService.getDiscussion({ quizId: $stateParams.quizId }).then(function (data) {
                angular.forEach(data, function (comment) {
                    var question = $filter('filter')(q.questions, function (question) {
                        return comment.questionId === question.id;
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

        function postComment(question) {

            var comment = {
                questionId: question.id,

            }
            
        }

        function removeComment(question,comment) {
            var index = question.comments.indexOf(comment);
            question.comments.splice(index, 1);

            quizService.removeDiscussion({ id: comment.id });

        }

    }


})();


