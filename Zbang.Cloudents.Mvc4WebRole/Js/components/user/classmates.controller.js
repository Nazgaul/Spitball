"use strict";
var app;
(function (app) {
    "use strict";
    var Members = (function () {
        function Members(profileData, userService) {
            this.profileData = profileData;
            this.userService = userService;
            this.friends = [];
            this.friendLoading = false;
            this.friendPage = 0;
            this.loadFriends();
        }
        Members.prototype.loadFriends = function () {
            var _this = this;
            var self = this;
            self.friendLoading = true;
            return self.userService.friends(self.profileData.id, self.friendPage).then(function (response) {
                _this.friends = _this.friends.concat(response);
                if (response.length) {
                    _this.friendPage++;
                    _this.friendLoading = false;
                }
            });
        };
        return Members;
    }());
    Members.$inject = ["profileData", "userService"];
    angular.module("app.user").controller("classmates", Members);
})(app || (app = {}));
//# sourceMappingURL=classmates.controller.js.map