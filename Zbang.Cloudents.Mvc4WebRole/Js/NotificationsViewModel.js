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
        notificationsWrpr = eById('notifyWpr'),
        consts = {
            inviteTemplate: 'inviteTemplate',
            messageTemplate: 'messageTemplate',
            pageSize: 12
        },
        page = 1, notificationsData;

    function Invite(data) {
        var that = this;
        that.id = data.msgId;
        that.boxId = data.boxid;
        that.boxName = data.boxName;
        that.senderName = data.userName;
        that.senderImg = data.userPic;
        that.isRead = !data.isRead ? ' new' : '';
        that.isNew = data.isNew ;
        that.date = data.date;
        that.url = data.url;// + '?r=siteheader&s=invite';
    }

    function Message(data) {
        var that = this;
        that.id = data.msgId

        that.senderName = data.userName;
        that.senderImg = data.userPic;
        that.message = data.message;
        that.date = data.date;
        that.isRead = !data.isRead ? ' new' : '';
        that.isNew = data.isNew;
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
            if (notificationsData[i].isNew) {
                count++;
            }

        }
        if (!count) {
            notificationsCounter.classList.remove('invitesCounterShow');            
            notificationsCounter.textContent = '';
            return;
        }
        
        notificationsCounter.classList.add('invitesCounterShow');
        notificationsCounter.textContent = count;
    }

    function registerEvents() {
        var $notificationsList = $(notificationsList),
            $notificationsWrpr = $(notificationsWrpr);

        $(notifications).click(function (e) {
            e.stopPropagation();

            !notificationsList.children.length ? notificationsWrpr.classList.add('noResults') : notificationsWrpr.classList.remove('noResults');
            $('ul.userMenu').slideUp();//close maybe class
            if ($notificationsWrpr.is(':visible')) {
                $notificationsWrpr.slideUp(150);
                return;
            }

            dataContext.notificationOld({
                success: function () {
                    notificationsCounter.classList.remove('invitesCounterShow');
                    notificationsCounter.textContent = '';
                }
            });

            $notificationsWrpr.slideDown(150);

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
        })
        .on('click', 'li', function () {
            var that = this;
            dataContext.notificationAsRead({
                data: { messageId: that.id },
                success: function () {
                    that.classList.remove('new');
                }
            });
        });

        $('body').click(function () {
            $notificationsWrpr.slideUp(150);
        });

        $(headerNotifications).on('click', '[data-navigation="Box"]', function () {
            analytics.trackEvent('Site header', 'Notificaitons', 'User clicked an invitation');
        });
    }

})(cd, cd.data, jQuery, ko, cd.analytics);