﻿(function () {
    angular.module('app.quiz').controller('QuizCreateController', quizCreate);
    quizCreate.$inject = ['quizService', 'draft', 'boxUrl', '$stateParams', '$q', '$location', '$scope', '$uibModal'];



    function quizCreate(quizService, draft, boxUrl, $stateParams, $q, $location, $scope, $uibModal) {
        var self = this;
        self.boxUrl = boxUrl;
        draft = draft || {
            questions: [new question(), new question()]
        };
        self.name = draft.name;
        self.quizNameDisabled = self.name.length;
        self.id = draft.id;

        self.openClose = openClose;

        for (var k = draft.questions.length; k < 2; k++) {
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
        function isQuestionOk(q) {
            if (q.text && q.correctAnswer && q.answers.length > 1) {
                for (var j = 0; j < q.answers.length; j++) {
                    if (!q.answers[j].text) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        self.questions = draft.questions;

        var defer = $q.defer();
        defer.resolve();
        var currentPromise = defer.promise;

        function addToCurrent(promise) {
            currentPromise.then(promise);
        }

        self.saveName = function () {
            if (self.id) {
                addToCurrent(quizService.updateQuiz(self.id, self.name));
            } else {
                addToCurrent(quizService.createQuiz($stateParams.boxId, self.name).then(function (response) {
                    self.id = response;
                }));
            }
        };
        self.saveNameOnBlur = function () {
            if (self.id) {
                return;
            }
            addToCurrent(quizService.createQuiz($stateParams.boxId, self.name).then(function (response) {
                self.id = response;
            }));

        }
        self.saveQuestion = function (q) {
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
        self.saveAnswer = function (q, a) {

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
        self.deleteAnswerShow = function (q) {
            return q.answers.length > 2;
        }
        self.deleteAnswer = function (q, a) {
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
        self.editQuestion = function (q) {
            q.done = false;
        }
        self.deleteQuestionShow = function () {
            return self.questions.length > 1;
        }
        self.deleteQuestion = function (q) {
            currentPromise.then(function () {

                addToCurrent(quizService.deleteQuestion(q.id));
                var index = self.questions.indexOf(q);

                self.questions.splice(index, 1);


            });
        }

        self.markcorrect = function (q, a) {
            //if (!q.id) {
            //    quizService.createQuestion(self.id, q.text).then(function (response) {
            //        q.id = response;
            //        quizService.createAnswer(q.id, a.text).then(function (response2) {
            //            a.id = response2;
            //            quizService.markCorrect(a.id);
            //        });
            //    });
            //    return;
            //}
            //if (!a.id) {
            //    quizService.createAnswer(q.id, a.text).then(function (response2) {
            //        a.id = response2;
            //        quizService.markCorrect(a.id);
            //    });
            //    return;
            //}
            currentPromise.then(function () {
                q.correctAnswer = a.id;
                addToCurrent(quizService.markCorrect(a.id));
            });
        }
        self.addQuestion = function () {
            self.questions.push(new question());
        };
        self.addAnswer = function (q) {
            q.answers.push(new answer());
        }



        self.publish = function () {
            if (!self.name) {
                alert('need name');
                return;
            }
            for (var k = 0; k < self.questions.length; k++) {
                var q = self.questions[k];
                if (!q.text) {
                    alert('need question text');
                    return;
                }
                if (q.correctAnswer == null) {
                    alert('need correct answer');
                    return;
                }
                if (q.answers.length < 2) {
                    alert('need minimum 2 answers');
                    return;
                }
                for (var l = 0; l < q.answers.length; l++) {
                    if (q.answers[l].text === '') {
                        alert('need answer text');
                        return;
                    }
                }
            }
            quizService.publish(self.id).then(function (response) {
                $location.url(response);
            }, function (response) {
                alert(response);
            });
        }

        self.template = function (q) {
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
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'quiz-create-leave-template.html',
                controller: 'quizCreateCloseController as c',
                backdrop: 'static',
                keyboard: false,
                backdropClass: 'quiz-create-backdrop',
                windowClass: 'quiz-create-window',
                resolve: {
                    boxUrl: function () { return self.boxUrl; },
                    quizId: function () {
                        return self.id;
                    }
                }
            });

            modalInstance.result.then(function (response) {
                if (response === 'publish') {
                    self.publish();
                }
                
                //    start();
                //}, function () {
                //    afraid();
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
        //a.rightAnswer = false;
    }

})();