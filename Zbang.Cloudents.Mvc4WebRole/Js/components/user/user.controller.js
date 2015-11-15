﻿(function () {
    angular.module('app.user').controller('UserController', user);
    user.$inject = ['userService', '$stateParams', 'userData'];

    function user(userService, $stateParams, userData) {
        var self = this;
        var boxesPage = 0, friendPage = 0, itemsPage = 0, commentPage = 0;
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




        self.changeTab = function (tab) {
            self.tab = tab;
            switch (self.tab) {
                case self.state.item:
                    loadItems(true);
                case self.state.post:
                    loadComment(true);
                case self.state.quiz:
                    //
                case self.state.friend:
                    loadFriends(true);
                default:
                    loadboxes(true);
            }
        }

        self.template = function () {
            switch (self.tab) {
                case self.state.item:
                    return 'item-template.html';
                case self.state.post:
                    return 'user-feed-template.html';
                case self.state.quiz:
                    //
                case self.state.friend:
                    return 'user-template.html';
                default:
                    return 'box-template.html';
            }
        }

        self.changeTab(self.tab);
        //boxes
        //loadboxes();
        //self.boxesLoadMore = function () {
        //    boxesPage++;
        //    loadboxes();
        //}

        //friends
        //loadFriends();
        //self.friendLoadMore = function () {
        //    friendPage++;
        //    loadFriends();
        //}


        //items
        // loadItems();
        //self.itemsLoadMore = function() {
        //    itemsPage++;
        //    loadItems();
        //}

        function loadItems(fromTab) {
            if (haveData(fromTab,files)) {
                return;
            }

            self.itemsLoading = true;
            userService.files($stateParams.userId, itemsPage).then(function (response) {
                files = files.concat(response);
                if (self.tab === self.state.item) {
                    self.elements = files;
                }
                if (response.length) {
                    self.itemsLoading = false;
                }

            });
        }
        function loadboxes(fromTab) {
            if (haveData(fromTab, boxes)) {
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
                }
            });
        }

        function haveData(fromTab, arr) {
            if (fromTab && arr.length) {
                self.elements = arr;
                return true;
            }
            return false;
        }

        //loadComment();
        //self.commentLoadMore = function () {
        //    commentPage++;
        //    loadComment();
        //}
        function loadComment(fromTab) {
            if (haveData(fromTab, feed)) {
                return;
            }
            self.commentsLoading = true;
            userService.feed($stateParams.userId, commentPage).then(function (response) {
                feed = feed.concat(response);
                if (self.tab === self.state.box) {
                    self.elements = feed;
                }
                if (response.length) {
                    self.commentsLoading = false;
                }
            });
        }


        function loadFriends(fromTab) {
            if (haveData(fromTab, friends)) {
                return;
            }
            self.friendLoading = true;
            userService.friends($stateParams.userId, friendPage).then(function (response) {
                friends = friends.concat(response);
                if (self.tab === self.state.friend) {
                    self.elements = friends;
                }
                if (response.length) {
                    self.friendLoading = false;
                }
            });
        }


       


    }
})();


