var app;
(function (app) {
    var ValidQuestion;
    (function (ValidQuestion) {
        ValidQuestion[ValidQuestion["AnswerNeedText"] = 1] = "AnswerNeedText";
        ValidQuestion[ValidQuestion["QuestionNeedText"] = 2] = "QuestionNeedText";
        ValidQuestion[ValidQuestion["QuestionCorrectAnswer"] = 3] = "QuestionCorrectAnswer";
        ValidQuestion[ValidQuestion["QuestionMoreAnswer"] = 4] = "QuestionMoreAnswer";
        ValidQuestion[ValidQuestion["EmptyQuestion"] = 5] = "EmptyQuestion";
        ValidQuestion[ValidQuestion["Ok"] = 6] = "Ok";
    })(ValidQuestion || (ValidQuestion = {}));
    var quizId;
    var saveInProgress = false;
    function finishUpdate() {
        saveInProgress = false;
    }
    var QuizData = (function () {
        function QuizData() {
            this.questions = [new Question()];
        }
        QuizData.prototype.completeData = function () {
            if (!this.questions.length) {
                this.questions.push(new Question());
            }
        };
        QuizData.prototype.deserialize = function (input) {
            this.questions = [];
            this.id = input.id;
            this.name = input.name;
            for (var i = 0; i < input.questions.length; i++) {
                this.questions.push(new Question().deserialize(input.questions[i]));
            }
            this.completeData();
            return this;
        };
        return QuizData;
    }());
    var Question = (function () {
        function Question() {
            this.answers = [new Answer(), new Answer()];
        }
        Question.prototype.completeQuestion = function () {
            for (var i = this.answers.length; i < 2; i++) {
                this.answers.push(new Answer());
            }
        };
        Question.prototype.addAnswer = function () {
            this.answers.push(new Answer());
        };
        Question.prototype.validQuestion = function () {
            var emptyAnswer = true;
            var retVal = [];
            if (!this.text) {
                retVal.push(ValidQuestion.QuestionNeedText);
            }
            else {
                emptyAnswer = false;
            }
            for (var j = 0; j < this.answers.length; j++) {
                var valid = this.answers[j].validAnswer();
                if (valid === ValidQuestion.AnswerNeedText) {
                    retVal.push(valid);
                }
                else {
                    emptyAnswer = false;
                }
            }
            if (this.correctAnswer == null) {
                retVal.push(ValidQuestion.QuestionCorrectAnswer);
            }
            else {
                emptyAnswer = false;
            }
            if (!retVal.length) {
                return ValidQuestion.Ok;
            }
            if (emptyAnswer) {
                return ValidQuestion.EmptyQuestion;
            }
            return retVal[0];
        };
        Question.prototype.deserialize = function (input) {
            this.answers = [];
            this.id = input.id;
            this.text = input.text;
            for (var i = 0; i < input.answers.length; i++) {
                this.answers.push(new Answer().deserialize(input.answers[i]));
            }
            if (angular.isNumber(input.correctAnswer)) {
                this.correctAnswer = input.correctAnswer;
            }
            else {
                this.correctAnswer = this.answers.findIndex(function (x) { return x.id === input.correctAnswer; });
            }
            this.completeQuestion();
            return this;
        };
        return Question;
    }());
    app.Question = Question;
    var Answer = (function () {
        function Answer() {
        }
        Answer.prototype.validAnswer = function () {
            if (this.text) {
                return ValidQuestion.Ok;
            }
            return ValidQuestion.AnswerNeedText;
        };
        Answer.prototype.deserialize = function (input) {
            this.id = input.id;
            this.text = input.text;
            return this;
        };
        return Answer;
    }());
    var QuizCreateController = (function () {
        function QuizCreateController($mdDialog, $state, $stateParams, $scope, quizService, quizData, resManager, $q) {
            var _this = this;
            this.$mdDialog = $mdDialog;
            this.$state = $state;
            this.$stateParams = $stateParams;
            this.$scope = $scope;
            this.quizService = quizService;
            this.quizData = quizData;
            this.resManager = resManager;
            this.$q = $q;
            this.quizNameDisabled = true;
            this.newName = function () {
                var self = _this;
                _this.$scope.$watch(function () { return _this.quizData.name; }, function (newVal, oldVal) {
                    if (newVal === oldVal) {
                        return;
                    }
                    var form = self.$scope['quizName'];
                    if (!form.$valid) {
                        return;
                    }
                    form.$setPristine();
                    if (!saveInProgress) {
                        submitQuizName();
                        function finishSaveName() {
                            finishUpdate();
                            if (!form.$submitted) {
                                submitQuizName();
                            }
                        }
                        function submitQuizName() {
                            form.$setSubmitted();
                            saveInProgress = true;
                            if (quizId) {
                                self.quizService.updateQuiz(quizId, newVal).finally(finishSaveName);
                            }
                            else {
                                self.quizService.createQuiz(self.$stateParams.boxId, newVal).then(function (response) {
                                    self.$state.go('quizCreate', {
                                        boxtype: self.$stateParams["boxtype"],
                                        universityType: self.$stateParams["universityType"],
                                        boxId: self.$stateParams.boxId,
                                        boxName: self.$stateParams["boxName"],
                                        quizid: response,
                                        name: self.$stateParams["name"]
                                    });
                                }).finally(finishSaveName);
                            }
                        }
                    }
                });
            };
            this.navigateBackToBox = function () {
                _this.$state.go("box.quiz", {
                    boxtype: _this.$stateParams["boxtype"],
                    universityType: _this.$stateParams["universityType"],
                    boxId: _this.$stateParams.boxId,
                    boxName: _this.$stateParams["boxName"]
                });
            };
            this.boxName = $stateParams["name"] || $stateParams["boxName"];
            quizId = $stateParams["quizid"];
            this.newName();
            if (quizData) {
                this.quizData = new QuizData().deserialize(quizData);
            }
            else {
                this.quizData = new QuizData();
            }
            this.quizData.completeData();
            console.log(this.quizData);
        }
        QuizCreateController.prototype.addQuestion = function () {
            var _this = this;
            var self = this;
            var $qArray = [this.$q.when()];
            for (var i = 0; i < this.quizData.questions.length; i++) {
                var question = this.quizData.questions[i];
                var validQuestion = question.validQuestion();
                if (validQuestion === ValidQuestion.Ok) {
                    (function (question, index) {
                        $qArray.push(self.quizService.createQuestion(quizId, question)
                            .then(function (response) {
                            self.quizData.questions[index] = new Question().deserialize(response);
                        }));
                    })(question, i);
                }
                else {
                    this.showQuestionErrors(validQuestion, i);
                    return;
                }
            }
            this.$q.when($qArray)
                .then(function () {
                _this.quizData.questions.push(new Question());
            });
        };
        QuizCreateController.prototype.showQuestionErrors = function (valid, index) {
            var form = this.$scope["createQuestions"];
            form['valids_' + index].$setValidity('server', false);
            switch (valid) {
                case ValidQuestion.AnswerNeedText:
                    this.error = this.resManager.get("quizCreateNeedAnswerText");
                    break;
                case ValidQuestion.QuestionNeedText:
                    this.error = this.resManager.get("quizCreateNeedQuestionText");
                    break;
                case ValidQuestion.QuestionCorrectAnswer:
                    this.error = this.resManager.get("quizCreateCorrectAnswer");
                    break;
                case ValidQuestion.QuestionMoreAnswer:
                    this.error = this.resManager.get("quizCreateNeedAnswers");
                    break;
                default:
            }
        };
        QuizCreateController.prototype.removeQuestionFromArray = function (question) {
            var index = this.quizData.questions.indexOf(question);
            this.quizData.questions.splice(index, 1);
        };
        QuizCreateController.prototype.deleteQuestion = function (question) {
            if (question.id) {
                this.quizService.deleteQuestion(question.id);
            }
            this.removeQuestionFromArray(question);
        };
        QuizCreateController.prototype.editQuestion = function (question) {
            var canEdit = true;
            var i = this.quizData.questions.length;
            while (i--) {
                var question_1 = this.quizData.questions[i];
                var validQuestion = question_1.validQuestion();
                if (validQuestion === ValidQuestion.Ok) {
                    continue;
                }
                if (validQuestion === ValidQuestion.EmptyQuestion) {
                    this.removeQuestionFromArray(question_1);
                    continue;
                }
                this.showQuestionErrors(validQuestion, i);
                canEdit = false;
            }
            if (canEdit) {
                if (question.id) {
                    this.quizService.deleteQuestion(question.id);
                }
                angular.forEach(question.answers, function (a) {
                    a.id = null;
                });
                question.id = null;
            }
        };
        QuizCreateController.prototype.close = function (ev) {
            var _this = this;
            if (!quizId) {
                this.navigateBackToBox();
                return;
            }
            var confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('quizLeaveTitle'))
                .textContent(this.resManager.get('quizLeaveContent'))
                .targetEvent(ev)
                .ok(this.resManager.get('quizDelete'))
                .cancel(this.resManager.get('quizSaveAsDraft'));
            this.$mdDialog.show(confirm).then(function () {
                _this.quizService.deleteQuiz(quizId).then(_this.navigateBackToBox);
            }, function () {
                _this.navigateBackToBox();
            });
        };
        QuizCreateController.prototype.template = function (question) {
            if (question.id && question.validQuestion() === ValidQuestion.Ok) {
                return 'quiz-question-template.html';
            }
            return 'create-quiz-question-template.html';
        };
        QuizCreateController.$inject = ["$mdDialog", "$state", "$stateParams", "$scope",
            "quizService", "quizData", "resManager", "$q"];
        return QuizCreateController;
    }());
    angular.module('app.quiz.create').controller('QuizCreateController', QuizCreateController);
})(app || (app = {}));
