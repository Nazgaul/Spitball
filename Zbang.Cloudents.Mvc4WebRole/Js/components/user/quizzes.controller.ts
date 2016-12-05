module app {
    "use strict";
    class Quiz {
        quiz = [];
        quizLoading = false;
        quizzesPage = 0;
        static $inject = ["profileData", "userService"
        ];
        constructor(
            private profileData: IUserData,
            private userService: IUserService
        ) {
            this.loadQuiz();
        }

        loadQuiz() {
            //if (fromTab && self.quiz.length) {
            //    return returnEmptyPromise();
            //}
            //if (self.quizLoading) {
            //    return returnEmptyPromise();
            //}
            var self = this;
            self.quizLoading = true;
            return this.userService.quiz(self.profileData.id, self.quizzesPage).then(response => {
                for (let i = 0; i < response.length; i++) {
                    response[i].publish = true;
                }
                this.quiz = this.quiz.concat(response);
                if (response.length) {
                    this.quizLoading = false;
                    self.quizzesPage++;
                }

            });
        }
    }

    angular.module("app.user").controller("quiz", Quiz);
}