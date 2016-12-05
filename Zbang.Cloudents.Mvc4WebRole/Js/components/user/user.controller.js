var app;
(function (app) {
    'use strict';
    var User = (function () {
        function User(user, profileData, $state, $mdMedia, $rootScope) {
            this.user = user;
            this.profileData = profileData;
            this.$state = $state;
            this.$mdMedia = $mdMedia;
            this.$rootScope = $rootScope;
            this.isUserProfile = user.id === profileData.id;
            this.details = profileData;
        }
        User.prototype.isActive = function (state) {
            return state === this.$state.current.name;
        };
        User.prototype.sendMessage = function () {
            var userData = this.profileData;
            if (this.$mdMedia('gt-xs')) {
                this.$rootScope.$broadcast('open-chat-user', {
                    name: userData.name,
                    id: userData.id,
                    image: userData.image,
                    url: userData.url,
                    online: userData.online
                });
                this.$rootScope.$broadcast("expandChat");
            }
            else {
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
        };
        User.$inject = [
            "user", "profileData", "$state", "$mdMedia", "$rootScope"
        ];
        return User;
    }());
    angular.module('app.user').controller('UserController', User);
})(app || (app = {}));
