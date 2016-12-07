module app {

    class Gamification {
        static $inject = ["$state", "$scope", "userService"];
        levels;
        doneLevel = false;
        badges;
        badgeInfo;
        leaderboard;
        leaderboardMyself = false;
        leaderboardPage = 0;

        //badgesState = {
        //    levels: 'l',
        //    badges: 'b',
        //    community: 'c'
        //}
        //badgesTab = this.badgesState.levels;
        constructor(private $state: angular.ui.IStateService,
            private $scope: angular.IScope,
            private userService: IUserService) {
            $scope["$state"] = this.$state;
            if (!$state.params["type"]) {
                this.$state.go($state.current.name, { type: "level" });
            }
            $scope.$watch(() => { return $state.params["type"] },
                (newVal) => {
                    //console.log(newVal, oldVal);
                    if (newVal === "level") {
                        this.levelTab();
                    }
                    if (newVal === "badge") {
                        this.badgeTab();
                    }
                    if (newVal === "community") {
                        this.communityTab();
                    }
                });




        }
        isActive(state) {
            return this.$state.params["type"] === state;
        }

        private levelTab() {
            if (this.doneLevel) {
                return;
            }
            this.levels = {};
            this.userService.levels(this.$state.params["userId"])
                .then(response => {
                    var i: number;
                    //var progress = { progress: 0 }
                    for (i = 0; i < response.number; i++) {
                        this.levels["l" + i] = { progress: 100 };
                    }
                    this.levels["l" + response.number] = { progress: response.score / response.nextLevel * 100 };
                    for (i = response.number + 1; i < 5; i++) {
                        this.levels["l" + i] = { progress: 0 };
                    }
                    this.doneLevel = true;
                    // this.levels = response;
                });
        }

        private badgeTab() {
            if (this.badges) {
                return;
            }
            this.userService.badges(this.$state.params["userId"])
                .then(response => {
                    console.log(response);
                    this.badges = response.badges;
                    angular.forEach(this.badges,
                        (v) => {
                            var badge = response.model.find(f => f.badge === v.name);
                            if (badge) {
                                v.progress = badge.progress;
                            } else {
                                v.progress = 0;
                            }
                        });
                });
        }
        private communityTab() {
            //if (this.leaderboard) {
            //    return;
            //}
            this.userService.leaderboard(this.$state.params["userId"], this.leaderboardMyself, this.leaderboardPage)
                .then(response => {
                    for (let i = 0; i < response.length; i++) {
                        const elem = response[i];
                        if (i === 0) {
                            elem.progress = 100;
                            continue;
                        }
                        elem.progress = elem.score / response[0].score * 100;
                    }
                    this.leaderboard = response;
                });
        }
        goToSelf() {
            this.leaderboardPage = 0;
            this.leaderboardMyself = !this.leaderboardMyself;
            this.communityTab();
        }
        showBadge(badge) {
            if (!badge) {
                return;
            }
            const badgeIndex = this.badges.indexOf(badge);
            this.badgeInfo = {
                data: badge,
                next: this.badges[badgeIndex + 1],
                prev: this.badges[badgeIndex - 1]
            };
        }

    }

    //badges = [
    //    {
    //        name: 'Spitballer',
    //        image: '',
    //        progress: 100,
    //        condition: 'Register',
    //        points: 500
    //    },
    //    {
    //        name: 'Explorer',
    //        image: '',
    //        progress: 100,
    //        condition: 'Follow 3 Classes',
    //        points: 500
    //    },
    //    {
    //        name: 'Harambe',
    //        image: '',
    //        progress: 80,
    //        condition: 'The ways to get this badge are secret. Seek and you shall find',
    //        points: 500
    //    },
    //    {
    //        name: 'Influencer',
    //        image: '',
    //        progress: 80,
    //        condition: 'Spread the word. Share a class or post on Facebook',
    //        points: 500
    //    },
    //    {
    //        name: 'Explorer',
    //        image: '',
    //        progress: 80,
    //        condition: 'Create a new class or department',
    //        points: 500
    //    },
    //    {
    //        name: 'Quizzy Lizzy',
    //        image: '',
    //        progress: 80,
    //        condition: 'Create a quiz',
    //        points: 500
    //    },
    //    {
    //        name: 'Helpful Harry',
    //        image: '',
    //        progress: 80,
    //        condition: 'You have one job: upload 3 documents to your class',
    //        points: 500
    //    },
    //    {
    //        name: 'Kiss from Keanu',
    //        image: '',
    //        progress: 80,
    //        condition: 'Like three posts or docs that you really think are of great value to unlock this badge',
    //        points: 500
    //    },
    //    {
    //        name: 'Narrator',
    //        image: '',
    //        progress: 80,
    //        condition: 'Your voice echoes through! Comment 3 times to get this badge',
    //        points: 500
    //    },
    //    {
    //        name: 'Quite Likely',
    //        image: '',
    //        progress: 80,
    //        condition: 'Like 10 items on our site to get this badge',
    //        points: 500
    //    },
    //    {
    //        name: 'Sharing is Caring',
    //        image: '',
    //        progress: 80,
    //        condition: 'Share a post or item 5 times. Stellar!',
    //        points: 500
    //    },
    //    {
    //        name: 'Early Bird',
    //        image: '',
    //        progress: 80,
    //        condition: 'Create a new university to get this badge',
    //        points: 500
    //    },
    //    {
    //        name: 'Quiz Master',
    //        image: '',
    //        progress: 80,
    //        condition: 'Score a 100% on a quiz',
    //        points: 500
    //    },
    //    {
    //        name: 'Secret Sally',
    //        image: '',
    //        progress: 80,
    //        condition: 'Create a private study group',
    //        points: 500
    //    },
    //    {
    //        name: 'Chatterbox',
    //        image: '',
    //        progress: 80,
    //        condition: 'Use the chat feature to communicate with a classmate',
    //        points: 500
    //    },
    //    {
    //        name: 'Private I',
    //        image: '',
    //        progress: 80,
    //        condition: 'Join or create a private department',
    //        points: 500
    //    }
    //];
    //communityUsers = [
    //    {
    //        name: "Irena Dorfman",
    //        image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
    //        points: 50800
    //    },
    //    {
    //        name: "user 2",
    //        image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
    //        points: 800
    //    },
    //    {
    //        name: "user 3",
    //        image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
    //        points: 800000
    //    },
    //    {
    //        name: "user 4",
    //        image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
    //        points: 800000
    //    },
    //    {
    //        name: "user 5",
    //        image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
    //        points: 800000
    //    },
    //    {
    //        name: "user 6",
    //        image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
    //        points: 800000
    //    }];


    angular.module("app.user").controller("gamification", Gamification);
}