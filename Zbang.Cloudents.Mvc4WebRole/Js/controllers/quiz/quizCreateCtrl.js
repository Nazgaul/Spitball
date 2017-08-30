mQuiz.controller('QuizCreateCtrl', ['$scope', '$rootScope', '$location', '$routeParams', '$timeout', '$q', 'sModal', 'sQuiz', 'sUserDetails', '$analytics', 'sNotify', 'sGmfcnHandler',
    function ($scope, $rootScope, $location, $routeParams, $timeout, $q, sModal, sQuiz, sUserDetails, $analytics, sNotify, sGmfcnHandler) {
        "use strict";
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
            minQuestions: 2,
            minAnswers: 2,
            isDraft: false,
            focus: true
        };

        $scope.quiz = {
            questions: []
        };

        $scope.initQuiz = function () {
            $scope.quiz.courseId = $routeParams.boxId;
            $scope.quiz.courseName = $routeParams.boxName;
            var quizId = $location.search().quizId;
            if (!quizId) {
                $scope.params.isDraft = false;

                for (var i = 0; i < $scope.params.minQuestions; i++) {
                    $scope.addQuestion(false);
                }

                $scope.params.focus = true;
                return;
            }

            $scope.quiz.id = quizId;

            sQuiz.getDraft({ quizId: $scope.quiz.id }).then(function (draft) {
                $scope.quiz.name = draft.name;
                $scope.quiz.questions = draft.questions;
                $scope.params.isDraft = true;
                for (var i = 0, l = $scope.quiz.questions.length; i < l ; i++) {
                    var answersLength = $scope.quiz.questions[i].answers.length;
                    for (var z = 0; z < 2 && answersLength < 2; z++) {
                        $scope.quiz.questions[i].answers.push(new Answer());
                        answersLength++;

                    }
                }

                for (var j = 0, questionsRemain = $scope.params.minQuestions - $scope.quiz.questions.length; j < questionsRemain; j++) {
                    $scope.addQuestion(false);
                }

                $scope.params.focus = true;
            });
        };



        $scope.closeQuiz = function (isValid) {
            if ($scope.isEmptyQuiz()) {
                if (!$scope.quiz.id) {
                    goToQuizzes();
                    return;
                }
                deleteQuiz();
                return;
            }

            sModal.open('quitQuiz', {
                callback: {
                    close: function (response) {
                        switch (response) {
                            case 'publish':
                                publish(isValid);
                                break;
                            case 'delete':
                                deleteQuiz();
                                break;
                            case 'draft':
                                saveDraft();
                                break;
                        }

                        $location.search('quizId', null);
                    }
                }
            });

        };

        $scope.saveQuiz = function () {
            if (!$scope.quiz.id) {
                createQuiz();
                return;
            }

            sQuiz.update({ id: $scope.quiz.id, name: $scope.quiz.name });
        };

        function publish(isValid) {
            $scope.params.focus = false;
            $scope.submit(isValid);
        }

        function deleteQuiz() {
            if (!$scope.quiz.id) {
                return;
            }


            var quizId = $scope.quiz.id;


            sQuiz.delete({
                id: quizId
            }).then(function () {
                goToQuizzes();
            });

            $analytics.eventTrack('Deleted Quiz', {
                category: 'Quiz Create'
            });
        }

        function saveDraft() {

            $analytics.eventTrack('Save Draft', {
                category: 'Quiz Create'
            });
            goToQuizzes();

        }

        $scope.addQuestion = function (focus) {
            var question = new Question();
            question.focus = focus;
            $scope.quiz.questions.push(question);
            for (var i = 0; i < $scope.params.minAnswers; i++) {
                $scope.addAnswer(question, false);
            }

            if (!focus) {
                return;
            }
            $analytics.eventTrack('Add Question', {
                category: 'Quiz Create'
            });
        };
        $scope.saveQuestion = function (question) {
            if (!$scope.quiz.id) {
                createQuiz().then(function () {
                    createQuestion(question);
                });
                return;
            }

            if (!question.id) {
                createQuestion(question);
                return;
            }

            question.text = question.text || '';
            sQuiz.question.update({ id: question.id, text: question.text });
        };

        $scope.removeQuestion = function (index) {
            if (index === -1) {
                return; //something went wrong
            }
            if (index === 0) {
                return;  //don't want to remove the first question
            }

            $analytics.eventTrack('Remove Question', {
                category: 'Quiz Create'
            });

            var question = $scope.quiz.questions[index];

            $scope.quiz.questions.splice(index, 1);

            if (!question.id) {
                var interval = setInterval(function () {
                    if (!question.id) {
                        return;
                    }

                    clearInterval(interval);
                    postDelete();


                }, 20);
                return;
            }

            postDelete();

            function postDelete() {
                sQuiz.question.delete({ id: question.id });
            }

        };

        $scope.addTabAnswer = function (e, question, answer) {
            if (e.keyCode !== 9) {
                return;
            }

            if (question.answers[question.answers.length - 1] !== answer) {
                return;
            }
            e.preventDefault();
            $scope.addAnswer(question, true);
        }

        $scope.addAnswer = function (question, focus) {
            var answer = new Answer();
            answer.focus = focus;
            question.answers.push(answer);
            $rootScope.$broadcast('update-scroll');
            $analytics.eventTrack('Add Answer', {
                category: 'Quiz Create'
            });

        };

        $scope.removeAnswer = function (question, index) {
            var answer = question.answers[index];

            if (index === -1) {
                return; //something went wrong
            }

            question.answers.splice(index, 1);
            $rootScope.$broadcast('update-scroll');


            $analytics.eventTrack('Remove Answer', {
                category: 'Quiz Create'
            });

            if (question.correctAnswer === answer.id) {
                question.correctAnswer = null;
            }

            if (!answer.id) {
                var interval = setInterval(function () {
                    if (!answer.id) {
                        return;
                    }

                    clearInterval(interval);
                    postDelete();


                }, 20);
                return;
            }

            postDelete();

            function postDelete() {
                sQuiz.answer.delete({ id: answer.id });
            }

            //TODO analytics

        };

        $scope.toggleRadioBtn = function (question, answer) {
            var text = answer.text || '';
            if (!text.length && question.correctAnswer === answer.id) {
                question.correctAnswer = null;
            }
        };

        $scope.saveAnswer = function (question, answer) {
            var answerText = answer.text || '';
            if (answer.id && !answerText.length) {
                sQuiz.answer.delete({ id: answer.id }).then(function () {
                    answer.id = null;
                });
                return;
            }

            if (!answerText.length) {
                return;
            }

            if (!$scope.quiz.id) {
                createQuiz().then(function () {
                    createQuestion(question).then(function () {
                        createAnswer(question, answer);
                    });
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
                createAnswer(question, answer).then(function () {
                    sQuiz.answer.update({ id: answer.id, text: answer.text });
                });
                return;
            }

            sQuiz.answer.update({ id: answer.id, text: answer.text });
        };

        $scope.markCorrect = function (question, answer) {
            if (question.correctAnswer === answer.id) {
                return;
            }

            if ((answer.id && answer.id.length === 0) || answer.text === 0) {
                return;
            }

            $analytics.eventTrack('Mark Correct', {
                category: 'Quiz Create'
            });

            sQuiz.answer.markCorrect({ answerId: answer.id });
        };

        $scope.previewQuiz = function () {
            $scope.quiz.showPreview = !$scope.quiz.showPreview;
            $analytics.eventTrack('Preview Quiz', {
                category: 'Quiz Create'
            });
        };

        //#region helpers
        var creatingQuiz, creatingQuestion, creatingAnswer,
        createQuiz = function () {
            if (creatingQuiz) {
                return creatingQuiz;
            }
            creatingQuiz = sQuiz.create({
                boxId: $scope.quiz.courseId,
                name: $scope.quiz.name
            }).then(function (data) {
                $scope.quiz.id = data;
                return data;
            }).finally(function () {
                creatingQuiz = null;
            });

            return creatingQuiz;
        },
        createQuestion = function (question) {
            if (creatingQuestion) {
                return creatingQuestion;
            }

            creatingQuestion = sQuiz.question.create({ quizId: $scope.quiz.id, text: question.text }).then(function (data) {
                question.id = data;
                return data;
            }).finally(function () {
                creatingQuestion = null;
            });

            return creatingQuestion;

        },
          createAnswer = function (question, answer) {
              return sQuiz.answer.create({ questionId: question.id, text: answer.text }).then(function (data) {
                  answer.id = data;
                  return data;
              });
          };
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

        $scope.submit = function (isValid, event) {

            if (event) {
                event.preventDefault();
            }

            if ($scope.isEmptyQuiz()) {
                $scope.quiz.empty = true;
                $scope.quiz.validated = false;
                $analytics.eventTrack('Empty Quiz', {
                    category: 'Quiz Create',
                    label: 'User tried to publish an empty quiz'
                });
                return;
            }

            $scope.quiz.validated = true;
            $scope.quiz.empty = false;

            if (!isValid) {

                $analytics.eventTrack('Invalid Quiz', {
                    category: 'Quiz Create',
                    label: 'User tried to publish an invalid quiz'

                });
                return;
            }


            $analytics.eventTrack('Save Quiz', {
                category: 'Quiz Create',
                label: 'User publish a quiz'
            });

            sQuiz.save({
                quizId: $scope.quiz.id
            }
            ).then(function () {
                sGmfcnHandler.addPoints({ type: 'quiz' });
                goToQuizzes();
            }, function (data) {
                sNotify.alert(data);
            });
        };

        $scope.checkTab = function (question, lastAnswer, e) {
            e.preventDefault();
            if (lastAnswer && e.keyCode === 9) {
                $scope.addAnswer(question, true);
            }
        };

        $scope.initQuiz();

        function goToQuizzes() {
            var path = $location.path().slice(0, -1),
                index = path.lastIndexOf('/');

            path = path.substring(0, index) + '/';
            $location.path(path).hash('quizzes');
        }

        $timeout(function () {
            $rootScope.$broadcast('viewContentLoaded');
        });

    }]).directive('quizPreview', ['$rootScope', '$timeout', function ($rootScope, $timeout) {
        return function (scope, element, attrs) {
            "use strict";
            scope.$watch(attrs.show,
              function (newValue) {

                  var $mainDiv = angular.element(document.getElementById('main'));

                  if (!newValue) {
                      hidePreview();
                      return;
                  }

                  showPreview();

                  function showPreview() {
                      element[0].style.display = 'block';
                      $timeout(function () { //fix for animation
                          $mainDiv.addClass('jsQuizPreview');
                      });

                      $timeout(function () { //fix for animationB                                                    
                          $mainDiv.addClass('topBarFix');
                          $('body').addClass('jsDark');
                          $rootScope.$broadcast('update-scroll');
                      }, 700);
                  }

                  function hidePreview() {
                      $mainDiv.removeClass('jsQuizPreview topBarFix');
                      element[0].style.display = 'none';
                      $('body').removeClass('jsDark');
                  }


              }, true);
        };
    }]).
    directive('bindTextarea', [function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.on('blur', function () {
                    scope.$apply(attrs.onBlur);
                });

                scope.$on('$destory', function () {
                    element.off('blur');
                });

            }
        };
    }]).
