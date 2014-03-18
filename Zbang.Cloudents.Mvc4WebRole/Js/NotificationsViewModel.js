(function (cd, dataContext, $, ko, analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('ntfvm')) {
        return;
    }

    if (!cd.register()) {
        return;
    }

    var eById = document.getElementById.bind(document),
        notifications = eById('invites'),
        notificationsCounter = eById('invitesCounter'),
        notificationsList = eById('invitesList'),
        headerNotifications = eById('headerInvites'),
        consts = {
            inviteTemplate: 'inviteTemplate',
            messageTemplate: 'messageTemplate',
            pageSize: 12
        },
        page = 1, notificationsData;

    function Invite(data) {
        var that = this;
        that.boxId = data.boxid;
        that.boxName = data.boxName;
        that.senderName = data.userName;
        that.senderImg = data.userPic;
        that.isRead = data.isRead;
        that.date = data.date;
        that.url = data.url + '?r=siteheader&s=invite';
    }

    function Message(data) {
        var that = this;
        that.senderName = data.userName;
        that.senderImg = data.userPic;
        that.message = data.message;
        that.date = data.date;
        that.isRead = data.isRead;
        //that.url = data.url; //TODO!!!!!!!!!!!!!!!!!
    }

    getData();
    registerEvents();

    function getData() {
        dataContext.getNotifications({
            success: function (data) {
                notificationsData = data || {};
                parseData(notificationsData);
                showNewNotifications();
            }
        });
    }


    function parseData() {
        var item, html, result = '',
            currentPageItems = notificationsData.slice((page - 1) * consts.pageSize, page * consts.pageSize);
        for (var i = 0, l = currentPageItems.length ; i < l; i++) {
            if (currentPageItems[i].message) {
                item = new Message(currentPageItems[i]);
                html = cd.attachTemplateToData(consts.messageTemplate, item);
            } else {
                item = new Invite(currentPageItems[i]);
                html = cd.attachTemplateToData(consts.inviteTemplate, item);
            }

            result += html;
        }

        notificationsList.insertAdjacentHTML('beforeend', result);

       
    }

    function showNewNotifications() {
        var count = 0;
        for (var i = 0, l = notificationsData.length ; i < l; i++) {
            if (!notificationsData[i].isRead) {
                count++;
            }

        }
        if (!count) {
            notificationsCounter.classList.remove('invitesCounterShow');
            notifications.classList.add('noInvites');
            notificationsCounter.textContent = '';
            return;
        }

        notifications.classList.remove('noInvites');
        notificationsCounter.classList.add('invitesCounterShow');
        notificationsCounter.textContent = count;
    }

    function registerEvents() {
        var $notificationsList = $(notificationsList);
        $(notifications).click(function (e) {
            if (!notificationsList.children.length) {
                return;
            }
            $('ul.userMenu').slideUp(150);//close settings - maybe should be in class
            if ($notificationsList.is(':visible')) {
                $notificationsList.slideUp(150);
                return;
            }
            e.stopPropagation();
            $notificationsList.slideDown(150);

        });

        $notificationsList.on('scroll', function (e) {
            e.stopPropagation();
            if ($notificationsList.scrollTop() + $notificationsList.innerHeight() >= $notificationsList[0].scrollHeight) {
                if ($notificationsList.children().length === notificationsData.length) {
                    return;
                }
                page++;
                parseData(); // render items from the current page set
            }
        });

        $('body').click(function () {
            $notificationsList.slideUp(150);
        });

        $(headerNotifications).on('click', '[data-navigation="Box"]', function () {
            analytics.trackEvent('Site header', 'Notificaitons', 'User clicked an invitation');
        });
    }

})(cd, cd.data, jQuery, ko, cd.analytics);