module app {
    'use strict';
    class UserDetailsController {
        static $inject = ['accountService', '$scope', 'userDetailsFactory'];
        constructor(
            private accountService,
            private $scope: angular.IScope,
            private userDetailsFactory: IUserDetailsFactory
        ) {
            const response = this.userDetailsFactory.get();
            if (response.id) {
                var self = this;
                this.assignValues(response);
                $scope.$on('userDetailsChange', () => {
                    self.assignValues(this.userDetailsFactory.get());
                });
            }
        }

        signup(e) {
            const url = this.getParameterByName('returnUrl', e.target.href);
            e.target.href = e.target.href.replace(encodeURIComponent(url), encodeURIComponent(window.location.href));
        }

        logOut() {
            // we want to remove the user data and not the html
            sessionStorage.clear();
            Intercom("shutdown");
        };
        id: number;
        name: string;
        image: string;
        score: number;
        badges: number;
        private assignValues(response: IUserData) {
            this.id = response.id;
            this.name = response.name;
            this.image = response.image;
            this.score = response.score;
            this.badges = response.badges;
        }
        private getParameterByName(name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        }
    }
    angular.module('app.user.details').controller('UserDetailsController', UserDetailsController);
}



