/// <reference path="/Scripts/jquery-1.6.2-vsdoc.js" />

function Zbox() {
    /// <summary>
    /// Zbox.    
    /// </summary>            
}
Zbox.LocalOffsetInMillis = new Date().getTimezoneOffset() * 60 * 1000;
Zbox.OneSecondInMillis = 1000;
Zbox.OneMinuteInMillis = Zbox.OneSecondInMillis * 60;
Zbox.TwoMinutesInMillis = Zbox.OneMinuteInMillis * 2;
Zbox.OneHourInMillis = Zbox.OneMinuteInMillis * 60;
Zbox.TwoHoursInMillis = Zbox.OneHourInMillis * 2;
Zbox.OneDayInMillis = Zbox.OneHourInMillis * 24;
Zbox.TwoDaysInMillis = Zbox.OneDayInMillis * 2;
Zbox.OneWeekInMillis = Zbox.OneDayInMillis * 7;
Zbox.OneYearInMillis = Zbox.OneWeekInMillis * 52;

//Zbox.OneKilobyteInBytes = Math.pow(2, 10);
//Zbox.OneMegabyteInBytes = Math.pow(Zbox.OneKilobyteInBytes, 2);
//Zbox.OneGigabyteInBytes = Math.pow(Zbox.OneMegabyteInBytes, 2);

Zbox.ConvertToLocalTime = function (time) {
    return time - Zbox.LocalOffsetInMillis;
};

Zbox.UpdateScreenTime = function () {
    $('span.item-time').each(function () {
        Zbox.RenderItemTime($(this));
    });
};

Zbox.RenderTimeLabel = function (time) {
    var now = new Date();
    var now_utc = new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds());
    var delta = now_utc.getTime() - new Date(time).getTime();

    var timeLabel;

    if (delta < Zbox.OneMinuteInMillis)
        timeLabel = 'a few sec. ago';
    else if (delta >= Zbox.OneMinuteInMillis && delta < Zbox.TwoMinutesInMillis)
        timeLabel = 'a min. ago';
    else if (delta >= Zbox.TwoMinutesInMillis && delta < Zbox.OneHourInMillis)
        timeLabel = Math.round((delta / Zbox.OneMinuteInMillis)) + ' min. ago';
    else if (delta >= Zbox.OneHourInMillis && delta < Zbox.TwoHoursInMillis)
        timeLabel = 'an hour ago';
    else if (delta >= Zbox.TwoHoursInMillis && delta < Zbox.OneDayInMillis)
        timeLabel = Math.round((delta / Zbox.OneHourInMillis)) + ' hours ago';
    else if (delta >= Zbox.OneDayInMillis && delta < Zbox.TwoDaysInMillis)
        timeLabel = 'yesterday';
    else if (delta >= Zbox.TwoDaysInMillis && delta < Zbox.OneWeekInMillis)
        timeLabel = Math.round((delta / Zbox.OneDayInMillis)) + ' days ago';
    else if (delta >= Zbox.OneWeekInMillis && delta < Zbox.OneYearInMillis){
        var time = new Date(time);
        timeLabel = time.getDate() + ' ' + time.getMonth();
        }
    else
        timeLabel = new Date(time).toDateString();

    return timeLabel;
};

Zbox.RenderItemTime = function (itemTimeContainer) {
    var itemTime = itemTimeContainer.children().first('input').val();
    try {
        var timeLabel = this.RenderTimeLabel(itemTime);
        itemTimeContainer.find('span').text(timeLabel);
    }
    catch (err) {
        console.log(err);
        //can happen because not all item are already posses date
    }
};

Zbox.KeepSessionAlive = function () {

    var request = new ZboxAjaxRequest({
        url: "/Account/KeepAlive",
        success: function (data) {            
        },
        error: function (msg) {
        }
    });

    request.Post();
}

