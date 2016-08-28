var app;
(function (app) {
    //var emptyForm = true;
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
        QuizData.build = function (data) {
            var temp = new QuizData();
            temp.id = data.id;
            temp.name = data.name;
            if (data.question) {
                temp.questions = data.question;
            }
            temp.completeData();
            return temp;
        };
        return QuizData;
    }());
    var Question = (function () {
        function Question() {
            this.answers = [new Answer(), new Answer()];
            this.done = false;
        }
        Question.prototype.template = function () {
            if (this.done) {
                return 'quiz-question-template.html';
            }
            return 'create-quiz-question-template.html';
        };
        Question.prototype.completeQuestion = function () {
            for (var i = this.answers.length; i < 2; i++) {
                this.answers.push(new Answer());
            }
        };
        Question.prototype.addAnswer = function () {
            this.answers.push(new Answer());
        };
        return Question;
    }());
    var Answer = (function () {
        function Answer() {
        }
        return Answer;
    }());
    var QuizCreateController = (function () {
        function QuizCreateController($mdDialog, $state, $stateParams, $scope, quizService, quizData, resManager) {
            var _this = this;
            this.$mdDialog = $mdDialog;
            this.$state = $state;
            this.$stateParams = $stateParams;
            this.$scope = $scope;
            this.quizService = quizService;
            this.quizData = quizData;
            this.resManager = resManager;
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
                this.quizData = QuizData.build(quizData);
            }
            else {
                this.quizData = new QuizData();
            }
            this.quizData.completeData();
        }
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
                //self.saveDraft();
            });
        };
        QuizCreateController.$inject = ["$mdDialog", "$state", "$stateParams", "$scope",
            "quizService", "quizData", "resManager"];
        return QuizCreateController;
    }());
    angular.module('app.quiz.create').controller('QuizCreateController', QuizCreateController);
})(app || (app = {}));
