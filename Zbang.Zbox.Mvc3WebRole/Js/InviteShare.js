(function (mmc, $) {
    "use strict";
    var popUpElement,
    mboxName,
    firstTimeFired = true,
    cache = {},
    $inviteInput = $('.inviteInput'), $inviteList = $('.inviteList'),
    template = '<li><span class="tagNameSelected">{0}</span><button type="button" class="tagSelectedX"></button><input type="hidden" value="{1}" name="Recepients"></li>';
    mmc.invitapp = function (elem) {
        popUpElement = $(elem);
    };
    if (mmc.page.box) {
        mmc.invite = function (elem) {
            $(elem).click(function () {
                mmc.invitePopUp()
            });

        }
    }
    mmc.invitePopUp = function (boxid, boxname) {
        var regularInvite = boxid === undefined;
        if (regularInvite) {
            boxid = Zbox.getParameterByName('boxuid');
        }
        mboxName = regularInvite ? $('#boxName').text() : boxname;
        if (firstTimeFired) {
            firstTimeFired = false;
            registerEvents();
        }
        popUpElement.show();
        mmc.modal(function () {
            popUpElement.hide();
            cleanUp();
        }, 'invite');
        cleanUp();
        if (!regularInvite) {
            $('#iCancel').val(ZboxResources.Skip);
        }
        $inviteInput.focus();
        popUpElement.find('#BoxUid').val(boxid);
        $('#iboxName').text(mboxName).css('overflow', 'hidden'); // overflow is for the ellipsis (property set in css caused problems)
    }
    function cleanUp() {
        $('.inviteMsg').val('');
        $('#fInviteNote').val(ZboxResources.IdLikeToShare.format(mboxName)+$('#userName').text());
        $inviteList.empty();
        var forms = $('.popupForm');
        for (var i = 0; i < forms.length; i++) {
            mmc.clearErrors(forms[i]);
        }
        $('.field-validation-error').each(function () {
            $(this).text('');
        });
        $inviteInput.val('');
        $('#iCancel').val(ZboxResources.Cancel);
        changeInputWidth();
    }
    function addToMailList(name, email) {
        $inviteList.append(template.format(name, email));
        changeInputWidth();
    }
    function registerEvents() {
        var $imprtCntct = $('button.addInvite');
        if (window.cloudsponge === undefined) {
            async_load('https://api.cloudsponge.com/address_books.js', false);
            var interval = window.setInterval(function () {
                if (window.cloudsponge !== undefined) {
                    cloudsponge.init({
                        domain_key: "XRUPDE8S8C7FEYUJ58ZW",
                        afterSubmitContacts: function (contacts, source, owner) {
                            for (var i = 0; i < contacts.length; i++) {
                                addToMailList(contacts[i].fullName(), contacts[i].selectedEmail());
                            }
                        }
                    });
                    $imprtCntct.removeAttr('disabled');
                    window.clearInterval(interval);
                }
            }, 200);
        }
        $imprtCntct.click(function () {
            cloudsponge.launch();
        });
        $inviteList.on('click', 'button.tagSelectedX', function () {
            $(this).parent().remove();
        });
        $inviteInput.autocomplete({
            delay: 500,
            source: function (request, response) {
                var term = request.term;
                if (term in cache) {
                    response(cache[term]);
                    return;
                }
                $.post("/User/FriendsByPrefix", request, function (data, status, xhr) {
                    if (data.Success) {
                        cache[term] = data.Payload;
                        response(cache[term]);
                    }
                });
            },
            select: function (e, ui) {
                addToMailList(ui.item.label, ui.item.value);
                $inviteInput.val('');
                return false;
            },
            change: function (e, ui) {
                if (!ui.item) {
                    validateEmailAndPush(e.target.value);
                }
            },
            focus: function (e, ui) {
                $inviteInput.val(ui.item.label);
                return false;
            }
        });
        function validateEmailAndPush(email)
        {
            email = $.trim(email);
            var regex = /^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,6}|[0-9]{1,3})(\]?)$/i;
            if (email === '') {
                return;
            }
            if (!regex.test(email)) {
                mmc.notification('email is in the wrong format');
                $inviteInput.filter(':visible').focus();
                return;
            }
            addToMailList(email, email);
            $inviteInput.val('');
        }
        $inviteInput.keydown(function (e) {
            if (e.keyCode === $.ui.keyCode.BACKSPACE && $inviteInput.filter(':visible').val().length === 0) {
                $inviteList.find('li:last').remove();
            }
            if (e.keyCode === $.ui.keyCode.ENTER
                || e.keyCode === $.ui.keyCode.SPACE
                || e.keyCode === 186
                || e.keyCode === $.ui.keyCode.TAB
                || e.keyCode === 188) {
                validateEmailAndPush(e.target.value);
            }
            //so the form will not submit
            if (e.keyCode === $.ui.keyCode.ENTER) {
                return false;
            }
        });

        $('.popupForm').submit(function () {
            validateEmailAndPush($inviteInput.filter(':visible').val());
        });
        
    }

    var maxwidth = $inviteInput.width();
    function changeInputWidth() {
        var space = $inviteList.filter(':visible').width();
        $inviteInput.width(maxwidth - space)
    }
    mmc.inviteSuccess = function (data) {
        successResult(data, $('#fInvite'));
    }
    mmc.shareSuccess = function (data) {
        successResult(data, $('#fShare'));
    }
    mmc.messageSuccess = function (data) {
        successResult(data, $('#fMessage'));
    }
    function successResult(data, $formid)
    {
        if (data.Success) {
            $('.modal').click();
            mmc.clearErrors($formid);
        }
        else {
            mmc.modelErrors($formid, data.Payload);
        }
    }

    /*Share section */
    var $shareElem,
        shareFirstTime = true;
    mmc.shrapp = function (elem) {
        $shareElem = $(elem);
    }

    if (mmc.page.box || mmc.page.item) {
        mmc.share = function (elem) {
            $(elem).click(function () {
                var sharename = getShareName(mmc.page);
                if (sharename === null) { return; }
                $('#fShareNote').val(ZboxResources.IdLikeToShare.format(sharename) + $('#userName').text());
                $shareElem.show();
                $('#sboxName').text(sharename);
                mmc.modal(function () {
                    $shareElem.hide();
                    shareCleanUp();
                }, 'share');

                RegisterShareEvents();
                shareFirstTime = false;
                if (firstTimeFired) {
                    firstTimeFired = false;
                    registerEvents();
                }
                if (window.addthis === undefined) {
                    loadAddThis();
                }
            });
        }
    }

    function getShareName(page) {
        if (page.box) {
            return $('#boxName').text();
        }
        if (page.item) {
            return $('#itemName').text();
        }
        return null;
    }

    function shareCleanUp() {
        cleanUp();
        $('#fShare').hide();
        $('#separatorV').hide();
    }
    function RegisterShareEvents() {
        if (window.clipboardData) // IE
        {
            if (shareFirstTime) {
                $('#CopyLink').click(function () {
                    if (window.clipboardData.setData('text', $(this).attr('data-url'))) {
                        mmc.notification(ZboxResources.UrlCopied);
                        //Boxes.ChangeBoxPrivacy();
                    }
                });
            }
        }
        else {
            ZeroClipboard.setMoviePath('/Scripts/ZeroClipboard.swf');
            var clip = new ZeroClipboard.Client();
            clip.setText('');
            clip.addEventListener('mousedown', function () {
                clip.setText(document.getElementById('sUrl').value);
                //Boxes.ChangeBoxPrivacy();
            });

            clip.setHandCursor(true);
            clip.addEventListener('complete', function () {
                mmc.notification(ZboxResources.UrlCopied);
            });
            clip.glue('sCopyLink', 'CopyLink');
        }
        if (shareFirstTime) {
            $('#openMail').click(function () {
                $('#fShare').show();
                $('#separatorV').show();
                $inviteInput.focus();
            });
        }
    };

    /*Message section*/
    mmc.msgdiag = function (elem) {
        $("body").on('click','button.msg',function () {
        //$('button.msg').click(function () {
            var $this = $(this);
            $(elem).show();
            addToMailList($this.data('name'), $this.data('uid'));
            mmc.modal(function () {
                $(elem).hide();
                cleanUp();
            }, 'message');

            if (firstTimeFired) {
                firstTimeFired = false;
                registerEvents();
            }
        });
    }
    
}(window.mmc = window.mmc || {}, jQuery))