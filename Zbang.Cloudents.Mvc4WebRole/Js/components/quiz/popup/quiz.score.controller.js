(function () {
    'use strict';
    angular.module('app.quiz.score', []).controller('QuizScoreController', popup);

    popup.$inject = ['$modalInstance', 'topUsers','sheet'];

    function popup($modalInstance, topUsers,sheet) {
        var s = this;

        s.topUsers = topUsers;        
        s.sheet = sheet;

        s.retakeQuiz = function () {
            $modalInstance.close();
        };

        s.seeAnswers = function () {
            $modalInstance.dismiss();
        };
    }
})();