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


    }

    
})();


