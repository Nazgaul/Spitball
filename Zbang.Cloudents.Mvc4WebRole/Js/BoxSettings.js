(function (cd, dataContext, $, pubsub, ZboxResources, analytics) {
    if (window.scriptLoaded.isLoaded('bs')) {
        return;
    }

    "use strict";
    var boxSettings, boxSettingsData = {}, members = [], settingsFirstTime = true, membersFirstTime = true,
        eById = document.getElementById.bind(document), CDISABLED = 'disabled', MINHEIGHT = 420, MAXHEIGHT = 600;
    pubsub.subscribe('boxSettings', function (data) {
        if (!boxSettings) {
            boxSettings = eById('box_settings');
            registerEvents();
        }
        setMembersTab(data.settings.boxUid); //we have cache in ajax

        boxSettingsData = data.settings;
        eById('boxSettingName').textContent = boxSettingsData.name; // firefox doenst support innertext

        setCurrentTab(data.tabName);
        setSettingsTab();
        changeViewBoxSettings(true);
    });

    function Member(data) {
        var _self = this;
        _self.id = data.uid;
        _self.name = data.name;
        _self.image = data.image || $('body').data('pic');
        _self.status = parseStatus(data.sUserStatus);
        _self.cMemStat = data.sUserStatus === 'Invite' ? 'memberPending' : '';
        _self.identifier = cd.guid();
        _self.tooltipClass = data.sUserStatus === 'Subscribe' || data.sUserStatus === 'Owner' ? ' calloutTrgr' : ''; //space is needed        
        _self.url = data.url + '?r=box&s=members';
    }

    var statusData;
    function parseStatus(status) {
        statusData = statusData || boxSettings.getElementsByClassName('membersList')[0];
        if (status === 'Subscribe' || status === 'Owner') {
            return statusData.getAttribute('data-member');
        } else if (status === 'Invite') {
            return statusData.getAttribute('data-pending');
        }

    }

    function setCurrentTab(tabName) {

        var popupClassList = boxSettings.querySelector('.popupTopBar').classList,
            boxType = boxSettingsData.boxType.toLowerCase();
        tabName = 'tab' + tabName;

        var boxSettingsContentCL = boxSettings.getElementsByClassName('boxSettingsContent')[0].classList;
        boxSettingsContentCL.remove('private');
        boxSettingsContentCL.remove('academic');
        boxSettingsContentCL.add(boxType);
        if (popupClassList.contains(tabName)) {
            return;
        }
        popupClassList.remove('tabSettings');
        popupClassList.remove('tabMembers');
        popupClassList.add(tabName);
        calculatePopupHeight();

        setScroll();
    }

    var privacySettingsContent;
    function setSettingsTab() {
        var privacySettings = eById('privacySettings');
        privacySettingsContent = privacySettingsContent || privacySettings.innerHTML;
        boxSettings.querySelector('.thumbImg img').src = boxSettingsData.image;
        eById('boxSettingsName').value = boxSettingsData.name;
        if (boxSettingsData.boxType === 'academic') {
            eById('boxSettingsCourse').value = boxSettingsData.courseNumber || '';
            eById('boxSettingsProfessor').value = boxSettingsData.professor || '';
            privacySettings.innerHTML = '';
        } else {
            boxSettings.getElementsByClassName('ownerName')[0].textContent = boxSettingsData.ownerName || '';
            if (boxSettingsData.userType === 'owner') {
                if (!privacySettings.innerHTML) {
                    eById('privacySettings').insertAdjacentHTML('afterbegin', privacySettingsContent);
                }
                $('#privacy_' + boxSettingsData.privacy).prop('checked', true);
            }
            else {
                privacySettings.innerHTML = '';
            }
        }

        if (!boxSettingsData.willDelete()) {
            eById('boxSettingsDelete').textContent = ZboxResources.Unfollow;
        }
        else {
            eById('boxSettingsDelete').textContent = ZboxResources.Delete;
        }
        dataContext.getNotification({
            data: { boxUid: boxSettingsData.boxUid },
            success: function (data) {
                $('#noti_' + data).prop('checked', true);
                boxSettingsData.notify = data;
            }
        });
        if (settingsFirstTime) {
            settingsFirstTime = false;
            registerEvents();
        }

        function registerEvents() {
            eById('boxSettingsForm').onsubmit = function (e) {
                var self = this, submitBtn = self.querySelector('input[type=submit]');
                e.preventDefault();
                var data = $(this).serializeArray();
                data.push({ name: 'BoxUid', value: boxSettingsData.boxUid });
                changeViewBoxSettings(false, true);

                pubsub.publish('updateBoxInfo', {
                    name: eById('boxSettingsName').value,
                    courseId: eById('boxSettingsCourse').value,
                    professor: eById('boxSettingsProfessor').value,
                    privacy: $(self).find('input[name="BoxPrivacy"]:checked').val()
                });
                //submitBtn.setAttribute(CDISABLED, CDISABLED);
                dataContext.updateBoxInfo({
                    data: data,
                    success: function (data) {
                        pubsub.publish('updateBoxUrl', data.queryString);

                    }
                    //error: function (e) {
                    //    cd.displayErrors(self, e);
                    //},
                    //always: function () {
                    //    submitBtn.removeAttribute(CDISABLED);
                    //}
                });
            };

            eById('boxSettingsCancel').onclick = function (e) {
                var selectedObj = {}, changed = false;
                //check if anything changed and prompt user
                selectedObj.name = eById('boxSettingsName').value;
                selectedObj.notify = boxSettings.querySelector('.radioBtns input:checked').value;

                if (boxSettingsData.boxType === 'private') {
                    if (boxSettingsData.userType === 'owner') {
                        selectedObj.privacy = eById('privacySettings').querySelector('input:checked').value;
                    }
                } else {
                    selectedObj.courseNumber = eById('boxSettingsCourse').value;
                    selectedObj.professor = eById('boxSettingsProfessor').value;
                }
                for (var input in selectedObj) {
                    if (selectedObj[input] === "") {
                        selectedObj[input] = undefined;
                    }

                    if (selectedObj[input] !== boxSettingsData[input]) {
                        changed = true;
                    }
                }
                if (!changed) {
                    changeViewBoxSettings(false, true);
                    return;
                }
                cd.confirm('Changes will be discarded, are you sure you want to cancel?',
                    function () {
                        changeViewBoxSettings(false, true);
                    },
                    function () {
                        e.preventDefault();
                    }
                );
            };

            eById('boxSettingsDelete').onclick = function () {
                var sentence = '';
                switch (boxSettingsData.willDelete()) {
                    case 1:
                        sentence = ZboxResources.SureYouWant + ZboxResources.ToDeleteBox;
                        break;
                    case 2:
                        sentence = 'You have created an empty course, if you unfollow this course it will be deleted. Do you want to delete the course?';
                        break;
                    default:
                        sentence = ZboxResources.SureYouWant + ZboxResources.ToLeaveGroup;
                }
                if (!confirm(sentence)) {
                    return;
                }
                var that = this;
                that.setAttribute(CDISABLED, CDISABLED);
                analytics.trackEvent('Unfollow/Delete', 'click in box settings');
                dataContext.removeBox2({
                    data: { boxUid: boxSettingsData.boxUid },
                    success: function () {
                        changeViewBoxSettings(false, true);
                        that.removeAttribute(CDISABLED);
                        cd.sessionStorageWrapper.clear();
                        cd.pubsub.publish('nav', '/');

                    }
                });
            };

            eById('closeBoxSettings').onclick = function () {
                changeViewBoxSettings(false, true);
            };

        }
    }

    function setMembersTab(boxid) {
        var membersQuery;
        var pendingMembers = [];
        dataContext.boxMembers({
            data: { BoxUid: boxid },
            success: function (data) {
                membersQuery = data
                members = [];
                var membersCount = boxSettings.querySelector('.membersCount');
                membersCount.textContent = membersQuery.length + ' ' + membersCount.getAttribute('data-label');
                for (var i = 0, l = membersQuery.length; i < l; i++) {
                    members.push(new Member(membersQuery[i]));
                }
                members.sort(function (a, b) {
                    return cd.sortMembersByName(a.name, b.name);
                });

                members.filter(function (x) {
                    if (x.status === 'Pending') {
                        pendingMembers.push(x);
                        return true;
                    }
                    return false;
                });

                if (pendingMembers.length > 0) {
                    for (var i = 0, l = pendingMembers.length; i < l; i++) {
                        members.splice(members.indexOf(pendingMembers[i]), 1);
                    }

                    members = pendingMembers.concat(members);
                }

                appendMembers(members);
            }
        });
        if (membersFirstTime) {
            membersFirstTime = false;
            registerEvents();
        }

        var membersList;
        function appendMembers(memberToShow) {
            membersList = membersList || boxSettings.getElementsByClassName('membersList')[0];

            cd.appendData(membersList, 'memberItemTemplate', memberToShow, 'beforeend', true);
            calculatePopupHeight();
            setScroll();

            $('#member' + cd.userDetail().id).find('.inviteUserBtn,.removeUserBtn').remove(); //user who show the box cant invite himself or remove himself
            if (boxSettingsData.userType !== 'owner') {
                $(membersList).find('.removeUserBtn').remove(); //only owner can remove users
            }
            for (var i = 0, l = memberToShow.length; i < l; i++) {
                $.data(eById(memberToShow[i].identifier), memberToShow[i]);
            }
        }

        function searchMembers(value) {
            var foundList = [];

            for (var i = 0, l = members.length; i < l ; i++) {
                if (members[i].name.toLowerCase().indexOf(value.toLowerCase()) > -1) {
                    foundList.push(members[i]);
                }
            }
            foundList.sort(function (a, b) {
                return cd.sortMembersByName(a.name, b.name, value);
            });
            appendMembers(foundList);
        }

        function getListItem(target) {
            while (target.nodeName !== 'LI') {
                target = target.parentElement;
            }
            return target;
        }

        function inviteMembers(members) {
            changeViewBoxSettings(false, false);
            pubsub.publish('messageFromPopup', { id: boxSettings.id, data: members });
        }

        function removeMember(memberid, elem) {
            var p = cd.confirm2(JsResources.DeleteUser);
            p.done(function () {
                elem.setAttribute(CDISABLED, CDISABLED);
                dataContext.removeUser({
                    data: { boxUid: boxSettingsData.boxUid, userId: memberid },
                    error: function () {
                        window.clearTimeout(interval);
                        $('#member' + memberid).show().removeClass('uninvited');
                        elem.removeAttribute(CDISABLED);

                    }
                });
                var membersLengthElement = boxSettings.querySelector('.membersCount'),
                          membersLength = parseInt(membersLengthElement.textContent, 10);

                membersLengthElement.textContent = (membersLength - 1) + ' ' + membersLengthElement.getAttribute('data-label');;

                var uninvitedText = boxSettings.getElementsByClassName('membersList')[0].getAttribute('data-uninvited');
                $('#member' + memberid).find('.memberStatus').append('<span class="uninvite">' + uninvitedText + '</span>');
                window.setTimeout(function () {
                    $('#member' + memberid).addClass('uninvited');
                    // $('#member' + memberid).remove();
                }, 10);

                var interval = window.setTimeout(function () {
                    $('#member' + memberid).remove();
                    setScroll();
                    calculatePopupHeight();
                }, 3000);
            });

        };

        function reinviteMember(memberid, elem) {
            elem.setAttribute(CDISABLED, CDISABLED);
            dataContext.inviteBox({
                data: { Recepients: memberid, BoxUid: boxSettingsData.boxUid }
            });

            var reinvitedText = boxSettings.getElementsByClassName('membersList')[0].getAttribute('data-reinvited');
            $('#member' + memberid)
            .find('.memberStatus').append('<span class="reinvite">' + reinvitedText + '</span>');
            window.setTimeout(function () {
                $('#member' + memberid).addClass('reinvited');

            }, 10);

        }


        function registerEvents() {
            var sendAMsgBtm = eById('mbrSetingsSndMsg');
            eById('membersSettings').addEventListener('change', function () {
                var checkboxes = boxSettings.getElementsByClassName('checkbox'),
                    state = this.checked;
                for (var i = 0, l = checkboxes.length; i < l; i++) {
                    checkboxes[i].checked = state;
                }
                if (state) {
                    $(checkboxes).trigger('change');
                }
            }, true);

            $(boxSettings).on('change', '.checkbox', function (e) {
                var checkboxes = boxSettings.querySelectorAll('.memberItem input:checked');
                if (checkboxes.length) {
                    sendAMsgBtm.style.display = 'inline-block';
                    return;
                }
                sendAMsgBtm.style.display = 'none';

            });

            sendAMsgBtm.onclick = function () {
                var checkboxes = boxSettings.querySelectorAll('.memberItem input:checked'), memberItem,
                    selectedMembers = [];
                if (!checkboxes.length) {
                    return;
                }
                for (var i = 0, l = checkboxes.length; i < l; i++) {
                    memberItem = $.data(eById(checkboxes[i].id));
                    selectedMembers.push({ id: memberItem.id, name: memberItem.name, userImage: memberItem.image });

                }
                inviteMembers(selectedMembers);

            };

            eById('membersSearch').onkeyup = function () {
                searchMembers(this.value);
            };

            //member actions

            var memberActionsList = boxSettings.getElementsByClassName('memberActions'),
                guid, member;
            $(boxSettings).on('click', '.memberActions', function (e) {
                /// <summary></summary>
                /// <param name="e" type="HTMLElement"></param>
                if (!e.target) {
                    return;
                }

                guid = getListItem(e.target).firstElementChild.id;
                member = $.data(eById(guid));
                var classList = e.target.classList;

                if (classList.contains('inviteUserBtn')) {
                    inviteMembers([{ id: member.id, name: member.name, userImage: member.image }]);
                } else if (classList.contains('removeUserBtn')) {
                    removeMember(member.id, e.target);
                }
                else if (classList.contains('reinviteUserBtn')) {
                    reinviteMember(member.id, e.target);
                }
            });
        }

    }

    function clearSettings(clearInput) {
        var checkboxes = document.getElementsByClassName('checkbox');
        for (var i = 0, l = checkboxes.length; i < l; i++) {
            checkboxes[i].checked = false;
        }
        eById('mbrSetingsSndMsg').style.display = 'none';

        if (clearInput) {
            eById('membersSearch').value = '';
        }

    }

    function changeViewBoxSettings(isShow, clearInput) {
        if (isShow) {
            boxSettings.style.display = 'block';
            return;
        }
        boxSettings.style.display = 'none';
        clearSettings(clearInput);
    }

    function registerEvents() {

        boxSettings.getElementsByClassName('popupTopBar')[0].onclick = function (e) {
            if (!e.target) {
                return;
            }
            if (e.target.classList.contains('tabBtn')) {
                setCurrentTab(e.target.getAttribute('data-id'), boxSettingsData.boxType, boxSettingsData.userType);

            }
        };

        pubsub.subscribe('boxclear', function () {
            changeViewBoxSettings(false, true);
            clearSettings();

        });
    }


    function setScroll() {
        var $popup = $('#box_settings'),
            $scrollElem = $('#membersListBox'),
            maxHeight = parseInt($scrollElem.css('max-height'), 10);

        if (!$popup.is(':visible')) {
            return;
        }

        var margin = $popup.find('.popupHeader').height() +
                    $popup.find('.popupTopBar').outerHeight() +
                    $popup.find('.membersTop').outerHeight(),

            height = $popup.find('.popupWrppr').height() - margin;

        if (maxHeight >= height) {
            $scrollElem.height(height);
            cd.innerScroll($scrollElem, height+10);
            if (Modernizr.touch) {
                return;//no need to continue for touch devices
            }

            //fix scroll position
            var scrollDirection = window.getComputedStyle(document.getElementsByTagName('html')[0], null).getPropertyValue('direction') == 'ltr' ? 'right' : 'left',//rtl
                scrollBar = boxSettings.querySelector('.membersContent .slimScrollBar'),
                scrollDiv = boxSettings.querySelector('.membersContent .slimScrollDiv'),
                scrollRail = boxSettings.querySelector('.membersContent .slimScrollRail');

            scrollDiv.style.overflow = 'visible';
            //scrollBar.style.removeProperty('right'); scrollBar.style.removeProperty('left'); //clean direction
            scrollBar.style[scrollDirection] = '-12px';
            scrollRail.style[scrollDirection] = '-12px';


        }
    }

    cd.pubsub.subscribe('windowChanged', function () {
        var $popup = $('#box_settings .boxSettings');
        if ($popup.length && $popup.is(':visible')) {
            calculatePopupHeight();
            setScroll();
        }

    });

    function calculatePopupHeight() {
        var $popup = $('#box_settings .boxSettings'),
            $scrollElem = $('#membersListBox'),
            topBar = boxSettings.querySelector('.popupTopBar');

        if (!($popup.length && $popup.is(':visible'))) {
            return;
        }
        $scrollElem.css('height', '');
        $popup.css({ height: '', top: '' });

        if (boxSettings.querySelector('.slimScrollDiv')) {
            $scrollElem.slimScroll({ 'destroy': '' });
        }

        if (!topBar.classList.contains('tabMembers')) {
            return;
        }



        var popupHeight = $popup.outerHeight(),
            windowHeight = window.innerHeight;

        if (popupHeight < MINHEIGHT) {
            $popup.height(MINHEIGHT);
            popupHeight = MINHEIGHT;
        }
        if (popupHeight > MAXHEIGHT) {
            $popup.height(MAXHEIGHT);
            popupHeight = MAXHEIGHT;
        }
        if (popupHeight > windowHeight) {
            $popup.height(windowHeight);
            popupHeight = windowHeight;
        }
        if (popupHeight + $popup.offset().top > windowHeight) {
            var top = (windowHeight - $popup.outerHeight()) / 2;
            $popup.css('top', top > 0 ? top : 0);
        }

    }
}(cd, cd.data, jQuery, cd.pubsub, ZboxResources, cd.analytics));