(function (mmc, $) {
    "use strict";

    mmc.page = {
        search: window.location.pathname.match(/search/i) !== null,
        account: window.location.pathname.match(/account/i) !== null,
        friend: window.location.pathname.match(/friend/i) !== null,
        //dashboard: window.location.pathname === '/' || window.location.pathname.match(/friend/i) !== null,
        box: window.location.pathname.match(/box/i) !== null,
        item: window.location.pathname.match(/item/i) !== null,


        //my: window.location.hash === '' || window.location.hash.toLowerCase() === '#my',
        //library : window.location.hash.toLowerCase() === '#library'
    };
    mmc.register = $('#userDetails').length;
    $(function () {

        if (top !== self) {
            //detect iframe
            return;
        }
        registerDropDowns();
        putPlaceHolder();
        putFooterAtTheBottom();
        checkGlobalSearchSubmit();
        registerSubscribePopUp();
        registerPopUpCloseEvent();
        $('[data-strap]').each(function (i) {
            var section = $(this).data('strap');

            if (mmc[section] !== null) {
                mmc[section](this);
            }
        });


    });
    mmc.notification = function (msg) {
        alert(msg);
    }; //var firstTime = true;
    mmc.confirm = function (msg) {
        return confirm(msg);

        ////$('footer').after('<div class="popupWrppr confirm txtshd1" id="ConfirmPop"><header class="popupHeader"></header><div class="message"></div><input type="submit" class="btn1 next" /><input type="button" class="btn2 cancel" value="Cancel" /><button class="closeDialog shareSprite"></button></div>');
        //var $confirm = $('#ConfirmPop');
        //$confirm.show();
        //setPopupVals($confirm);

        //if (firstTime) {
        //    firstTime = false;
        //    registerEvents();
        //}

        //function registerEvents() {
        //    //console.log("registering......");
        //    $confirm.click(function (e) {
        //        var $target = $(e.target);
        //        if ($target.hasClass('btn2') || $target.hasClass('closeDialog')) {
        //            //$confirm.remove();
        //            $confirm.hide();
        //            return false;
        //        }
        //        if ($target.hasClass('btn1')) {
        //            //$confirm.remove();
        //            $confirm.hide();
        //            return true;
        //        }
        //    });
        //}
        //function setPopupVals($confirm) {
        //    $confirm.find('header').text("bla");
        //    $confirm.find('.message').text(msg);
        //    $confirm.find('.btn1').val("bla");
        //}
    };

    function putFooterAtTheBottom() {
        $('footer').removeClass('fe');
        if ($('.page').length) {
            $('#MainContent').css('minHeight', $(window).height() - $('.page').offset().top - $('.footer').outerHeight(true));
        }
    }
    function checkGlobalSearchSubmit() {
        if (!Modernizr.input.required) {
            $('#searchFrom').submit(function () {
                if ($(this).find('input[type="text"]').val() === '') {
                    return false;
                }
            });
        }
    }
    function registerPopUpCloseEvent() {
        $('.closeDialog,.cancel').click(function () {
            $('.modal').trigger('close');
        });
    }
    function registerSubscribePopUp() {
        if (!mmc.register) {
            var modal = null;
            $('#commentsSection').on('click', 'a.commentAction', function (e) {
                registerPopUp(e);
            });
            $('.boxUploadbtn,textarea,#edit,#delete,[data-strap="share"]').click(function (e) {
                registerPopUp(e);
            });
            $('#signUpNow').click(function () {
                window.location.replace($('.signUp').find('a').attr('href'));
            });
            $('#joinLater').click(function () {
                modal.trigger('close');
            });
        }
        function registerPopUp(e) {
            e.preventDefault();
            $(this).blur();
            var $regPopup = $('#regPopup');
            $regPopup.show();
            modal = mmc.modal(function () {
                $regPopup.hide();
            }, 'register');
        }
    }
    function registerDropDowns() {
        if (mmc.register) {
            var $userMenu = $('ul.userMenu');
            $('.userDrpSettings').click(function (e) {
                $('#invitesList').slideUp(); //close invite - maybe should be in class
                if ($userMenu.is(':visible')) {
                    $userMenu.slideUp(150);
                    return;
                }
                if (window.UserVoice === undefined) {
                    var url = ('https:' == document.location.protocol ? 'https://' : 'http://') + 'widget.uservoice.com/UkUMA0g4Tp2dVp21PCpCRA.js';
                    $.getScript(url);
                }
                e.stopPropagation();
                $userMenu.slideDown(150);
            });
            $('body').click(function () {
                $userMenu.slideUp(150);
            });
        }
    }
    function putPlaceHolder() {
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
            }
        });
        $('form').submit(function () {
            $(this).find('*[placeholder]').each(function () {
                if ($(this).val() === $(this).attr('placeholder')) {
                    $(this).val('');
                }
            });
        });

    }

    var modalCache = {};
    mmc.modal = function (jsExtra, popupElem, css) {
        if (popupElem === undefined) {
            throw 'modal argument should get popupElem';
        }
        var cssClass = ["modal"];
        if (css !== undefined) {
            cssClass.push(css);
        }
        var modal = $('.' + cssClass.join('.'));
        if (!modal.length) {
            modal = $('<div></div>');
            for (var i = 0; i < cssClass.length; i++) {
                modal.addClass(cssClass[i]);
            }
            modal.appendTo($('body'));
        }
        modal.show();
        if (popupElem in modalCache) {
            return modal;
        }
        modal.on('close', function () {
            if (!$.isFunction(jsExtra)) {
                modal.hide();
            }
            if (jsExtra() !== false) {
                modal.hide();
            }


        });

        //modal.click(popupElem, function (e) {
        //    if (e.isTrigger) {
        //    if ($.isFunction(jsExtra)) {
        //        jsExtra();
        //    }
        //    modal.hide();
        //    }
        //});
        modalCache[popupElem] = true;
        return modal;
    };

    mmc.permission = function () {
        var x = $('[data-strap="permission"]').data('value');
        window.sec = new permission(x);
    };
    if (mmc.page.search) {
        mmc.search = function () {
            ko.applyBindings(new SearchViewModel());
        };
    }
    if (mmc.page.account) {
        //mmc.SignUp = function () {
        //    new LogOn(); // TODO: need to do that
        //};
    }
    mmc.dateToShow = function (date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        return day + "." + month + "." + year;
    };
    mmc.modelErrors = function (form, errors) {
        function showGeneralError(err) {
            var container = $(form).find('[data-valmsg-summary=true]'), list = container.find('ul');

            list.empty().append("<li>" + err + "</li>");
            container.addClass("validation-summary-errors").removeClass("validation-summary-valid");
        }
        if ($.isArray(errors)) {
            for (var i = 0; i < errors.length; i++) {
                var $validate = $(form).find('span[data-valmsg-for="' + errors[i].Key + '"]');
                if ($validate.length > 0) {
                    $validate.text(errors[i].Value[0]);
                }
                else {
                    //show only one error
                    showGeneralError(errors[i].Value[0]);
                    break;
                }
            }
            return;
        }
        if (typeof (errors) === 'string') {
            showGeneralError(errors);
        }
    };
    mmc.clearErrors = function (form) {
        var container = $(form).find('[data-valmsg-summary=true]'), list = container.find('ul');
        list.empty();
        container.addClass("field-validation-valid").removeClass("field-validation-error");
    };
    mmc.autocomplete = function (elem, userparams, datasourceUrl) {
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
                //focus: function (event, ui) {
                //    elem.val(ui.item.label);
                //    return false;
                //},

                minLength: 2,
                change: function (event, ui) {
                    if (ui.item)
                    { return; }
                    if (cache[elem.val()] !== undefined && cache[elem.val()].length) {
                        elem.val(cache[elem.val()][0]);
                        return;
                    }
                    elem.val('');
                },
                autoFocus: true,
                position: {
                    my: "right top"
                  , at: "right bottom+3"
                }
            };
        elem.keypress(function (e) {
            if (e.which == 13) {
                elem.blur();
            }
        });
        var params = $.extend({}, $.ui.autocomplete.prototype.options, defaultParms, userparams);
        $(elem).autocomplete(params);


    };
    mmc.modelErrors2 = function (form, errors) {

        if ($.isArray(errors)) {
            for (var i = 0; i < errors.length; i++) {
                var label = $(form).find($('label[for=' + errors[i].Key + ']'));
                if (label.length) {
                    generateFieldError(label, errors[i].Value[0]);
                    continue;
                }
                var input = $(form).find($('input[name=' + errors[i].Key + ']'));
                if (input.length) {
                    generateFieldError(input, errors[i].Value[0]);
                    continue;
                }
                generateSummaryError(form.find(':first'), errors[i].Value[0]);
            }
        }
        if (typeof (errors) === 'string') {
            generateSummaryError(form.find(':first'), errors);
        }

        function generateFieldError(elem, error) {
            $(elem).after('<span class="field-validation-error">' + error + '</span>');
        }
        function generateSummaryError(elem, error) {
            $(elem).before('<div class="validation-summary-errors">' + error + '</div>');
        }

    };
    mmc.resetForm = function (form) {
        var $form = $(form);
        $form.validate().resetForm();

        $form.find("[data-valmsg-summary=true]")
            .removeClass("validation-summary-errors")
            .addClass("validation-summary-valid")
            .find("ul").empty();

        $form.find("[data-valmsg-replace]")
            .removeClass("field-validation-error")
            .addClass("field-validation-valid")
            .empty();

        $form.find('div.validation-summary-errors').remove();
        $form.find('span.field-validation-error').remove();
        return $form;
    };


    mmc.updateQueryStringParameter = function (uri, key, value) {
        var re = new RegExp("([?|&])" + key + "=.*?(&|$)", "i");
        var separator = uri.indexOf('?') !== -1 ? "&" : "?";
        if (uri.match(re)) {
            return uri.replace(re, '$1' + key + "=" + value + '$2');
        }
        else {
            return uri + separator + key + "=" + value;
        }
    };
}(window.mmc = window.mmc || {}, jQuery));



