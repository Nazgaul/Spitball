(function (cd) {
    if (window.scriptLoaded.isLoaded('ps')) {
        return;
    }
    var topics = {}, subUid = -1;
    var subscribe = function (topic, func) {
        if (!topics[topic]) {
            topics[topic] = [];
        }
        var token = (++subUid).toString();
        topics[topic].push({
            token: token,
            func: func
        });
        return token;
    };

    var publish = function (topic, args, callback) {
        if (args === null || args === undefined) {
            args = args || {};
        }
        if (!topics[topic]) {
            if (typeof callback === 'function') {
                callback();
            }
           
        }
        setTimeout(function () {
            var subscribers = topics[topic],
                len = subscribers ? subscribers.length : 0;

            while (len--) {
                subscribers[len].func(args);
                
            }
            if (typeof callback === 'function'){
                callback();
            }
        }, 0);
    };

    var unsubscribe = function (token) {
        for (var m in topics) {
            if (topics[m]) {
                for (var i = 0, j = topics[m].length; i < j; i++) {
                    if (topics[m][i].token === token) {
                        topics[m].splice(i, 1);
                        return token;
                    }
                }
            }
        }
        return false;
    };
    cd.pubsub = {
        subscribe: subscribe,
        publish: publish,
        unsubscribe: unsubscribe
    };
})(window.cd = window.cd || {});