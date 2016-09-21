module app {
    "use strict";

    interface ISerializable<T> {
        deserialize(input: Object): T;
    }

    enum ValidQuestion {
        AnswerNeedText = 1,
        QuestionNeedText,
        QuestionCorrectAnswer,
        QuestionMoreAnswer,
        EmptyQuestion,
        Ok
    }

    //var emptyForm = true;
    var quizId: number;
    var saveInProgress = false;
    var canNavigateBack = false;

    function finishUpdate() {
        saveInProgress = false;
    }

    class QuizData implements ISerializable<QuizData> {
        id;
        name;
        questions = [new Question()];

        completeData() {
            if (!this.questions.length) {
                this.questions.push(new Question());
            }
        }
        deserialize(input) {
            this.questions = [];

            this.id = input.id;
            this.name = input.name;
            for (let i = 0; i < input.questions.length; i++) {
                this.questions.push(new Question().deserialize(input.questions[i]));
            }
            this.completeData();
            return this;
        }
    }

    export class Question implements ISerializable<Question> {
        text: string;
        id;
        correctAnswer;
        answers = [new Answer(), new Answer()];

        completeQuestion() {
            for (let i = this.answers.length; i < 2; i++) {
                this.answers.push(new Answer());
            }
        }
        addAnswer() {
            this.answers.push(new Answer());
        }
        validQuestion() {
            let emptyQuestion = true;
            const retVal: Array<ValidQuestion> = [];
            if (!this.text) {
                retVal.push(ValidQuestion.QuestionNeedText);
            } else {
                emptyQuestion = false;
            }
            for (let j = 0; j < this.answers.length; j++) {
                const valid = this.answers[j].validAnswer();
                if (valid === ValidQuestion.AnswerNeedText) {
                    retVal.push(valid);
                } else {
                    emptyQuestion = false;
                }
            }
            if (this.correctAnswer == null) {
                retVal.push(ValidQuestion.QuestionCorrectAnswer);
            } else {
                emptyQuestion = false;
            }

            if (!retVal.length) {
                return ValidQuestion.Ok;
            }
            if (emptyQuestion) {
                return ValidQuestion.EmptyQuestion;
            }
            return retVal[0];
        }

        deserialize(input) {
            this.answers = [];
            this.id = input.id;
            this.text = input.text;
            for (let i = 0; i < input.answers.length; i++) {
                this.answers.push(new Answer().deserialize(input.answers[i]));
            }
            if (angular.isNumber(input.correctAnswer)) {
                this.correctAnswer = input.correctAnswer;
            } else {
                this.correctAnswer = this.answers.findIndex(x => x.id === input.correctAnswer);
            }
            this.completeQuestion();
            return this;
        }
    }
    class Answer implements ISerializable<Answer> {
        id;
        text;

        validAnswer() {
            if (this.text) {
                return ValidQuestion.Ok;
            }
            return ValidQuestion.AnswerNeedText;
        }
        deserialize(input) {
            this.id = input.id;
            this.text = input.text;
            return this;
        }
    }

    class QuizCreateController {
        static $inject = ["$mdDialog", "$state", "$stateParams", "$scope",
            "quizService", "quizData", "resManager", "$q", "$window"];
        private boxName;
        quizNameDisabled = true;
        error: string;
        submitDisabled = false;
        constructor(private $mdDialog: angular.material.IDialogService,
            private $state: angular.ui.IStateService,
            private $stateParams: spitaball.ISpitballStateParamsService,
            private $scope: angular.IScope,
            private quizService: IQuizService,
            private quizData: QuizData,
            private resManager: IResManager,
            private $q: angular.IQService,
            private $window: angular.IWindowService
        ) {

            this.boxName = $stateParams["name"] || $stateParams["boxName"];
            quizId = $stateParams["quizid"];
            this.newName();

            if (quizData) {
                this.quizData = new QuizData().deserialize(quizData);
            } else {
                this.quizData = new QuizData();
            }

            $window.onbeforeunload = () => {
                for (let i = 0; i < this.quizData.questions.length; i++) {
                    const question = this.quizData.questions[i];
                    if (!question.id) {
                        return this.resManager.get('quizLeaveTitle');
                    }

                }
            };
            $scope.$on('$destroy', () => {
                $window.onbeforeunload = undefined;
            });
            $scope.$on("$stateChangeStart",
                (event: angular.IAngularEvent) => {
                    if (canNavigateBack) {
                        return;
                    }
                    if (!canNavigate()) {
                        event.preventDefault();
                        $scope.$emit('state-change-start-prevent');
                    }
                });


            function canNavigate(): boolean {
                for (let i = 0; i < this.quizData.questions.length; i++) {
                    const question = this.quizData.questions[i];
                    if (!question.id) {
                        if (!confirm('Are you sure you want to leave this page?')) {
                            return false;
                        }
                    }
                }
                return true;
            }
        }




        private newName = () => {
            var self = this;
            this.$scope.$watch(() => { return this.quizData.name; },
                (newVal: string, oldVal) => {
                    if (newVal === oldVal) {
                        return;
                    }
                    var form: angular.IFormController = self.$scope['quizName'];

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
                            } else {
                                self.createQuiz(newVal).finally(finishSaveName);
                                //self.quizService.createQuiz(self.$stateParams.boxId, newVal).then((response) => {
                                //    quizId = response;
                                //    self.$state.go('quizCreate',
                                //        {
                                //            boxtype: self.$stateParams["boxtype"],
                                //            universityType: self.$stateParams["universityType"],
                                //            boxId: self.$stateParams.boxId,
                                //            boxName: self.$stateParams["boxName"],
                                //            quizid: response,
                                //            name: self.$stateParams["name"]
                                //        });
                                //}).finally(finishSaveName);
                            }
                        }
                    }

                });
        }


        private createQuiz(name: string) {
            var self = this;
            return self.quizService.createQuiz(self.$stateParams.boxId, name).then((response) => {
                quizId = response;
                this.$state.go('quizCreate',
                    {
                        boxtype: self.$stateParams["boxtype"],
                        universityType: self.$stateParams["universityType"],
                        boxId: self.$stateParams.boxId,
                        boxName: self.$stateParams["boxName"],
                        quizid: response,
                        name: self.$stateParams["name"]
                    });
            });
        }

        addQuestion() {
            var self = this;
            var $qArray = [this.$q.when()];
            var promiseCreateQuiz = this.$q.when();
            if (!quizId) {
                promiseCreateQuiz = this.createQuiz(this.quizData.name);
            }
            promiseCreateQuiz.then(() => {
                for (let i = 0; i < this.quizData.questions.length; i++) {
                    const question = this.quizData.questions[i];
                    const validQuestion = question.validQuestion();
                    if (validQuestion !== ValidQuestion.Ok) {
                        this.showQuestionErrors(validQuestion, i);
                        return;
                    }

                    if (validQuestion === ValidQuestion.Ok && !question.id) {
                        ((question, index) => {
                            $qArray.push(self.quizService.createQuestion(quizId, question)
                                .then((response) => {
                                    self.quizData.questions[index] = new Question().deserialize(response);

                                }));
                        })(question, i);
                    }
                }
                this.$q.when($qArray)
                    .then(() => {
                        this.quizData.questions.push(new Question());
                    });
            });
        }
        private showQuestionErrors(valid: ValidQuestion, index: number): void {
            const form: angular.IFormController = this.$scope["createQuestions"];
            form['valids_' + index].$setValidity('server', false);

            switch (valid) {
                case ValidQuestion.AnswerNeedText:
                    this.error = this.resManager.get("quizCreateNeedAnswerText");
                    break;
                case ValidQuestion.QuestionNeedText:
                case ValidQuestion.EmptyQuestion:
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
        }

        private removeQuestionFromArray(question: Question) {
            const index = this.quizData.questions.indexOf(question);
            this.quizData.questions.splice(index, 1);
        }

        deleteQuestion(question: Question) {
            if (question.id) {
                this.quizService.deleteQuestion(question.id);
            }
            this.removeQuestionFromArray(question);
        }

        editQuestion(question: Question) {
            let canEdit = true;
            let i = this.quizData.questions.length;
            while (i--) {

                //}
                //for (let i = 0; i < this.quizData.questions.length; i++) {
                const question = this.quizData.questions[i];
                const validQuestion = question.validQuestion();

                if (validQuestion === ValidQuestion.Ok) {
                    continue;
                }
                if (validQuestion === ValidQuestion.EmptyQuestion) {
                    this.removeQuestionFromArray(question);
                    continue;
                }
                this.showQuestionErrors(validQuestion, i);
                canEdit = false;
                break;

            }
            if (canEdit) {
                if (question.id) {
                    this.quizService.deleteQuestion(question.id);
                }
                //const valid = question.validQuestion();
                //if (valid === ValidQuestion.EmptyQuestion) {
                //    this.removeQuestionFromArray(question);
                //}
                angular.forEach(question.answers,
                    (a: Answer) => {
                        a.id = null;
                    });
                question.id = null;
            }
        }

        close(ev) {
            canNavigateBack = true;
            if (!quizId) {
                this.navigateBackToBox();
                return;
            }
            const confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('quizLeaveTitle'))
                .textContent(this.resManager.get('quizLeaveContent'))
                .targetEvent(ev)
                .ok(this.resManager.get('quizDelete'))
                .cancel(this.resManager.get('quizSaveAsDraft'));

            this.$mdDialog.show(confirm).then(() => {
                this.quizService.deleteQuiz(quizId).then(this.navigateBackToBox);
            }, () => {
                var $qArray = [this.$q.when()], self = this;
                for (let i = 0; i < this.quizData.questions.length; i++) {
                    const question = this.quizData.questions[i];
                    if (question.validQuestion() === ValidQuestion.EmptyQuestion) {
                        continue;
                    }
                    if (!question.id) {
                        ((question) => {
                            $qArray.push(self.quizService.createQuestion(quizId, question));
                        })(question);
                    }
                }
                this.$q.when($qArray)
                    .then(this.navigateBackToBox);
            });
        }


        template(question: Question) {
            if (question.id && question.validQuestion() === ValidQuestion.Ok) {
                return 'quiz-question-template.html';
            }
            return 'create-quiz-question-template.html';
        }

        private navigateBackToBox = () => {
            this.$state.go("box.quiz",
                {
                    boxtype: this.$stateParams["boxtype"],
                    universityType: this.$stateParams["universityType"],
                    boxId: this.$stateParams.boxId,
                    boxName: this.$stateParams["boxName"]
                });
        }

        publish() {
            if (!this.quizData.name) {
                var form: angular.IFormController = this.$scope['quizName'];
                form["name"].$setValidity("required", false);
                return;
            }
            var $qArray = [this.$q.when()];
            for (let i = 0; i < this.quizData.questions.length; i++) {
                const question = this.quizData.questions[i];
                const validQuestion = question.validQuestion();

                if (validQuestion !== ValidQuestion.Ok) {
                    this.showQuestionErrors(validQuestion, i);
                    return;
                }
                if (validQuestion === ValidQuestion.Ok && !question.id) {
                    $qArray.push(this.quizService.createQuestion(quizId, question));
                }

            }
            this.submitDisabled = true;
            this.$q.when($qArray)
                .then(() => {
                    this.quizService.publish(quizId)
                        .then(() => {
                            canNavigateBack = true;
                            this.navigateBackToBox();
                        })
                        .finally(() => {
                            this.submitDisabled = false;
                        })
                        .catch(() => {});
                });
        }

    }

    angular.module('app.quiz.create').controller('QuizCreateController', QuizCreateController);

}