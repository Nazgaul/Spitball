module app {
    "use strict";
    class Members {
        friends = [];
        friendLoading = false;
        friendPage = 0;
        static $inject = ["profileData", "userService"];
        constructor(
            private profileData: IUserData,
            private userService: IUserService
        ) {
            this.loadFriends();
        }

        loadFriends() {
            const self = this;
            
            self.friendLoading = true;
            return self.userService.friends(self.profileData.id, self.friendPage).then(response => {
                this.friends = this.friends.concat(response);
                if (response.length) {
                    this.friendPage++;
                    this.friendLoading = false;
                }
            });
        }
    }

    angular.module("app.user").controller("classmates", Members);
}