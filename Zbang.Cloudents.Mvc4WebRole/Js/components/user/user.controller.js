(function () {
    angular.module('app.user').controller('UserController', user);
    user.$inject = ['userService', '$stateParams'];

    function user(userService, $stateParams) {
        var self = this;
        var boxesPage = 0, friendPage = 0;
        self.friends = [];
        self.boxes = [];


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

        userService.getfiles($stateParams.userId).then(function (response) {
            self.files = response;

        });

        userService.getfeed($stateParams.userId).then(function (response) {
            self.feed = response;
        });



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


