interface Array<T> {
    findIndex(predicate: (search: T) => boolean): number;
}
module app {


    interface ISerializable<T> {
        deserialize(input: Object): T;
    }

    enum ValidQuestion {
        AnswerNeedText = 1,
        QuestionNeedText,
        QuestionCorrectAnswer,
        QuestionMoreAnswer,
        Ok
    }

    //var emptyForm = true;
    var quizId: number;
    var saveInProgress = false;

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
            if (!this.text) {
                return ValidQuestion.QuestionNeedText;
            }
            for (let j = 0; j < this.answers.length; j++) {
                const valid = this.answers[j].validAnswer();
                if (!valid) {
                    return valid;
                }
            }
            if (this.correctAnswer == null) {
                return ValidQuestion.QuestionCorrectAnswer;
            }
            return ValidQuestion.Ok;
        }
        isEmpty() {
            
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
            "quizService", "quizData", "resManager", "$q"];
        private boxName;
        quizNameDisabled = true;
        error: string;

        constructor(private $mdDialog: angular.material.IDialogService,
            private $state: angular.ui.IStateService,
            private $stateParams: ISpitballStateParamsService,
            private $scope: angular.IScope,
            private quizService: IQuizService,
            private quizData: QuizData,
            private resManager: IResManager,
            private $q: angular.IQService
        ) {

            this.boxName = $stateParams["name"] || $stateParams["boxName"];
            quizId = $stateParams["quizid"];
            this.newName();

            if (quizData) {
                this.quizData = new QuizData().deserialize(quizData);
            } else {
                this.quizData = new QuizData();
            }

            this.quizData.completeData();
            console.log(this.quizData);

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
                                self.quizService.createQuiz(self.$stateParams.boxId, newVal).then((response) => {
                                    self.$state.go('quizCreate',
                                        {
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
        }



        addQuestion() {
            var self = this;
            var $qArray = [this.$q.when()];
            for (let i = 0; i < this.quizData.questions.length; i++) {
                const question = this.quizData.questions[i];
                //if (question.id) {
                //    continue;
                //}
                if (this.validateAndShowErrorsQuestion(question, i)) {
                    ((question, index) => {
                        $qArray.push(self.quizService.createQuestion(quizId, question)
                            .then((response) => {
                                self.quizData.questions[index] = new Question().deserialize(response);

                            }));
                    })(question, i);

                } else {
                    return;
                }
            }
            this.$q.when($qArray)
                .then(() => {
                    this.quizData.questions.push(new Question());
                });
        }
        private validateAndShowErrorsQuestion(question: Question, i: number): boolean {
            const valid = question.validQuestion();
            if (valid !== ValidQuestion.Ok) {
                const form: angular.IFormController = this.$scope["createQuestions"];
                form['valids_' + i].$setValidity('server', false);

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
                return false;
            }
            return true;
        }


        deleteQuestion(question: Question) {
            if (question.id) {
                this.quizService.deleteQuestion(question.id);
            }
            const index = this.quizData.questions.indexOf(question);
            this.quizData.questions.splice(index, 1);
        }

        editQuestion(question: Question) {

            if (question.id) {
                this.quizService.deleteQuestion(question.id);
            }
            question.id = null;
        }

        close(ev) {
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
                this.navigateBackToBox();
                //self.saveDraft();
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

    }

    angular.module('app.quiz.create').controller('QuizCreateController', QuizCreateController);

}