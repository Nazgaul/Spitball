/// <reference path="/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="/Js/Resource/ZboxResources.js" />

(function (Zbox, $, undefined) {
    //Zbox.RegisterMEvents = function () {
    //    if (!Modernizr.input.placeholder) {
    //        $('input[placeholder] , textarea[placeholder]').each(function () {
    //            $(this).watermark($(this).attr('placeholder'), { useNative: false });
    //        });
    //    }

    //    $('#popupclose').click(function () {
    //        $(this).parent().hide();
    //    });
    //    //if user is not registered
    //    //if ($('#register').length) {
           
    //    //    $('#fadeMeTrans').detach().appendTo('#BodyContent');
    //    //    //Zbox.IsRegister = false;
    //    //    var $homeForm = $('div.homeForm');
    //    //    $('#register').click(function () {
    //    //        var $homeForm = $('div.homeForm');
    //    //        $homeForm.fadeToggle("fast", "linear");
    //    //        $('div.fadeMeTrans').show();
    //    //        return false;
    //    //    });

    //    //    $('div.fadeMeTrans').click(function () {
    //    //        $('div.homeForm').fadeToggle();
    //    //        $('div.fadeMeTrans').hide();
    //    //        return false;
    //    //    });

    //    //    LogOn.RegisterEvents();
    //    //}
    //};
    //Zbox.IsRegister = true;
    
   



    //Zbox.ShowNotification = function (text) {
    //    $('#popUp').text(text).parent().show().delay(10000).fadeOut(2000);
    //};
    //Zbox.LocalOffsetInMillis = new Date().getTimezoneOffset() * 60 * 1000;
    //Zbox.OneSecondInMillis = 1000;
    //Zbox.OneMinuteInMillis = Zbox.OneSecondInMillis * 60;
    //Zbox.TwoMinutesInMillis = Zbox.OneMinuteInMillis * 2;
    //Zbox.OneHourInMillis = Zbox.OneMinuteInMillis * 60;
    //Zbox.TwoHoursInMillis = Zbox.OneHourInMillis * 2;
    //Zbox.OneDayInMillis = Zbox.OneHourInMillis * 24;
    //Zbox.TwoDaysInMillis = Zbox.OneDayInMillis * 2;
    //Zbox.OneWeekInMillis = Zbox.OneDayInMillis * 7;
    //Zbox.OneYearInMillis = Zbox.OneWeekInMillis * 52;

    //Zbox.Interval = false;
    //Zbox.UpdateScreenTime = function () {
    //    $('span.commentTime').each(function () {
    //        Zbox.RenderItemTime($(this));
    //    });
    //    if (!this.Interval) {
    //        window.setInterval('Zbox.UpdateScreenTime()', 10000);
    //        this.Interval = true;
    //    }
    //};

    //Zbox.RenderItemTime = function (itemTimeContainer) {
    //    var itemTime = itemTimeContainer.attr('data-Time');
    //    try {
    //        var timeLabel = this.RenderTimeLabel(itemTime);
    //        itemTimeContainer.text(timeLabel);
    //    }
    //    catch (err) {
    //        //can happen because not all item are already posses date
    //    }
    //};
    //Zbox.RenderLoading = function (elem) {
    //    var x = $('<div class="loading"></div>');
    //    var handle = window.setTimeout(function () {
    //        $(elem).append(x);
    //    }, 2000);
    //    return function () {
    //        window.clearTimeout(handle);
    //        $(elem).find(x).remove();
    //    };
    //};
    //Zbox.RenderTimeLabel = function (time) {
    //    var now = new Date();
    //    var itemCreation = new Date(time);
    //    var delta = now.getTime() - itemCreation.getTime();

    //    var timeLabel;

    //    if (delta < Zbox.OneMinuteInMillis) {
    //        timeLabel = ZboxResources.AFewSecAgo;
    //    }
    //    else if (delta >= Zbox.OneMinuteInMillis && delta < Zbox.TwoMinutesInMillis) {
    //        timeLabel = ZboxResources.AMinAgo;
    //    }
    //    else if (delta >= Zbox.TwoMinutesInMillis && delta < Zbox.OneHourInMillis) {
    //        timeLabel = ZboxResources.MinAgo.format(Math.round((delta / Zbox.OneMinuteInMillis)));
    //    }
    //    else if (delta >= Zbox.OneHourInMillis && delta < Zbox.TwoHoursInMillis) {
    //        timeLabel = ZboxResources.AnHourAgo;
    //    }
    //    else if (delta >= Zbox.TwoHoursInMillis && delta < Zbox.OneDayInMillis) {
    //        timeLabel = ZboxResources.HoursAgo.format(Math.round((delta / Zbox.OneHourInMillis)));
    //    }
    //    else if (delta >= Zbox.OneDayInMillis && delta < Zbox.TwoDaysInMillis) {
    //        timeLabel = ZboxResources.Yesterday;
    //    }
    //    else if (delta >= Zbox.TwoDaysInMillis && delta < Zbox.OneWeekInMillis) {
    //        timeLabel = ZboxResources.DaysAgo.format(Math.round((delta / Zbox.OneDayInMillis)));
    //    }
    //    else if (delta >= Zbox.OneWeekInMillis && delta < Zbox.OneYearInMillis) {
    //        var timedate = new Date(time);
    //        timeLabel = timedate.format('j/n');
    //    }
    //    else {
    //        timeLabel = new Date(time).toDateString();
    //    }

    //    return timeLabel;
    //};

    Zbox.RenderSizeOfFile = function (size) {
        if (size === undefined) {
            return '';
        }
        if (size > 1073741824) {
            return Math.round(size * 100 / 1073741824) / 100 + ZboxResources.Gb;
        }
        if (size > 1048576) {
            return Math.round(size * 100 / 1048576) / 100 + ZboxResources.Mb;
        }
        if (size > 1024) {
            return Math.round(size * 100 / 1024) / 100 + ZboxResources.Kb;
        }
        return size + ZboxResources.B;
    };

    //Zbox.changeTemplateText = function (template, data) {

    //    var findPattern = new RegExp('{(.*?)}|%7B.*?%7D', 'g');

    //    var items = jQuery.unique(template.match(findPattern));

    //    for (var i = 0; i < items.length; i++) {
    //        var index = items[i].substring(1, items[i].length - 1);
    //        //Mozilla problem
    //        index = index.replace('7B', '');
    //        index = index.replace('%7', '');
    //        var pattern = new RegExp(items[i], 'g');
    //        if (data[index] === null) {
    //            template = template.replace(pattern, '');
    //        }
    //        else {
    //            template = template.replace(pattern, data[index]);
    //        }
    //    }
    //    return template;
    //};

    //Zbox.GetHashStateToArray = function (hashName, delimiter) {
    //    var hash = $.bbq.getState(hashName);
    //    if (hash === undefined || hash === '') {
    //        return [];

    //    }
    //    return hash.split(delimiter);
    //};


    //Zbox.CropFileName = function (fileName, size) {
    //    if (fileName.length > size) {
    //        //var fileExtension = /[^.]+$/.exec(FileName);
    //        fileName = fileName.slice(0, size);
    //        //fileName += "..." + fileExtension;
    //    }
    //    return fileName;
    //};
    //Zbox.CropBoxName = function (boxName, size) {
    //    if (boxName == null) {
    //        return '';
    //    }
    //    if (boxName.length > size) {
    //        boxName = boxName.substr(0, size);
    //        boxName += '...' + '"';
    //    }
    //    return boxName;
    //};
    
    //Zbox.RegisterValidation = function (elem, validimg) {
    //    $('form').addTriggersToJqueryValidate().triggerElementValidationsOnFormValidation();


    //    elem.elementValidAndInvalid(function () {
    //        elem.parent().removeClass().addClass('txtBoxWrap validWrap marginTop4');
    //        elem.parent().find('span.inputErrImg').remove();
    //    }, function () {
    //        elem.parent().addClass("esi");
    //        if (elem.closest('.' + validimg).children('.inputErrImg').length === 0) {
    //            elem.parent().append("<span class='inputErrImg'></span>");
    //        }

    //    });
    //};
    //Zbox.AssignValidation = function () {
    //    $('.field-validation-error').each(function (index, value) {
    //        var input = $(value).attr('data-valmsg-for');
    //        $('#' + input).trigger('elementValidationError');
    //    });

    //};
    //Zbox.RegisterPopUp = function () {
    //    $('#LinkBox,#CommentText,div.mainSearch,#UploadB,#Delete,input.CommentReply,#CreateUrlLink').click(function (e) {
    //        e.preventDefault();
    //        $.get('/Global/RegisterPopUp', function (data) {
    //            $('#BodyContent').append(data);
    //            $('#RegPopup').focus();
    //            $('textarea').blur();
    //            $('#PopupFade').show();
    //            $('#PopupFade,#JoinLater').click(function () {
    //                $('#PopupFade').remove();
    //                $('#RegPopup').remove();
    //            });
    //            $('#SignUpNow').click(function () {
    //                $('#PopupFade').remove();
    //                $('#RegPopup').remove();
    //                var $homeForm = $('div.homeForm');
    //                $homeForm.fadeToggle("fast", "linear");
    //                $('div.fadeMeTrans').show();
    //                if ($('#combinedForms2').hasClass('invisible')) {
    //                    switchVisiablity();
    //                }

    //            });
    //        });
    //    });
    //};

    //Zbox.RegisterViewType = function (funcToExeInClick, elementToClear) {
    //    var $TypeOfView = $('#TypeOfView');
    //    var viewType = Zbox.getItem('boxesView', 'List');
    //    $TypeOfView.find('span[data-ViewType = "' + viewType + '"]').addClass('current');

    //    funcToExeInClick(viewType);

    //    $TypeOfView.delegate('span.btnLt', 'click', function () {
    //        $TypeOfView.find('span.current').removeClass('current');
    //        $(this).addClass("current");
    //        var viewType = $(this).attr('data-ViewType');
    //        Zbox.setItem('boxesView', viewType);
    //        if ($('#' + elementToClear).length) {
    //            $('#' + elementToClear).empty();
    //        }
    //        funcToExeInClick();
    //    });
    //};
    //Zbox.GetViewType = function () {
    //    return $('#TypeOfView').find('span.current').attr('data-ViewType');
    //};
    //cookie section
    //Zbox.setCookie = function (cName, value, exdays) {
    //    var exdate = new Date();
    //    exdate.setDate(exdate.getDate() + exdays);
    //    var cValue = escape(value) + ((exdays === null) ? "" : "; expires=" + exdate.toUTCString());
    //    document.cookie = cName + "=" + cValue;
    //};
    //Zbox.getCookie = function (cName) {
    //    var i, x, y, arRcookies = document.cookie.split(";");
    //    for (i = 0; i < arRcookies.length; i++) {
    //        x = arRcookies[i].substr(0, arRcookies[i].indexOf("="));
    //        y = arRcookies[i].substr(arRcookies[i].indexOf("=") + 1);
    //        x = x.replace(/^\s+|\s+$/g, "");
    //        if (x === cName) {
    //            return unescape(y);
    //        }
    //    }
    //}; //local storage - maybe use session storage
    //Zbox.setItem = function (cName, item) {
    //    if (Modernizr.localstorage) {
    //        var data = { user: $('#userName').attr('data-id'), val: item };
    //        localStorage.setItem(cName, JSON.stringify(data));
    //    }
    //    else {
    //        Zbox.setCookie(cName, item, 1);
    //    }
    //};
    //Zbox.getItem = function (cName, defaultValue) {
    //    if (Modernizr.localstorage) {
    //        var rawdata = localStorage.getItem(cName);
    //        try {
    //            var data = JSON.parse(rawdata);
    //            if (data.user === $('#userName').attr('data-id')) {
    //                return data.val || defaultValue;
    //            }
    //            else {
    //                return defaultValue;
    //            }
    //        } catch (e) {
    //            return defaultValue;
    //        }

    //    }
    //    else {
    //        return Zbox.getCookie(cName) || defaultValue;
    //    }
    //};
    //Zbox.clearItem = function (cName) {
    //    localStorage.removeItem(cName);
    //};
    Zbox.getParameterByName = function (name, windowToCheck) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS, 'i');
        windowToCheck = windowToCheck || window;
        var results = regex.exec(windowToCheck.location.search);
        if (results === null) {
            return "";
        }
        else {
            return decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    };
}(window.Zbox = window.Zbox || {}, jQuery));


String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] !== 'undefined'
          ? args[number]
          : match
        ;
    });
};


