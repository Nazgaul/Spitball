﻿(function () {
    angular.module('app.quiz').controller('QuizCreateController', quizCreate);
    quizCreate.$inject = ['quizService', 'draft', 'boxUrl', '$stateParams', '$q', '$location', '$scope', '$uibModal', 'boxName', '$rootScope', 'resManager'];



    function quizCreate(quizService, draft, boxUrl, $stateParams, $q, $location, $scope, $uibModal, boxName, $rootScope, resManager) {
        var self = this;
        self.boxUrl = boxUrl;
        draft = draft || {
            questions: [new question()]
        };
        self.name = draft.name || '';
        self.boxName = boxName;
        self.quizNameDisabled = self.name.length;
        self.id = draft.id;

        self.openClose = openClose;

        for (var k = draft.questions.length; k < 1; k++) {
            draft.questions.push(new question);
        }


        for (var i = 0; i < draft.questions.length; i++) {
            var q1 = draft.questions[i];
            if (isQuestionOk(q1)) {
                q1.done = true;
            }
            for (var j = q1.answers.length; j < 2; j++) {
                q1.answers.push(new answer());
            }
        }


        self.questions = draft.questions;

        var defer = $q.defer();
        defer.resolve();
        var currentPromise = defer.promise;



        self.saveName = saveName;
        self.saveNameOnBlur = saveNameOnBlur;
        self.saveQuestion = saveQuestion;
        self.saveAnswer = saveAnswer;
        self.deleteAnswerShow = deleteAnswerShow;
        self.deleteAnswer = deleteAnswer;
        self.editQuestion = editQuestion;
        self.deleteQuestionShow = deleteQuestionShow;
        self.deleteQuestion = deleteQuestion;

        self.markcorrect = markcorrect;
        self.addQuestion = addQuestion;
        self.addAnswer = addAnswer;
        self.template = template;
        self.publish = publish;
        function isQuestionOk(q) {
            if (q.text && q.correctAnswer && q.answers.length > 1) {
                for (var s = 0; s < q.answers.length; s++) {
                    if (!q.answers[s].text) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        function addToCurrent(promise) {
            currentPromise.then(promise);
        }
        function saveName() {
            if (self.id) {
                addToCurrent(quizService.updateQuiz(self.id, self.name));
            } else {
                addToCurrent(quizService.createQuiz($stateParams.boxId, self.name).then(function (response) {
                    self.id = response;
                }));
            }
        };
        function saveNameOnBlur() {
            self.quizNameDisabled = true;
            if (self.id) {
                return;
            }

            addToCurrent(quizService.createQuiz($stateParams.boxId, self.name).then(function (response) {
                self.id = response;
            }));

        }
        function saveQuestion(q) {
            currentPromise.then(function () {
                if (!q.id) {
                    addToCurrent(quizService.createQuestion(self.id, q.text).then(function (response) {
                        q.id = response;
                    }));

                } else {
                    addToCurrent(quizService.updateQuestion(q.id, q.text));
                }
            });

        }
        function saveAnswer(q, a) {

            currentPromise.then(function () {
                if (!a.text) {
                    if (a.id) {
                        if (q.correctAnswer === a.id) {
                            q.correctAnswer = null;
                        }
                        addToCurrent(quizService.deleteAnswer(a.id));
                        a.id = null;
                    }
                    return;
                }
                if (!q.id) {
                    addToCurrent(quizService.createQuestion(self.id, q.text).then(function (response) {
                        q.id = response;
                        quizService.createAnswer(q.id, a.text).then(function (response2) {
                            a.id = response2;
                        });
                    }));
                    return;
                }
                if (!a.id) {
                    addToCurrent(quizService.createAnswer(q.id, a.text).then(function (response2) {
                        a.id = response2;
                    }));
                    return;
                }
                addToCurrent(quizService.updateAnswer(a.id, a.text));

            });
        }
        function deleteAnswerShow(q) {
            return q.answers.length > 2;
        }
        function deleteAnswer(q, a) {
            currentPromise.then(function () {
                if (a.id) {

                    if (q.correctAnswer === a.id) {
                        q.correctAnswer = null;
                    }
                    addToCurrent(quizService.deleteAnswer(a.id));
                    //a.id = null;


                }
                var index = q.answers.indexOf(a);
                q.answers.splice(index, 1);
            });
        }
        function editQuestion(q) {
            q.done = false;
        }
        function deleteQuestionShow() {
            return self.questions.length > 1;
        }
        function deleteQuestion(q) {
            currentPromise.then(function () {

                addToCurrent(quizService.deleteQuestion(q.id));
                var index = self.questions.indexOf(q);

                self.questions.splice(index, 1);


            });
        }
        function markcorrect(q, a) {

            currentPromise.then(function () {
                q.correctAnswer = a.id;
                addToCurrent(quizService.markCorrect(a.id));
            });
        }
        function addQuestion() {
            var valid = true;
            for (var u = 0; u < draft.questions.length; u++) {
                var q2 = draft.questions[u];
                if (isQuestionOk(q2)) {
                    q2.done = true;
                }
                else {
                    valid = false;
                }
            }
            if (valid) {
                self.questions.push(new question());
            }
        };
        function addAnswer(q) {
            q.answers.push(new answer());
        }
        function publish() {
            if (!self.name) {
                $scope.app.showToaster(resManager.get('quizCreateNeedName'), 'quiz-error');
                return;
            }
            for (var v = 0; v < self.questions.length; v++) {
                var q = self.questions[v];
                if (!q.text) {
                    $scope.app.showToaster(resManager.get('quizCreateNeedQuestionText'), 'create-question');
                    return;
                }
                if (q.answers.length < 2) {
                    $scope.app.showToaster(resManager.get('quizCreateNeedAnswers'), 'create-question');
                    return;
                }
                if (q.correctAnswer == null) {
                    $scope.app.showToaster(resManager.get('quizCreateCorrectAnswer'), 'create-question');
                    return;
                }

                for (var l = 0; l < q.answers.length; l++) {
                    if (q.answers[l].text === '') {
                        $scope.app.showToaster(resManager.get('quizCreateNeedAnswerText'), 'publish-btn');
                        return;
                    }
                }
            }
            self.submitDisabled = true;
            quizService.publish(self.id).then(function (response) {
                $rootScope.$broadcast('from-back'); // prevent going back to quiz create page
                $location.url(response.url);
            }, function (response) {
                $scope.app.showToaster(response, 'quizCreate');
            }).finally(function () {
                self.submitDisabled = false;
            });
        }


        function template(q) {
            if (q.done) {
                return 'quiz-question-template.html';
            }
            return 'create-quiz-question-template.html';
        }

        $scope.$on('question-ok', function (e, args) {
            var q = self.questions.filter(function (obj) {
                return obj.$$hashKey == args;
            });
            q[0].done = true;
        });

        function openClose() {
            $uibModal.open({
                animation: true,
                templateUrl: 'quiz-create-leave-template.html',
                controller: 'quizCreateCloseController as c',
                backdropClass: 'quiz-create-backdrop',
                windowClass: 'quiz-create-window',
                resolve: {
                    boxUrl: function () { return self.boxUrl; },
                    quizId: function () {
                        return self.id;
                    }
                }
            });
        };
    }

    function question() {
        var q = this;
        q.text = '';
        q.id = null;
        q.correctAnswer = null;
        q.answers = [new answer(), new answer()];
        q.done = false;
    }

    function answer() {
        var a = this;
        a.id = null;
        a.text = '';
    }

})();