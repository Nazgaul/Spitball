(function (cd, $, analytics, dataContext) {

    if (window.scriptLoaded.isLoaded('u')) {
        return;
    }

    var notification = function (msg) {
        alert(msg);
    };

    var confirm = function (msg, trueCallback, falseCallback) {
        if (trueCallback && !$.isFunction(trueCallback)) {
            throw 'trueCallback should be function';
        }
        if (falseCallback && !$.isFunction(falseCallback)) {
            throw 'falseCallback should be function';
        }
        if (window.confirm(msg)) {
            if (trueCallback) {
                trueCallback();
            }
        } else {
            if (falseCallback) {
                falseCallback();
            }
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
        if (!str)
            return '';

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
                var label = $form.find('label[for=' + errors[i].Key + ']').first();
                if (label.length) {
                    generateFieldError(label, errors[i].Value[0]);
                    continue;
                }
                var input = $form.find('input[name=' + errors[i].Key + ']').first();
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

    //#region Set text direction
    function setElementChildrenDirection(container, element, subtree) {
        var list = container.querySelectorAll(element),
            textAlign = $('html').css('direction') === 'ltr' ? 'ltr' : 'rtl',
            item;
        for (var i = 0, l = list.length; i < l; i++) {
            item = list[i];

            loopElements(item.children);

            if (!subtree) {
                continue;
            }

            loopElements(item.getElementsByTagName('*'));
        }
        function loopElements(list) {
            for (var i = 0, l = list.length; i < l; i++) {
                setDirection(list[i]);
            }
        }

        function setDirection(item) {
            if (item.getAttribute('data-ignore-rtl')) {
                $(item).css('direction', textAlign);
                return;
            }

            cd.setElementDirection(item);
        }

    }


    //#endregion

    //#region Dates in the system    
    var actionTimeInterval;


    if (!actionTimeInterval) {
        actionTimeInterval = setInterval(updateTimeActions, 60000);
    }

    function parseActionTime(date) {
        if (!date) {
            return;
        }

        var oneDay = 24 * 60 * 60 * 1000, // hours*minutes*seconds*milliseconds                                         
            today = new Date(),
            dateDifference = calculateDayDifference(),
            months = [JsResources.January, JsResources.February, JsResources.March, JsResources.April,
                      JsResources.May, JsResources.June, JsResources.July, JsResources.August,
                      JsResources.September, JsResources.October, JsResources.November, JsResources.December];

        switch (dateDifference) {
            case 0:
                var timeObj = calculateSecondsDifferece();
                if (timeObj.hours >= 1) {

                    return ZboxResources.HoursAgo.format(Math.round(timeObj.hours));
                }
                if (timeObj.minutes >= 1) {
                    return ZboxResources.MinAgo.format(Math.round(timeObj.minutes));
                }

                return JsResources.JustNow;
                break;
            case 1:
                return ZboxResources.Yesterday;
                break;

            default:
                var dateMonth = date.getMonth() + 1,
                    todayMonth = today.getMonth() + 1;

                if (dateMonth < todayMonth) {
                    return date.getDate() + ' ' + months[dateMonth - 1];
                } else if (dateMonth > todayMonth) {
                    return date.getDate() + ' ' + months[dateMonth - 1] + ', ' + date.getFullYear();
                } else {
                    return ZboxResources.DaysAgo.format(dateDifference);
                }
                break;
        }

        function calculateDayDifference() {
            return diffDays = Math.round(Math.abs((date.getTime() - today.getTime()) / (oneDay)));
        }
        function calculateSecondsDifferece() {
            var time1 = date.getTime(),
                time2 = today.getTime();

            var timeDifference = time2 - time1;
            return {
                seconds: (timeDifference / 1000) % 60,
                minutes: (timeDifference / (1000 * 60)) % 60,
                hours: (timeDifference / (1000 * 60 * 60)) % 24
            }

        }
    }

    function updateTimeActions(container) {
        if (!container) {
            container = document.body;
        }
        var $timedObjects = $(container).find('[data-time]'),
        text;
        for (var i = 0, l = $timedObjects.length; i < l; i++) {
            parseTimeString($timedObjects[i]);
        }
    }

    function parseTimeString(element) {
        if (element instanceof jQuery) {
            element = element[0];
        }
        var time = element.getAttribute('data-time');
        if ($.isNumeric(time)) {
            time = parseInt(time, 10);
        }
        text = parseActionTime(new Date(time));
        element.textContent = text;
    }
    //#endregion
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


    var getParameterByNameFromString = function (param, text) {
        param = param.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + param + "=([^&#]*)";
        var regex = new RegExp(regexS, 'i');
        var results = regex.exec(text);
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

    var getUTCDate = function (date) {
        if (date) {
            date = new Date(date);
            return new Date(date.getTime() + date.getTimezoneOffset() * 60000);
        }

        return new Date(new Date().getTime() + new Date().getTimezoneOffset() * 60000);
    }

    var renderLoading = function (elem, timeout) {
        if (loaderOn()) {
            return function () { };
        }
        var $elem, loaderHtml, topLocation, x;
        timeout = timeout || 2000;
        $elem = $(elem);
        $elem.css('position', 'relative');
        topLocation = $elem.css('position') === 'relative' ? 20 : $elem.position().top + 20;

        if (Modernizr.cssanimations) {
            loaderHtml = '<div class="loading"><div class="spinner"></div></div>';
            x = $(loaderHtml).css({ top: topLocation });
        }
        else {
            loaderHtml = '<div class="loadingGif"></div>';
            x = $(loaderHtml);
        }

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


    var getExtension = function (fileName, type) {
        type = type || '';
        fileName = fileName || '';
        if (type.toLowerCase() === 'link') {
            return 'www';
        }

        var x = fileName.lastIndexOf('.');
        if (x === -1) {
            return '';
        }

        return fileName.slice(x + 1)
    }

    function getExtensionColor(fileName, type) {
        var prefix = 'mF';
        type = type || '';
        fileName = fileName || '';
        if (type.toLowerCase() === 'link') {
            return prefix + 'link';
        }
        var cssClass = '',
            extension = getExtension(fileName, type);

        if (extension.length > 3) {
            cssClass += 'fourLetterExtension ';
        }
        return cssClass += prefix + extension.toLowerCase();
    }

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

    //#region diacritics 
    var defaultDiacriticsRemovalMap = [
    { 'base': 'A', 'letters': /[\u0041\u24B6\uFF21\u00C0\u00C1\u00C2\u1EA6\u1EA4\u1EAA\u1EA8\u00C3\u0100\u0102\u1EB0\u1EAE\u1EB4\u1EB2\u0226\u01E0\u00C4\u01DE\u1EA2\u00C5\u01FA\u01CD\u0200\u0202\u1EA0\u1EAC\u1EB6\u1E00\u0104\u023A\u2C6F]/g },
    { 'base': 'AA', 'letters': /[\uA732]/g },
    { 'base': 'AE', 'letters': /[\u00C6\u01FC\u01E2]/g },
    { 'base': 'AO', 'letters': /[\uA734]/g },
    { 'base': 'AU', 'letters': /[\uA736]/g },
    { 'base': 'AV', 'letters': /[\uA738\uA73A]/g },
    { 'base': 'AY', 'letters': /[\uA73C]/g },
    { 'base': 'B', 'letters': /[\u0042\u24B7\uFF22\u1E02\u1E04\u1E06\u0243\u0182\u0181]/g },
    { 'base': 'C', 'letters': /[\u0043\u24B8\uFF23\u0106\u0108\u010A\u010C\u00C7\u1E08\u0187\u023B\uA73E]/g },
    { 'base': 'D', 'letters': /[\u0044\u24B9\uFF24\u1E0A\u010E\u1E0C\u1E10\u1E12\u1E0E\u0110\u018B\u018A\u0189\uA779]/g },
    { 'base': 'DZ', 'letters': /[\u01F1\u01C4]/g },
    { 'base': 'Dz', 'letters': /[\u01F2\u01C5]/g },
    { 'base': 'E', 'letters': /[\u0045\u24BA\uFF25\u00C8\u00C9\u00CA\u1EC0\u1EBE\u1EC4\u1EC2\u1EBC\u0112\u1E14\u1E16\u0114\u0116\u00CB\u1EBA\u011A\u0204\u0206\u1EB8\u1EC6\u0228\u1E1C\u0118\u1E18\u1E1A\u0190\u018E]/g },
    { 'base': 'F', 'letters': /[\u0046\u24BB\uFF26\u1E1E\u0191\uA77B]/g },
    { 'base': 'G', 'letters': /[\u0047\u24BC\uFF27\u01F4\u011C\u1E20\u011E\u0120\u01E6\u0122\u01E4\u0193\uA7A0\uA77D\uA77E]/g },
    { 'base': 'H', 'letters': /[\u0048\u24BD\uFF28\u0124\u1E22\u1E26\u021E\u1E24\u1E28\u1E2A\u0126\u2C67\u2C75\uA78D]/g },
    { 'base': 'I', 'letters': /[\u0049\u24BE\uFF29\u00CC\u00CD\u00CE\u0128\u012A\u012C\u0130\u00CF\u1E2E\u1EC8\u01CF\u0208\u020A\u1ECA\u012E\u1E2C\u0197]/g },
    { 'base': 'J', 'letters': /[\u004A\u24BF\uFF2A\u0134\u0248]/g },
    { 'base': 'K', 'letters': /[\u004B\u24C0\uFF2B\u1E30\u01E8\u1E32\u0136\u1E34\u0198\u2C69\uA740\uA742\uA744\uA7A2]/g },
    { 'base': 'L', 'letters': /[\u004C\u24C1\uFF2C\u013F\u0139\u013D\u1E36\u1E38\u013B\u1E3C\u1E3A\u0141\u023D\u2C62\u2C60\uA748\uA746\uA780]/g },
    { 'base': 'LJ', 'letters': /[\u01C7]/g },
    { 'base': 'Lj', 'letters': /[\u01C8]/g },
    { 'base': 'M', 'letters': /[\u004D\u24C2\uFF2D\u1E3E\u1E40\u1E42\u2C6E\u019C]/g },
    { 'base': 'N', 'letters': /[\u004E\u24C3\uFF2E\u01F8\u0143\u00D1\u1E44\u0147\u1E46\u0145\u1E4A\u1E48\u0220\u019D\uA790\uA7A4]/g },
    { 'base': 'NJ', 'letters': /[\u01CA]/g },
    { 'base': 'Nj', 'letters': /[\u01CB]/g },
    { 'base': 'O', 'letters': /[\u004F\u24C4\uFF2F\u00D2\u00D3\u00D4\u1ED2\u1ED0\u1ED6\u1ED4\u00D5\u1E4C\u022C\u1E4E\u014C\u1E50\u1E52\u014E\u022E\u0230\u00D6\u022A\u1ECE\u0150\u01D1\u020C\u020E\u01A0\u1EDC\u1EDA\u1EE0\u1EDE\u1EE2\u1ECC\u1ED8\u01EA\u01EC\u00D8\u01FE\u0186\u019F\uA74A\uA74C]/g },
    { 'base': 'OI', 'letters': /[\u01A2]/g },
    { 'base': 'OO', 'letters': /[\uA74E]/g },
    { 'base': 'OU', 'letters': /[\u0222]/g },
    { 'base': 'P', 'letters': /[\u0050\u24C5\uFF30\u1E54\u1E56\u01A4\u2C63\uA750\uA752\uA754]/g },
    { 'base': 'Q', 'letters': /[\u0051\u24C6\uFF31\uA756\uA758\u024A]/g },
    { 'base': 'R', 'letters': /[\u0052\u24C7\uFF32\u0154\u1E58\u0158\u0210\u0212\u1E5A\u1E5C\u0156\u1E5E\u024C\u2C64\uA75A\uA7A6\uA782]/g },
    { 'base': 'S', 'letters': /[\u0053\u24C8\uFF33\u1E9E\u015A\u1E64\u015C\u1E60\u0160\u1E66\u1E62\u1E68\u0218\u015E\u2C7E\uA7A8\uA784]/g },
    { 'base': 'T', 'letters': /[\u0054\u24C9\uFF34\u1E6A\u0164\u1E6C\u021A\u0162\u1E70\u1E6E\u0166\u01AC\u01AE\u023E\uA786]/g },
    { 'base': 'TZ', 'letters': /[\uA728]/g },
    { 'base': 'U', 'letters': /[\u0055\u24CA\uFF35\u00D9\u00DA\u00DB\u0168\u1E78\u016A\u1E7A\u016C\u00DC\u01DB\u01D7\u01D5\u01D9\u1EE6\u016E\u0170\u01D3\u0214\u0216\u01AF\u1EEA\u1EE8\u1EEE\u1EEC\u1EF0\u1EE4\u1E72\u0172\u1E76\u1E74\u0244]/g },
    { 'base': 'V', 'letters': /[\u0056\u24CB\uFF36\u1E7C\u1E7E\u01B2\uA75E\u0245]/g },
    { 'base': 'VY', 'letters': /[\uA760]/g },
    { 'base': 'W', 'letters': /[\u0057\u24CC\uFF37\u1E80\u1E82\u0174\u1E86\u1E84\u1E88\u2C72]/g },
    { 'base': 'X', 'letters': /[\u0058\u24CD\uFF38\u1E8A\u1E8C]/g },
    { 'base': 'Y', 'letters': /[\u0059\u24CE\uFF39\u1EF2\u00DD\u0176\u1EF8\u0232\u1E8E\u0178\u1EF6\u1EF4\u01B3\u024E\u1EFE]/g },
    { 'base': 'Z', 'letters': /[\u005A\u24CF\uFF3A\u0179\u1E90\u017B\u017D\u1E92\u1E94\u01B5\u0224\u2C7F\u2C6B\uA762]/g },
    { 'base': 'a', 'letters': /[\u0061\u24D0\uFF41\u1E9A\u00E0\u00E1\u00E2\u1EA7\u1EA5\u1EAB\u1EA9\u00E3\u0101\u0103\u1EB1\u1EAF\u1EB5\u1EB3\u0227\u01E1\u00E4\u01DF\u1EA3\u00E5\u01FB\u01CE\u0201\u0203\u1EA1\u1EAD\u1EB7\u1E01\u0105\u2C65\u0250]/g },
    { 'base': 'aa', 'letters': /[\uA733]/g },
    { 'base': 'ae', 'letters': /[\u00E6\u01FD\u01E3]/g },
    { 'base': 'ao', 'letters': /[\uA735]/g },
    { 'base': 'au', 'letters': /[\uA737]/g },
    { 'base': 'av', 'letters': /[\uA739\uA73B]/g },
    { 'base': 'ay', 'letters': /[\uA73D]/g },
    { 'base': 'b', 'letters': /[\u0062\u24D1\uFF42\u1E03\u1E05\u1E07\u0180\u0183\u0253]/g },
    { 'base': 'c', 'letters': /[\u0063\u24D2\uFF43\u0107\u0109\u010B\u010D\u00E7\u1E09\u0188\u023C\uA73F\u2184]/g },
    { 'base': 'd', 'letters': /[\u0064\u24D3\uFF44\u1E0B\u010F\u1E0D\u1E11\u1E13\u1E0F\u0111\u018C\u0256\u0257\uA77A]/g },
    { 'base': 'dz', 'letters': /[\u01F3\u01C6]/g },
    { 'base': 'e', 'letters': /[\u0065\u24D4\uFF45\u00E8\u00E9\u00EA\u1EC1\u1EBF\u1EC5\u1EC3\u1EBD\u0113\u1E15\u1E17\u0115\u0117\u00EB\u1EBB\u011B\u0205\u0207\u1EB9\u1EC7\u0229\u1E1D\u0119\u1E19\u1E1B\u0247\u025B\u01DD]/g },
    { 'base': 'f', 'letters': /[\u0066\u24D5\uFF46\u1E1F\u0192\uA77C]/g },
    { 'base': 'g', 'letters': /[\u0067\u24D6\uFF47\u01F5\u011D\u1E21\u011F\u0121\u01E7\u0123\u01E5\u0260\uA7A1\u1D79\uA77F]/g },
    { 'base': 'h', 'letters': /[\u0068\u24D7\uFF48\u0125\u1E23\u1E27\u021F\u1E25\u1E29\u1E2B\u1E96\u0127\u2C68\u2C76\u0265]/g },
    { 'base': 'hv', 'letters': /[\u0195]/g },
    { 'base': 'i', 'letters': /[\u0069\u24D8\uFF49\u00EC\u00ED\u00EE\u0129\u012B\u012D\u00EF\u1E2F\u1EC9\u01D0\u0209\u020B\u1ECB\u012F\u1E2D\u0268\u0131]/g },
    { 'base': 'j', 'letters': /[\u006A\u24D9\uFF4A\u0135\u01F0\u0249]/g },
    { 'base': 'k', 'letters': /[\u006B\u24DA\uFF4B\u1E31\u01E9\u1E33\u0137\u1E35\u0199\u2C6A\uA741\uA743\uA745\uA7A3]/g },
    { 'base': 'l', 'letters': /[\u006C\u24DB\uFF4C\u0140\u013A\u013E\u1E37\u1E39\u013C\u1E3D\u1E3B\u017F\u0142\u019A\u026B\u2C61\uA749\uA781\uA747]/g },
    { 'base': 'lj', 'letters': /[\u01C9]/g },
    { 'base': 'm', 'letters': /[\u006D\u24DC\uFF4D\u1E3F\u1E41\u1E43\u0271\u026F]/g },
    { 'base': 'n', 'letters': /[\u006E\u24DD\uFF4E\u01F9\u0144\u00F1\u1E45\u0148\u1E47\u0146\u1E4B\u1E49\u019E\u0272\u0149\uA791\uA7A5]/g },
    { 'base': 'nj', 'letters': /[\u01CC]/g },
    { 'base': 'o', 'letters': /[\u006F\u24DE\uFF4F\u00F2\u00F3\u00F4\u1ED3\u1ED1\u1ED7\u1ED5\u00F5\u1E4D\u022D\u1E4F\u014D\u1E51\u1E53\u014F\u022F\u0231\u00F6\u022B\u1ECF\u0151\u01D2\u020D\u020F\u01A1\u1EDD\u1EDB\u1EE1\u1EDF\u1EE3\u1ECD\u1ED9\u01EB\u01ED\u00F8\u01FF\u0254\uA74B\uA74D\u0275]/g },
    { 'base': 'oi', 'letters': /[\u01A3]/g },
    { 'base': 'ou', 'letters': /[\u0223]/g },
    { 'base': 'oo', 'letters': /[\uA74F]/g },
    { 'base': 'p', 'letters': /[\u0070\u24DF\uFF50\u1E55\u1E57\u01A5\u1D7D\uA751\uA753\uA755]/g },
    { 'base': 'q', 'letters': /[\u0071\u24E0\uFF51\u024B\uA757\uA759]/g },
    { 'base': 'r', 'letters': /[\u0072\u24E1\uFF52\u0155\u1E59\u0159\u0211\u0213\u1E5B\u1E5D\u0157\u1E5F\u024D\u027D\uA75B\uA7A7\uA783]/g },
    { 'base': 's', 'letters': /[\u0073\u24E2\uFF53\u00DF\u015B\u1E65\u015D\u1E61\u0161\u1E67\u1E63\u1E69\u0219\u015F\u023F\uA7A9\uA785\u1E9B]/g },
    { 'base': 't', 'letters': /[\u0074\u24E3\uFF54\u1E6B\u1E97\u0165\u1E6D\u021B\u0163\u1E71\u1E6F\u0167\u01AD\u0288\u2C66\uA787]/g },
    { 'base': 'tz', 'letters': /[\uA729]/g },
    { 'base': 'u', 'letters': /[\u0075\u24E4\uFF55\u00F9\u00FA\u00FB\u0169\u1E79\u016B\u1E7B\u016D\u00FC\u01DC\u01D8\u01D6\u01DA\u1EE7\u016F\u0171\u01D4\u0215\u0217\u01B0\u1EEB\u1EE9\u1EEF\u1EED\u1EF1\u1EE5\u1E73\u0173\u1E77\u1E75\u0289]/g },
    { 'base': 'v', 'letters': /[\u0076\u24E5\uFF56\u1E7D\u1E7F\u028B\uA75F\u028C]/g },
    { 'base': 'vy', 'letters': /[\uA761]/g },
    { 'base': 'w', 'letters': /[\u0077\u24E6\uFF57\u1E81\u1E83\u0175\u1E87\u1E85\u1E98\u1E89\u2C73]/g },
    { 'base': 'x', 'letters': /[\u0078\u24E7\uFF58\u1E8B\u1E8D]/g },
    { 'base': 'y', 'letters': /[\u0079\u24E8\uFF59\u1EF3\u00FD\u0177\u1EF9\u0233\u1E8F\u00FF\u1EF7\u1E99\u1EF5\u01B4\u024F\u1EFF]/g },
    { 'base': 'z', 'letters': /[\u007A\u24E9\uFF5A\u017A\u1E91\u017C\u017E\u1E93\u1E95\u01B6\u0225\u0240\u2C6C\uA763]/g }
    ];
    var changes;
    function removeDiacritics(str) {
        if (!changes) {
            changes = defaultDiacriticsRemovalMap;
        }
        for (var i = 0; i < changes.length; i++) {
            str = str.replace(changes[i].letters, changes[i].base);
        }
        return str;
    }
    //#endregion
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


    var dateToShow = function (date, delimeter, twoDigitsYear) {
        var day = date.getDate(),
            month = date.getMonth() + 1,
            year = date.getFullYear().toString(),//we use to string so we can use substring
            delimeter = delimeter || '/';
        if (twoDigitsYear) {
            year = year.substr(2, 2);
        }
        if (day < 10) {
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

    var deleteAllow = function (userType, ownerId) {
        if (userType === 'none' || userType === 'invite') { //user is unsubscribed
            return false;
        }

        //check if user is owner of the file or box owner
        if (userType === 'owner' || ownerId === cd.userDetail().nId) {
            return true;
        }

        return false;
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
    var placeHolderDefined = false;
    var putPlaceHolder = function (x) {
        x = x || $('input[placeholder] , textarea[placeholder]');
        if (Modernizr.input.placeholder) {
            return;
        }
        x.each(function () {
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


        if (placeHolderDefined) {
            return;
        }
        placeHolderDefined = true;
        Object.defineProperty(window.HTMLInputElement.prototype, 'placeholder', {
            get: function () {
                return this.getAttribute('placeholder');
            }

        });

    };



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

    //var setTitle = function () {
    //    var name = $('[data-name]:visible').data('name');
    //    if (name) {
    //        document.title = 'Cloudents ' + $('[data-name]:visible').data('name');
    //    }
    //    else {
    //        document.title = 'Cloudents';
    //    }
    //};

    var getElementPosition = function (e) {
        o = e;
        var l = o.offsetLeft; var t = o.offsetTop;
        while (o = o.offsetParent)
            l += o.offsetLeft;
        o = e;
        while (o = o.offsetParent)
            t += o.offsetTop;
        return { left: l, top: t };
    }

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
            if (response && response.post_id) {
                $('[data-ddcbox]').each(function (i, e) { e.checked = false }); // close all popups
                analytics.trackSocial(url, 'share');
                cd.pubsub.publish('addPoints', { type: 'shareFb' });
                var postId = response.post_id.split('_')[1]; //takes the post id from *user_id*_*post_id*
                cd.data.fbRep({
                    data: { postId: postId }
                });
            }
        });
        //var sharer = "https://www.facebook.com/sharer/sharer.php?u=",
        //       x = screen.width / 2 - 700 / 2,
        //       y = screen.height / 2 - 450 / 2;
        //window.open(sharer + encodeURIComponent(url), 'sharer', 'width=700,height=450,left=' + x + ',top=' + y);
        //analytics.trackSocial(url, 'share');
    };

    var alreadyPosted = false;
    var postFb = function (name, description, link, picture, caption) {

        if (alreadyPosted) {
            return;
        }

        alreadyPosted = true;


        var params = {
            name: name,
            description: description,
            link: link,
            picture: '/images/cloudents.png',
            caption: 'CLOUDENTS'
        }

        FB.api('/me/feed', 'post', params, function (response) {            
        });
    };

    var highlightSearch = function (term, name, className) {
        var className = className || 'boldPart',
            firstPart = '<span class="' + className + '" data-ignore-rtl="true">',
            lastPart = '</span>',
            boldStringLength = firstPart.length + lastPart.length;

        if (!name) {
            return false;
        }
        if (!term) {
            return name;
        }
        term = term.trim().replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&");
        name = escapeHtmlChars(name);


        multiSearch(term);

        if (term.indexOf(' ') > -1) {
            multiSearch(term.replace(/ /g, ''));
        }



        return name;

        function multiSearch(eTerm) {
            var reg = new RegExp(eTerm, 'gi'),
            m, indeces = [];

            while (m = reg.exec(name)) {
                indeces.push(m.index);
            }

            for (var i = 0, l = indeces.length; i < l; i++) {
                name = highlight(name, indeces[i] + i * boldStringLength, indeces[i] + eTerm.length + i * boldStringLength);
            }
        };

        function highlight(str, start, end) {
            var text = firstPart + str.substring(start, end) + lastPart;

            return str.substring(0, start) + text + str.substring(end);
        };
    };


    var innerScroll = function (elem, height, width, distance) {
        var direction = $('html').css('direction') === 'ltr' ? 'right' : 'left';
        if (Modernizr.touch) {
            elem.css({ height: height, overflow: 'auto', '-webkit-overflow-scrolling': 'touch' });
            return;
        }

        elem.slimScroll({
            height: height,
            position: direction,
            disableFadeOut: true,
            distance: distance || '3px',
            width: width || 'none'
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

    var isElementInViewport = function (el) {
        var rect = el.getBoundingClientRect();

        return (
            rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) && /*or $(window).height() */
            rect.right <= (window.innerWidth || document.documentElement.clientWidth) /*or $(window).width() */
        );
    }

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
            userData.img = userImg ? userImg.getAttribute('src') : $('body').data('pic');
            if (userName) {
                userData.name = userName.getAttribute('data-name');
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
    cd.updateTimeActions = updateTimeActions;
    cd.isElementInViewport = isElementInViewport;
    cd.parseTimeString = parseTimeString;
    cd.highlightSearch = highlightSearch;
    cd.removeDiacritics = removeDiacritics;
    cd.getElementPosition = getElementPosition;
    cd.sessionStorageWrapper = sessionStorageWrapper;
    cd.localStorageWrapper = localStorageWrapper;
    cd.debounce = debounce;
    cd.conversion = conversion;
    cd.getExtensionColor = getExtensionColor;
    cd.getExtension = getExtension;
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
    cd.getParameterByNameFromString = getParameterByNameFromString;
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
    cd.setElementChildrenDirection = setElementChildrenDirection;
    //cd.switchBackToMobile = switchBackToMobile;
    cd.loaderOn = loaderOn;
    cd.shareFb = shareFb;
    cd.postFb = postFb;
    //cd.setTitle = setTitle;

    cd.clone = clone;

    cd.getUTCDate = getUTCDate;
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

})(window.cd = window.cd || {}, jQuery, cd.analytics, cd.data);