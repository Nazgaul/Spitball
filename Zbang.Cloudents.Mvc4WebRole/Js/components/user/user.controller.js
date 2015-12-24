﻿(function () {
    angular.module('app.user').controller('UserController', user);
    user.$inject = ['userService', '$stateParams', 'userData', 'itemThumbnailService', '$q'];

    function user(userService, $stateParams, userData, itemThumbnailService, $q) {
        var self = this;
        var boxesPage = 0, friendPage = 0, itemsPage = 0, commentPage = 0, quizzesPage = 0;
        self.friends = [];
        self.boxes = [];
        self.files = [];
        self.feed = [];
        self.quiz = [];


        self.details = userData;
        self.state = {
            box: 'b',
            item: 'u',
            post: 'p',
            quiz: 'q',
            friend: 'f'

        }
        self.tab = self.state.box;
        //self.elements = [];

        self.changeTab = function (tab) {
            self.tab = tab;
            switch (self.tab) {
                case self.state.item:
                    loadItems(true);
                    //self.angularGridAttribute = 20;
                    //self.flexSm = 33;
                    //self.flexMd = 33;
                    //self.flexXl = 15;
                    //self.flexXs = 50;
                    break;
                case self.state.post:
                    loadComment(true);
                    //self.angularGridAttribute = 100;
                    //self.flexSm = 100;
                    //self.flexMd = 100;
                    //self.flexXl = 100;
                    //self.flexXs = 100;
                    break;
                case self.state.quiz:
                    loadQuiz(true);
                    //self.angularGridAttribute = 20;
                    //self.flexSm = 33;
                    //self.flexMd = 33;
                    //self.flexXs = 50;
                    //self.flexXl = 15;
                    break;
                case self.state.friend:
                    loadFriends(true);
                    //self.angularGridAttribute = 50;
                    //self.flexSm = 100;
                    //self.flexXs = 100;
                    //self.flexMd = 50;
                    //self.flexXl = 25;
                    break;
                default:
                    loadboxes(true);
                    //self.angularGridAttribute = 33;
                    //self.flexXs = 100;
                    //self.flexSm = 100;
                    //self.flexMd = 50;
                    //self.flexXl = 33;
            }
        }

        //self.template = function () {
        //    switch (self.tab) {
        //        case self.state.item:
        //            return 'item-template.html';
        //        case self.state.post:
        //            return 'user-feed-template.html';
        //        case self.state.quiz:
        //            return 'quiz-template.html';
        //        case self.state.friend:
        //            return 'user-template.html';
        //        default:
        //            return 'box-template.html';
        //    }
        //}
        self.myPagingFunction = function () {
            switch (self.tab) {
                case self.state.item:
                    return loadItems();
                case self.state.post:
                    return loadComment();
                case self.state.quiz:
                    return loadQuiz();
                case self.state.friend:
                    return loadFriends();
                default:
                    return loadboxes();
            }
        }



        self.changeTab(self.tab);

        //function haveData(fromTab, arr) {
        //    if (fromTab && arr.length) {
        //        self.elements = arr;
        //        return true;
        //    }
        //    return false;
        //}
        //TODO: write this as one function.
        function loadItems(fromTab) {
            if (fromTab && self.files.length) {
                return returnEmptyPromise();
            }
            if (self.itemsLoading) {
                return returnEmptyPromise();
            }
            self.itemsLoading = true;
            return userService.files($stateParams.userId, itemsPage).then(function (response) {
                response = itemThumbnailService.assignValues(response);
                self.files = self.files.concat(response);
                if (response.length) {
                    self.itemsLoading = false;
                    itemsPage++;
                }
            });
        }
        function loadQuiz(fromTab) {
            if (fromTab && self.quiz.length) {
                return returnEmptyPromise();
            }
            if (self.quizLoading) {
                return returnEmptyPromise();
            }
            self.quizLoading = true;
            return userService.quiz($stateParams.userId, quizzesPage).then(function (response) {
                for (var i = 0; i < response.length; i++) {
                    response[i].publish = true;
                }
                self.quiz = self.quiz.concat(response);
                if (response.length) {
                    self.quizLoading = false;
                    quizzesPage++;
                }

            });
        }
        function loadboxes(fromTab) {
            if (fromTab && self.boxes.length){
                return returnEmptyPromise();
            }
            if (self.boxesLoading) {
                return returnEmptyPromise();
            }
            self.boxesLoading = true;
            return userService.boxes($stateParams.userId, boxesPage).then(function (response) {
                self.boxes = self.boxes.concat(response);
                if (response.length) {
                    self.boxesLoading = false;
                    boxesPage++;
                }

            });
        }



        function loadComment(fromTab) {
            if (fromTab && self.feed.length) {
                return returnEmptyPromise();
            }
            if (self.commentsLoading) {
                return returnEmptyPromise();
            }
            self.commentsLoading = true;
            return userService.feed($stateParams.userId, commentPage).then(function (response) {
                self.feed = self.feed.concat(response);

                if (response.length) {
                    self.commentsLoading = false;
                    commentPage++;
                }
            });
        }


        function loadFriends(fromTab) {
            if (fromTab && self.friends.length) {
                return returnEmptyPromise();
            }
            if (self.friendLoading) {
                return returnEmptyPromise();
            }
            self.friendLoading = true;
            return userService.friends($stateParams.userId, friendPage).then(function (response) {
                self.friends = self.friends.concat(response);
                if (response.length) {
                    friendPage++;
                    self.friendLoading = false;
                }
            });
        }

        function returnEmptyPromise() {
            var defer = $q.defer();
            defer.resolve();
            return defer.promise;
        }
    }
})();