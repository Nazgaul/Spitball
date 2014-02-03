<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Zbang.Zbox.ViewModel.DTOs" %>
<%@ Import Namespace="Zbang.Zbox.Domain.Common" %>
<%/*%><script src="~/Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script><%*/%>

<script type="text/javascript">
    //need to remove this global stuff
    var friendEmails = [];
    var friendEmailsCache = ['loading'];
    var recipients = [];

    $(document).ready(function () {
        registerFriendEntryEvents();
        createEmailsAutoComplete();

        $('#btnShareBox').click(function (event) {
            if (Zbox.Box.boxes.GetCurrentBox().UserPermission < 4) {
                $('#btnShareBox').button('disable');
            }
            else {
                loadFriendEmails();
                ShareBox();
            }
            return false;
        });

        //        $('div#dialog-share-Box #To').bind('focusin', function (event) {
        //            $('div#dialog-share-Box #ErrorMail').text('');
        //        });
        $('#dialog-share-Box #password').bind('focusin', function (event) {
            $('#dialog-share-Box #ErrorMail').text('');
        });

        $('#dialog-share-Box #checkPassword').bind('click', function (event) {
            PasswordShowClick(this);
        });
        $('#dialog-share-Box').bind('dialogopen', function (event) {
            //add css to button class dynamic
            //send
            $('.ui-dialog-buttonset > button:first').addClass('style45');
            $('.ui-dialog-buttonset > button:first').addClass('dialogInputSend');
            $('.ui-dialog-buttonset > button:first').addClass('dialogInputSend :hover');

            //cancel
            $('.ui-dialog-buttonset > button:last').addClass('style33Dialog');

        });

        $('#To').keydown(function (event) {
            $('#dialog-share-Box #ErrorMail').text('');
            if (event.keyCode === $.ui.keyCode.ENTER || event.keyCode === $.ui.keyCode.SPACE) {
                var email = $(this).val();
                if (pushRecipientEmail(email)) {
                    $(this).val('');
                    createEmailLabel(email);
                }
            }
        });

        $('#To').blur(function (event) {

            var email = $(this).val();
            if (pushRecipientEmail(email)) {
                $(this).val('');
                createEmailLabel(email);
            }
        });

        $('#recipients-display a.delete-email').live('click', function (e) {
            removeEmail($(this).prev('span').text());
            $(this).closest('span').remove();
            reCalcToInput();
        });

    });

    function RenderDragAndDrop() {
        $("#1, #2, #4").sortable({
            connectWith: $('#1,#2,#4'),
            axis: 'y',
            /*cursor: 'move',            */
            containment: $('#divFriends'),
            tolerance: 'pointer',
            helper: 'clone',
            revert: true,

            forcePlaceHolderSize: true,
            forceHelperSice: true,
            items: 'div:not(.sortableDisable)',

            over: function (event, ui) {
                $(event.target).children('.MoveFriends').toggle();

            },
            out: function (event, ui) {
                $(event.target).children('.MoveFriends').toggle();
            },
            receive: function (event, ui) {
                var data = {}
                console.log(event);
                //var permission = $(event.target).attr('id');
                data['NewPermission'] = $(event.target).attr('id');


                var url = '/Collaboration/ChangeBoxUserRights';
                var paramId = 'inviteId';
                var paramValue = $(ui.item).find('input:hidden').val();
                data['userEmail'] = paramValue;

                data['boxId'] = $('#selecteBoxId').val();
                ChangeFriendPermission(url, data, ui.sender);




            }
        });
    }

    function ChangeFriendPermission(url, dataparams, sender) {
        var request = new ZboxAjaxRequest({
            url: url,
            data: dataparams,
            success: function (data) {
                if (data != null) // User change himself the right                    
                    Zbox.Box.boxes.GetBox(dataparams['boxId']).UserPermission = dataparams['NewPermission'];
                
            },
            error: function (error) {
                Zbox.toaster(error);
                $(sender).sortable('cancel');
            }
        });
        request.Post();
    }

    function loadFriendEmails() {
        friendEmailsCache = friendEmails.splice(0);
        friendEmails = [];

        Zbox.GetUserFriends(
            function (friends) {
                $(friends).each(function (i, friend) {
                    friendEmails.push(friend.FriendEmailAddress);
                });
                friendEmailsCache = friendEmails.splice(0);
            },
            function (err) {
            }
        );
    }

    function createEmailsAutoComplete() {

        $('#To').autocomplete({
            source: function (request, response) {
                //our source is loaded lazily via ajax, we want to commune with the most recent cache each time
                response($.ui.autocomplete.filter(friendEmailsCache, request.term));
            },
            select: function (event, ui) {
                var email = ui.item.label;
                if (pushRecipientEmail(email)) {
                    createEmailLabel(email);
                    $('#dialog-share-Box #ErrorMail').text('');
                    $(this).val('');
                }
                return false;
            }

        });
    }

    function pushRecipientEmail(email) {
        var emailTester = /<%: Zbang.Zbox.Domain.Common.Validation.EMAIL_REGEX %>/;
        // if we hit enter while autocomplete is showing options both select and keydown events are fired,
        // as a result this method is run twice, once with an empty value, this is a "hack" prevents this.
        //

        if (email == '') return false;

        var trimmed = $.trim(email);

        // rest of the validation
        if (indexOfEmail(trimmed) > -1) return false;

        if (!emailTester.test(trimmed)) {
            //$('div#dialog-share-Box #ErrorMail').text(trimmed + ' is not a valid email.');
            return false;
        }

        recipients.push(trimmed);
        return true;
    }

    function indexOfEmail(email) {
        for (var i = 0; i < recipients.length; i++)
            if (recipients[i] == email)
                return i;

        return -1;
    }

    function removeEmail(email) {
        var trimmed = $.trim(email);
        var index = indexOfEmail(trimmed);
        if (index > -1)
            recipients.splice(index, 1);
    }

    function createEmailLabel(email) {
        $('#recipients-display').prepend(
            '<span class="style25 email-entry blue-border rounded"><span class="emailtxt gray">' +
            email +
            '</span><a class="delete-email" href="#"><img src="/Content/Images/list_delete_icon.png" alt="x" /></a></span>');

        reCalcToInput();
    }

    function reCalcToInput() {
        $('#dialog-share-Box #recipients-display input#To').width($('#dialog-share-Box #recipients-display').width() - $('#dialog-share-Box #recipients-display span').outerWidth(true) - 1);
    }

    function clearEmailStateAndDisplay() {
        recipients = [];
        $('#recipients-display .email-entry').remove();
        reCalcToInput();
    }

    function ShareBox() {

        var userName = $('#LogOnUserControl>div:first').text().replace('Hello', '');
        userName = $.trim(userName);
        $('#dialog-share-Box #privacySelect').val($('#PrivacyId').val());
        //$('div#dialog-share-Box #textAreaPersonalNote').text(userName + ' would like to share his ' + $('#selectedBoxName').text() + ' box with you on Zbox');


        if ($('#dialog-share-Box #privacySelect').val() == 'PasswordProtected') {
            $('#dialog-share-Box .ShareBoxdivPassword').css('display', 'block');
        }
        else {
            $('#dialog-share-Box #dialogMessage .ShareBoxdivPassword').css('display', 'none');
        }

        $('#dialog-share-Box').dialog({
            title: '<div class="style80"><img src="/Content/Images/envelope.png" /> Invitation to ' +
                    Zbox.Box.ShortBoxName($('#selectedBoxName').text(), 10) + ' box</div>',
            width: 'auto',
            height: 'auto',
            modal: true,
            resizable: false,
            open: function (event, ui) {
                ClearData();
                $('#dialog-share-Box #textAreaPersonalNote').val('I\'d like to share the ' + $('#selectedBoxName').text() + ' box on ZBOX with you. \n\r' + userName);
                $('#dialog-share-Box #privacySelect').selectbox();
                $('#dialog-share-Box #privacySelect').bind('change', function (event) {
                    IsPasswordShow();
                });
                $('#dialog-share-Box #recipients-display').css({
                    'max-height': Math.round($(this).height() * 0.3),
                    'max-width': $('#recipients-display').width()
                });
                reCalcToInput();
                //remove previous recepients
                //$('span', 'div#dialog-share-Box #recipients-display').remove()

            },
            close: function (event, ui) {
                $('#dialog-share-Box #privacySelect').unbind('change');
                $('#dialog-share-Box #privacySelect').parents('.jquery-selectbox').unselectbox();
            },
            buttons: {
                Send: function () {

                    if ($('#dialog-share-Box #privacySelect').val() == 'NotShared') {
                        $('#dialog-share-Box #ErrorMail').text('You cant share box with those privacy settings');
                        return false;
                    }

                    if (recipients.length === 0 || $.trim($('#To').val()).length > 0) {
                        $('#dialog-share-Box #ErrorMail').text('You did not specify valid email');
                        return false;
                    }

                    var password = Zbox.GetPassword('#dialog-share-Box #checkPassword', '#dialog-share-Box #password1', '#dialog-share-Box #password');

                    if ($('#dialog-share-Box #ShareBoxdivPassword').css('display') == 'block') { //checking password
                        if ($.trim(password) == '') {
                            $('#dialog-share-Box #ErrorMail').text('you need to fill a password');
                            return false;
                        }
                    }
                    //Everything is ok


                    $("#dialog-share-Box").dialog('close');
                    if ($('#dialog-share-Box #privacySelect').val() != Zbox.BoxPrivacySettings.CurrentBoxPrivacySettings || password != Zbox.GetPassword('div.privacypass #share-password-confirm', '#share-password1', '#share-password')) {
                        Zbox.BoxPrivacySettings.saveBoxPrivacy($('#selectedBoxSUID').val(), $('#dialog-share-Box #privacySelect').val(), password);
                        Zbox.BoxPrivacySettings.changeCurrentPrivacySettingsDisplay($('#dialog-share-Box #privacySelect').val());
                    }
                    SendInvitation(recipients);

                },
                Cancel: function () {
                    $("#dialog-share-Box").dialog('close');
                }
            }

        });
    }

    function ClearData() {
        clearEmailStateAndDisplay();
        $('#dialog-share-Box #ErrorMail').text('');
        $('#dialog-share-Box #To').val('');
        $('#dialog-share-Box #privacySelect').val($('#PrivacyId').val());
        $('#dialog-share-Box #password').val($('#share-password').val());
        IsPasswordShow();
    }
    function IsPasswordShow() {
        if ($('#dialog-share-Box #privacySelect').val() == 'PasswordProtected') {
            $('#dialog-share-Box #ShareBoxdivPassword').css('display', 'block');
        }
        else
            $('#dialog-share-Box #ShareBoxdivPassword').css('display', 'none');
    }


    function SendInvitation(recipientEmails) {

        var dataInvite = {};
        if (!dataInvite) {
            $('#spanFriendListMessage').text('please select at least one friend to share a box with...');
            return false;
        }

        //Get selectedBoxId and send it also
        //Change controller interface
        var boxId = parseInt($('#selecteBoxId').val());

        dataInvite['boxId'] = boxId;        
        dataInvite['to'] = Zbox.TransformArrayToObject(recipientEmails);


        dataInvite['personalNote'] = escape($('#textAreaPersonalNote').val());

        var request = new ZboxAjaxRequest({
            url: '/Collaboration/ShareBox',
            data: dataInvite,
            success: function (response) {
                loadBoxSubscribersAndInvitations($('#selecteBoxId').val(), $('#selectedBoxIdPermission').val());
                Zbox.toaster('shared successfully');
            },
            error: function (error) {
                $('#spanFriendListMessage').text('error sharing box: ' + error);
            }
        });

        request.Post();
        $('#spanFriendListMessage').empty();
    }



    function registerFriendEntryEvents() {
        $('.InvitationList div.friendEntry select').live('change', function () {
            var inviteId = $(this).parent().find('input:hidden').val();
            var request = new ZboxAjaxRequest({
                url: '/Collaboration/ChangeInviteRight',
                data: { inviteId: inviteId, boxId: $('#selecteBoxId').val(), NewPermission: $(this).val() },
                success: function (data) {

                },
                error: function (error) {
                    $('#spanFriendListMessage').text(error);
                }
            });
            Zbox.ShowConfirmDialog({
                ok: function () {
                    request.Post();
                    return true;
                }
            });

            return false;
        });

        $('div.friendEntry').live('mouseenter', function () {
            $(this).find('a>img').show();
        });

        $('div.friendEntry').live('mouseleave', function () {
            $(this).find('a>img').hide();
        });



        $('div.friendEntry>a').live('click', function () {
            var entry = $(this).parent();

            var request = new ZboxAjaxRequest({
                url: '/Collaboration/DeleteUserFromBox',
                data: { userToDeleteEmail: $(this).attr('data-email'), boxId: $('#selecteBoxId').val() },
                success: function (data) {
                    $(entry).remove();
                },
                error: function (error) {
                    $('#spanFriendListMessage').text(error);
                }
            });
            var boxName = $('#selectedBoxName').text()
            var contactName = $(this).siblings('span').text();
            RemoveMember(contactName, boxName, request);

            return false;
        });
    };

    function RemoveMember(contactName, boxName, request) {
        Zbox.ShowConfirmDialog({
            title: '<img class="float-left icon-popupAlert" src="/Content/Images/icon-popupAlert.png" /><div class="style80 float-left">Remove Member</div><br class="clear" />',
            message: '<div class="style81">You are about to remove ' + contactName + ' from the memebers of the ' + boxName + ' box.</div>' +
                              '<div class="style81">Are you sure?</div>',
            ok: function () {
                request.Post();
                return true;
            }
        });
    }

    function PasswordShowClick(checkBox) {
        Zbox.PasswordShow($('#dialog-share-Box #checkPassword'), $('div#dialog-share-Box .password:last'), $('div#dialog-share-Box .password:first'), 'inline');
    }
</script>
<div id="divFriends">
    <div id="4" class="FriendSortable">
        <span class="MoveFriends rounded style33 ">Move to Manager </span>
        <label class="txt-11 blue-3 uppernoDecoration">
            Manager</label>
    </div>
    <div id="2" class="FriendSortable">
        <span class="MoveFriends rounded style33 ">Move to Read & Write</span>
        <label class="txt-11 blue-3 uppernoDecoration">
            Read & Write
        </label>
    </div>
    <div id="1" class="FriendSortable">
        <span class="MoveFriends rounded style33 ">Move to Read </span>
        <label class="txt-11 blue-3 uppernoDecoration">
            Read
        </label>
    </div>    
</div>
