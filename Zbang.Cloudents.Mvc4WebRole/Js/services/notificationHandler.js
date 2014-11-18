app.factory('sNotification',
    ['$rootScope', 'sShare', '$timeout',
    function ($rootScope, sShare, $timeout) {
        "use strict";

        var notifications;
        getNotifications();

        $timeout(checkNotifications, 60000);


        var service = {
            getAll: function () {
                return notifications;
            },
            remove: function (id) {
                //for future
                //var notification = getById(id);
                //var index = notifications.indexOf(notification);
                //notification.splice(index,1);
                //ajax

            },
            setRead: function (id) {
                var notification = getById(id);
                notification.isRead = true;

                sShare.markNotificationAsRead({ messageId: notification.msgId });
            },
            getUnreadLength: function () {

                var unreadNotifications = _.filter(notifications, function (notification) {
                    return !notification.isRead;
                });

                return unreadNotifications.length;
            }



        };

        return service;

        function getNotifications() {
            sShare.getNotifications().then(function (response) {
                //notifications = response || [];
                notifications = [{ "msgId": "579c7036-6137-4edf-89a8-a3e600dd182a", "userPic": "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg", "userName": "ram y", "date": "2014-11-17T11:24:58.9507837Z", "isRead": true, "isNew": false, "boxName": "test1'", "url": "/box/my/21929/test1/" }, { "msgId": "1c3f1a05-6a0b-4d8e-9a83-a3e700e4af5d", "userPic": "https://graph.facebook.com/1792674869/picture?type=square", "userName": "Pocket Aces", "date": "2014-11-18T13:52:37.016588Z", "isRead": false, "isNew": true, "boxName": "asdasdas", "url": "/box/my/86279/asdasdas/" }];

                $rootScope.$broadcast('newNotifications')

            });
        }
        function checkNotifications() {
            var newNotifications = false;
            sShare.getNotifications().then(function (notifies) {
                _.forEach(notifies, function (note) {
                    var nExists = getById(note.msgId);

                    if (nExists) {
                        return;
                    }

                    notifications.push(note);
                    newNotifications = true;

                });
            });
            if (!newNotifications) {
                return;
            }

            $rootScope.$broadcast('newNotifications')
        }
        function getById(id) {
            return _.find(notifications, function (notification) {
                return notification.msgId === id;
            });

        }
    }]
);