Zbox.toaster = function (notice) {
    var noticeHtml = $('<span></span>').html(notice);
    var notification = $('<div class="notice"></div>')
            .append('<div class="skin"></div>')
            .append($('<div class="content"></div>').html($(noticeHtml)))
            .hide()
            .appendTo('#growl');
    $(notification).slideDown(1000).delay(2500).slideUp(1000);
}

Zbox.PasswordShow = function (checkBox, textInput, passwordInput, cssStyle) {
    if ($(checkBox).attr('checked')) {
        $(textInput).val($(passwordInput).val());
        $(textInput).css('display', cssStyle);
        $(passwordInput).css('display', 'none');
    }
    else {
        $(passwordInput).val($(textInput).val());
        $(passwordInput).css('display', cssStyle);
        $(textInput).css('display', 'none');
    }
}

Zbox.GetPassword = function (chceckBox, filedtext, filedPass) {
    var passwordValue;
    if ($(chceckBox).attr('checked')) {
        passwordValue = $(filedtext);
    }
    else {
        passwordValue = $(filedPass);
    }
    return passwordValue.val();
}

Zbox.LoadUserStatus = function () {
    /// <summary>
    /// Load Storage Quota.    
    /// </summary>    
    var $spanQuota = $('#spanQuota');
    var request = new ZboxAjaxRequest({
        beforeSend: function () {
            $spanQuota.text('(Loading...)');
        },
        url: "/Account/GetUserDertails",
        success: function (data) {
            $spanQuota.text(data.quota.UsagePercentage + '% of ' + data.quota.TotalSize + data.quota.Unit);
            if (data.IsVerified) {
                $('#emailVerificationStatus').hide().data('verifed', data.IsVerified);
            } else {
                $('#emailVerificationStatus').show().data('verifed',data.IsVerified);                
            }
        },
        error: function (msg) {
            $spanQuota.text(msg);
        }
    });

    request.Get();
}
Zbox.GetUserFriends = function (successCallback, errorCallback) {
    var getFriendsRequest = new ZboxAjaxRequest({
        url: '/Collaboration/GetUserFriends',
        success: successCallback,
        error: errorCallback
    });

    getFriendsRequest.Get();
}

Zbox.ClearSubscribersAndInvitationsFromFriendList = function () {
    $('#divFriends').find('.friendEntry').remove()
};



Zbox.RenderInvitationsInFriendList = function (invitations) {
    for (var i = 0; i < invitations.length; i++) {
        var InvitationId = invitations[i].UserId;
        var email = invitations[i].Email;
        var userPermission = invitations[i].UserPermission;
        var entry = [
                '<div class="friendEntry Invite" title="drag to change">',
                '<input type="hidden" value="', email, '" name="friendId" data-email="', email, '" />',
                '<img title="Invited" class="friendInvitationStatus" src="/Content/Images/friend_pending.png" alt="Invitation pending" />',
                '<span id="', InvitationId, '">', email, '</span>',
                '<a href="#" style="color: Red; float:right;margin:0 6px 0 0" data-email="', email, '"><img alt="" src="/Content/Images/list_delete_icon.png" class="deleteListIcon"  /></a>',
                '</div>'
        ];

        $('#' + userPermission).append(entry.join(""));
    }

};

Zbox.RenderSubscribersInFriendList = function (subscribers, permission) {
    for (var i = 0; i < subscribers.length; i++) {
        var email = subscribers[i].Email;
        var userPermission = subscribers[i].UserPermission;
        var entry = [];
        if (permission >= 4) {
            entry.push('<div class="friendEntry Subscriber" title="drag to change">');
        }
        else {
            entry.push('<div class="friendEntry Subscriber sortableDisable" title="drag to change">');
        }

        entry.push('<input type="hidden" value="', email, '" name="Subscriber" data-email="', email, '" />');
        entry.push('<img title="Subscriber" class="friendInvitationStatus" src="/Content/Images/friend_subscriber.png" alt="Subscriber" />');
        entry.push('<span>', email, '</span>');
        if (permission === 8) {
            entry.push('<a href="#" style="color: Red; float:right; margin:0 6px 0 0" data-email="', email, '"><img alt="" src="/Content/Images/list_delete_icon.png" class="deleteListIcon"  /></a>');
        }
        entry.push('</div>');
        $('#' + userPermission).append(entry.join(""));
        if (permission >= 4) {
            $('#permissionDropDown').val(userPermission);
        }
    }

    //$(friendListContainer).append(entry.join(""));
};

