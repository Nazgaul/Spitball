var app;
(function (app) {
    "use strict";
    var Quiz = (function () {
        function Quiz(profileData, userService) {
            this.profileData = profileData;
            this.userService = userService;
            this.quiz = [];
            this.quizLoading = false;
            this.quizzesPage = 0;
            this.loadQuiz();
        }
        Quiz.prototype.loadQuiz = function () {
            var _this = this;
            var self = this;
            self.quizLoading = true;
            return this.userService.quiz(self.profileData.id, self.quizzesPage).then(function (response) {
                for (var i = 0; i < response.length; i++) {
                    response[i].publish = true;
                }
                _this.quiz = _this.quiz.concat(response);
                if (response.length) {
                    _this.quizLoading = false;
                    self.quizzesPage++;
                }
            });
        };
        Quiz.$inject = ["profileData", "userService"
        ];
        return Quiz;
    }());
    angular.module("app.user").controller("quiz", Quiz);
})(app || (app = {}));
