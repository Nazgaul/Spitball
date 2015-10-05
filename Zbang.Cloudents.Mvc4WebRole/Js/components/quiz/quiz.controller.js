(function () {
    angular.module('app.quiz').controller('QuizController', quiz);

    quiz.$inject = ['$stateParams', 'quizService', '$sce', '$location'];

    function quiz($stateParams, quizService, $sce, $location) {
        var q = this;      

        quizService.getQuiz($stateParams.boxId, $stateParams.quizId).then(function (data) {            
            q.name = data.quiz.name
            q.questions = data.quiz.questions;
            q.sheet = data.sheet;
            q.topUsers = data.quiz.topUsers;
            q.boxUrl = data.quiz.boxUrl;
        });

        q.createId = getId;
        q.checkAnswers = checkAnswers;
        q.retakeQuiz = retakeQuiz;
        q.pause = pause;
        q.resume= resume;
                
        q.timerControl = {};

        function resume() {

        }

        function pause() {

        }

        function checkAnswers() {
            console.log(q.timerControl.getTime());


        }

        function retakeQuiz() {

        }

        function getId(item) {
            return item.id.slice(0, 6);
        }

        
    }

    
})();


