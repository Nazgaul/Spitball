(function (cd, ZboxResources, analytics, dataContext) {
    if (window.scriptLoaded.isLoaded('mI')) {
        return;
    }
    var firstTimeFired = true,

        boxid,
        $inviteInput = $('.inviteInput'),
        maxwidth = $inviteInput.width(),
         template = '<li><span class="emailText">{0}</span><button type="button" class="removeItem"></button><input type="hidden" value="{1}" name="Recepients"></li>';
    cd.pubsub.subscribe('invite', function (d) {
        //analytics.trackEvent('box', 'invite', d.name);
        boxid = d.id;
        maxwidth = $inviteInput.width();
        //boxprivacy = d.privacy;
        ////$note.val(welcomeText.format(d.name, $('#userName').text()));
        //$('#iboxName').text(d.name).css('overflow', 'hidden'); // overflow is for the ellipsis (property set in css caused problems)
        //inviteVisible();
        if (firstTimeFired) {
            firstTimeFired = false;
            registerEvents();
        }
    });
    //function registerAutoComplete(append) {
    //    cd.autocomplete($inviteInput, {
    //        delay: 500,
    //        select: function (e, ui) {
    //            addToMailList(ui.item.label, ui.item.value);
    //            $inviteInput.val('');
    //            return false;
    //        },
    //        change: function (e, ui) {
    //            if (!ui.item) {
    //                validateEmailAndPush(e.target.value);
    //            }
    //        },
    //        appendTo: append
    //    }, "/User/FriendsByPrefix");
    //}
    function validateEmailAndPush(email) {
        email = $.trim(email);
        var regex = /^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,6}|[0-9]{1,3})(\]?)$/i;
        if (email === '') {
            return;
        }
        if (!regex.test(email)) {
            cd.notification(JsResources.InvalidEmail);
            $inviteInput.filter(':visible').focus();
            return;
        }
        addToMailList(email, email);
        $inviteInput.val('');
        return;
    }
    function sharedEvents() {
        $inviteInput = $('.inviteInput');
        $inviteList = $('.inviteList');

        $inviteInput.keydown(function (e) {
            if (e.keyCode === $.ui.keyCode.BACKSPACE &&
                $inviteInput.filter(':visible').val().length === 0) {
                $inviteList.find('li:last').remove();
            }
            if (e.keyCode === $.ui.keyCode.SPACE
                || e.keyCode === 186
                || e.keyCode === $.ui.keyCode.TAB
                || e.keyCode === 188) {
                //    nonSubmitPress = false;
                validateEmailAndPush(e.target.value);
            }
            //so the form will not submit

            //nonSubmitPress = true;
        });
    }
    function registerEvents() {
        var $fInvite = $('#fInvite')
        // registerAutoComplete($fInvite);
        sharedEvents();
        $inviteList.on('click', 'button.removeItem', function () {
            $(this).parent().remove();
        });


        $fInvite.submit(function (e) {
            e.preventDefault();
            validateEmailAndPush($inviteInput.filter(':visible').val());

            var recepients = $fInvite.find('input[name="Recepients"]').length;
            if (!recepients) {
                //var error = [{ Key: "Recepients", Value: ['- This field is required.'] }];
                cd.displayErrors($fInvite, JsResources.SpecifyRecipient);
                return;
            }

            if (!$fInvite.valid || $fInvite.valid()) {
                var data = $fInvite.serializeArray();
                data.push({ name: 'BoxUid', value: boxid });
                analytics.trackEvent('box', 'invite', boxid);
                cd.resetForm($fInvite);
                $('.inviteList').find('li').remove();
                location.hash = '';
                dataContext.inviteBox({
                    data: data,
                    error: function (data) {
                        cd.notification(JsResources.ProblemSending);
                    },
                    isJson: false
                });
                $inviteInput.css('width', '');
            }
        });

    }
    function addToMailList(name, email) {
        $inviteList.append(template.format(name, email));
        changeInputWidth();
    }

    function changeInputWidth() {
        var space = $inviteList.filter(':visible').width();
        $inviteInput.width(maxwidth - space);
    }

})(cd, JsResources, cd.analytics, cd.data);