directive('quizFocus', [function () {
    return {
        restrict: 'A',
        link: function (scope, element) {
            var input;
            if (element.is('textarea') || element.is('input:text')) {
                input = element[0];
            } else {
                input = element[0].querySelector('[contenteditable]');
            }
            var listener = scope.$watch(function () {
                return element.attr('data-focus');
            }, function (newValue) {

                if (newValue === 'true') {
                    setTimeout(function () {
                        if (!input) {
                            return;
                        }
                        input.focus();
                    }, 10);
                }
            });



            scope.$on('$destroy', function () {
                listener();
            });
        }
    };
}]).
    directive('highlightBox', [function () {
        return {
            restrict: 'A',
            link: function (scope, element) {
                element.on('focus', '[contenteditable],[highlighter]', setHighlight);


                scope.$on('$destroy', function () {
                    element.off('focus', setHighlight);
                });

                function setHighlight() {
                    angular.element('[highlight-box]').removeClass('focus');
                    element.addClass('focus');
                }
            }
        };
    }]).
directive('requiredTwo', function () {
    return {
        restrict: 'A',
        link: function (scope, element) {
            var watchFn = function () {
                return element[0].querySelectorAll('.quizAnswer:not(.disabled)').length;
            }
            var listener = scope.$watch(watchFn, function (newValue) {
                scope.questionForm.$setValidity('required', newValue > 1);
            });

            scope.$on('$destroy', function () {
                listener();
            });
        }
,
    };
});