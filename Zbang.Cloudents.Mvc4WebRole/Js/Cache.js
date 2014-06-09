(function (pubsub, $, cd) {

    if (window.scriptLoaded.isLoaded('c')) {
        return;
    }

    var cache = {},
    userid = cd.userDetail().id,
    version = $('[data-version]').data('version');

    checkVersion();
    function checkVersion() {
        var key;
        try { // ie9 doesnt throw exception if sessionStorage is empty
            key = cd.sessionStorageWrapper.getItem(0);
        } catch (e) {
            cd.sessionStorageWrapper.clear();
            return;
        }
        var stringElement = cd.sessionStorageWrapper.getItem(key);
        if (!stringElement) {
            return null;
        }
        var obj = JSON.parse(stringElement);
        if (obj.version !== version) {
            cd.sessionStorageWrapper.clear();
        }
    }
    var minute = 60000;
    pubsub.subscribe('add_cache', function (args) {
        var valueToCache = JSON.stringify({ userid: userid, version: version, value: args.value, ttl: new Date().getTime() + args.ttl });        
        cd.sessionStorageWrapper.setItem(buildKey(args.key, args.params), valueToCache);
    });
    pubsub.subscribe('clear_cache', function () {
        cd.sessionStorageWrapper.clear();
    });
    pubsub.subscribe('remove_cache', function (key) {
        cd.sessionStorageWrapper.removeItem(key);
        window.setTimeout(function () {
            var keys = [];
            for (var i = 0, length = cd.sessionStorageWrapper.length; i < length; i++) {
                var someKey = cd.sessionStorageWrapper.key(i);
                if (someKey.split('_')[0] === key) {
                    keys.push(key);
                }
                for (var j = 0; j < keys.length; j++) {
                    cd.sessionStorageWrapper.removeItem(keys[j]);
                }
            }
        }, 1);
    });


    function buildKey(key, params) {
        params = params || '';
        return key + '_' + JSON.stringify(params);
    }

    function getFromCache(key, params) {
        key = buildKey(key, params);
        var stringElement = cd.sessionStorageWrapper.getItem(key);// cache[key];
        if (!stringElement) {
            return null;
        }
        var obj = JSON.parse(stringElement, cd.isoDateReviver);
        if (obj.userid !== userid) {
            cd.sessionStorageWrapper.clear();
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
        getFromCache: getFromCache
        // clear: sessionStorage.clear
    };

})(cd.pubsub, jQuery, window.cd = window.cd || {});