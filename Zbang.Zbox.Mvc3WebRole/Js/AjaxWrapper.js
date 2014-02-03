(function ($) {
    var cache = {};
    $.ajaxRequest = function (options) {
        postRegular(options);
    };
    $.ajaxRequest.get = function (options) {
        getRegular(options);
    };
    $.ajaxRequest.getJson = function (options) {
        getJson(options);
    };
    $.ajaxRequest.post = function (options) {
        postRegular(options);
    };
    $.ajaxRequest.postJson = function (options) {
        postJson(options);
    };
    function getRegular(options) {
        options.type = 'get';
        ajaxRequest(options);
    }
    function postRegular(options) {
        options.type = 'post';
        ajaxRequest(options);
    }
    function getJson(options) {
        options.type = 'get';
        ajaxRequest(options, true);
    }
    function postJson(options) {
        options.type = 'post';
        ajaxRequest(options, true);
    }
    function ajaxRequest(options, isJson) {
        var isjson = isJson || false;

        var ajaxParams = {
            url: options.url,
            type: options.type,
            data: isjson ? ko.toJSON(options.data) : ko.toJS(options.data),
            contentType: isjson ? "application/json" : "application/x-www-form-urlencoded",
            statusCode: {
                403: function () {
                    document.location.href = '/Account?ReturnUrl=' + encodeURIComponent(document.location.href);
                },
                404: error//,
                //500: error
            },
            beforeSend: function () {
                if ($.isFunction(options.beforeSend)) {
                    return options.beforeSend();
                }
            }
        };
        function error() {
            document.location.href = '/Error';
        }
        //var parms = $.extend({}, $.ajaxSettings, options, requestoptions)
        var ajaxParamsString = ko.toJSON(ajaxParams);
        //if (ajaxParamsString in cache) {
        //    options.done(cache[ajaxParamsString]);
        //    return;
        //}
        var request = $.ajax(ajaxParams);

        request.fail(function (jqXHR, textStatus, errorThrown) {
            if (options.error !== undefined) {
                options.error();
                return;
            }
            error();

        });
        request.done(function (data, textStatus, jqXHR) {
            if (!$.isPlainObject(data)) {
                fail('not proper response');
            }
            if (data.Success) {
                cache[ajaxParamsString] = data.Payload;
                options.done(data.Payload);
            } else {
                if (options.error !== undefined) {
                    options.error(data.Payload);
                }
            }
            //options.done(data.Payload);
        });
        request.always(function () {
            if ($.isFunction(options.always)) {
                options.always();
            }
        });
    }
})(jQuery);

//(function ($) {
//    var o = $({});
//    $.pubsub = {};
//    $.pubsub.subscribe = function (topic, args) {

//        o.on.apply(o, arguments);
//    };
//    $.pubsub.unsubscribe = function (topic) {
//        o.off.apply(o, arguments);
//    };
//    $.pubsub.publish = function () {
//        o.trigger.apply(o, arguments);
//    };

//}(jQuery));