"use strict";
mQuiz.controller('QuizCreateCtrl', ['$scope', '$rootScope', '$timeout', 'sModal', 'sQuiz', 'sUserDetails',
    function ($scope, $rootScope, $timeout, sModal, sQuiz, sUserDetails) {
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
            minQuestions: 3,
            minAnswers: 2,
            isDraft: false,
            focus: true
        };

        $scope.reset = function () {
            $scope.quiz = {
                id: null,
                name: null,
                questions: [],
                courseId: null,
                courseName: null
            };

            $scope.params.isDraft = false;
        };

        $scope.reset();


        $scope.$on('initQuiz', function (e, data) {
            $scope.initQuiz(data);
        });

        $scope.initQuiz = function (data) {
            if ($scope.quiz.id) {
                if ($scope.quiz.id === data.quizId) {
                    return;
                }
                $scope.reset();
                $rootScope.$broadcast('update-scroll');
            }
            $scope.quiz.courseId = data.boxId;
            $scope.quiz.courseName = data.boxName;

            if (!data.quizId) {
                $scope.params.isDraft = false;

                for (var i = 0; i < $scope.params.minQuestions; i++) {
                    $scope.addQuestion(false);
                }

                $scope.params.loadCreateQuiz = true;
                $scope.params.showCreateQuiz = true;
                $scope.params.focus = true;
                $rootScope.$broadcast('update-scroll');
                return;
            }

            $scope.quiz.id = data.quizId;

            sQuiz.getDraft({ quizId: data.quizId }).then(function (draft) {
                $scope.quiz.name = draft.payload.name;
                $scope.quiz.questions = draft.payload.questions;
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

                $scope.params.loadCreateQuiz = true;
                $scope.params.showCreateQuiz = true;
                $scope.params.focus = true;
                $rootScope.$broadcast('update-scroll');
            });
        };

        $scope.closeQuiz = function (isValid) {
            if ($scope.isEmptyQuiz()) {
                $scope.params.showCreateQuiz = false;
                $rootScope.options.quizOpen = false;
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
                    }
                }
            });

        };

        $scope.saveQuiz = function () {
            if ($scope.isEmptyQuiz()) {
                return;
            }
            if (!$scope.quiz.id) {
                createQuiz();
                return;
            }

            sQuiz.update({ id: $scope.quiz.id, name: $scope.quiz.name }).then(function (response) {
                if (!response) {
                    return;
                }
                addItemToBox(false);
            });
        };

        function publish(isValid) {
            $scope.params.focus = false;
            $scope.submit(isValid);

        }

        function deleteQuiz() {
            if (!$scope.quiz.id) {
                return;
            }
            var quizId = $scope.quiz.id,
                boxId = $scope.quiz.courseId;

            $scope.reset();
            $scope.params.focus = false;
            $scope.params.showCreateQuiz = false;

            sQuiz.delete({ id: quizId }).then(function () {
                $rootScope.$broadcast('QuizDeleted', { boxId: boxId, quizId: quizId });
            });
        }

        function saveDraft() {
            $scope.params.showCreateQuiz = false;
            $scope.params.focus = false;

            if ($scope.isEmptyQuiz()) {
                if (!$scope.quiz.id) {
                    return;
                }
                $scope.reset();

                return;
            }
            $scope.reset();
        }

        $scope.addQuestion = function (focus) {
            var question = new Question();
            question.focus = focus;
            $scope.quiz.questions.push(question);
            for (var i = 0; i < $scope.params.minAnswers; i++) {
                $scope.addAnswer(question, false);
            }

            $rootScope.$broadcast('update-scroll');
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

            sQuiz.question.update({ id: question.id, text: question.text });
        };

        $scope.removeQuestion = function (index) {
            if (index === -1) {
                return; //something went wrong
            }
            if (index === 0) {
                return;  //don't want to remove the first question
            }

            var question = $scope.quiz.questions[index];

            $scope.quiz.questions.splice(index, 1);
            $rootScope.$broadcast('update-scroll');

            if (!question.id) {
                return;
            }
            sQuiz.question.delete({ id: question.id }).then(function () {
            });
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
        };

        $scope.removeAnswer = function (question, index) {
            var answerId = question.answers[index].id;
            question.answers.splice(index, 1);
            $rootScope.$broadcast('update-scroll');

            if (!answerId) {
                return;
            }

            if (question.correctAnswer === answerId) {
                question.correctAnswer = null;
            }

            sQuiz.answer.delete({ id: answerId }).then(function () {
            });

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
            sQuiz.answer.update({ id: answer.id, text: answer.text });
        };

        $scope.markCorrect = function (question, answer) {
            if (question.correctAnswer === answer.id) {
                return;
            }

            if ((answer.id && answer.id.length === 0) || answer.text === 0) {
                return;
            }

            sQuiz.answer.markCorrect({ answerId: answer.id }).then(function (data) {
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
            return sQuiz.create({
                boxId: $scope
                    .quiz.courseId, name: $scope.quiz.name
            }).then(function (data) {
                $scope.quiz.id = data.payload;
                addItemToBox(false);
                return data.payload;
            });
        };
        var createQuestion = function (question) {
            return sQuiz.question.create({ quizId: $scope.quiz.id, text: question.text }).then(function (data) {
                question.id = data.payload;
                return data.payload;
            });
        };
        var createAnswer = function (question, answer) {
            return sQuiz.answer.create({ questionId: question.id, text: answer.text }).then(function (data) {
                answer.id = data.payload;
                return data.payload;
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
        function addItemToBox(isPublish, url) {
            var quiz = {
                id: $scope.quiz.id,
                boxId: $scope.quiz.courseId,
                name: $scope.quiz.name || '',
                publish: isPublish,
                description: isPublish && getContent(),
                rate: 0,
                ownerId: sUserDetails.getDetails().id,
                owner: sUserDetails.getDetails().name,
                userUrl: sUserDetails.getDetails().url,
                type: 'sQuiz',
                url: url,
                date: new Date()
            };

            $rootScope.$broadcast('QuizAdded', quiz);

            function getContent() {
                var result = '',
                    questions = $scope.quiz.questions;
                for (var i = 0, l = questions.length; i < l; i++) {
                    if (questions[i].text) {
                        result += questions[i].text;
                    }

                }

                return result;
            }
        }
        $scope.submit = function (isValid, event) {

            if (event) {
                event.preventDefault();
            }

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


            sQuiz.save({
                quizId: $scope.quiz.id, boxId: $scope.quiz.courseId, universityName: cd.getParameterFromUrl(1), boxName: $scope.quiz.courseName, quizName: $scope.quiz.name
            }
            ).then(function (data) {
                if (!data.success) {
                    alert(data.payload || data.Payload);
                    return;
                }
                cd.pubsub.publish('addPoints', { type: 'quiz' });
                addItemToBox(true, data.payload.url);
                $scope.params.showCreateQuiz = false;
                $rootScope.options.quizOpen = false;
            });
        };

        $scope.checkTab = function (question, lastAnswer, e) {
            e.preventDefault();
            if (lastAnswer && e.keyCode === 9) {
                $scope.addAnswer(question, true);
            }
        };

        $scope.$on('closeQuizCreate', function (e, quizId) {
            if (quizId === $scope.quiz.id) {
                $scope.params.showCreateQuiz = false;

                $rootScope.options.quizOpen = false;
            }
        });

        $scope.$on('$locationChangeStart', function () {
            $scope.quiz.showPreview = false;
        });

    }]).directive('quizPreview', ['$rootScope','$timeout', function ($rootScope,$timeout) {
        return function (scope, element, attrs) {
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
                          $mainDiv.addClass('previewQuiz');
                      });

                      $timeout(function () { //fix for animationB                                                    
                          $mainDiv.addClass('topBarFix');
                          $('body').addClass('jsDark');
                          $rootScope.$broadcast('update-scroll');
                      }, 700);
                  }

                  function hidePreview() {
                      $mainDiv.removeClass('previewQuiz topBarFix');
                      element[0].style.display = 'none';
                      $('body').removeClass('jsDark');
                  }

              }, true);
        };
    }]).
    directive('quizFocus', ['$timeout', function ($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                scope.$watch(function () {
                    return element.attr('data-focus');
                }, function (newValue) {
                    if (newValue === 'true') {
                        $timeout(function () { element.focus(); }, 10);
                    }

                });
            }
        };
    }]).
    directive('requiredTwo', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var watchFn = function () {
                    return element[0].querySelectorAll('.quizAnswer:not(.disabled)').length;
                }
                scope.$watch(watchFn, function (newValue, oldValue) {
                    scope.questionForm.$setValidity('required', newValue > 1);
                });

            }
        };
    });