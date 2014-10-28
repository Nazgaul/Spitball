app.factory('sShare',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/Share/' + path + '/';
        }
        return {
            cloudentsFriends: function (data) {
                return ajaxService.get('/User/Friends/', data);
            },
            googleFriends: function (data) {
                return ajaxService.post('/User/GoogleContacts/', data);
            },
            message: function (data) {
                return ajaxService.post(buildPath('Message'), data);
            },
            invite: {
                box: function (data) {
                    return ajaxService.post(buildPath('InviteBox'), data);
                },
                cloudents: function (data) {
                    return ajaxService.post(buildPath('Invite'), data);
                }
            },
            facebookInvite: {
                box: function (data) {
                    return ajaxService.post(buildPath('InviteBoxFacebook'), data);
                },
                cloudents: function (data) {
                    return ajaxService.post(buildPath('InviteFacebook'), data);
                }
            },
            facebookReputation: function () {
                return ajaxService.post(buildPath('Facebook'), null, true);
            },
            getNotifications: function () {
                return ajaxService.get(buildPath('Notifications'));
            },
            markNotificationAsRead: function (data) {
                return ajaxService.post(buildPath('NotificationAsRead'), data);
            },
            markNotificationsAsOld: function (data) {
                return ajaxService.post(buildPath('NotificationOld'), data);
            }

        };
    }
    ]);


