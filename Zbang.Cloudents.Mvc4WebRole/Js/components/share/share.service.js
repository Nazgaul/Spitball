"use strict";
var app;
(function (app) {
    'use strict';
    var Share = (function () {
        function Share(ajaxService, $mdDialog) {
            this.ajaxService = ajaxService;
            this.$mdDialog = $mdDialog;
        }
        Share.prototype.googleFriends = function (token) {
            return this.ajaxService.post('/user/googlecontacts/', {
                token: token
            });
        };
        Share.prototype.users = function (term, boxId, page) {
            return this.ajaxService.get('/search/membersinbox/', {
                term: term,
                boxId: boxId,
                page: page
            });
        };
        Share.prototype.inviteToSystem = function (recipients) {
            return this.ajaxService.post('/share/invite/', {
                recipients: recipients
            });
        };
        Share.prototype.inviteToBox = function (recipients, boxId) {
            return this.ajaxService.post('/share/invitebox/', {
                recipients: recipients,
                boxId: boxId
            });
        };
        Share.prototype.shareDialog = function (what, id) {
            return this.$mdDialog.show({
                templateUrl: "/share/sharedialog/",
                //targetEvent: ev,
                clickOutsideToClose: true,
                locals: {
                    data: {
                        what: what,
                        id: id
                    }
                },
                controller: "ShareDialog",
                controllerAs: "sd",
                fullscreen: false
            });
        };
        Share.$inject = ["ajaxService2", "$mdDialog"];
        return Share;
    }());
    angular.module("app").service("shareService", Share);
})(app || (app = {}));
;
//# sourceMappingURL=share.service.js.map