(function () {
    'use strict';
    angular.module('app.user').controller('UserController', user);
    user.$inject = ['userService', 'userData', 'itemThumbnailService', '$q',
        'userDetailsFactory', '$mdDialog', 'resManager',
        'boxService', '$rootScope', "$state", "$mdMedia"];

    function user(userService, userData, itemThumbnailService, $q, userDetailsFactory,
        $mdDialog, resManager, boxService, $rootScope, $state, $mdMedia) {
        var self = this;
        var boxesPage = 0, friendPage = 0, itemsPage = 0, commentPage = 0, quizzesPage = 0, disablePaging = false;
        self.friends = [];
        self.boxes = [];
        self.files = [];
        self.feed = [];
        self.quiz = [];

        self.isUserProfile = userDetailsFactory.get().id === userData.id;
        self.details = userData;
        self.state = {
            //box: 'b',
            badges: 'b',
            item: 'u',
            post: 'p',
            quiz: 'q',
            friend: 'f'

        }
        self.badgesState = {
            levels: 'l',
            badges: 'b',
            community: 'c'
        }
        //self.tab = self.state.box;
        self.tab = self.state.badges;
        self.badgesTab = self.badgesState.levels;
        self.showInfo = showInfo;
        //self.elements = [];

        self.changeTab = function (tab) {
            self.tab = tab;
            switch (self.tab) {
                case self.state.item:
                    loadItems(true);
                    break;
                case self.state.post:
                    loadComment(true);
                    break;
                case self.state.quiz:
                    loadQuiz(true);
                    break;
                case self.state.friend:
                    loadFriends(true);
                    break;
                    //default:
                    //    loadboxes(true);
            }
        }
        self.changeBadgesTab = function (tab) {
            self.badgesTab = tab;
            switch (self.badgesTab) {
                case self.badgesState.levels:
                    break;
                case self.badgesState.badges:
                    break;
                case self.badgesState.community:
                    break
            }
        }
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
        self.deleteItem = deleteItem;
        self.sendMessage = sendMessage;
        self.showBadge = showBadge;
        self.badgeInfo = null;
        self.communityFilter = "This Month";
        self.badges = [
            {
                name: 'Spitballer',
                image: '',
                progress: 100,
                condition: 'Register',
                points: 500
            },
            {
                name: 'Explorer',
                image: '',
                progress: 100,
                condition: 'Follow 3 Classes',
                points: 500
            },
            {
                name: 'Harambe',
                image: '',
                progress: 80,
                condition: 'The ways to get this badge are secret. Seek and you shall find',
                points: 500
            },
            {
                name: 'Influencer',
                image: '',
                progress: 80,
                condition: 'Spread the word. Share a class or post on Facebook',
                points: 500
            },
            {
                name: 'Explorer',
                image: '',
                progress: 80,
                condition: 'Create a new class or department',
                points: 500
            },
            {
                name: 'Quizzy Lizzy',
                image: '',
                progress: 80,
                condition: 'Create a quiz',
                points: 500
            },
            {
                name: 'Helpful Harry',
                image: '',
                progress: 80,
                condition: 'You have one job: upload 3 documents to your class',
                points: 500
            },
            {
                name: 'Kiss from Keanu',
                image: '',
                progress: 80,
                condition: 'Like three posts or docs that you really think are of great value to unlock this badge',
                points: 500
            },
            {
                name: 'Narrator',
                image: '',
                progress: 80,
                condition: 'Your voice echoes through! Comment 3 times to get this badge',
                points: 500
            },
            {
                name: 'Quite Likely',
                image: '',
                progress: 80,
                condition: 'Like 10 items on our site to get this badge',
                points: 500
            },
            {
                name: 'Sharing is Caring',
                image: '',
                progress: 80,
                condition: 'Share a post or item 5 times. Stellar!',
                points: 500
            },
            {
                name: 'Early Bird',
                image: '',
                progress: 80,
                condition: 'Create a new university to get this badge',
                points: 500
            },
            {
                name: 'Quiz Master',
                image: '',
                progress: 80,
                condition: 'Score a 100% on a quiz',
                points: 500
            },
            {
                name: 'Secret Sally',
                image: '',
                progress: 80,
                condition: 'Create a private study group',
                points: 500
            },
            {
                name: 'Chatterbox',
                image: '',
                progress: 80,
                condition: 'Use the chat feature to communicate with a classmate',
                points: 500
            },
            {
                name: 'Private I',
                image: '',
                progress: 80,
                condition: 'Join or create a private department',
                points: 500
            }
        ];
        self.communityUsers = [
                {
                    name: "Irena Dorfman",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 50800
                },
                {
                    name: "user 2",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 800
                },
                {
                    name: "user 3",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 800000
                },
                {
                    name: "user 4",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 800000
                },
                {
                    name: "user 5",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 800000
                },
                {
                    name: "user 6",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 800000
                }];
        function sendMessage() {
            if ($mdMedia('gt-xs')) {
                $rootScope.$broadcast('open-chat-user',
                {
                    name: userData.name,
                    id: userData.id,
                    image: userData.image,
                    url: userData.url,
                    online: userData.online
                });
                $rootScope.$broadcast("expandChat");
            } else {
                $state.go("chat", {
                    conversationData: {
                        name: userData.name,
                        id: userData.id,
                        image: userData.image,
                        url: userData.url,
                        online: userData.online
                    }
                });
            }
        }

        $rootScope.$on('disablePaging', function () {
            disablePaging = true;
        });
        $rootScope.$on('enablePaging', function () {
            disablePaging = false;
        });
        function deleteItem(ev, item) {
            disablePaging = true;
            var confirm = $mdDialog.confirm()
                 .title(resManager.get('deleteItem'))
                 .targetEvent(ev)
                 .ok(resManager.get('dialogOk'))
                 .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = self.files.indexOf(item);
                self.files.splice(index, 1);
                self.details.numItem--;
                boxService.deleteItem(item.id);
            }).finally(function () {
                disablePaging = false;
            });
        }

        //TODO: write this as one function.
        function loadItems(fromTab) {
            if (fromTab && self.files.length) {
                return returnEmptyPromise();
            }
            if (self.itemsLoading) {
                return returnEmptyPromise();
            }
            if (disablePaging) {
                return returnEmptyPromise();
            }
            self.itemsLoading = true;
            return userService.files(userData.id, itemsPage).then(function (response) {
                angular.forEach(response, function (value) {
                    //value.downloadLink = value.url + 'download/';
                    var retVal = itemThumbnailService.assignValue(value.source);
                    value.thumbnail = retVal.thumbnail;
                    value.nameExtension = value.name.replace(/\.[^/.]+$/, "");
                    value.downloadLink = value.url + 'download/';
                });
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
            return userService.quiz(userData.id, quizzesPage).then(function (response) {
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
            if (fromTab && self.boxes.length) {
                return returnEmptyPromise();
            }
            if (self.boxesLoading) {
                return returnEmptyPromise();
            }
            self.boxesLoading = true;
            return userService.boxes(userData.id, boxesPage).then(function (response) {
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
            return userService.feed(userData.id, commentPage).then(function (response) {
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
            return userService.friends(userData.id, friendPage).then(function (response) {
                self.friends = self.friends.concat(response);
                if (response.length) {
                    friendPage++;
                    self.friendLoading = false;
                }
            });
        }

        function returnEmptyPromise() {
            return $q.when();
        }

        function showBadge(badge) {
            var badgeIndex = self.badges.indexOf(badge);
            self.badgeInfo = {
                data: badge,
                next: self.badges[badgeIndex + 1],
                prev: self.badges[badgeIndex - 1]
            };

        }

        function showInfo() {
            $mdDialog.show({
                controller: DialogController,
                templateUrl: '/user/infodialog/',
                parent: angular.element(document.body),
                clickOutsideToClose: true,
                //fullscreen: true
            });
        }

        function DialogController($scope, $mdDialog) {
            $scope.hide = function () {
                $mdDialog.hide();
            };
        }
    }
})();