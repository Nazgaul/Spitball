(function() {
    angular.module('app').service('shareService', share);

    share.$inject = ['ajaxService'];
    function share(ajaxService) {
        var self = this;

        self.googleFriends = function(token) {
            return ajaxService.post('/user/googlecontacts/', {
                token: token

            });
        }

        self.inviteToSystem = function(recipients) {
            return ajaxService.post('/share/invite', { recipients: recipients });
        }
    }
})()