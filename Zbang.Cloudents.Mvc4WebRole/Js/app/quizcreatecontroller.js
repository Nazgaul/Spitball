var quizCreate = angular.module('QuizCreate', ['apiService', 'ngModal', 'main.directives']);

quizCreate.controller('QuizCreateController', ['$scope', 'QuizService', function ($scope, Quiz) {
    function Question(data) {
        data = data || {};
        var that = this;
        that.id = data.id || '';
        that.text = data.text || '';
        that.correctAnswer = data.correctAnswer || null;
        that.answers = [];
    }

    function Answer(data) {
        data = data || {};
        var that = this;
        that.id = data.id;
        that.text = data.text || '';
    }

    $scope.params = {
        showCreateQuiz: false,
        minQuestions: 3,
        minAnswers: 2
    };

    $scope.quiz = {
        id: '',
        name: '',
        courseId: '',
        courseName: '',
        questions: []
    }



    $scope.initQuiz = function (data) {
        if ($scope.quiz.id) {
            return;
        }
        $scope.quiz.courseId = data.boxId;
        $scope.quiz.courseName = data.boxName;

        if (!data.quizId) {
            for (var i = 0; i < $scope.params.minQuestions; i++) {
                $scope.addQuestion();
            }
            $scope.params.showCreateQuiz = true;
            return;
        }

        $scope.quiz.id = data.quizId;

        Quiz.getDraft({ quizId: data.quizId }).then(function (data) {
            $scope.params.showCreateQuiz = true;
            $scope.quiz.name = data.payload.name;
            $scope.quiz.questions = data.payload.questions;
        });
    };

    $scope.showQuiz = function () {
        document.getElementById('main').classList.remove('noQuiz');
        $scope.$broadcast('rebuild:quiz');
    };

    $scope.saveQuiz = function () {
        if (!$scope.quiz.id) {
            createQuiz();
            return;
        }

        Quiz.update({ id: $scope.quiz.id, name: $scope.quiz.name }).then(function (response) {
            if (!response) {
                console.log('error updating quiz')
            }
        });
    };

    $scope.publishQuiz = function () {

    };



    $scope.addQuestion = function () {
        var question = new Question();
        $scope.quiz.questions.push(question);
        for (var i = 0; i < $scope.params.minAnswers; i++) {
            $scope.addAnswer(question);
        }
    };
    $scope.saveQuestion = function (question) {
        if (!$scope.quiz.id) {
            createQuiz().then(function (quizId) {
                createQuestion(question);
            });
            return;
        }

        if (!question.id) {
            createQuestion(question).then(function (questionId) {

            });
            return;
        }

        Quiz.question.update({ id: question.id, text: question.text });
    }

    $scope.removeQuestion = function (index) {
        if (index > -1) {
            var question = $scope.quiz.questions[index];
            $scope.quiz.questions.splice(index, 1);
            if (!question.id) {
                return;
            }
            Quiz.question.delete({ id: question.id }).then(function () {
            });
        }
    };

    $scope.addAnswer = function (question) {
        var answer = new Answer();
        question.answers.push(answer);
    };

    $scope.saveAnswer = function (question, answer) {
        if (!answer.text && answer.id) {
            Quiz.answer.delete({ id: answer.id }).then(function (data) {
                answer.id = null;
            });
            return;
        }


        if (!$scope.quiz.id) {
            createQuiz().then(function (quizId) {
                createQuestion(question);
            }).then(function (questionId) {
                createAnswer(question,answer);
            });
            return;
        }

        if (!question.id) {
            createQuestion(question).then(function (questionId) {
                createAnswer(question,answer);
            });
            return;            
        }

        if (!answer.id) {            
            createAnswer(question,answer);
            return;
        }
        Quiz.answer.update({id : answer.id, text: answer.text});
    };

    $scope.isEmptyAnswer = function (answer) {
        return answer.text.length === 0;
    };

    $scope.isCorrect = function (question, answer) {
        return question.correctAnswer === answer.id;
    };

    $scope.markCorrect = function (question, answer) {
        if (question.correctAnswer === answer.id) {
            return;
        }
        if ($scope.isEmptyAnswer(answer)) {
            return;
        }
        question.correctAnswer = answer.id;
        Quiz.answer.markCorrect({ answerId: answer.id }).then(function (data) {
            if (!data) {
                console.log('error mark answer as true');
            }
        });
    };

    $scope.previewQuiz = function () {

    };

    //#region helpers
    var createQuiz = function() {
        return Quiz.create({ boxId: $scope.quiz.courseId, name: $scope.quiz.name }).then(function(data) {
            $scope.quiz.id = data.payload;
            return data.payload;
        });
    };
    var createQuestion = function(question) {
        return Quiz.question.create({ quizId: $scope.quiz.id , text: question.text }).then(function (data) {
            question.id = data.payload;
            return data.payload;
        });
    }
    var createAnswer = function(question,answer) {
        return Quiz.answer.create({ questionId: question.id , text : answer.text }).then(function(data) {
            answer.id = data.payload;
            return data.payload;
        });
    }    
    //#endregion

}]);