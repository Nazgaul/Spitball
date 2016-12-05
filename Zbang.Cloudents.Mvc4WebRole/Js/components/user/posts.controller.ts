module app {
    "use strict";
    class Feed {
        feed = [];
        commentsLoading = false;
        commentPage = 0;
        static $inject = ["profileData", "userService"];
        constructor(
            private profileData: IUserData,
            private userService: IUserService
        ) {
            this.loadComment();
        }

        loadComment() {
            const self = this;
            self.commentsLoading = true;
            return self.userService.feed(self.profileData.id, self.commentPage).then(response => {
                this.feed = this.feed.concat(response);

                if (response.length) {
                    this.commentsLoading = false;
                    this.commentPage++;
                }
            });
        }
    }

    angular.module("app.user").controller("feed", Feed);
}