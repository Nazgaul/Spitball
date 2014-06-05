var quizCreate = angular.module('QuizCreate', ['apiService', 'ngModal', 'main.directives']);

quizCreate.controller('QuizCreateController', ['$scope', 'QuizService', function ($scope, Quiz) {
    function Question(data) {
        data = data || {};
        var that = this;
        that.id = data.id;
        that.text = data.text;
        that.correctAnswer = data.correctAnswer;
        that.answers = [];
    }

    function Answer(data) {
        data = data || {};
        var that = this;
        that.id = data.id;
        that.text = data.text;
    }

    $scope.params = {
        loadCreateQuiz: false,
        showCreateQuiz: false,
        showCloseDialog : false,
        minQuestions: 3,
        minAnswers: 2
    };

    $scope.reset = function () {
        $scope.quiz = {
            id: null,
            name : null,
            questions: [],
            courseId: null,
            courseName: null            
        }
    }

    $scope.reset();

    $scope.initQuiz = function (data) {
        if ($scope.quiz.id) {
            if ($scope.quiz.id === data.quizId) {
                return;
            }
            $scope.reset();
        }
        $scope.quiz.courseId = data.boxId;
        $scope.quiz.courseName = data.boxName;

        if (!data.quizId) {
            for (var i = 0; i < $scope.params.minQuestions; i++) {
                $scope.addQuestion();
            }

            $scope.params.loadCreateQuiz = true;
            $scope.params.showCreateQuiz = true;

            return;
        }

        $scope.quiz.id = data.quizId;

        Quiz.getDraft({ quizId: data.quizId }).then(function (data) {
            $scope.quiz.name = data.payload.name;
            $scope.quiz.questions = data.payload.questions;

            for (var i = 0, l = $scope.quiz.questions.length; i < l ; i++) {
                var answersLength = $scope.quiz.questions[i].answers.length;
                for (var z = 0; z < 2 && answersLength < 2; z++) {
                    $scope.quiz.questions[i].answers.push(new Answer());
                    answersLength++;

                }
            }

            $scope.params.loadCreateQuiz = true;
            $scope.params.showCreateQuiz = true;

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
                //console.log('error updating quiz');
            }

            addItemToBox(false);
        });
    };


    $scope.publish = function () {
        $scope.params.showCloseDialog = false;

        $scope.submit(this.quizForm.$valid);
    }

    $scope.deleteQuiz = function () {
        if (!$scope.quiz.id) {
            closed();
            return;
        }

            closed();
        });

        function closed() {
            $scope.reset();
            $scope.params.showCreateQuiz = false;
            $scope.params.showCloseDialog = false;
        }
    };

    $scope.saveDraft = function () {
        $scope.params.showCloseDialog = false;
        $scope.params.showCreateQuiz = false;

        if ($scope.isEmptyQuiz()) {
            if (!$scope.quiz.id) {
                return;
            }
                $scope.reset();
            });
            return;
        }
        $scope.reset();
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
            createQuiz().then(function () {
                createQuestion(question);
            });
            return;
        }

        if (!question.id) {
            createQuestion(question).then(function () {

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


    $scope.toggleRadioBtn = function (question, answer) {
        var text = answer.text || '';
        if (!text.length && question.correctAnswer === answer.id) {
            question.correctAnswer = null;
        }
    };

    $scope.saveAnswer = function (question, answer) {
        if (answer.id && !answer.text.length) {
            Quiz.answer.delete({ id: answer.id }).then(function () {                
                answer.id = null;                
            });
            return;
        }

        if (!answer.text) {
            return;
        }

        if (!$scope.quiz.id) {
            createQuiz().then(function () {
                createQuestion(question);
            }).then(function () {
                createAnswer(question, answer);
            });
            return;
        }

        if (!question.id) {
            createQuestion(question).then(function () {
                createAnswer(question, answer);
            });
            return;
        }

        if (!answer.id) {
            createAnswer(question, answer);
            return;
        }
        Quiz.answer.update({ id: answer.id, text: answer.text });
    };

    $scope.markCorrect = function (question, answer) {
        if (question.correctAnswer === answer.id) {
            return;
        }
        Quiz.answer.markCorrect({ answerId: answer.id }).then(function (data) {
            if (!data) {
                console.log('error mark answer as true');
            }
        });
    };

    $scope.previewQuiz = function () {
        $scope.quiz.showPreview = !$scope.quiz.showPreview;
    };



    //#region helpers
    var createQuiz = function () {
        return Quiz.create({ boxId: $scope
            .quiz.courseId, name: $scope.quiz.name }).then(function (data) {
                $scope.quiz.id = data.payload;
                addItemToBox(false);
            return data.payload;
        });
    };
    var createQuestion = function (question) {
        return Quiz.question.create({ quizId: $scope.quiz.id, text: question.text }).then(function (data) {
            question.id = data.payload;
            return data.payload;
        });
    }
    var createAnswer = function (question, answer) {
        return Quiz.answer.create({ questionId: question.id, text: answer.text }).then(function (data) {
            answer.id = data.payload;
            return data.payload;
        });
    }
    //#endregion

    //#region validation 

    $scope.isEmptyText = function (elm) {

        if (elm.text) {
            return false;
        }
        return true;
    };

    //question
    $scope.isAnswersValid = function (answers) {

        var validAnswers = 0;
        for (var i = 0, l = answers.length; i < l && validAnswers < 2; i++) {
            answers[i].text = answers[i].text || '';
            if ((answers[i].text && answers[i].text.length)) {
                validAnswers++;
            }
        }

        return validAnswers < 2;

    };

    $scope.isEmptyQuiz = function () {

        if ($scope.quiz.name && $scope.quiz.name.length) {
            return false;
        }

        for (var i = 0, l = $scope.quiz.questions.length; i < l; i++) {
            if (!$scope.isEmptyQuestion($scope.quiz.questions[i])) {
                return false;
            }
        }

        return true;
    };
    $scope.removeEmptyQuestions = function () {
        for (var i = 0, l = $scope.quiz.questions.length; i < l; i++) {
            if ($scope.isEmptyQuestion($scope.quiz.questions[i])) {
                var index = $scope.quiz.questions.indexOf($scope.quiz.questions[i]);
                $scope.quiz.questions.splice(index, 1);
                i--;
                l--;
            }
        }
        return true; // we need that to call the next function
    };
    $scope.isEmptyQuestion = function (question) {
        if (!$scope.isEmptyText(question)) {
            return false;
        }

        if (question.correctAnswer) {
            return false;
        }

        for (var j = 0, le = question.answers.length; j < le; j++) {
            if (!$scope.isEmptyText(question.answers[j])) {
                return false;
            }
        }

        return true;
    };




    //#endregion
    function addItemToBox(isPublish, url) {
        var quiz = {
            id: $scope.quiz.id,
            boxid: $scope.quiz.courseId,
            name: $scope.quiz.name,
            publish: isPublish,
            description: isPublish || getContent(),
            rate: 0,
            ownerId: cd.userDetail().nId,
            owner: cd.userDetail().name,
            userUrl: cd.userDetail().url,
            type: 'quiz',
            url: url,
            date: cd.getUTCDate()
        }

        cd.pubsub.publish('addItem', quiz);

        function getContent() {
            var result = '',
                questions = $scope.quiz.questions;
            for (var i = 0, l = questions.length; i < l; i++) {
                result += questions[i].text;
            }

            return result;
        }
    }
    $scope.submit = function (isValid) {        

        if ($scope.isEmptyQuiz()) {
            $scope.quiz.empty = true;
            $scope.quiz.validated = false;
            return;
        }

        $scope.quiz.validated = true;
        $scope.quiz.empty = false;


        if (!isValid) {
            return;
        }
        
        Quiz.save({
            quizId: $scope.quiz.id, boxId: $scope.quiz.courseId, universityName: cd.getParameterFromUrl(1), boxName: $scope.quiz.courseName, quizName: $scope.quiz.name }
        ).then(function (data) {
            if (data.success) {
                addItemToBox(true, data.payload);
                $scope.params.showCreateQuiz = false;
            }
            
        });
      
    };

    $scope.closeDialog = function () {
        $scope.params.showCloseDialog = true;
    };
}]);

quizCreate.directive('quizPreview', function () {
    return function (scope, element, attrs) {
        scope.$watch(attrs.show,
          function (newValue) {
              
              var mainDiv = document.getElementById('main');

              if (!newValue) {
                  hidePreview();
                  return;
              }

              showPreview();


              function showPreview() {
                element[0].style.display = 'block';
                  setTimeout(function() { //fix for animation
                      mainDiv.classList.add('previewQuiz');
                  }, 0);

                setTimeout(function () { //fix for animationB
                      mainDiv.classList.add('topBarFix');
                    $('.siteHeader').hide();
                    scope.$broadcast('update-scroll');
                    scope.$emit('update-scroll');
                    mainDiv.classList.add('topBarFix');
                    $('.siteHeader').hide();
                    scope.$broadcast('update-scroll');
                }, 700)
>>>>>>> 05044e53fa9bc68de18a3f3f1c96e820b692e86d
              }

              function hidePreview() {
                  mainDiv.classList.remove('previewQuiz');
                  mainDiv.classList.remove('topBarFix');
                  element[0].style.display = 'none';
                  $('.siteHeader').show();
              }
                  
          }, true);
    };
});