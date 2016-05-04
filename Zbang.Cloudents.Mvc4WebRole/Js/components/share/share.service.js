'use strict';
(function () {
    angular.module('app').service('shareService', share);

    share.$inject = ['ajaxService'];
    function share(ajaxService) {
        var self = this;

        self.googleFriends = function (token) {
            return ajaxService.post('/user/googlecontacts/', {
                token: token

            });
        }
        self.users = function (term, boxId, page) {
            return ajaxService.get('/search/members', {
                term: term,
                boxId: boxId,
                page: page
            });
        }

        self.inviteToSystem = function (recipients) {
            return ajaxService.post('/share/invite/', {
                recipients: recipients
            }).then(function (response) {
                ajaxService.post('/share/');
            });;
        }
        self.inviteToBox = function (recipients, boxId) {
            return ajaxService.post('/share/invitebox/', {
                recipients: recipients,
                boxId: boxId
            }).then(function (response) {
                ajaxService.post('/share/');
            });
        }
    }
})()