(function (cd, dataContext) {
    if (window.scriptLoaded.isLoaded('sh')) {
        return;
    }

    var google, firstTime = true, loadedContacts = false,
    eById = document.getElementById.bind(document),
    fromPopup = {},
    messageDialog = $('#sendMsgDialog').dialog({
        submitCallBack: function (url, data, form) {
            cd.autocomplete2('attemptValidate');

            if (!form.find('[name="Recepients"]').length) {
                cd.displayErrors(form, JsResources.FieldRequired);
                return false;
            }
            var input = form.find('input[type="submit"]').attr('disabled', 'disabled');

            var tempArray = [];
            if (!form.valid || form.valid()) {
                var fdata = form.serializeArray();
                for (var i = 0, l = fdata.length - 1; i < l; i++) {
                    if (fdata[i].value.indexOf(',') > -1) {
                        tempArray = fdata[i].value.split(',');
                        fdata.splice(i, 1);
                        l--;
                    }
                }
                for (var i = 0, l = tempArray.length; i < l; i++) {
                    fdata.push({ name: 'Recepients', value: tempArray[i] });
                }

                dataContext.message({
                    data: fdata,
                    success: function (itemData) {
                        form[0].reset();
                        messageDialog.dialog('hide');
                        cd.autocomplete2('clear');
                    },
                    error: function (msg) {
                        cd.notification(msg);
                    },
                    always: function () {
                        input.removeAttr('disabled');
                        cd.autocomplete2('clear');
                    }
                });
            }
        },
        cancelCallBack: function () {
            cd.autocomplete2('clear');
            if (fromPopup.id) {
                eById(fromPopup.id).style.display = 'block'; //maybe we can do that better
            }
        }
    });

    cd.pubsub.subscribe('message', messageCallback);

    cd.pubsub.subscribe('messageFromPopup', messageCallback);

    function messageCallback(d) {
        fromPopup.id = d.id;
        fromPopup.data = d.data;
        openMessagePopup(d);
    }

    cd.pubsub.subscribe('gAuthSuccess', function () {
        if (firstTime) {
            return;
        }
        $('.gConnect').hide();
        cd.pubsub.publish('gGetContacts');
    });

    cd.pubsub.subscribe('gContacts', function (contacts) {
        if (!$('.popupWrppr.invite').is(':visible')) {
            return;
        }

        if (firstTime || loadedContacts) {
            return;
        }
        loadedContacts=true;

        cd.autocomplete2('insertData', contacts);
    });

    cd.pubsub.subscribe('gAuthFail', function () {
        if (firstTime) {
            return;
        }
        $('.gConnect').css('display', 'inline-block').click(function () {
            cd.google.register(false);
        });
    });

    function openMessagePopup(d) {
        cd.pubsub.publish('hide_members_dialog');
        if (firstTime) {
            cd.autocomplete2('init', { sInputElement: 'messageInput', sOutputElement: 'emailMenu', sSelectedList: 'emailList' });

            getCloudentsContacts();
            firstTime = false;
        }
        if (!(d.data && d.data.length === 1)) {// not loading google when sending a message to a single contact
            if (cd.google.connected) {
                cd.pubsub.publish('gGetContacts');
            } else {
                cd.google.register(true);
            }
        } 

        if (d.data) {
            cd.autocomplete2('addEmailsToList', d.data);
        }

        messageDialog.dialog('show');
        document.getElementById('messageInput').blur();
        cd.autocomplete2('setWidth', $('.emailUserWpr .emailUser').width());
        if (cd.google.connected) {
            cd.autocomplete2('calculateContainerWidth', false);
        }

    }

    function getCloudentsContacts() {
        dataContext.getFriends({
            success: function (friends) {
                friends = friends.my;
                if (!friends)
                    return;

                var list = [];
                for (var i = 0, l = friends.length; i < l; i++) {
                    var friend = friends[i];
                    list.push({ name: friend.name, userImage: friend.image, defaultImage: $('body').data('pic'), id: friend.uid });
                }

                cd.autocomplete2('insertData', list);
            }
        });
    }
}(cd, cd.data));