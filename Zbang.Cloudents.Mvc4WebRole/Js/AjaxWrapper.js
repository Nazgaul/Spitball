(function ($) {
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
            data: isjson ? JSON.stringify(options.data) : options.data,
            contentType: isjson ? "application/json" : "application/x-www-form-urlencoded",
            statusCode: {
                403: function () {
                    document.location.href = '/Account?ReturnUrl=' + encodeURIComponent(document.location.href);
                },
                404: error
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
     
        var request = $.ajax(ajaxParams);
        function fail(jqXHR) {
            if (options.error !== undefined) {
                options.error();
                return;
            }

        }
        request.fail(function (jqXHR, textStatus, errorThrown) {
          
            fail(textStatus);
         
        });
        request.done(function (data) {
            if (!$.isPlainObject(data)) {
                fail('not proper response');
            }
            if (data.Success) {
                options.done(data.Payload);
            } else {
                if (options.error !== undefined) {
                    options.error(data.Payload);
                }
            }
        });
        request.always(function () {
            if ($.isFunction(options.always)) {
                options.always();
            }
        });
    }
})(jQuery);
