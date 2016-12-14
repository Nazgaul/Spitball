var app;
(function (app) {
    "use strict";
    var Feed = (function () {
        function Feed(profileData, userService) {
            this.profileData = profileData;
            this.userService = userService;
            this.feed = [];
            this.commentsLoading = false;
            this.commentPage = 0;
            this.loadComment();
        }
        Feed.prototype.loadComment = function () {
            var _this = this;
            var self = this;
            self.commentsLoading = true;
            return self.userService.feed(self.profileData.id, self.commentPage).then(function (response) {
                _this.feed = _this.feed.concat(response);
                if (response.length) {
                    _this.commentsLoading = false;
                    _this.commentPage++;
                }
            });
        };
        Feed.$inject = ["profileData", "userService"];
        return Feed;
    }());
    angular.module("app.user").controller("feed", Feed);
})(app || (app = {}));
