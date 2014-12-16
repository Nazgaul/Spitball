app.factory('sShare',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/share/' + path + '/';
        }
        return {
            cloudentsFriends: function (data) {
                return ajaxService.get('/user/friends/', data);
            },
            googleFriends: function (data) {
                return ajaxService.post('/user/googlecontacts/', data);
            },
            message: function (data) {
                return ajaxService.post(buildPath('message'), data);
            },
            invite: {
                box: function (data) {
                    return ajaxService.post(buildPath('invitebox'), data);
                },
                cloudents: function (data) {
                    return ajaxService.post(buildPath('invite'), data);
                }
            },
            facebookInvite: {
                box: function (data) {
                    return ajaxService.post(buildPath('inviteboxfacebook'), data);
                },
                cloudents: function (data) {
                    return ajaxService.post(buildPath('invitefacebook'), data);
                }
            },
            facebookReputation: function (data) {
                return ajaxService.post(buildPath('facebook'), data, true);
            },
            getNotifications: function () {
                return ajaxService.get(buildPath('notifications'));
            },
            markNotificationAsRead: function (data) {
                return ajaxService.post(buildPath('notificationasread'), data);
            }
        };
    }
    ]);


