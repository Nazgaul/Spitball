﻿(function (cd, $, analytics) {

    if (window.scriptLoaded.isLoaded('u')) {
        return;
    }

    var notification = function (msg) {
        alert(msg);
    };

    var confirm = function (msg, trueCallback, falseCallback) {
        if (!$.isFunction(trueCallback)) {
            throw 'trueCallback should be function';
        }
        if (!$.isFunction(falseCallback)) {
            throw 'falseCallback should be function';
        }
        if (window.confirm(msg)) {
            trueCallback();
        } else {
            falseCallback();
        }
    };

    var clone = function (obj) {
        var target = {};
        for (var i in obj) {
            if (obj.hasOwnProperty(i)) {
                target[i] = obj[i];
            }
        }
        return target;
    }


    var confirm2 = function (msg) {
        var d = $.Deferred();
        if (window.confirm(msg)) {
            d.resolve();
        }
        else {
            d.reject();
        }
        return d.promise();
    }

    var validateEmail = function (email) {
        var regex = /^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-z0-9]{1}[a-z0-9\-]{0,62}[a-z0-9]{1})|[a-z])\.)+[a-z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$/i;
        return regex.test(email);
    };

    var escapeHtmlChars = function (str) {
        return str
             .replace(/&/g, "&amp;")
             .replace(/</g, "&lt;")
             .replace(/>/g, "&gt;")
             .replace(/"/g, "&quot;")
             .replace(/'/g, "&#039;");
    };

    var displayErrors = function (form, errors) {
        var $form = $(form),
        validationSummaryErrorClassName = 'validation-summary-errors';
        if (typeof (errors) === 'string') {
            generateSummaryError($form.find(':first:not(.' + validationSummaryErrorClassName + ')'), errors);
        }
        if ($.isArray(errors)) {
            for (var i = 0; i < errors.length; i++) {
                var label = $form.find($('label[for=' + errors[i].Key + ']'));
                if (label.length) {
                    generateFieldError(label, errors[i].Value[0]);
                    continue;
                }
                var input = $form.find($('input[name=' + errors[i].Key + ']'));
                if (input.length) {
                    generateFieldError(input, errors[i].Value[0]);
                    continue;
                }
                generateSummaryError($form.find(':first:not(.' + validationSummaryErrorClassName + ')'), errors[i].Value[0]);
            }
        }
        function generateFieldError(elem, error) {
            $(elem).siblings('.field-validation-error').remove();
            $(elem).after('<span class="field-validation-error">' + error + '</span>');
        }
        function generateSummaryError(elem, error) {
            $(elem).parent().find('div.' + validationSummaryErrorClassName).remove();
            var validationExists = $(form).find('[data-valmsg-summary]');
            if (validationExists.length) {
                $(form).find('[data-valmsg-summary]')
                    .removeClass('validation-summary-valid')
                    .addClass(validationSummaryErrorClassName)
                    .find('ul').empty().append($('<li>').text(error));
            }
            else {
                $(elem).before($('<div data-error-msg="true">').addClass(validationSummaryErrorClassName).text(error));
            }
        }

    };
    var resetErrors = function (form) {
        var $form = $(form);

        $form.find('[data-valmsg-summary]').removeClass('validation-summary-errors')
            .addClass('validation-summary-valid').find('ul').empty();
        $form.find('[data-error-msg]').remove();
        $form.find('span.field-validation-error').remove();
        $form.find("[data-valmsg-summary=true]")
            .removeClass("validation-summary-errors")
            .addClass("validation-summary-valid")
            .find("ul").empty();

        $form.find("[data-valmsg-replace]")
            .removeClass("field-validation-error")
            .addClass("field-validation-valid")
            .empty();

        $form.validate();
        return $form;
    };
    var resetErrors2 = function (form) {
        $(form).trigger('onReset.unobtrusiveValidation');
    };
    var resetForm = function (form) {
        var $form = $(form);
        //$form[0].reset();

        resetErrors($form)[0].reset();
        return $form;

    };

    //depend on jquery ui
    var autocomplete = function (elem, userparams, datasourceUrl) {
        var cache = {},
            defaultParms = {
                source: function (request, response) {
                    var term = request.term;
                    if (term in cache) {
                        response(cache[term]);
                        return;
                    }

                    $.getJSON(datasourceUrl, request, function (data) {
                        if (data.Success) {
                            cache[term] = data.Payload;
                            response(cache[term]);
                        }
                    });
                },
                minLength: 2,
                autoFocus: true,
                position: {
                    my: "right top"
                  , at: "right bottom+3"
                }
            };
        elem.keypress(function (e) {
            if (e.which === 13) {
                elem.blur();
            }
        });
        var params = $.extend({}, $.ui.autocomplete.prototype.options, defaultParms, userparams);
        $(elem).autocomplete(params);
    };
    var getParameterByName = function (name, windowToCheck) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS, 'i');
        windowToCheck = windowToCheck || window;
        var results = regex.exec(windowToCheck.location.search);
        if (results === null) {
            results = regex.exec(windowToCheck.location.hash); // checking has as well
            //return "";
        }
        if (results === null) {
            return "";
        }
        else {
            return decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    };



    var registerScroll = function (action, elemPageVisisble) {
        if (!$.isFunction(action)) {
            throw 'action';
        }
        $(window).scroll(function () {
            if ($(window).scrollTop() >= $(document).height() - $(window).height() - 50) {
                if ($(elemPageVisisble).is(':visible')) {
                    action();
                }
            }
        });
    };
    var loaderOn = function () {
        var loader = document.querySelector('.loading');
        if (!loader) {
            return false;
        }

        return getComputedStyle(loader).getPropertyValue('display') !== 'none';
    };

    var renderLoading = function (elem, timeout) {
        if (loaderOn()) {
            return function () { };
        }
        var $elem, loaderHtml, topLocation, x;
        timeout = timeout || 2000;
        //, position= $elem.css('position');
        if (Modernizr.cssanimations) {
            loaderHtml = '<div class="loading"><div class="spinner"></div></div>';
            $elem = $(elem);
            x = $(loaderHtml).css({ top: topLocation });

        }
        else {
            loaderHtml = '<div class="loadingGif"></div>';
            x = $(loaderHtml);
            $elem = $(elem);
        }
        $elem.css('position', 'relative');
        topLocation = $elem.css('position') === 'relative' ? 20 : $elem.position().top + 20;
        //var leftLocation = $elem.position().left + $elem.outerWidth(true) / 2;

        var handle = window.setTimeout(function () {
            $elem.append(x);
            x.show();
        }, timeout);
        return function () {
            //$elem.position
            window.clearTimeout(handle);
            x.remove();
        };
    };




    //var getParameterFromHash = function (index) {
    //    var url = window.location.hash;
    //    if (url.charAt(0) === '#') {
    //        url = url.substr(1);
    //    }
    //    var fragments = url.split('/');
    //    return fragments[index] ? decodeURIComponent(fragments[index]) : null;
    //};

    //var changeParameterInHash = function (index, newValue) {
    //    var url = window.location.hash;
    //    if (url.charAt(0) === '#') {
    //        url = url.substr(1);
    //    }
    //    var fragments = url.split('/');
    //    fragments[index] = newValue;
    //    location.hash = fragments.join('/');
    //};
    function removeChildren(node) {
        var last;
        while (last = node.lastChild) node.removeChild(last);
    }

    //function userLogout(e) {
    //    e.preventDefault();
    //    var redirect = e.target.href;
    //    FB.getLoginStatus(function (response) {
    //        if (response.status === 'connected') {
    //            FB.logout(function (respone) {
    //                window.location = redirect;
    //            });
    //        } else {
    //            window.location = redirect;
    //        }
    //    });



    //}

    function appendData(element, template, data, position, clearData) {
        var func = function () { };
        if (clearData === true) {
            func = function () {
                $(element).empty();//we use this because we want to wipe jquery data as well
            };
        }
        if (typeof clearData === "function") {
            func = clearData;
        }

        if (!element) {
            return;
        }
        var html = cd.attachTemplateToData(template, data);
        if (clearData) {
            func();
            //$(element).empty(); 
            //cd.removeChildren(element); //Clear the list before appending the data
        }
        element.insertAdjacentHTML(position, html);

    };

    var sessionStorageWrapper = {
        enabled: true,
        getItem: function (key) {
            if (!this.enabled) {
                return;
            }
            return sessionStorage.getItem(key);
        },
        setItem: function (key, value) {
            if (!this.enabled) {
                return;
            }
            sessionStorage.setItem(key, value);
        },
        removeItem: function (key) {
            if (!this.enabled) {
                return;
            }
            sessionStorage.removeItem(key);
        },
        clear: function () {
            if (!this.enabled) {
                return;
            }
            sessionStorage.clear();
        },
        length: function () {
            if (!this.enabled) {
                return;
            }
            return sessionStorage.length();
        },
        key: function (rKey) {
            if (!this.enabled) {
                return;
            }
            return sessionStorage.getKey(rKey);
        },
        check: function () {
            if (!window.sessionStorage) {
                this.enabled = false;
                analytics.trackEvent('Browser', 'Session Storage', 'Session  storage is not supported');
            }
        }
    };
    sessionStorageWrapper.check();

    var localStorageWrapper = {
        enabled: true,
        getItem: function (key) {
            if (!this.enabled) {
                return;
            }
            return localStorage.getItem(key);
        },
        setItem: function (key, value) {
            if (!this.enabled) {
                return;
            }
            localStorage.setItem(key, value);
        },
        removeItem: function (key) {
            if (!this.enabled) {
                return;
            }
            localStorage.removeItem(key);
        },
        clear: function () {
            if (!this.enabled) {
                return;
            }
            localStorage.clear();
        },
        length: function () {
            if (!this.enabled) {
                return;
            }
            return localStorage.length();
        },
        key: function (rKey) {
            if (!this.enabled) {
                return;
            }
            return localStorage.getKey(rKey);
        },
        check: function () {
            if (!window.sessionStorage) {
                this.enabled = false;
                analytics.trackEvent('Browser', 'Local Storage', 'Local storage is not supported');
            }
        }
    };
    localStorageWrapper.check();

    var docCookies = {
        getItem: function (sKey) {
            return decodeURIComponent(document.cookie.replace(new RegExp("(?:(?:^|.*;)\\s*" + encodeURIComponent(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*([^;]*).*$)|^.*$"), "$1")) || null;
        },
        setItem: function (sKey, sValue, vEnd, sPath, sDomain, bSecure) {
            if (!sKey || /^(?:expires|max\-age|path|domain|secure)$/i.test(sKey)) { return false; }
            var sExpires = "";
            if (vEnd) {
                switch (vEnd.constructor) {
                    case Number:
                        sExpires = vEnd === Infinity ? "; expires=Fri, 31 Dec 9999 23:59:59 GMT" : "; max-age=" + vEnd;
                        break;
                    case String:
                        sExpires = "; expires=" + vEnd;
                        break;
                    case Date:
                        sExpires = "; expires=" + vEnd.toUTCString();
                        break;
                }
            }
            document.cookie = encodeURIComponent(sKey) + "=" + encodeURIComponent(sValue) + sExpires + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "") + (bSecure ? "; secure" : "");
            return true;
        },
        removeItem: function (sKey, sPath, sDomain) {
            if (!sKey || !this.hasItem(sKey)) { return false; }
            document.cookie = encodeURIComponent(sKey) + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT" + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "");
            return true;
        },
        hasItem: function (sKey) {
            return (new RegExp("(?:^|;\\s*)" + encodeURIComponent(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=")).test(document.cookie);
        },
        keys: /* optional method: you can safely remove it! */ function () {
            var aKeys = document.cookie.replace(/((?:^|\s*;)[^\=]+)(?=;|$)|^\s*|\s*(?:\=[^;]*)?(?:\1|$)/g, "").split(/\s*(?:\=[^;]*)?;\s*/);
            for (var nIdx = 0; nIdx < aKeys.length; nIdx++) { aKeys[nIdx] = decodeURIComponent(aKeys[nIdx]); }
            return aKeys;
        }
    };


    function setCookie(cName, value, exdays) {
        docCookies.removeItem(cName, '/');
        docCookies.removeItem(cName, location.pathname.substring(0, location.pathname.length - 1));

        //delcookie(cName, document.domain, '/');
        //delcookie(cName, document.domain, location.pathname);
        var exdate = new Date();
        exdate.setDate(exdate.getDate() + exdays);
        docCookies.setItem(cName, value, exdate, '/');
        // var cValue = escape(value) + ((exdays === null) ? "" : "; expires=" + exdate.toUTCString()) + "; path=/";
        //document.cookie = cName + "=" + cValue;
    }

    //function delcookie(name, domain, path) {
    //    //var domain = domain || document.domain;
    //    path = path || "/";
    //    document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:01 GMT;" + "; path=" + path;
    //}

    //function getCookie(c_name) {
    //    var i, x, y, ARRcookies = document.cookie.split(";");
    //    for (i = 0; i < ARRcookies.length; i++) {
    //        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
    //        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
    //        x = x.replace(/^\s+|\s+$/g, "");
    //        if (x === c_name) {
    //            return unescape(y);
    //        }
    //    }
    //}


    var dateToShow = function (date,delimeter,twoDigitsYear) {
        var day = date.getDate(),
            month = date.getMonth() + 1,
            year = date.getFullYear().toString(),//we use to string so we can use substring
            delimeter = delimeter || '/';
        if (twoDigitsYear) {
            year = year.substr(2, 2);
        }
        if (day< 10) {
            day = '0' + day;
        }
        if (month < 10) {
            month = '0' + month;
        }
        return day + delimeter + month + delimeter + year;
    };

    var register = function () {
        //first regular second mobile
        //TODO: merge
        return $('#userDetails').length || $('[data-log]').data('log');
    };
    var unregisterAction = function (elem) {
        cd.pubsub.publish('register');
        $(elem).blur();
    };

    var deleteAllow = function (permission, userId) {
        return (permission === 'subscribe' || permission === 'owner') && (permission === 'owner' || userId === cd.userDetail().id);
    };
    var elementToInputwithoutFunc = function (elem) {

        var input = $('<input type="text" class="inputText" style="display:block" />');
        input.val($(elem).text())
            .width(elem.css('max-width').replace('px', ''))//.width())
            .height(elem.height()).insertAfter(elem);
        elem.hide();
        input.focus();

        function show() {
            elem.show();
            input.remove();
        }
        return {
            input: input,
            show: show

        };
    };
    var sortMembersByName = function (aName, bName, value) {
        aName = aName.toLowerCase();
        bName = bName.toLowerCase();
        value = value ? value.toLowerCase() : value;

        if (value && aName.indexOf(value) < bName.indexOf(value)) {
            return -1;
        }
        else if (value && aName.indexOf(value) > bName.indexOf(value)) {
            return 1;
        }
        else {
            if (aName < bName) {
                return -1;
            } else if (aName > bName) {
                return 1;
            }

            return 0;
        }

        return 0;
    };
    var elementToInput = function (elem, done) {
        var retval = elementToInputwithoutFunc(elem);

        var input = retval.input, lastVal = '';
        input.focusout(function () {
            finishProcess();
        })
        .keyup(function (e) {
            if (e.keyCode === 13) {
                finishProcess();
            }
            if (e.keyCode === 27) {
                retval.show();
            }
        })
        .click(function (e) {
            //stop propagation not working - we need to prevent <a> from clicking
            return false;
        });
        function finishProcess() {

            if (input.val() === elem.text()) {
                retval.show();
                return;
            }
            if (input.val() === lastVal) {
                input.focus();
                return;
            }
            done(input.val());
            lastVal = input.val();

        }

        return retval;
    };

    var contentEditable = function (elem) {
        /// <summary></summary>
        /// <param name="elem" type="jQuery"></param>

        var $elem = $(elem);
        text = $elem.attr('title') || $elem.text();
        $elem.attr({ 'contenteditable': true, spellcheck: false }).addClass('editable').text(text);
        setEndOfContenteditable($elem[0]);
        function finish() {
            if ($elem.hasClass('editable')) {
                $elem.removeClass('editable').removeAttr('contenteditable').blur();
            }
        }
        return {
            finish: finish,
            input: $elem
        };
        function setEndOfContenteditable(contentEditableElement) {
            var range, selection;
            if (document.createRange)//Firefox, Chrome, Opera, Safari, IE 9+
            {
                range = document.createRange();//Create a range (a range is a like the selection but invisible)
                range.selectNodeContents(contentEditableElement);//Select the entire contents of the element with the range
                range.collapse(false);//collapse the range to the end point. false means collapse to end rather than the start
                selection = window.getSelection();//get the selection object (allows you to change selection)
                selection.removeAllRanges();//remove any selections already made
                selection.addRange(range);//make the range you have just created the visible selection
            }
            //else if (document.selection)//IE 8 and lower
            //{
            //    range = document.body.createTextRange();//Create a range (a range is a like the selection but invisible)
            //    range.moveToElementText(contentEditableElement);//Select the entire contents of the element with the range
            //    range.collapse(false);//collapse the range to the end point. false means collapse to end rather than the start
            //    range.select();//Select the range (make it the visible selection
            //}
        }
    };
    var contentEditableFunc = function (elem, done) {

        if ($(elem).attr('contenteditable')) {
            return;
        }
        var retVal = contentEditable(elem),
        $elem = retVal.input.focus(), value = $(elem).text();


        $elem.focusout(function (e) {
            e.preventDefault();
            finishProcess();
        }).keypress(function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                finishProcess();
            }
        }).keydown(function (e) {
            if (e.keyCode === 27) {
                e.preventDefault();
                $elem.text(value);
                $elem.removeAttr('contenteditable').removeClass('editable');
            }
        });
        function finishProcess() {
            if ($elem.text() === '') {
                $elem.text(value);
                retVal.finish();
                alert('Tab name wasn\'t changed');
            } else if (value === $elem.text()) {
                retVal.finish();
            } else {
                done($elem.text());
            }
            $elem.off('keypress focusout');
        }
        return retVal;
    };

    //We have a bug in here since not the entire website is uploaded
    var putPlaceHolder = function () {
        if (Modernizr.input.placeholder) {
            return;
        }
        $('input[placeholder] , textarea[placeholder]').each(function () {
            var defaultValue = this.getAttribute('placeholder');
            if (this.value === '') {
                this.value = defaultValue;
            }
            this.onfocus = function () {
                if (this.value === defaultValue) this.value = '';
            };
            this.onblur = function () {
                if (this.value === '') this.value = defaultValue;
            };
            this.onchange = function () {
                if (this.value === '') this.value = defaultValue;
            };
        });
        $(document).on('form', 'submit', function () {
            $(this).find('*[placeholder]').each(function () {
                if ($(this).val() === $(this).attr('placeholder')) {
                    $(this).val('');
                }
            });
        });

        //HTMLInputElement.prototype.placeholder2 = function () {
        //    this.getAttribute('placeholder');
        //}
        Object.defineProperty(window.HTMLInputElement.prototype, 'placeholder', {
            get: function () {
                return this.getAttribute('placeholder');
            }

        });

    };

    //#region dropdowns
    $(window).unload(function () {  //firefoxfix
        $('[data-ddcbox]').prop('checked', false);
    });
    $('body').on('click', function (e) {
        var $target = $(e.target);

        if (e.target.nodeName === "LABEL") {
            return;
        }

        if ($target.attr('data-dropdown') || $target.parents('[data-dropdown]').length > 0) {
            return;
        }

        $('[data-ddcbox]').not('[data-ddcbox="' + $target.attr('data-ddcbox') + '"]').prop('checked', false);
        //$('[data-dropdown]').not('[data-dropdown="' + $target.attr('data-ddcbox') + '"]').removeClass('showOtakim');

        if ($target.attr('data-ddcbox') === undefined) {
            $('[data-ddcbox]').prop('checked', false);
            //$('.dropDown').removeClass('showOtakim');
        }
    });



    //#endregion

    //var elemList = [];
    cd.menu = function (elem, menu, menuShow, menuClosed) {
        var slideSpeed = 150;

        menu = $(menu);
        //if (elem in elemList) {
        //    return
        //}
        $(elem).click(function (e) {
            e.stopPropagation();
            if (menu.is(':visible')) {
                menu.slideUp(slideSpeed);
                if ($.isFunction(menuClosed)) {
                    menuClosed();
                }
                return;
            }

            menu.slideDown(slideSpeed, function () {
                if ($.isFunction(menuShow)) {
                    menuShow();
                }
            });
        });
        $('body').click(function () {
            menu.slideUp(slideSpeed);
            if ($.isFunction(menuClosed)) {
                menuClosed();
            }
        });
        // elemList.push(elem);
    };

    var setTitle = function () {
        var name = $('[data-name]:visible').data('name');
        if (name) {
            document.title = 'Cloudents ' + $('[data-name]:visible').data('name');
        }
        else {
            document.title = 'Cloudents';
        }
    };

    var shareFb = function (url, name, caption, description, picture) {
        url = url || cd.location();
        FB.ui({
            method: 'feed',
            link: url,
            name: name,
            caption: caption,
            description: description,
            picture: location.origin + (picture || '/images/cloudents-share-FB.png'),
            display: 'popup'
        }, function (response) {
            if (response) {
                $('[data-ddcbox]').each(function (i, e) { e.checked = false }); // close all popups
                analytics.trackSocial(url, 'share');
                cd.pubsub.publish('addPoints', 'shareFb');
            }
        });
        //var sharer = "https://www.facebook.com/sharer/sharer.php?u=",
        //       x = screen.width / 2 - 700 / 2,
        //       y = screen.height / 2 - 450 / 2;
        //window.open(sharer + encodeURIComponent(url), 'sharer', 'width=700,height=450,left=' + x + ',top=' + y);
        //analytics.trackSocial(url, 'share');
    };

    var innerScroll = function (elem, height) {
        var direction = $('html').css('direction') === 'ltr' ? 'right' : 'left';
        if (Modernizr.touch) {
            elem.css({ height: height, overflow: 'auto', '-webkit-overflow-scrolling': 'touch' });
            return;
        }

        elem.slimScroll({
            height: height,
            position: direction,
            disableFadeOut: true,
            distance: '3px'
        });
    };

    var loadModel = function (elem, topic, func) {
        var token = '';
        if (document.getElementById(elem) === null) {
            token = cd.pubsub.subscribe(topic, registerKo);
        }
        else {
            func();
        }
        function registerKo() {
            cd.pubsub.unsubscribe(token);
            func('async');
        }
    };



    var userDetail = function () {
        var userData = {
            img: '',
            name: '',
            id: '',
            nId: -1,
            url: ''
        };

        if (!userData.id) {
            var userName = document.getElementById('userName');
            var userImg = document.getElementById('userImg');
            userData.img = userImg ? userImg.getAttribute('src') : '/images/emptystate/user-pic.png';
            if (userName) {
                userData.name = userName.textContent;
                userData.id = userName.getAttribute('data-id'); //TODO: need to because number
                userData.nId = parseInt(userData.id, 10);
                userDataElement = document.querySelector('.navUser a');
                userData.url = userDataElement ? userDataElement.href : '';
            }

        }

        return userData;
    };
    var ConvertToDate = function (date) {
        if (date.constructor === Date) {
            return date;
        }
        return new Date(parseInt(date.replace("/Date(", "").replace(")/", ""), 10));
    };

    var loadImages = function (list) {
        var images = $(list).find('[data-src]');
        var image;
        for (var i = 0, l = images.length; i < l; i++) {
            image = images[i];
            image.src = image.getAttribute('data-src');
            image.removeAttribute('data-src');
        }
    };

    var debounce = function (func, wait, immediate) {
        var timeout;
        return function () {
            var context = this, args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(function () {
                timeout = null;
                if (!immediate) func.apply(context, args);
            }, wait);
            if (immediate && !timeout) func.apply(context, args);
        };
    };

    var conversion = {
        table: {
            'e': 'ק', 'r': 'ר', 't': 'א', 'y': 'ט', 'u': 'ו', 'i': 'ן', 'o': 'ם',
            'p': 'פ', 'a': 'ש', 's': 'ד', 'd': 'ג', 'f': 'כ', 'g': 'ע', 'h': 'י',
            'j': 'ח', 'k': 'ל', 'l': 'ך', ';': 'ף', 'z': 'ז', 'x': 'ס', 'c': 'ב',
            'v': 'ה', 'b': 'נ', 'n': 'מ', 'm': 'צ', ',': 'ת', '.': 'ץ'
        },
        convert: function (term) {
            var result = '';
            for (var i = 0, l = term.length; i < l ; i++) {
                result += this.table[term[i].toLowerCase()] || term[i];
            }
            return result;
        }
    };

    cd.sessionStorageWrapper = sessionStorageWrapper;
    cd.localStorageWrapper = localStorageWrapper;
    cd.debounce = debounce;
    cd.conversion = conversion;
    cd.loadImages = loadImages;
    cd.ConvertToDate = ConvertToDate;
    //var eById = document.getElementById.bind(document);
    //cd.eById = eById;
    //cd.userLogout = userLogout;
    cd.userDetail = userDetail;
    cd.loadModel = loadModel;
    cd.innerScroll = innerScroll;
    cd.notification = notification;
    cd.confirm = confirm;
    cd.confirm2 = confirm2;
    cd.resetForm = resetForm;
    cd.resetErrors = resetErrors;
    cd.displayErrors = displayErrors;
    cd.validateEmail = validateEmail;
    cd.autocomplete = autocomplete;
    cd.escapeHtmlChars = escapeHtmlChars;
    cd.getParameterByName = getParameterByName;
    cd.registerScroll = registerScroll;
    cd.renderLoading = renderLoading;

    cd.removeChildren = removeChildren;
    //cd.getParameterFromHash = getParameterFromHash;
    //cd.updateHash = changeParameterInHash;
    cd.setCookie = setCookie;
    cd.getCookie = docCookies.getItem;
    //cd.renderSizeOfFile = renderSizeOfFile;
    cd.dateToShow = dateToShow;
    cd.register = register;
    cd.elementToInputwithoutFunc = elementToInputwithoutFunc;
    cd.elementToInput = elementToInput;
    cd.sortMembersByName = sortMembersByName;
    cd.contentEditableFunc = contentEditableFunc;
    cd.contentEditable = contentEditable;
    cd.appendData = appendData;
    cd.deleteAllow = deleteAllow;
    cd.unregisterAction = unregisterAction;
    cd.putPlaceHolder = putPlaceHolder;

    //cd.switchBackToMobile = switchBackToMobile;
    cd.loaderOn = loaderOn;
    cd.shareFb = shareFb;
    cd.setTitle = setTitle;

    cd.clone = clone;

    cd.OneSecond = 1000;
    cd.OneMinute = cd.OneSecond * 60;
    cd.OneHour = cd.OneMinute * 60;
    cd.OneDay = cd.OneHour * 24;
    cd.OneWeek = cd.OneDay * 7;
    //var _private = cd._private = cd._private || {},
    //_seal = cd._seal = cd._seal || function () {
    //    delete cd._private;
    //    delete cd._seal;
    //    delete cd._unseal;
    //}

    //_private = 'baraba';
    //cd._private = _private;

    /**
    * Simple template
    * @param {String} template - Id of templte
    * @param {Object} data - the data to inject
    */
    var cache = {};
    cd.attachTemplateToData = function (template, data) {
        data = [].concat(data);
        if (!cache[template]) {
            cache[template] = document.getElementById(template).innerHTML;
        }
        var i = 0;
        template = cache[template];
        len = data.length,
        fragment = [];

        var cacheRegularExp = {};
        // For each item in the object, make the necessary replacement
        function replace(obj) {
            var t, key, reg;

            for (key in obj) {
                if (!(key in cacheRegularExp)) {
                    cacheRegularExp[key] = new RegExp('{{' + key + '}}', 'ig');
                    //= reg;
                }
                //reg = new RegExp('{{' + key + '}}', 'ig');
                reg = cacheRegularExp[key];
                t = (t || template).replace(reg, obj[key] === null ? '' : obj[key]);

                //TODO: check if match is more efficient
            }
            if (t) {
                return t;
            } else {
                return template;
            }
        }

        for (; i < len; i++) {
            fragment.push(replace(data[i]));
        }

        return fragment.join('');
    };

    cd.orientationDetection = function () {
        var landscapeClass = 'landscape';
        if (window.addEventListener) {
            if (Math.abs(window.orientation / 90) === 1) {
                $('body').addClass(landscapeClass);
            }
            window.addEventListener("orientationchange", function () {
                if (Math.abs(window.orientation / 90) === 1) {

                    $('body').addClass(landscapeClass);
                }
                else {
                    $('body').removeClass(landscapeClass);
                }
            }, false);
        }
        return $('body').hasClass(landscapeClass);
    };
    cd.guid = function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    };

    String.prototype.insert = function (index, string) {
        if (index >= 0)
            return this.substring(0, index) + string + this.substring(index, this.length);
        else
            return string + this;
    };

    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] !== 'undefined'
              ? args[number]
              : match
            ;
        });
    };
    if (!String.prototype.trim) {
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, '');
        };
    }
    String.prototype.trunc = String.prototype.trunc ||
      function (n) {
          return this.length > n ? this.substr(0, n - 1) + '...' : this;
      };


    //ie not supposrt that
    if (!window.location.origin) {
        window.location.origin = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port : '');
    }

    //ie8 console.log issue
    if (typeof console === "undefined" || typeof console.log === "undefined") {
        console = {};
        console.log = function () { };
    }



    $.extend($.expr[':'], {
        startsWith: function (elem, v, match) {
            //console.log(elem);
            //console.log(text);

            return (elem.textContent || elem.innerText || "").indexOf(match[3]) === 0;
        }
    });

    cd.isoDateReviver = function (key, value) {
        if (typeof value === 'string') {
            var a = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)(?:([\+-])(\d{2})\:(\d{2}))?Z?$/.exec(value);
            if (a) {
                var utcMilliseconds = Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4], +a[5], +a[6]);
                return new Date(utcMilliseconds);
            }
        }
        return value;
    }

    //if (typeof document !== "undefined" && !("classList" in document.documentElement)) {

    //}
    //$.extend($.expr[':'], {
    //    startsWith: function (a) {
    //        return $(a).css('display') === 'inline';
    //    }
    //});

})(window.cd = window.cd || {}, jQuery, cd.analytics);