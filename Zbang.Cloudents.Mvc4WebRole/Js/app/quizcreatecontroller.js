var quizCreate = angular.module('QuizCreate', ['apiService', 'ngModal', 'main.directives']);

quizCreate.controller('QuizCreateController', ['$scope', 'Quiz', function ($scope, Quiz) {
    function Question(data) {
        data = data || {};
        var that = this;
        that.id = data.id || '';
        that.text = data.text || '';
        that.correctAnswerId = data.correctAnswerId || null;
        that.answers = [];
    }

    function Answer(data) {
        data = data || {};
        var that = this;
        that.id = data.id || Math.random();
        that.text = data.text || '';
    }

    $scope.params = {
        showCreateQuiz: false,
        minQuestions: 3,
        minAnswers: 2
    };

    $scope.quiz = {
        id : '',
        name : '',
        courseId : '',
        courseName : '',
        questions : []   
    }
    


    $scope.initQuiz = function (data) {
        $scope.quiz.courseId = data.boxId;
        $scope.quiz.courseName = data.boxName;

        $scope.params.showCreateQuiz = true;        
        if (!data.quizId) {
            for (var i = 0; i < $scope.params.minQuestions; i++) {
                $scope.addQuestion();                
            }

            return;
        }

        $scope.quiz.id = data.quiz.id;

        Quiz.getDraft({
            data: { quizId: data.quizId },
            success: function (data) {

            },
            error: function (data) {

            }
        });
    };

    $scope.showQuiz = function () {
        document.getElementById('main').classList.remove('noQuiz');
        $scope.$broadcast('rebuild:quiz');
    };

    $scope.saveQuiz = function () {
        if (!$scope.quiz.id) {
            Quiz.create({
                data: { boxId: $scope.quiz.courseId, name: $scope.quiz.name },
                success: function (id) {
                    $scope.quiz.id = id;
                }
            });
            return;
        }        
        
        Quiz.update({
            data: { id: $scope.quiz.id, name: $scope.quiz.name }            
        });

    };

    $scope.publishQuiz = function () {

    };

    $scope.addAnswer = function (question) {
        var answer = new Answer();
        question.answers.push(answer);
    };
    $scope.saveAnswer = function (question,answer) {
        if (!answser.id) {
            //save
            console.log('save');


            return;
        }

        console.log('update');

        //update
    };


    $scope.addQuestion = function () {
        var question = new Question();
        $scope.quiz.questions.push(question);
        for (var i = 0; i < $scope.params.minAnswers; i++) {
            $scope.addAnswer(question);
        }
    };
    $scope.saveQuestion = function(question) {
        if (!question.id) {
            console.log('save');
            //save


            return;
        }

        console.log('update');
        //update
    }

    $scope.removeQuestion = function (index) {       
        if (index > -1) {
            $scope.quiz.questions.splice(index, 1);
        }
        
    };

    $scope.isEmptyAnswer = function (answer) {
        return answer.text.length === 0;
    };

    $scope.isCorrect = function (question, answer) {        
        return question.correctAnswerId === answer.id;
    };

    $scope.markCorrect = function (question, answer) {
        if ($scope.isEmptyAnswer(answer)) {
            return;
        }

        console.log('marked ' + answer.id);
        question.correctAnswerId = answer.id;
    };

    $scope.previewQuiz = function () {

    };

}]);