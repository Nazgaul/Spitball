(function (pubsub, $, cd) {

    if (window.scriptLoaded.isLoaded('c')) {
        return;
    }

    var cache = {},
    userid = cd.userDetail().id
    version = $('[data-version]').data('version');

    checkVersion();
    function checkVersion() {
        var key;
        try { // ie9 doesnt throw exception if sessionStorage is empty
            key = sessionStorage.key(0);
        } catch (e) {
            sessionStorage.clear();
            return;
        }
        var stringElement = sessionStorage.getItem(key);
        if (!stringElement) {
            return null;
        }
        var obj = JSON.parse(stringElement);
        if (obj.version !== version) {
            sessionStorage.clear();
        }
    }
    var minute = 60000;
    pubsub.subscribe('add_cache', function (args) {
        var valueToCache = JSON.stringify({ userid: userid, version: version, value: args.value, ttl: new Date().getTime() + args.ttl });
        sessionStorage.setItem(buildKey(args.key, args.params), valueToCache);
    });
    pubsub.subscribe('clear_cache', function () {
        sessionStorage.clear();
    });
    pubsub.subscribe('remove_cache', function (key) {
        sessionStorage.removeItem(key);
        window.setTimeout(function () {
            var keys = [];
            for (var i = 0, length = sessionStorage.length; i < length; i++) {
                var someKey = sessionStorage.key(i);
                if (someKey.split('_')[0] === key) {
                    keys.push(key);
                }
                for (var j = 0; j < keys.length; j++) {
                    sessionStorage.removeItem(keys[j]);
                }
            }
        }, 1);
    });


    function buildKey(key, params) {
        params = params || '';
        return key + '_' + JSON.stringify(params);
    }

    function getFromCache(key, params) {
        params = params || {};
        key = buildKey(key, params);
        var stringElement = sessionStorage.getItem(key);// cache[key];
        if (!stringElement) {
            return null;
        }
        var obj = JSON.parse(stringElement, cd.isoDateReviver);
        if (obj.userid !== userid) {
            sessionStorage.clear();
            return null;
        }
        if (obj.version !== version) {
            return null;
        }
        if (obj.ttl < new Date().getTime()) {
            return null;
        }
        return obj.value;
    }
    cd.cache = {
        getFromCache: getFromCache,
        // clear: sessionStorage.clear
    };

})(cd.pubsub, jQuery, window.cd = window.cd || {});