"use strict";
var app;
(function (app) {
    'use strict';
    var UserDetailsController = (function () {
        function UserDetailsController(accountService, $scope, userDetailsFactory) {
            var _this = this;
            this.accountService = accountService;
            this.$scope = $scope;
            this.userDetailsFactory = userDetailsFactory;
            var response = this.userDetailsFactory.get();
            if (response.id) {
                var self = this;
                this.assignValues(response);
                $scope.$on('userDetailsChange', function () {
                    self.assignValues(_this.userDetailsFactory.get());
                });
            }
        }
        UserDetailsController.prototype.signup = function (e) {
            var url = this.getParameterByName('returnUrl', e.target.href);
            e.target.href = e.target.href.replace(encodeURIComponent(url), encodeURIComponent(window.location.href));
        };
        UserDetailsController.prototype.logOut = function () {
            // we want to remove the user data and not the html
            sessionStorage.clear();
            Intercom("shutdown");
        };
        ;
        UserDetailsController.prototype.assignValues = function (response) {
            this.id = response.id;
            this.name = response.name;
            this.image = response.image;
            this.score = response.score;
            this.badges = response.badges;
        };
        UserDetailsController.prototype.getParameterByName = function (name, url) {
            if (!url)
                url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"), results = regex.exec(url);
            if (!results)
                return null;
            if (!results[2])
                return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        };
        UserDetailsController.$inject = ['accountService', '$scope', 'userDetailsFactory'];
        return UserDetailsController;
    }());
    angular.module('app.user.details').controller('UserDetailsController', UserDetailsController);
})(app || (app = {}));
//# sourceMappingURL=userdetails.controller.js.map