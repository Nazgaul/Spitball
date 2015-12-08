(function () {
    angular.module('app.user').controller('UserController', user);
    user.$inject = ['userService', '$stateParams', 'userData', 'itemThumbnailService'];

    function user(userService, $stateParams, userData, itemThumbnailService) {
        var self = this;
        var boxesPage = 0, friendPage = 0, itemsPage = 0, commentPage = 0, quizzesPage = 0;
        var friends = [], boxes = [], files = [], feed = [], quiz = [];


        self.details = userData;
        self.state = {
            box: 'b',
            item: 'u',
            post: 'p',
            quiz: 'q',
            friend: 'f'

        }
        self.tab = self.state.box;
        self.elements = [];

        ;



        self.changeTab = function (tab) {
            self.tab = tab;
            switch (self.tab) {
                case self.state.item:
                    loadItems(true);
                    self.bootstrapClass = 'col-md-3';
                    self.angularGridAttribute = '25';
                    break;
                case self.state.post:
                    loadComment(true);
                    self.bootstrapClass = 'col-md-12';
                    self.angularGridAttribute = '100';
                    break;
                case self.state.quiz:
                    loadQuiz(true);
                    self.bootstrapClass = 'col-md-3';
                    self.angularGridAttribute = '25';
                    break;
                case self.state.friend:
                    loadFriends(true);
                    self.bootstrapClass = 'col-md-6';
                    self.angularGridAttribute = '50';
                    break;
                default:
                    loadboxes(true);
                    self.bootstrapClass = 'col-md-4';
                    self.angularGridAttribute = '33';
            }
        }

        self.template = function () {
            switch (self.tab) {
                case self.state.item:
                    return 'item-template.html';
                case self.state.post:
                    return 'user-feed-template.html';
                case self.state.quiz:
                    return 'quiz-template.html';
                case self.state.friend:
                    return 'user-template.html';
                default:
                    return 'box-template.html';
            }
        }
        self.myPagingFunction = function () {
            switch (self.tab) {
                case self.state.item:
                    loadItems();
                    break;
                case self.state.post:
                    loadComment();
                    break;
                case self.state.quiz:
                    loadQuiz();
                    break;
                case self.state.friend:
                    loadFriends();
                    break;
                default:
                    loadboxes();
            }
        }



        self.changeTab(self.tab);

        function haveData(fromTab, arr) {
            if (fromTab && arr.length) {
                self.elements = arr;
                return true;
            }
            return false;
        }
        //TODO: write this as one function.
        function loadItems(fromTab) {
            if (haveData(fromTab, files)) {
                return;
            }
            if (self.itemsLoading) {
                return;
            }
            self.itemsLoading = true;
            userService.files($stateParams.userId, itemsPage).then(function (response) {
                response = itemThumbnailService.assignValues(response);
                files = files.concat(response);
                if (self.tab === self.state.item) {
                    self.elements = files;
                }
                if (response.length) {
                    self.itemsLoading = false;
                    itemsPage++;
                }
            });
        }
        function loadQuiz(fromTab) {
            if (haveData(fromTab, quiz)) {
                return;
            }
            if (self.quizLoading) {
                return;
            }
            self.quizLoading = true;
            userService.quiz($stateParams.userId, quizzesPage).then(function (response) {
                for (var i = 0; i < response.length; i++) {
                    response[i].publish = true;
                }
                quiz = quiz.concat(response);
                if (self.tab === self.state.quiz) {
                    self.elements = quiz;
                }
                if (response.length) {
                    self.quizLoading = false;
                    quizzesPage++;
                }

            });
        }
        function loadboxes(fromTab) {
            if (haveData(fromTab, boxes)) {
                return;
            }
            if (self.boxesLoading) {
                return;
            }
            self.boxesLoading = true;
            userService.boxes($stateParams.userId, boxesPage).then(function (response) {
                boxes = boxes.concat(response);
                if (self.tab === self.state.box) {
                    self.elements = boxes;
                }
                if (response.length) {
                    self.boxesLoading = false;
                    boxesPage++;
                }

            });
        }



        function loadComment(fromTab) {
            if (haveData(fromTab, feed)) {
                return;
            }
            if (self.commentsLoading) {
                return;
            }
            self.commentsLoading = true;
            userService.feed($stateParams.userId, commentPage).then(function (response) {
                feed = feed.concat(response);
                if (self.tab === self.state.post) {
                    self.elements = feed;
                }

                if (response.length) {
                    self.commentsLoading = false;
                    commentPage++;
                }
            });
        }


        function loadFriends(fromTab) {
            if (haveData(fromTab, friends)) {
                return;
            }
            if (self.friendLoading) {
                return;
            }
            self.friendLoading = true;
            userService.friends($stateParams.userId, friendPage).then(function (response) {
                friends = friends.concat(response);
                if (self.tab === self.state.friend) {
                    self.elements = friends;
                }
                if (response.length) {
                    friendPage++;
                    self.friendLoading = false;
                }
            });
        }
    }
})();