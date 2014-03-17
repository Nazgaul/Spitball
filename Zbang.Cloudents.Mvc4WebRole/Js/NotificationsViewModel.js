(function (cd,dataContext, $, ko, analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('ntfvm')) {
        return;
    }

    if (!cd.register()) {
        return;
    }

    function Invite(data) {
        data = data || {};
        var that = this;
        that.id = data.boxUid;
        that.name = data.boxName;
        that.owner = data.boxOwner;
        that.url = data.url + '?r=siteheader&s=invite';
    }

    function Message(data) {
        data = data || {};
        var that = this;
        that.senderName = data.senderName;
        that.sendImg = data.senderImage;
        that.content = data.content;
        that.sendTime = data.sendTime;
        //that.url = data.url; //TODO!!!!!!!!!!!!!!!!!
    }

    dataContext.getNotifications({
        success: function (data) {
            console.log(data);
        }
    });

})(cd,cd.data, jQuery, ko, cd.analytics);