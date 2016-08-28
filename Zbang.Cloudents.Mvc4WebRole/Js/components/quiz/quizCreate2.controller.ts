module app {
    //var emptyForm = true;
    var quizId: number;
    var saveInProgress = false;

    function finishUpdate() {
        saveInProgress = false;
    }

    class QuizData {
        id;
        name;
        questions = [new Question()];

        completeData() {
            if (!this.questions.length) {
                this.questions.push(new Question());
            }
        }
        static build(data) {
            const temp = new QuizData();
            temp.id = data.id;
            temp.name = data.name;
            if (data.question) {
                temp.questions = data.question;
            }
            temp.completeData();
            return temp;
        }
    }

    class Question {
        text;
        id;
        correctAnswer;
        answers = [new Answer(), new Answer()];
        done = false;
        template() {
            if (this.done) {
                return 'quiz-question-template.html';
            }
            return 'create-quiz-question-template.html';
        }
        completeQuestion() {
            for (let i = this.answers.length; i < 2; i++) {
                this.answers.push(new Answer());
            }
        }
        addAnswer() {
            this.answers.push(new Answer());
        }
    }
    class Answer {
        id;
        text;
    }

    class QuizCreateController {
        static $inject = ["$mdDialog", "$state", "$stateParams", "$scope",
            "quizService", "quizData", "resManager"];
        private boxName;
        quizNameDisabled = true;

        constructor(private $mdDialog: angular.material.IDialogService,
            private $state: angular.ui.IStateService,
            private $stateParams: ISpitballStateParamsService,
            private $scope: angular.IScope,
            private quizService: IQuizService,
            private quizData: any,
            private resManager: IResManager
           ) {

            this.boxName = $stateParams["name"] || $stateParams["boxName"];
            quizId = $stateParams["quizid"];
            this.newName();

            if (quizData) {
                this.quizData = QuizData.build(quizData);
            } else {
                this.quizData = new QuizData();
            }
            this.quizData.completeData();

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