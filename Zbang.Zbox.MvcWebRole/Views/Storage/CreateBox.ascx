<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Zbang.Zbox.Infrastructure.Enums" %>
<%/*%><script src="~/Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script><%*/%>
<script type="text/javascript">

    $(document).ready(function () {
        var passwordProtected = parseInt('<%: (int)BoxPrivacySettings.PasswordProtected %>');
        var notificationSettingOn = parseInt('<%: (int)NotificationSettings.On %>');
        var notificationSettingOff = parseInt('<%: (int)NotificationSettings.Off %>');
        var notShared = parseInt('<%: (int)BoxPrivacySettings.NotShared %>');
        var membersOnly = parseInt('<%: (int)BoxPrivacySettings.InvitationOnly %>');

        $('#dialog-CreateBoxcontent').find('#myCheck2').bind('click', function (event) {
            Zbox.PasswordShow($(this), $('#dialog-CreateBoxcontent').find('#Text1'), $('#dialog-CreateBoxcontent').find('#password2'), 'inline-block');
        });

        $('#btnAddNewBox').click(function (event) {

            $('#dialog-create-Box').bind('dialogopen', function (event) {
                //add css to button class dynamic
                //send
                $('#dialog-create-Box').find('.ui-dialog-buttonset > button:first').addClass('style45');
                $('#dialog-create-Box').find('.ui-dialog-buttonset > button:first').addClass('dialogInputSend');
                $('#dialog-create-Box').find('.ui-dialog-buttonset > button:first').addClass('dialogInputSend :hover');

                //cancel
                $('#dialog-create-Box').find('.ui-dialog-buttonset > button:last').addClass('style33Dialog');
            });

            $("#boxName").keyup(function (event) {
                if (event.keyCode == 13) {
                    $('#dialog-create-Box').find('.ui-dialog-buttonset > button:first').click();
                }
            });


            var boxCreating = false;
            $('#dialog-create-Box').dialog({
                title: '<img class="float-left popIcon" src="/Content/Images/cube.png" /><div class="style80 float-left">Create a New box</div><br class="clear" />',
                width: 'auto',
                height: 'auto',
                open: function (event, ui) {
                    $('#dialog-create-Box #Notification').selectbox();
                    $('#dialog-create-Box #Privacy').selectbox();
                    $('#dialog-create-Box #Privacy').bind('change', function (event) {
                        if ($('#dialog-create-Box #Privacy').val() == passwordProtected) {
                            $('#dialog-create-Box #divPassword').css('display', 'block');
                        }
                        else
                            $('#dialog-create-Box #divPassword').css('display', 'none');
                    });
                },
                close: function (event, ui) {
                    $('#dialog-create-Box #Privacy').parents('.jquery-selectbox').unselectbox();
                    $('#dialog-create-Box #Privacy').unbind('change');

                    $('#dialog-create-Box #boxName').val('');
                    $('#spanCreateBoxMessage').text('');
                    $('#dialog-create-Box #Privacy').val(membersOnly);
                    $('#dialog-create-Box #divPassword').css('display', 'none');
                },
                modal: true,
                resizable: false,
                buttons: {
                    Create: function () {
                        if (!boxCreating) {
                            boxCreating = true;
                            var newBoxName = $('#dialog-create-Box #boxName').val();
                            if (newBoxName == "") {
                                $('#dialog-create-Box #spanCreateBoxMessage').text("Box name is empty");
                                return false;
                            }

                            if (IsNewBoxNameExists(newBoxName)) {
                                $('#dialog-create-Box #spanCreateBoxMessage').text("Box name exists");
                                return false;
                            }

                            var newBoxPassword;
                            if ($("#dialog-create-Box #myCheck2").attr('checked')) {
                                newBoxPassword = $('#dialog-create-Box #Text1');
                            }
                            else {
                                newBoxPassword = $('#dialog-create-Box #password2');
                            }

                            if ($('#dialog-create-Box #divPassword').css('display') == 'block') { //checking password
                                if (newBoxPassword.val() == null || newBoxPassword.val() == '' || newBoxPassword.val().replace(' ', '') == '') {
                                    $('#dialog-create-Box #spanCreateBoxMessage').text('you need to fill a password');
                                    return false;
                                }
                            }
                            var notificationSettings = 2;
                            if ($('#dialog-create-Box #myCheck').attr('checked')) {
                                notificationSettings = notificationSettingOn;
                            }
                            else {
                                notificationSettings = notificationSettingOff;
                            }

                            var privacySettings = $('#dialog-create-Box #Privacy').val();
                            $('#dialog-create-Box #spanCreateBoxMessage').text('');

                            var newBoxRequest = new ZboxAjaxRequest({
                                beforeSend: function () {
                                    $('#dialog-create-Box #spanCreateBoxMessage').text('Creating box...');
                                },
                                url: '/Box/CreateBox',
                                data: { newBoxName: newBoxName, notificationSettings: notificationSettings, privacySettings: privacySettings, privacyPassword: newBoxPassword.val() },
                                success: function (box) {
                                    Zbox.Box.AddBox(box);
                                    Zbox.toaster('Box created');
                                    $('#boxEntry' + box.shortUid).trigger('click');

                                    return true;

                                },
                                error: function (error) {
                                    $('#spanCreateBoxMessage').text("Error creating box:" + error);
                                },
                                complete: function (text) {
                                    $('#dialog-create-Box #spanCreateBoxMessage').text('');
                                    $('#dialog-create-Box #Privacy').val(membersOnly)
                                    $("#dialog-create-Box").dialog('close');

                                }
                            });
                            newBoxRequest.Post();
                        }
                    },
                    Cancel: function () {
                        $("#dialog-create-Box").dialog('close');
                    }

                }
            });
        });
    });



    function IsNewBoxNameExists(newBoxName) {
        /// <summary>
        /// Checks if Box name already exists in Boxes list.    
        /// </summary>    

        var exists = false;
        var lowerBoxName = newBoxName.toLowerCase();
        var spans = $('#divBoxes').find('span.boxName')
        for (var i = 0; i < spans.length; i++) {
            if (lowerBoxName == $.trim($(spans[i]).text().toLowerCase())) {
                exists = true;
                break;
            }
        }
        return exists;
    }
</script>
<div id="createBoxHeader">
    <div class="spaceLeft">
        <span id="spanQuota" class="black333 txt-13"></span>
    </div>
</div>
<div class="clear">
</div>
<div class="bluish-dark txt-11 subscribed-boxes-title" id="boxes-title">
    <span>YOUR BOXES</span>
    <%--  <div class="borderGray" style="margin: 2px 0 0 0">
    </div>--%>
    <div id="addNewBoxButtonContainer" class="rounded title-button aaastyle7 btnBlue">
        <div class="my-button">
            <a id="btnAddNewBox" class="txt-13 txt-shadow " href="#">New box</a></div>
    </div>
</div>
<div id="hiddenBoxCreateDialog" style="display: none;">
</div>
