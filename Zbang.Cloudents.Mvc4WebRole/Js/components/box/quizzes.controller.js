(function () {
    angular.module('app.box.quizzes').controller('QuizzesController', quizzes);
    quizzes.$inject = ['boxService', '$stateParams'];

    function quizzes(boxService, $stateParams) {
        var q = this;        
        boxService.getQuizzes($stateParams.boxId).then(function (response) {
            q.quizzes = response;
        });

        q.getQuizLetter = function (name) {
            if (angular.isUndefined(name)) {
                return;
            }
            return name[0].toUpperCase();
        }

        q.quizBgColor = function (name) {
            var length = name.length % 17;
            return 'color' + length;


        }
    }
})();