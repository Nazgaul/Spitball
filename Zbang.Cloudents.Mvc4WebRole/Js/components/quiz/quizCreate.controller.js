(function() {
    angular.module('app.quiz').controller('QuizCreateController', quizCreate);
    quizCreate.$inject = ['quizService','$scope'];

    function quizCreate(quizService, $scope) {
        var self = this;




        
        function question() {
            var q = this;
            q.htmlText = '';
            q.answers = [new answer(), new answer()];
        }

        function answer() {
            var a = this;
            a.text = '';
            a.rightAnswer = false;
        }

        self.name = '';
        self.questions = [new question(), new question()];


       

        self.saveName = function () {

           // console.log(self.name);
        };
        self.saveQuestion = function(q) {
           // console.log(q);
        }
        self.addQuestion = function() {
            self.questions.push(new question());
        };
        self.publish = function() {
            
        }


    }

})();