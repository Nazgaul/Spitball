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
        var createPromise  = defer.promise;


        self.saveName = function () {
            if (self.id) {
                quizService.updateQuiz(self.id, self.name);
            } else {
                createPromise = quizService.createQuiz($stateParams.boxId, self.name).then(function (response) {
                    self.id = response;
                });
            }
        };
        self.saveNameOnBlur = function () {
            if (self.id) {
                return;
            }
            createPromise = quizService.createQuiz($stateParams.boxId, self.name).then(function (response) {
                self.id = response;
            });

        }
        self.saveQuestion = function (q) {
            createPromise.then(function() {
               if (!q.id) {
                   quizService.createQuestion(self.id, q.text).then(function(response) {
                       q.id = response;
                   });

               } else {
                   quizService.updateQuestion(q.id, q.text);
               }
            });
            
        }
        self.addQuestion = function () {
            self.questions.push(new question());
        };
        self.addAnswer = function(q) {
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