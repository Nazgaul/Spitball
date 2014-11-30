app.factory('sNotification',
    ['$rootScope', 'sShare', 
    function ($rootScope, sShare) {
        "use strict";

        var notifications;
        getNotifications();

        //$interval(checkNotifications, 60000);


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
                notifications = response;

                $rootScope.$broadcast('newNotifications');

            });
        }
        //function checkNotifications() {
        //    var newNotifications = false;
        //    sShare.getNotifications().then(function (notifies) {
        //        _.forEach(notifies, function (note) {
        //            var nExists = getById(note.msgId);

        //            if (nExists) {
        //                return;
        //            }

        //            notifications.push(note);
        //            newNotifications = true;

        //        });

        //        if (!newNotifications) {
        //            return;
        //        }

        //        $rootScope.$broadcast('newNotifications')
        //    });
         
        //}
        function getById(id) {
            return _.find(notifications, function (notification) {
                return notification.msgId === id;
            });

        }
    }]
);