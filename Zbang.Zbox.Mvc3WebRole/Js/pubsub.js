cd.pubsub = function (cd) {
    var obj = {}
    function subscribe(event, callback, caller) {

        if (!(event in obj)) {
            obj.event = [];
        }
        obj.event.push({ exec: callback, caller: caller });
    }

    function publish(event, data) {
        for (var i = 0; i < obj[event].length; i++) {
            obj[event].exec(data);
        };
    }

    function unsubscribe(event, callback, caller) {
        var index = ko.utils.arrayIndexOf(obj[event], function (i) {
            return i.caller === caller;
        });
        if (index != -1) {
            obj[event].splice(index, 1);
        }
    }

    return {
        subscribe: subscribe,
        publish: publish,
        unsubscribe: unsubscribe,
        obj: obj
    };

}(cd = window.cd || {});