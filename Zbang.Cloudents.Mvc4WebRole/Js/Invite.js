(function (cd, dataContext, pubsub, $, analytics) {
    if (window.scriptLoaded.isLoaded('i')) {
        return;
    }

    var listContainer, invSources, inviteContentWrapper, inviteMain, inviteInput, emailsSelectedElement, searchByName,
        emailsSelecetd = [],  currentTab, currentTabName, statusChecked = false,
        eById = document.getElementById.bind(document),
        cloudentsContacts = [], facebookContacts = [], googleContacts = [],
        connectStatus = {}, boxid, loader, loading = false, fbInviteResponse, cdgInviteResponse,
         defaultTabName, connectHeader, facebookInviteUrl;

    cd.loadModel('invite', 'InviteContext', InviteViewModel);

    function InviteViewModel() {

        initInvite();

        //#region setup
        function initInvite() {

            listContainer = document.getElementsByClassName('invList')[0];
            invSources = document.getElementsByClassName('inviteSources ')[0];
            inviteContentWrapper = document.getElementsByClassName('invContentWpr')[0];
            inviteMain = document.getElementsByClassName('invMain')[0];
            emailsSelectedElement = eById('inviteEmailList');
            inviteInput = eById('inviteInput');
            searchByName = eById('inviteSearchByName');
            connectHeader = eById('inviteConnectMore');
            checkAccountStatus();
            registerEvents();
        }
        function setupInviteToBox(box) {
            if (!$.isEmptyObject(box)) {
                facebookInviteUrl = box.boxUrl;
                document.querySelector('.invHeaderText span').textContent = box.name;
                document.querySelector('.invHeaderText').href = box.boxUrl;
            } else {
                facebookInviteUrl = eById('inviteBoxName').href;
            }

            fbInviteResponse = fbBoxInvite;
            cdgInviteResponse = cdgBoxInvite;

            toogleCloudentsFriends(true);
        }
        function toogleCloudentsFriends(shouldAppear) {
            if (shouldAppear) {
                $(inviteMain).addClass('invBox');
            }
            else {
                $(inviteMain).removeClass('invBox');
            }

        }

        function setupInviteToCloudents() {
            toogleCloudentsFriends(false);
            fbInviteResponse = fbInvite;
            cdgInviteResponse = gInvite;
            facebookInviteUrl = window.location.origin + '/account/' + $('html').data('culture');
            document.querySelector('.invHeaderText span').textContent = '';
            document.querySelector('.invHeaderText').href = '/dashboard/';

        }
        function setCurrentTab(tabName) {

            var prevTab = invSources.querySelector('.invTab.current'),
                loadContacts = true;
            //searchByName.value = ''; // remove the search when we switch betten tabs
            currentTab = invSources.querySelector('.invTab.' + tabName);
            if (prevTab) {

                if ((/*prevTab.classList.contains('gPlus') ||
                    */ $(prevTab).hasClass('gmail'))
                    && (/*currentTab.classList.contains('gPlus')
                    || */$(currentTab).hasClass.contains('gmail'))) {
                    loadContacts = false;
                }

                $(prevTab).removeClass('current');
            }            

            $(inviteContantClassList).removeClass('cloudentsContent fbContent gPlusContent gmailContent');
            $(inviteContantClassList).addClass(tabName + 'Content');

            $(currentTab).addClass('current');

            cd.removeChildren(listContainer);
            changeState();
            loader = cd.renderLoading(inviteContentWrapper, 50);
            loading = true;
            if (tabName === 'cloudents') {
                getCloudentsContacts();
            } else if (tabName === 'fb') {
                if (!connectStatus.fb) {
                    loader();
                    loading = false;
                    changeState('notConnected');
                    return;
                }
                getFacebookContacts();

            } else if (tabName === 'gPlus' || tabName === 'gmail') {
                if (loadContacts) {
                    if (!connectStatus.google) {
                        loader();
                        loading = false;
                        changeState('notConnected');
                        return;
                    }
                    getGoogleContacts();
                }
                else {
                    setTimeout(function () {
                        getGoogleContacts();
                    }, Math.round(Math.random() * 500));
                }
            }


        }
        function changeState(state) {
            $(inviteContentWrapper).removeClass('notConnected empty');

            if (state) {
                $(inviteContentWrapper).addClass(state);
                if (loader) {
                    loader();
                    loading = false;
                }
            }
        }

        //#endregion

        //#region check connection and connect if needed
        cd.pubsub.subscribe('gAuthSuccess', function (isAuto) {
           // var gmailBtn = document.querySelector('.connectMore.gmail'),
           //     gPlusBtn = document.querySelector('.connectMore.gPlus');

            $(connectHeader).removeClass('gpNone gmailNone');
            connectStatus.google = true;
            if (!isAuto) {
                if (currentTabName === 'gmail') {
                    setCurrentTab('gmail');
                }
            }
        });

        cd.pubsub.subscribe('gAuthFail', function () {
            var gmailBtn = document.querySelector('.connectMore.gmail');
              //  gPlusBtn = document.querySelector('.connectMore.gPlus');

            $(connectHeader).addClass('gpNone gmailNone');
            connectStatus.google = false;
            gmailBtn.onclick = function () {
                //cd.pubsub.publish('gRegister');
                cd.google.register(false);
            };
        });



        cd.pubsub.subscribe('fbLoggedIn', function () {
            $(connectHeader).removeClass('fbNone');
            connectStatus.fb = true;
            setCurrentTab('fb');
        });

        cd.pubsub.subscribe('fbLoginFail', function () {

        });

        cd.pubsub.subscribe('fbStatus', function (status) {

          var  fbBtn = document.querySelector('.connectMore.fb');

            if (status !== 'connected') {
                $(connectHeader).addClass('fbNone');
                connectStatus.fb = false;
                fbBtn.onclick = function () {
                    cd.facebook.login();
                    //cd.pubsub.publish('fbLogin');
                };
            } else {
                $(connectHeader).removeClass('fbNone');
                connectStatus.fb = true;
            }
            if (currentTabName === 'fb') {
                setCurrentTab('fb');
            }

        });


        function checkAccountStatus() {
            if (statusChecked) {
                return;
            }
            statusChecked = true;

            var interval = setInterval(function () {
                if (window.FB) {
                    cd.pubsub.publish('fbGetStatus');
                    //cd.pubsub.publish('gRegister');
                    cd.google.register(true);
                    clearInterval(interval);
                }
            }, 50);

        }
        //#endregion


        //#region get contacts
        function getCloudentsContacts() {
            if (cloudentsContacts.length > 0) {
                if (searchByName.value.length > 0 && searchByName.value !== searchByName.placeholder) {
                    searchContacts();
                    return;
                }

                appendContacts(cloudentsContacts, 'inviteFriendTemplate');
                return;
            }
            dataContext.getFriends({
                success: function (friends) {
                    friends = friends.my;
                    if (!friends)
                        return;

                    for (var i = 0, l = friends.length; i < l; i++) {
                        var friend = friends[i];
                        cloudentsContacts.push({ name: friend.name, userImage: friend.image, defaultImage: $('body').data('pic'), id: friend.id, delay: i * 200, url: friend.url });
                    }

                    cloudentsContacts.sort(function (a, b) {
                        return cd.sortMembersByName(a.name, b.name);
                    });

                    if (searchByName.value.length > 0 && searchByName.value !== searchByName.placeholder) {
                        searchContacts();
                        return;
                    }


                    appendContacts(cloudentsContacts, 'inviteFriendTemplate');

                    //TODO: change state
                    //changeState();
                }
            });
        }
        function getFacebookContacts() {
            if (facebookContacts.length > 0) {
                if (searchByName.value.length > 0 && searchByName.value !== searchByName.placeholder) {
                    searchContacts();
                    return;
                }

                appendContacts(facebookContacts);
                return;
            }

            cd.pubsub.publish('fbGetContacts');
        }
        function getGoogleContacts() {
            if (googleContacts.length > 0) {
                if (searchByName.value.length > 0 && searchByName.value !== searchByName.placeholder) {
                    searchContacts();
                    return;
                }
                appendContacts(googleContacts);
                return;
            }

            if (!connectStatus.google) {
                return;
            }

            cd.pubsub.publish('gGetContacts');
        }
        //#endregion

        //#region append contacts
        function appendContacts(contacts, templateName) {
            templateName = templateName || 'inviteFriendOtherCloudents';
            cd.appendData(listContainer, templateName, contacts, 'beforeend', true);
            loader();
            loading = false;
            loadImages();
            appendAnimation();

        }

        function loadImages() {
            //loop through list items get the image link and apply it to the src tag
            var listImages = listContainer.getElementsByClassName('friendImg'), i = 0,
                onErrorFunc = imageLoadFail;

            (function loadImage(listImages, i) {
                setTimeout(function () {
                    var img = listImages[i];
                    if (!img) { //if image is null the tab changed
                        return;
                    }
                    var dataSrc = img.getAttribute('data-src');
                    if (dataSrc !== 'null') {
                        img.src = dataSrc;
                        img.onerror = onErrorFunc;
                    }
                    if (++i < listImages.length) loadImage(listImages, i);
                }, 30);
            })(listImages, i);

        }
        function imageLoadFail(e) {
            var image = e.target;
            image.onerror = null;
            if (currentTabName === 'cloudents') {
                image.src = '/images/emptyState/my_default3.png';
                return;
            }
            if (currentTabName === 'fb') {
                image.src = '/images/emptyState/my_default3.png';
                return;
            }

            image.src = '/Images/user-gmail-pic.jpg';
        }
        $(listContainer).bind('webkitAnimationEnd oanimationend msAnimationEnd animationend', function (e) {
            var $e = $(e.target);
            if (!$e.css('opacity')) {
                $e.css('opacity', 1);
            }
        });
        function appendAnimation() {
            //var listItems = listContainer.getElementsByClassName('invItem');
            //for (var i = 0, l = listItems.length; i < l; i++) {
            //    if (Modernizr.cssanimations) {
            //        listItems[i].style.cssText = "-webkit-animation-delay:" + i * 50 + "ms;"
            //             + "-moz-animation-delay:" + i * 50 + "ms;"
            //             + "-o-animation-delay:" + i * 50 + "ms;"
            //             + "animation-delay:" + i * 50 + "ms;";
            //    }
            //    else {
            //        listItems[i].style.opacity = 1;
            //    }
            //}
        }

        //#endregion

        //#region events
        function registerEvents() {
            invSources.onclick = function (e) { //tab change
                if (loading) {
                    return;
                }

                var tab = e.target, tabName, tabs = ['cloudents', 'fb', 'gPlus', 'gmail'];


                if (!tab) {
                    return;
                }

                tabName = $(tab).attr('class').split(' ')[1];

                if (tabs.indexOf(tabName) === -1) {
                    return;
                }
                currentTabName = tabName;
                setCurrentTab(currentTabName);
                analytics.trackEvent('Invite', 'Select tab', 'Clicking on ' + tabName);
            };

            listContainer.onclick = function (e) { // invite click
                var elm = e.target;
                if (!(elm && $(elm).hasClass('invBtn')) || $(elm).hasClass('invited')) { //not an invite click or invite already sent
                    return;
                }
                elm.disabled = true;
                if (invSources.querySelector('.current.fb')) {
                    facebookInvite(elm);
                    return;
                } //Facebook

                cdGoogleInvite(elm);
                //Cd and G

            };

            document.querySelector('.invForm .inputText.emailUser').onclick = function () {
                inviteInput.focus();
            };
            inviteInput.onkeydown = function (e) {

                if (e.keyCode === 188 || e.keyCode === 13 || e.keyCode === 186) { // , ENTER ;
                    e.preventDefault();
                    return false;
                }
                if (e.keyCode === 8 && inviteInput.value === '') { //if backspace and value is empty delete last email
                    removeEmail(emailsSelectedElement.lastElementChild);
                    e.preventDefault();
                    return false;
                }
                if (e.keyCode === 9 && inviteInput.value !== '') {
                    e.preventDefault();
                    return false;
                }

            };
            inviteInput.onkeyup = function (e) {
                if (e.keyCode === 188 || e.keyCode === 13 || e.keyCode === 186 || e.keyCode === 9) { // , ENTER ; TAB 
                    if (inviteInput.value.length > 0) {
                        addEmail({ id: inviteInput.value, display: inviteInput.value }, cd.validateEmail(inviteInput.value));
                    }
                }
            };

            eById('inviteSendEmailForm').onsubmit = function (e) {
                var form = $(this), aside = document.getElementsByClassName('invSidebar rFloat')[0];

                attempValidate(); //check if the input is a valid email

                if (emailsSelecetd.length === 0) {
                    cd.notification(JsResources.AtLeastOne);
                    return false;
                }

                var responseObj = {
                    data: { Recepients: emailsSelecetd, boxId: boxid },
                    success: function () {
                        form[0].reset();
                        emailsSelecetd = [];
                        $(aside).addClass('invSuccess');
                        cd.removeChildren(emailsSelectedElement);
                        setTimeout(function () {
                            $(aside).removeClass('invSuccess');
                        }, 2000);
                    },
                    error: function () {
                        cd.notification(JsResources.TryAgain);
                    },
                    always: function () {
                        eById('inviteSendEmail').removeAttribute('disabled');
                    }

                };
                eById('inviteSendEmail').setAttribute('disabled', 'disabled');
                if (boxid !== '') {
                    dataContext.inviteBox(responseObj);
                } else {
                    dataContext.inviteCloudents(responseObj);
                }
                analytics.trackEvent('Invite', 'Sent email', 'User sent invites through emails');
                e.preventDefault();
            };

            searchByName.onkeyup = function () {
                searchContacts();
                analytics.trackEvent('Invite', 'Search', 'User is searching');
            };
            eById('inviteConnectAccount').onclick = function () {
                if (currentTabName === 'fb') {
                    cd.facebook.login();
                    //FB.login(function (response) {
                    //});
                    //cd.pubsub.publish('fbLogin');
                }
                if (currentTabName === 'gmail') {
                    //cd.pubsub.publish('gRegister');
                    cd.google.register(false);
                }
            };

        }
        //#endregion

        function inviteAnimation(elm) {
            elm.textContent = '';
            var pLoader = document.createElement('span'),
                pInner = document.createElement('span'),
                sent = document.createElement('span');

            pLoader.className = 'progressLoader';
            pInner.className = 'progressInner';
            sent.className = 'sent';
            sent.textContent = listContainer.getAttribute('data-inv-sent');

            pLoader.appendChild(pInner);
            elm.insertBefore(pLoader, elm.firstElementChild);

            setTimeout(function () {
                elm.removeChild(pLoader);
                elm.insertBefore(sent, elm.firstElementChild);

            }, 1000);

            setTimeout(function () {
                $(elm).addClass('invited');
            }, 1050);
        }

        //#region send invites
        function cdGoogleInvite(elm) {
            var id = elm.getAttribute('data-id');
            var obj = {
                data: { Recepients: id },
                success: function () {
                    inviteAnimation(elm);
                },
                error: function () {
                    cd.notification('Error');
                }
            };

            if (boxid !== '') {
                obj.data.BoxId = boxid;
                dataContext.inviteBox(obj);
                return;
            }

            dataContext.inviteCloudents(obj);

        }
        function facebookInvite(elm) {
            var fbId = elm.getAttribute('data-id'),
                friend = facebookContacts.filter(function (x) { return x.id === fbId; })[0], //can only be one
                fbDialog, fbBlocker;

            var fbObj = {
                data: { Id: friend.id, Username: friend.username || friend.id, FirstName: friend.firstname, MiddleName:friend.middlename, LastName: friend.lastname, Sex: friend.gender},
                success: function () {
                    inviteAnimation(elm);

                },
                error: function (msg) {
                    console.log(msg);
                    cd.notification(JsResources.ErrorInvite);
                }
            };

            if (boxid !== '') {
                fbObj.data.BoxId = boxid;
            }
            var offset = window.pageYOffset;
            FB.ui({
                method: 'send',
                link: facebookInviteUrl,
                to: fbId
            }, function (response) {
                window.scroll(0, offset);
                fbBlocker.remove();
                fbInviteResponse(elm, fbObj, response);
            });

            //append input change block
            fbDialog = $('.fb_dialog  iframe').closest('.fb_dialog');
            var interval = setInterval(function () {
                if (fbDialog.position().top > 0) {
                    fbBlocker = $('<div id="fbBlock" style="width:' + fbDialog.outerWidth() + 'px;height:90px;position:absolute;z-index:999999;opacity:1;top:' + fbDialog.position().top + 'px;left:' + fbDialog.position().left + 'px"></div>');
                    $('body').append(fbBlocker);
                    clearInterval(interval);
                }

            }, 50);


        }
        function fbBoxInvite(elm, fbObj, response) {
            if (!response) {
                return;
            }
            dataContext.fbBoxInvite(fbObj);
        }

        function fbInvite(elm, fbObj, response) {
            if (!response) {
                return;
            }
            dataContext.fbInvite(fbObj);
        }

        function cdgBoxInvite(elm, obj, response) {
            if (!response) {
                return;
            }
            dataContext.inviteBox(obj);
        }

        function gInvite(elm, cdgObj, response) {
            if (!response) {
                cd.notification('Error');
                return;
            }
            dataContext.inviteCloudents(cdgObj);
        }

        //#endregion 

        //#region search by name
        function searchContacts() {

            var foundList = [], searchList, value = searchByName.value;
            if (currentTabName === 'cloudents') {
                searchList = cloudentsContacts;
            }
            else if (currentTabName === 'fb') {
                searchList = facebookContacts;
            }
            else if (currentTabName === 'gmail' || currentTabName === 'gPlus') {
                searchList = googleContacts;
            } else {
                return;
            }
            for (var i = 0, l = searchList.length; i < l ; i++) {

                if (searchList[i].name.toLowerCase().indexOf(value.toLowerCase()) > -1) {
                    foundList.push(searchList[i]);
                }
            }

            foundList.sort(function (a, b) {
                return cd.sortMembersByName(a.name, b.name, value);
            });

            if (foundList.length > 0) {
                appendContacts(foundList, currentTabName === 'cloudents' ? 'inviteFriendTemplate' : null);
                changeState();
            } else {
                changeState('empty');
                loader();
                loading = false;
            }
        }
        //#endregion

        //#region send by email
        function addEmail(item, valid) {
            var lastElement;

            if (emailsSelecetd.indexOf(item.id) > -1) {//email  already exist
                inviteInput.value = inviteInput.value.slice(0, -1);
                cd.notification(JsResources.EmailExists);
                return;
            }

            cd.appendData(emailsSelectedElement, 'emailSelectedItemTemplate', { id: item.id, name: item.display }, 'beforeend', false);

            if (valid) {
                emailsSelecetd.push(item.id);
            } else {
                lastElement = emailsSelectedElement.lastElementChild;
                $(lastElement).addClass('invalidEmail');
            }

            if (!emailsSelectedElement.onclick) {
                emailsSelectedElement.onclick = removeEditEmailClick;
            }

            inviteInput.value = '';

            if (window.clipboardData) {
                setTimeout(function () { inviteInput.focus(); }, 10);
            } else {
                inviteInput.focus();
            }

            calculateInputWidth();
        }

        function removeEmail(emailElement) {
            var email = emailElement.getAttribute('data-id'),
            index = emailsSelecetd.indexOf(email);

            calculateInputWidth();
            inviteInput.focus();

            emailsSelectedElement.removeChild(emailElement);
            if (emailsSelectedElement.children.length === 0) {
                emailsSelectedElement.onclick = null;
            }

            if (index > -1) {
                emailsSelecetd.splice(index, 1);
            }
        }

        var consts = { ITEM_MARGIN_LEFT: 3, INPUT_MIN_WIDTH: 235 };
        function calculateInputWidth() {
            var emailElements = emailsSelectedElement.getElementsByClassName('emailItem'),
                inputElementWidth, width = $('.inviteEmailListWpr').width(),
                calculadtedWidth = 0, container = inviteInput.parentElement;
            inviteInput.style.display = 'none';
            for (var i = 0, l = emailElements.length; i < l; i++) {
                var bottom = (container.offsetHeight + container.offsetTop) - (emailElements[i].offsetTop + emailElements[i].offsetHeight),
                    marginBottom = parseInt(window.getComputedStyle(emailElements[i], null).getPropertyValue('margin-bottom'));

                if (bottom === marginBottom) {
                    calculadtedWidth += (emailElements[i].offsetWidth + consts.ITEM_MARGIN_LEFT);
                }
            }
            inputElementWidth = width - calculadtedWidth;
            inviteInput.style.width = (inputElementWidth < 135 ? consts.INPUT_MIN_WIDTH : (inputElementWidth - 4 * emailElements.length)) + 'px'; //4 * emailsLenth is because of display-inline-block
            inviteInput.style.display = 'inline-block';
        }

        function editInput(emailItem) {
            if (!emailItem)
                return;

            inviteInput.value = emailItem.textContent;
            removeEmail(emailItem);
            inviteInput.focus();
        }

        function removeEditEmailClick(e) {
            var target = e.target, emailItem;
            if (!target)
                return;
            emailItem = target;
            while (emailItem && emailItem.nodeName !== 'LI') {
                emailItem = emailItem.parentNode;
            }
            if ($(target).hasClass('removeItem')) {
                removeEmail(emailItem);
            } else {
                if ($(emailItem).hasClass('emailItem') && $(emailItem).hasClass('invalidEmail')) {
                    editInput(emailItem);
                }
            }
        }

        function attempValidate() {
            var value = inviteInput.value;
            if (value.length > 0 && cd.validateEmail(value)) {
                addEmail({ id: value, display: value }, true);
            }
        }
        //#endregion

        //#region pubsubs
        cd.pubsub.subscribe('fbInit', function () {
            checkAccountStatus();
        });
        cd.pubsub.subscribe('inviteclear', function () {
            searchByName.value = '';
        });
        cd.pubsub.subscribe('invite', function (d) {
            boxid = d.boxid || cd.getParameterFromUrl(2);
            if (boxid.indexOf('?') > -1) {
                boxid = '';
            }

            //check if cloudents or box        
            if (boxid === '') {
                setupInviteToCloudents();
                currentTabName = defaultTabName = 'fb';
            }
            else {
                setupInviteToBox(d);
                currentTabName = defaultTabName = 'cloudents';
            }

            if (defaultTabName === 'cloudents') {// || (defaultTabName === 'fb' && boxid === '')) {
                setCurrentTab(defaultTabName);
            } else {
                var interval = setInterval(function () {
                    if (connectStatus) {
                        setCurrentTab(defaultTabName);
                        clearInterval(interval);
                    }
                }, 25);

            }
            cd.pubsub.publish('invite_load');
        });


        cd.pubsub.subscribe('gContacts', function (contacts) {
            if (googleContacts.length > 0) {
                return;
            }
            googleContacts = contacts;
            if (currentTabName !== 'gmail') {
                return;
            }
            appendContacts(googleContacts);
            if (searchByName.value.length > 0 && searchByName.value !== searchByName.placeholder) {
                searchContacts();
            }
        });

        cd.pubsub.subscribe('fbContacts', function (contacts) {
            if (facebookContacts.length > 0) {
                return;
            }
            facebookContacts = contacts;
            if (currentTabName !== 'fb') {
                return;
            }
            appendContacts(facebookContacts);
            if (searchByName.value.length > 0 && searchByName.value !== searchByName.placeholder) {
                searchContacts();
            }
        });
        //#endregion

    }
}(cd, cd.data, cd.pubsub, jQuery, cd.analytics));