Zbox.changeTemplateText = function (template, data) {

    var findPattern = new RegExp('{(.*?)}|%7B.*?%7D', 'g');

    var items = jQuery.unique(template.match(findPattern));

    for (i = 0; i < items.length; i++) {
        var index = items[i].substring(1, items[i].length - 1);
        //Mozilla problem
        index = index.replace('7B', '');
        index = index.replace('%7', '');


        var pattern = new RegExp(items[i], 'g');

        template = template.replace(pattern, data[index]);
    }
    return template;
};

Zbox.TransformArrayToObject = function (arr, name) {
    /// <summary>
    /// Transforms the array parameter to an object filled with properties made of index / value of the array
    /// 
    /// e.g:
    /// var x = ['x', 'y'];
    /// var y = Zbox.TransformArrayToObject(x);
    ///
    /// y.0 is now 'x'
    /// y.1 is now 'y'    
    /// 
    /// </summary>

    var result = {};

    for (var i = 0; i < arr.length; i++)
        result[i] = arr[i];

    return result;
};

Zbox.ShowSearchResults = function (title, resultsByCategory) {
    var resultsContainer = $('#search-results');

    for (var category in resultsByCategory) {
        var results = resultsByCategory[category];
        resultsContainer.append(['<h2>By ', category, '</h2><p>', results.length, ' results</p>'].join(''));

        var currentBox;
        var currentBoxContainer;

        $(results).each(function (i, item) {

            if (item.ItemBoxName != currentBox) {
                currentBox = item.ItemBoxName;

                var currentBoxId = category + '_' + currentBox.replace(new RegExp('\ ', 'g'), '_');

                resultsContainer.append([
                    '<div class="box-results-container" id="', currentBoxId, '">',
                //'<input type="hidden" class="box-id" value="', item.BoxId, '" />',
                        '<h3>From box: <a href="#" class="box-name">', currentBox, '</a></h3>',
                    '</div>'].join(''));
                currentBoxContainer = resultsContainer.find('div#' + currentBoxId);
            }

            currentBoxContainer.append(Zbox.BoxItem.AddSearchBoxItemEntry(item, -1));
        });
    }
    Zbox.UpdateScreenTime();
    var w = $(window);
    var heightOfDialog = Math.round(w.height() * 0.8);

    var widthOfDialog = Math.round(w.width() * 0.8);

    $('#dialog-search-results').dialog({
        //autoOpen: false,
        close: function (event, ui) {
            $('#search-results').empty();
        },
        buttons: {
            'close': function (e, ui) {
                $(this).dialog('close');
            }
        },
        height: heightOfDialog,
        width: widthOfDialog,
        modal: true
    });
};

Zbox.DefaultDialogParams = {
    title: 'question',
    message: 'are you sure?',
    okLabel: 'ok',
    dialogClass: '',
    ok: function () { return true; },
    cancelLabel: 'cancel',
    width: 'auto',
    height: 'auto',
    cancel: function () { return true; },
    create: function (event, ui) { }
};

Zbox.ShowConfirmDialog = function (userParams) {
    var buttons = {};
    var params = {};

    $.extend(params, Zbox.DefaultDialogParams, userParams);
    buttons[params.okLabel] = function () {
        if (params.ok()) {
            $("#dialog-confirm").dialog('close');
        }
    };
    buttons[params.cancelLabel] = function () {
        if (params.cancel()) {
            $("#dialog-confirm").dialog('close');
        }
    };



    $("#dialog-content").html(params.message);

    var dialog = $("#dialog-confirm").dialog({
        title: params.title,
        resizable: false,
        height: params.height,
        width: params.width,
        create: params.create,
        modal: true,
        buttons: buttons,
        dialogClass: params.dialogClass
    });
};

