function ZboxAjaxRequest(userParams) {
    if (!('url' in userParams))
        throw "must provide a request url";
    var defaultParams = {
        url: '',
        success: function () { },
        error: function () { },
        complete: function () { },
        beforeSend: function () { },
        type: 'post',
        contentType: 'application/x-www-form-urlencoded',
        data: {}
    };
    this.params = {};
    $.extend(this.params, defaultParams, userParams);
}
ZboxAjaxRequest.prototype.Get = function () {
    this.params.type = 'get';
    return this.Send();
};
ZboxAjaxRequest.prototype.Post = function () {
    this.params.type = 'post';
    return this.Send();
};
ZboxAjaxRequest.prototype.Send = function () {
    var requestParams = this.params;
    var zboxRequest = this;
    var rawRequest = $.ajax({
        url: requestParams.url,
        type: requestParams.type,
        data: requestParams.data,
        context: zboxRequest,
        contentType: requestParams.contentType,

        beforeSend: requestParams.beforeSend,
        cache: requestParams.cache,
        success: zboxRequest.OnSuccess,
        error: zboxRequest.OnError,
        complete: zboxRequest.OnComplete,
        statusCode: {
            403: zboxRequest.On403,
            404: zboxRequest.On404,
            500: zboxRequest.On500
        }
    });
    return rawRequest;
};

ZboxAjaxRequest.prototype.On403 = function () {
    document.location.href = '/Account?ReturnUrl=' + encodeURIComponent(document.location.href);
};
ZboxAjaxRequest.prototype.On404 = function () {
    document.location.href = '/Error';
};
ZboxAjaxRequest.prototype.On500 = function () {
    document.location.href = '/Error';
};
ZboxAjaxRequest.prototype.OnError = function (jqXHR, textStatus, errorThrown) {
    var reportData = ['textStatus: ', textStatus, ', error: ', errorThrown, ', request url:', this.params.url].join('');
    this.params.error(reportData);
};
ZboxAjaxRequest.prototype.OnSuccess = function (data) {
    // check that resoponse is in proper format
    if (typeof (data) === undefined || data === '' || !('Success' in data && 'Payload' in data)) {
        this.OnError(null, 'response is not in a proper format', null);
    } else {
        if (data.Success) {
            this.params.success(data.Payload);
        } else {
            this.params.error(data.Payload);
        }
    }
};
ZboxAjaxRequest.prototype.OnComplete = function (jqXHR, textStatus) {
    this.params.complete(textStatus);
};