(function () {
    'use strict';
    angular.module('app.quiz.challenge', []).controller('QuizChallengeController', popup);

    popup.$inject = ['$modalInstance', 'solvers'];

    function popup($modalInstance, solvers) {
        var c = this;

        c.topUsers = solvers.users;
        c.classmatesCount = solvers.solversCount;
        //qp.score = sheet.score;
        //qp.correct = sheet.correct;
        //qp.wrong = sheet.wrong;


        c.chance = function () {
            $modalInstance.close();
        };

        c.scared = function () {
            $modalInstance.dismiss();
        };
    }
})();