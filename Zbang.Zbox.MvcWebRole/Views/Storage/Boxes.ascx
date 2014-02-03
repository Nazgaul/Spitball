<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%/*%><script src="~/Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script><%*/%>
<script type="text/javascript">

    $(document).ready(function () {
        registerBoxesForEvents();
        registerSubscribedBoxesForEvents();

        loadBoxes(function () { GenerateHash() });
        Zbox.LoadUserStatus();
        if (!Modernizr.history) {
            $(window).bind('hashchange', function (e) {
                var hashBoxID = $.param.fragment();
                loadBox(hashBoxID);
            });
        }

        $('#dialog-delete-Box').bind('dialogopen', function (event) {
            //add css to button class dynamic
            //send
            $('.ui-dialog-buttonset > button:first').addClass('style45');
            $('.ui-dialog-buttonset > button:first').addClass('dialogInputSend');
            $('.ui-dialog-buttonset > button:first').addClass('dialogInputSend :hover');

            //cancel
            //$('.ui-dialog-buttonset > button:last').addClass('style33Dialog');

        });
    });

    function GenerateHash() {
        var hashBoxID = jQuery.param.fragment();
        if (hashBoxID == '') {
            hashBoxID = $('#selecteBoxId').val();
        }
        if (hashBoxID != '' && $('#boxEntry' + hashBoxID).length > 0) {
            window.location.hash = hashBoxID;
            $(window).trigger('hashchange');
        }
        else if ($('div.boxEntry').length > 0) {
            hashBoxID = $('div.boxEntry:first').attr('data-BoxID');
            window.location.hash = hashBoxID;
            $(window).trigger('hashchange');
        }
        else {
            DisableButtons();
        }
        if (Modernizr.history) {
            window.history.pushState('', document.title, '/' + hashBoxID);
            loadBox(hashBoxID);
        }
    }


    function loadBox(hashBoxId) {
        var boxEntry = $('#boxEntry' + hashBoxId);

        if (boxEntry.length > 0) {
            OnBoxEntryClicked(boxEntry, function () {
                ChangeBox(Zbox.Box.boxes.GetBox(hashBoxId));
            });

        }
    }

    function loadBoxes(OnBoxLoadingComplete) {
        var loadBoxesRequest = new ZboxAjaxRequest({
            beforeSend: function () {
                $('#divBoxes').text("(Loading...)");
            },
            url: "/Box/GetBoxes",
            success: function (boxes) {
                $('#divBoxes').empty();
                $('#divSubscribedBoxes').empty();
                Zbox.Box.AddBoxes(boxes);
                OnBoxLoadingComplete();

            },
            error: function (error) {
                $('#spanBoxListMessage').text('error: ' + error);
            }
        });

        loadBoxesRequest.Get();
    }


    function loadBoxSubscribersAndInvitations(boxId, permission) {
        var requestUrl = '/Collaboration/GetBoxSubscribersAndInvitations?boxId=' + boxId;
        var request = new ZboxAjaxRequest({
            url: requestUrl,
            success: function (response) {
              loadBoxSubscribersAndInvitationsSuccess(response);
            },
            error: function (error) {
                $('#spanBoxListMessage').text('error: ' + error);
            }

        });

        request.Get();
    }

    function loadBoxSubscribersAndInvitationsSuccess(response) {
       var permission =  Zbox.Box.boxes.GetCurrentBox().UserPermission;
          Zbox.ClearSubscribersAndInvitationsFromFriendList();
                Zbox.RenderInvitationsInFriendList(response.Invitations);
                Zbox.RenderSubscribersInFriendList(response.Subscribers, permission);
                RenderDragAndDrop(); // on Friends.ascx
    }

    function registerBoxesForEvents() {
        $('div.boxEntry:not(.selected-box-entry)').live('mouseenter', function (event) {
            $(this).addClass('hovered-box-entry');
        });

        $('div.boxEntry:not(.selected-box-entry)').live('mouseleave', function (event) {
            $(this).removeClass('hovered-box-entry');
        });

        //we are using url instead of hash
        $('div.boxEntry:not(.selected-box-entry .invited)').live('click', function (e) {
            if (Modernizr.history) {
                window.history.pushState('', '', '/' + $(this).attr('data-BoxID'));
                loadBox($(this).attr('data-BoxID'));
            }
            else {
                window.location.hash = $(this).attr('data-BoxID');
            }
        });

        //invited to box
        $('div.boxEntry.invited').live('click', function (e) {
            var boxuid = $(this).attr('data-BoxID');
            Zbox.ShowConfirmDialog({
                title: '<img class="float-left icon-popupAlert" src="/Content/Images/icon-popupAlert.png" /><div class="style80 float-left">Invite to box</div><br class="clear" />',
                message: '<div class="style81">You need to subscribe to this box</div>' +
                              '<div class="style81">Are you sure?</div>',
                ok: function () {
                    //Temp solution
                    window.location.href = '/sharedbox/' + boxuid;
                    return true;
                }
            });
        });

        //we dont want to reoist on our selectted box
        $('div.boxEntry.selected-box-entry').live('click', function (event) {
            return false;
        });


        //rename
        $('#divBoxes > div.boxEntry.selected-box-entry span').live('click', function (event) {
            //<input class="boxName" type="text" value="{BoxName}" />

            var spanToReplace = $(this); //.children('span:visible');            
            var input = $('<input />').attr({
                type: 'text',
                'class': 'boxName', value: spanToReplace.text()
            });

            input.insertAfter(spanToReplace).show().focus();
            
            //spanToReplace.next().show();
            //spanToReplace.next().focus();
            spanToReplace.hide();
            return false;

        });

        //change box name
        $('div.boxEntry.selected-box-entry input:visible').live('focusout', function (e) {
            ChangeBoxName(this);
        });

        function ChangeBoxName(e) {
            var input = $(e);
            var boxId = input.parent().attr('data-boxid');
            //var boxId = Zbox.Box.boxes.GetBox(boxId)//input.parent().find('input:hidden').val();
            var newText = input.val();
            var oldText = input.siblings('span').text();
            if (newText.toString() != oldText.toString()) {
                Zbox.Box.ChangeBoxName(boxId, newText,
                     function () { ChangeBoxSuccess(input, newText) },
                     function () { ChangeBoxError(input) },
                     function () { });
            }
            else {
                ChangeBoxError(input);
            }
        }


        $('div.boxEntry').live('mouseenter', function (event) {
            $('.deleteListIcon', this).show();
        });
        $('div.boxEntry').live('mouseleave', function (event) {
            $('.deleteListIcon', this).hide();
        });


        //Delete Ownded box
        $('#divBoxes > div.boxEntry .deleteListIcon').live('click', function (event) {
            // we dont need to choose a box
            event.stopPropagation();
            event.preventDefault();

            var boxId = $(this).parent().attr('data-boxid');
            //var boxId = $(this).siblings('input:hidden').val();
            var boxName = $(this).siblings('span.boxName');
            var entry = $(this).parent();
            $('#dialog-delete-Box #boxName').text(boxName.text());

            $('#dialog-delete-Box').dialog({
                title: '<img class="float-left icon-popupAlert" src="/Content/Images/icon-popupAlert.png" /><div class="style80 float-left">Delete box</div><br class="clear" />',
                width: 'auto',
                height: 'auto',
                modal: true,
                resizable: false,
                buttons: {
                    Delete: function () {
                        Zbox.Box.DeleteBox(boxId,
                        function () { deleteBoxOnSuccess(entry, boxId) },
                        function () {//OnError
                            $('#spanBoxListMessage').text('error deleting box: ' + error)
                        },
                        function () { //BeforeSend
                            boxName.text('(Deleting...)');
                            $("#dialog-delete-Box").dialog('close');
                        });
                    },

                }
            });
        });
        $('#divSubscribedBoxes > div.boxEntry .deleteListIcon').live('click', function (event) {
            event.stopPropagation();
            event.preventDefault();

            var entry = $(this).parent();
            var boxId = $(this).parent().attr('data-boxid');

            var request = new ZboxAjaxRequest({
                url: '/Collaboration/DeleteOwnedSubscription',
                data: { boxid: boxId },
                success: function (data) {
                    deleteBoxOnSuccess(entry, boxId)
                },
                error: function (error) {
                    $('#spanBoxListMessage').text('error unsubscribe to box: ' + error)
                }
            });

            Zbox.ShowConfirmDialog({
                title: '<img class="float-left icon-popupAlert" src="/Content/Images/icon-popupAlert.png" /><div class="style80 float-left">Remove Member</div><br class="clear" />',
                message: '<div class="style81">You are about to unsubscibe from this box</div>' +
                              '<div class="style81">Are you sure?</div>',
                ok: function () {
                    request.Post();
                    return true;
                }
            });

            return false;



        });
    }

    function ChangeBoxSuccess(input, newText) {
        var spanText = input.parent().children('span');
        spanText.text(newText);
        input.remove();
        spanText.show();
        $('#selectedBoxName').text(newText);
    }
    function ChangeBoxError(input) {
        var spanText = input.parent().children('span');
        input.remove();
        spanText.show();
    }

    function deleteBoxOnSuccess(entry, boxId) {
        entry.remove();
        $('#divFiles').empty();
        if ($('div.boxEntry').length > 0) {
            if (boxId == $('#selecteBoxId').val()) {
                $('div.boxEntry:first').trigger('click');
            }
        }
        else {
            DisableButtons();
        }
    }

    function registerSubscribedBoxesForEvents() {
        $('div.subscribedBoxEntry:not(.selected-box-entry)').live('mouseover', function (event) {
            $(this).addClass('hovered-box-entry');
        });

        $('div.subscribedBoxEntry:not(.selected-box-entry)').live('mouseout', function () {
            $(this).removeClass('hovered-box-entry');
        });
    };

    function DisableButtons() {

        $('#btnShareBox').button('option', 'disabled', true);
        $('#downloadAllFiles').button('option', 'disabled', true);
        $('#eastPaneAccordion').accordion('option', 'disabled', true);
        resetData();
    }

    function resetData() {
        $('#selecteBoxId').val('');
        $('#selectedBoxIdPermission').val('');
        $('#selectedBoxName').text('');
        $('#privacy-settings-header').text('');
        $('#notification-settings-header').text('');
        $('#box-comments').remove();

    }
    function EnableButtons() {

        $('#btnShareBox').button('option', 'disabled', false);
        $('#downloadAllFiles').button('option', 'disabled', false);
        $('#eastPaneAccordion').accordion('option', 'disabled', false);
    }

    function OnBoxEntryClicked(boxElement, doChange) {
       
        $('div.selected-box-entry').removeClass('selected-box-entry');
        boxElement.removeClass('hovered-box-entry');
        boxElement.addClass('selected-box-entry');
        EnableButtons();
        doChange();
       

    }

    function ChangeBox(box) {
        var $selectedBoxSUID = $('#selectedBoxSUID');
        $('#selecteBoxId').val(box.BoxId);
        $selectedBoxSUID.val(box.shortUid);
        $('#selectedBoxIdPermission').val(box.UserPermission);
        $('#selectedBoxName').text(box.BoxName);
        $('#ownerLabel').text(box.BoxOwner);

        $('#selecteBoxId').change();
       // $selectedBoxSUID.
        $selectedBoxSUID.change();
        $('#selectedBoxIdPermission').change();        
        UpdateZboxData(box);

    }

    function UpdateZboxData(box){
        var $boxComments = $('#box-comments');
        var $divFiles = $('#divFiles');

        

        var loadBoxData = new ZboxAjaxRequest({
        beforeSend: function () {
            $divFiles.empty();
            
            //remove the reply to back to the templates.
            if (!$.contains(document.getElementById('templates'),document.getElementById('CommentReplyBubble'))) {
                $('#CommentReplyBubble').appendTo($('#templates'));
            }
            

            $boxComments.empty();
            $boxComments.text('[Loading box conversations...]');
            

        },
        url: "Box/GetBoxData",
        data: {boxId: box.shortUid},
        success: function (response) {
            Zbox.BoxItem.loadBoxFilesSucess(response.boxItem);
            
            loadBoxConversationSuccess(response.boxComment);

            loadBoxSubscribersAndInvitationsSuccess(response.boxUser);
        },
        error: function (error) {
            
        }
        });

        loadBoxData.Get();
       // Zbox.BoxItem.loadBoxFiles(box.BoxId, 0, box.UserPermission); //defined at Zbox-BoxItem.js
        //loadBoxConversations(box.BoxId);//defined at Zbox-Comments.js
        //loadBoxSubscribersAndInvitations(box.BoxId, box.UserPermission);
    }
</script>
<div id="boxes-layout">
    <div id="divBoxes">
    </div>
    <div id="subscribed-boxes-container">
        <div class="green txt-11 subscribed-boxes-title">
            SUBSCRIBED BOXES</div>
        <div id="divSubscribedBoxes">
        </div>
    </div>
    <div id="Invited-boxes-container">
        <div class="green txt-11 subscribed-boxes-title">
            INVITED BOXES</div>
        <div id="divInvitedBoxes">
        </div>
    </div>
</div>
<div id="spanBoxListMessage">
</div>
<input type="hidden" id="selecteBoxId" value="<%:  ViewData["boxid"] ?? 0 %>" />
<input type="hidden" id="selectedBoxSUID" value="<%:  ViewData["boxid"] ?? 0 %>"
    data-isloading="false" />
<input type="hidden" id="selectedBoxIdPermission" value="4" />
