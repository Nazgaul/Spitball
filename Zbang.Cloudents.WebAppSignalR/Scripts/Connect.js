/// <reference path="jquery-1.10.2.js" />
/// <reference path="jquery.signalR-1.1.3.js" />

$(function () {
    var connection = $.hubConnection(), connect = false, waitingCommand = [];
    var notificationHub = connection.createHubProxy('notificationHub');
    var recivingMessage = document.getElementById('form1').getAttribute('data-parent');
    notificationHub.on('fileAdded', function (d,boxid) {
        postMsg(['addedItem', { item: d, boxid: boxid }]);
    })
    .on('fileRemoved', function (d) {
        postMsg(['removeItem', d]);
    })
    .on('questionAdded', function (boxid, d) {
        postMsg(['addQuestion', { boxid: boxid, question: d }]);
    })
    .on('questionRemoved', function (boxid, d) {
        postMsg(['removeQuestion', d]);
    })
    .on('answerAdded', function (q, d, box) {
        postMsg(['addAnswer', { questionid: q, answer: d, box: box }]);
    })
    .on('answerRemoved', function (q, d) {
        postMsg(['removeAnswer', { questionid: q, answerid: d }]);
    })
    .on('InviteUser', function () {
        postMsg(['inviteuser']);
    });

    connection.start(
        //{ transport: ['serverSentEvents', 'foreverFrame', 'longPolling'] }
        ).done(function () {
            connect = true;
            for (var i = 0; i < waitingCommand.length; i++) {
                invokeNotificationHub(waitingCommand[i]);
            }

        }).fail(function () {
            console.log('fail');
        });
    postMsg(['loaded']);


    window.addEventListener('message', function (e) {
        /// <summary></summary>
        /// <param name="e" type="MessageEvent"></param>
        if (e.origin !== recivingMessage) {
            return;
        }
        var data = JSON.parse(e.data);
        if (!$.isArray(data)) {
            return;
        }
        var category = data[0];

        invokeNotificationHub(data);
        //if (category === 'joinbox') {
        //    notificationHub.invoke('JoinBox', e.data[1]);
        //}

    }, false);

    function invokeNotificationHub(d) {
        if (connect) {
            notificationHub.invoke.apply(notificationHub, d);
        }
        else {
            waitingCommand.push(d);
        }
    }


    function postMsg(obj) {
        parent.postMessage(obj, recivingMessage);
    }

    function stripTrailingSlash(str) {
        if (str.substr(-1) == '/') {
            return str.substr(0, str.length - 1);
        }
        return str;
    }
    //zboxHub.on('itemUploaded', function (retVal) {
    //    cd.pubsub.publish('addItem', retVal);
    //});
    //zboxHub.on('itemRemove', function (retVal) {
    //    cd.pubsub.publish('removeItem', retVal);
    //});
    //var connected = false, _actions = [];
    ////connection.start(
    ////    { transport: ['webSockets', 'longPolling'] }
    ////    ).done(RtEvents).fail(function () {
    ////    console.log('could not connect')
    ////});

    //function RtEvents() {
    //    connected = true;
    //    for (var i = 0; i < _actions.length; i++) {
    //        _actions[i]();
    //    }
    //}
    //cd.pubsub.subscribe('leaveBox', function (id) {
    //    if (connected) {
    //        zboxHub.invoke('LeaveBox', id);
    //        return;
    //    }
    //    _actions.push(function () { zboxHub.invoke('LeaveBox', id); });
    //});
    //cd.pubsub.subscribe('box', function (box) {
    //    if (connected) {
    //        zboxHub.invoke('JoinBox', box.id);
    //        return;
    //    }
    //    _actions.push(function () { zboxHub.invoke('JoinBox', box.id); });
    //});


    //parent.postMessage('test', 'https://ram.cloudents.com');



});