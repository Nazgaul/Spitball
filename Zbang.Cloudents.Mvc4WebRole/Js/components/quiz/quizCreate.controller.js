(function () {
    angular.module('app.quiz').controller('QuizCreateController', quizCreate);
    quizCreate.$inject = ['quizService', 'draft', 'boxUrl', '$stateParams', '$q'];



    function quizCreate(quizService, draft, boxUrl, $stateParams, $q) {
        var self = this;
        self.boxUrl = boxUrl;
        draft = draft || {
            questions: [new question(), new question()]
        };
        self.name = draft.name;
        self.id = draft.id;

        for (var i = 0; i < draft.questions.length; i++) {
            for (var j = draft.questions[i].answers.length; j < 2; j++) {
                draft.questions[i].answers.push(new answer());
            }
        }

        self.questions = draft.questions;

        var defer = $q.defer();
        defer.resolve();
        var currentPromise = defer.promise;


        self.saveName = function () {
            if (self.id) {
                quizService.updateQuiz(self.id, self.name);
            } else {
                currentPromise = quizService.createQuiz($stateParams.boxId, self.name).then(function (response) {
                    self.id = response;
                });
            }
        };
        self.saveNameOnBlur = function () {
            if (self.id) {
                return;
            }
            currentPromise = quizService.createQuiz($stateParams.boxId, self.name).then(function (response) {
                self.id = response;
            });

        }
        self.saveQuestion = function (q) {
            currentPromise.then(function () {
                if (!q.id) {
                    currentPromise = quizService.createQuestion(self.id, q.text).then(function (response) {
                        q.id = response;
                    });

                } else {
                    currentPromise = quizService.updateQuestion(q.id, q.text);
                }
            });

        }
        self.saveAnswer = function (q, a) {

            currentPromise.then(function () {
                if (a.text === '') {
                    if (a.id) {
                        if (q.correctAnswer === a.id) {
                            q.correctAnswer = null;
                        }
                        currentPromise = quizService.deleteAnswer(a.id);
                        a.id = null;
                    }
                    return;
                }
                if (!q.id) {
                    currentPromise = quizService.createQuestion(self.id, q.text).then(function (response) {
                        q.id = response;
                        quizService.createAnswer(q.id, a.text).then(function (response2) {
                            a.id = response2;
                        });
                    });
                    return;
                }
                if (!a.id) {
                    currentPromise = quizService.createAnswer(q.id, a.text).then(function (response2) {
                        a.id = response2;
                    });
                    return;
                }
                currentPromise = quizService.updateAnswer(a.id, a.text);

            });
        }
        self.deleteAnswer = function (q, a) {
            currentPromise.then(function() {
                if (a.id) {
                    
                    if (q.correctAnswer === a.id) {
                        q.correctAnswer = null;
                    }
                    currentPromise = quizService.deleteAnswer(a.id);
                    //a.id = null;
                    var index = q.answers.indexOf(a);

                    q.answers.splice(index, 1);

                }
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
                quizService.markCorrect(a.id);
            });
        }
        self.addQuestion = function () {
            self.questions.push(new question());
        };
        self.addAnswer = function (q) {
            q.answers.push(new answer());
        }

        self.publish = function () {
            console.log('here');
        }


    }

    function question() {
        var q = this;
        q.text = '';
        q.id = null;
        q.correctAnswer = null;
        q.answers = [new answer(), new answer()];
    }

    function answer() {
        var a = this;
        a.id = null;
        a.text = '';
        //a.rightAnswer = false;
    }

})();