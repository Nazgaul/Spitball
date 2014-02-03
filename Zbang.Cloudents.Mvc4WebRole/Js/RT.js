/// <reference path="../Views/Shared/_Layout3.cshtml" />
//var connection = $.hubConnection();
(function ($, cd) {
    if (window.scriptLoaded.isLoaded('rt')) {
        return;
    }

    if (!cd.register()) {
        return;
    }

    var connect = false, win;
    $(function () {
        win = document.getElementById('fconnect');
    });
    cd.pubsub.subscribe('boxGroup', function (boxids) {
        postMsg(['joinbox', boxids]);
    });
    cd.pubsub.subscribe('addItemNoti', function (d) {
        postMsg(['fileUploaded', d.BoxUid, d.item]);
    });
    cd.pubsub.subscribe('removeItemNotification', function (d) {
        postMsg(['removeFile', d.boxid, d.itemid]);
    });
    cd.pubsub.subscribe('addQuestionNoti', function (d) {
        postMsg(['addQuestion', d.boxid, d.question]);
    });
    cd.pubsub.subscribe('removeQuestionNoti', function (d) {
        postMsg(['removeQuestion', d.boxid, d.question]);
    });
    cd.pubsub.subscribe('addAnswerNoti', function (d) {
        postMsg(['addAnswer', d.boxid, d.questionId, d.answer]);
    });

    cd.pubsub.subscribe('removeAnswerNoti', function (d) {
        postMsg(['removeAnswer', d.boxid, d.questionId, d.answerId]);
    });
    cd.pubsub.subscribe('inviteNoti', function (d) {
        postMsg(['inviteUser', d.userUid]);
    });

    window.addEventListener("message", function (e) {
        connect = true;
        /// <summary>Get from signalR the data to share with other user</summary>
        /// <param name="e" type="MessageEvent"></param>
        if (e.origin !== stripTrailingSlash(win.src)) {
            return;
        }
        if (!$.isArray(e.data)) {
            return;
        }
        if (e.data[0] === 'loaded') {
            connect = true;
            sendQueuedMsg();
            return;
        }
        cd.pubsub.publish('clear_cache');
        cd.pubsub.publish(e.data[0], e.data[1]);

    }, false);

    function sendQueuedMsg() {
        for (var i = 0; i < waitingCommand.length; i++) {
            postMsg(waitingCommand[i]);
        }
    }

    function stripTrailingSlash(str) {
        if (str.substr(-1) == '/') {
            return str.substr(0, str.length - 1);
        }
        return str;
    }

    var waitingCommand = [];
    function postMsg(d) {
        if (connect) {
            win.contentWindow.postMessage(JSON.stringify(d), stripTrailingSlash(win.src));
        }
        else {
            waitingCommand.push(d);
        }
    }
})(jQuery, cd);