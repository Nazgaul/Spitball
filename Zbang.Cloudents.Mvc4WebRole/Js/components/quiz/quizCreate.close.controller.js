(function () {
    angular.module('app.quiz').controller('quizCreateCloseController', popUp);
    popUp.$inject = ['$modalInstance', 'quizService', 'boxUrl', 'quizId', '$location','$timeout'];

    function popUp($modalInstance, quizService, boxUrl, quizId, $location, $timeout) {
        var self = this;
        self.delete = del;
        self.draft = draft;
        
        function draft() {
            $modalInstance.close();
            $timeout(function() {
                $location.url(boxUrl);
            }, 5);

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