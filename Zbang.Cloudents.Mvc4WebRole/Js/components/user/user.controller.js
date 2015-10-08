(function () {
    angular.module('app.user').controller('UserController', user);
    user.$inject = ['userService', '$stateParams'];

    function user(userService, $stateParams) {
        var self = this;
        var boxesPage = 0, friendPage = 0, itemsPage = 0, commentPage = 0;
        self.friends = [];
        self.boxes = [];
        self.files = [];
        self.feed = [];


        userService.getDetails($stateParams.userId).then(function (response) {
            self.details = response;
        });


        //boxes
        loadboxes();
        self.boxesLoadMore = function () {
            boxesPage++;
            loadboxes();
        }

        //friends
        loadFriends();
        self.friendLoadMore = function () {
            friendPage++;
            loadFriends();
        }


        //items
        loadItems();
        self.itemsLoadMore = function() {
            itemsPage++;
            loadItems();
        }

        function loadItems() {
            self.itemsLoading = true;
            userService.files($stateParams.userId, itemsPage).then(function (response) {
                self.files = self.files.concat(response);
                if (response.length) {
                    self.itemsLoading = false;
                }

            });
        }

        loadComment();
        self.commentLoadMore = function () {
            commentPage++;
            loadComment();
        }
        function loadComment() {
            self.commentsLoading = true;
            userService.feed($stateParams.userId, commentPage).then(function (response) {
                self.feed = self.feed.concat(response);
                if (response.length) {
                    self.commentsLoading = false;
                }
            });
        }


        function loadFriends() {
            self.friendLoading = true;
            userService.friends($stateParams.userId, friendPage).then(function (response) {
                self.friends = self.friends.concat(response);
                if (response.length) {
                    self.friendLoading = false;
                }
            });
        }


        function loadboxes() {
            self.boxesLoading = true;
            userService.boxes($stateParams.userId, boxesPage).then(function (response) {
                self.boxes = self.boxes.concat(response);
                if (response.length) {
                    self.boxesLoading = false;
                }
            });
        }


    }
})();


