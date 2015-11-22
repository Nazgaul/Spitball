(function () {
    angular.module('app.quiz').controller('quizCreateCloseController', popUp);
    popUp.$inject = ['$modalInstance', 'quizService', 'boxUrl', 'quizId', '$location'];

    function popUp($modalInstance, quizService, boxUrl, quizId, $location) {
        var self = this;
        self.delete = del;
        self.draft = draft;
        
        function draft() {
            $modalInstance.close();
            $location.url(boxUrl);

        }

        function del() {
            if (quizId) {
                quizService.deleteQuiz(quizId).then(function() {
                    draft();
                });
                return;
            }
            draft();
        }
    }
})()