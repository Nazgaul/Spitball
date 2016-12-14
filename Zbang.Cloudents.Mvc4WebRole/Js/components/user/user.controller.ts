module app {
    'use strict';

    class User {
        isUserProfile: boolean;
        details;
        leaderboardUser: IUserGamification;
        static $inject = [
            "user", "profileData", "$state", "$mdMedia","$rootScope"
           
        ];
        constructor(
            private user: IUserData,
            private profileData,
            private $state: angular.ui.IStateService,
            private $mdMedia: angular.material.IMedia,
            private $rootScope: angular.IRootScopeService
        ) {
            this.isUserProfile = user.id === profileData.id;
            this.details = profileData;


            this.leaderboardUser = {
                name: profileData.name,
                image: profileData.image,
                levelName: profileData.levelName,
                progress: profileData.score / profileData.nextLevel * 100,
                points: profileData.score,
                rank: profileData.location

            };
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
    }
    angular.module('app.user').controller('UserController', User);
}