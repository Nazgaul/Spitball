(function () {
    angular.module('app.quiz').controller('QuizCreateController', quizCreate);
    quizCreate.$inject = ['quizService', 'draft', 'boxUrl'];

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

    function quizCreate(quizService, draft, boxUrl) {
        var self = this;
        console.log(draft, boxUrl);



        self.boxUrl = boxUrl;


        draft = draft || {
            questions : [new question(), new question()]
        };
        self.name = draft.name;
        self.id = draft.id;

        self.questions = draft.questions;




        self.saveName = function () {
            console.log(self.name);
        };
        self.saveQuestion = function (q) {
            console.log(q);
        }
        self.addQuestion = function () {
            self.questions.push(new question());
        };
        self.publish = function () {
            console.log('here');
        }


    }

})();