Zbox.LazyLoadOfImages = function (container, elements) {
    var windowWidth = $(window).width();
    var $container = $(container);
    var containerOffsetLeft = $container.offset().left;
    var widthRemaining = 0;
    if (containerOffsetLeft + $container.width() > windowWidth) {
        widthRemaining = windowWidth - containerOffsetLeft;
    }
    else {
        widthRemaining = $container.width();
    }

    $.each(elements, function (i, element) {
        $element = $(element);
        if ($element.attr('src') === undefined) {
            if ($element.offset().left < containerOffsetLeft + widthRemaining) {
                var s = $element.attr('original');
                $element.attr('src', s);
            }
            else {
                return false;
            }
        }
    });

}

Zbox.HasClass = function (el, selector) {
    ///<summary>
    ///pure js has class function - better then jquery one in performance
    /// </summary>   

    var className = " " + selector + " ";

    if ((" " + el.className + " ").replace(/[\n\t]/g, " ").indexOf(className) > -1) {
        return true;
    }

    return false;

};
Zbox.InsufficentPermission = function () {
    this.ShowConfirmDialog({
        title: '<div class="style57">Permission</div>',
        message: '<div class="style59">You do not have permission to perform this operation</div>',
        cancel: function () {
            return true;
        }
    });
}

Zbox.VerifyAccount = function () {
    this.ShowConfirmDialog({
        title: '<div class="style57">Verify Account</div>',
        message: '<div class="style59">You need to verify account to perform this operation</div>',
        cancel: function () {
            return true;
        }
    });
}

function ZboxAjaxRequest(userParams) {
    /// <summary>
    /// A standard request to server, very similar to normal jquery ajax request.
    /// all other parameters are not mandatory, except url
    /// params:
    /// {
    ///     url: '',
    ///     success: function(data) {}
    ///     error: function(error) {},
    ///     complete: function() {},
    ///     type: 'get',    // you can use zboxRequest.Get() / zboxRequest.Post() instead of specifying this
    ///     cache: false,    
    ///     data: { key: value } 
    /// };
    /// </summary>

    if (!('url' in userParams))
        throw "must provide a request url";

    var defaultParams = {
        url: '',
        success: function (data) { },
        error: function (error) { },
        complete: function () { },
        beforeSend: function () { },
        type: 'get',
        cache: false,        
        data: {}
    };

    this.params = {};

    $.extend(this.params, defaultParams, userParams);
}

ZboxAjaxRequest.prototype.Get = function () {
    this.params.type = 'get';

    return this.Send();
}

ZboxAjaxRequest.prototype.Post = function () {
    this.params.type = 'post';

    return this.Send();
}

ZboxAjaxRequest.prototype.Send = function () {
    var requestParams = this.params;
    var zboxRequest = this;

    var rawRequest = $.ajax({
        url: requestParams.url,
        type: requestParams.type,
        data: requestParams.data,
        context: zboxRequest,
        beforeSend: requestParams.beforeSend,
        cache: requestParams.cache,
        success: zboxRequest.OnSuccess,
        error: zboxRequest.OnError,
        complete: zboxRequest.OnComplete,
        statusCode: {
            401: zboxRequest.On401
        }
    });

    return rawRequest;
};

ZboxAjaxRequest.prototype.On401 = function () {
    this.params.error('Your session has expired...');
    document.location.href = '/';
};

ZboxAjaxRequest.prototype.OnError = function (jqXHR, textStatus, errorThrown) {
    var reportData = ['textStatus: ', textStatus, ', error: ', errorThrown, ', request url:', this.params.url].join('');
    this.params.error(reportData);
};

ZboxAjaxRequest.prototype.OnSuccess = function (data, textStatus, jqXHR) {
    // check that resoponse is in proper format
    if (typeof (data) == 'undefined' || data == '' || !('Success' in data && 'Payload' in data)) {
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

