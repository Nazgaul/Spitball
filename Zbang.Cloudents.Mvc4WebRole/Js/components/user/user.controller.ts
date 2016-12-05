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