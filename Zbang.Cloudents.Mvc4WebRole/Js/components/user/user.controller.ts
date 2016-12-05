module app {
    'use strict';
    

    class User {
        isUserProfile: boolean;
        details;
        static $inject = [
            "user", "profileData", "$state", "$mdMedia","$rootScope"
            //'itemThumbnailService', '$q',
            //'userDetailsFactory', '$mdDialog', 'resManager',
            //'boxService', '$rootScope', "$state", 
        ];
        constructor(
            private user: IUserData,
            private profileData,
            private $state: angular.ui.IStateService,
            private $mdMedia: angular.material.IMedia,
            
            //private itemThumbnailService: IItemThumbnailService,
            //private $q: angular.IQService,
            //private userDetailsFactory: IUserDetailsFactory,
            //private $mdDialog: angular.material.IDialogService,
            //private resManager: IResManager,
            //private boxService: IBoxService,
            private $rootScope: angular.IRootScopeService
            
            
        ) {

            this.isUserProfile = user.id === profileData.id;
            this.details = profileData;
        }
        isActive(state) {
            return state === this.$state.current.name;
        }
        sendMessage() {
            const userData = this.profileData;
            if (this.$mdMedia('gt-xs')) {
                this.$rootScope.$broadcast('open-chat-user',
                    {
                        name: userData.name,
                        id: userData.id,
                        image: userData.image,
                        url: userData.url,
                        online: userData.online
                    });
                this.$rootScope.$broadcast("expandChat");
            } else {
                this.$state.go("chat", {
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
       
       
        //self.badgesState = {
        //    levels: 'l',
        //    badges: 'b',
        //    community: 'c'
        //}
        //self.tab = self.state.box;
        //self.tab = self.state.badges;
        //self.badgesTab = self.badgesState.levels;
        //self.showInfo = showInfo;
        ////self.elements = [];

        
        //self.changeBadgesTab = function (tab) {
        //    self.badgesTab = tab;
        //    switch (self.badgesTab) {
        //        case self.badgesState.levels:
        //            break;
        //        case self.badgesState.badges:
        //            break;
        //        case self.badgesState.community:
        //            break
        //    }
        //}
       
        //self.changeTab(self.tab);
        //self.deleteItem = deleteItem;
        //self.sendMessage = sendMessage;
        //self.showBadge = showBadge;
        //self.badgeInfo = null;
        //self.communityFilter = "This Month";
        //self.badges = [
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
        //self.communityUsers = [
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
        

        

        
       
       



       


        

        //function returnEmptyPromise() {
        //    return $q.when();
        //}

        //function showBadge(badge) {
        //    if (badge) {
        //        var badgeIndex = self.badges.indexOf(badge);
        //        self.badgeInfo = {
        //            data: badge,
        //            next: self.badges[badgeIndex + 1],
        //            prev: self.badges[badgeIndex - 1]
        //        };
        //    }

        //}


        //function showInfo() {
        //    $mdDialog.show({
        //        templateUrl: '/user/infodialog/',
        //        parent: angular.element(document.body),
        //        clickOutsideToClose: true,
        //        controller: "gamificationDialog",
        //        controllerAs: "gd",
        //        //fullscreen: true
        //    });
        //}
    }

    angular.module('app.user').controller('UserController', User);
}