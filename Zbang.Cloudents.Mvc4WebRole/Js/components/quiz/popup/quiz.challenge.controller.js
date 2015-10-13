(function () {
    angular.module('app.quiz.challenge',[]).controller('QuizChallengeController', popup);

    popup.$inject = ['$modalInstance', 'topUsers'];

    function popup($modalInstance, topUsers) {
        var c = this;

        c.topUsers = topUsers;
        c.classmatesCount = 10;
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