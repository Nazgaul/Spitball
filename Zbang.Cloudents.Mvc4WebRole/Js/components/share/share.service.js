(function () {
    'use strict';
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
            return ajaxService.get('/search/membersinbox/', {
                term: term,
                boxId: boxId,
                page: page
            });
        }

        self.inviteToSystem = function (recipients) {
            return ajaxService.post('/share/invite/', {
                recipients: recipients
            });
        }
        self.inviteToBox = function (recipients, boxId) {
            return ajaxService.post('/share/invitebox/', {
                recipients: recipients,
                boxId: boxId
            })
        }
    }
})()