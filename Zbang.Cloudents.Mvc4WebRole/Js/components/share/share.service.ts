module app {
    'use strict';
    export interface IShareService {
        googleFriends(token): angular.IPromise<any>;
        users(term, boxId, page): angular.IPromise<any>;
        inviteToSystem(recipients): angular.IPromise<any>;
        inviteToBox(recipients, boxId): angular.IPromise<any>;
        shareDialog(): angular.IPromise<any>;
    }

    class Share implements IShareService {
        static $inject = ["ajaxService2", "$mdDialog"];
        constructor(private ajaxService: IAjaxService2,
            private $mdDialog: angular.material.IDialogService) {

        }

        googleFriends(token) {
            return this.ajaxService.post('/user/googlecontacts/', {
                token: token

            });
        }
        users(term, boxId, page) {
            return this.ajaxService.get('/search/membersinbox/', {
                term: term,
                boxId: boxId,
                page: page
            });
        }

        inviteToSystem(recipients) {
            return this.ajaxService.post('/share/invite/', {
                recipients: recipients
            });
        }
        inviteToBox(recipients, boxId) {
            return this.ajaxService.post('/share/invitebox/',
                {
                    recipients: recipients,
                    boxId: boxId
                });
        }
        shareDialog() {
            return this.$mdDialog.show({
                templateUrl: "/share/sharedialog/",
                //targetEvent: ev,
                clickOutsideToClose: true,
                //locals: {
                //    color1: "rgb(255, 203, 11)",
                //    color2: "rgba(0, 46, 98, 0.901961)"
                //},
                controller: "ShareDialog",
                controllerAs: "sd",
                fullscreen: false
            });
        }
    }
    angular.module("app").service("shareService", Share);

};