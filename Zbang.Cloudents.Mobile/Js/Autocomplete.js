//Currently used in Share 
(function (cd, jsResources) {
    "use strict";

    if (window.scriptLoaded.isLoaded('ac')) {
        return;
    }

    var sInputElement = 'sInputElement', sOutputElement = 'sOutputElement', sSelectedList = 'sSelectedList', eImg = 'eImg', eAddress = 'eAddress', eName = 'eName',
    beforeEnd = 'beforeend', emailMenuItemTemplate = 'emailMenuItemTemplate', emailSelectedItemTemplate = 'emailSelectedItemTemplate',
    contactsToDisplay = 'contactsToDisplay', sEmailSelectedList = 'sEmailSelectedList', maxWidth = 'maxWidth', dataSet = 'dataSet', contactsFound = 'contactsFound',
    settings = {}, maxMembers = 300,
    eById = document.getElementById.bind(document),
    Consts = { DEFAULT_INPUT_MAXWIDTH: 390, DEFAULT_ITEMS_LENGTH: 7, DEFAULT_INPUT_MININPUT: 113, ITEM_MARGIN_LEFT: 3 },
    methods = {
        init: function (options) {
            settings[sInputElement] = eById(options[sInputElement]);
            settings[sOutputElement] = eById(options[sOutputElement]);
            settings[sSelectedList] = eById(options[sSelectedList]);
            settings[contactsToDisplay] = options.contactsToDisplay || Consts.DEFAULT_ITEMS_LENGTH;
            settings[sEmailSelectedList] = [];
            settings[dataSet] = [];
            settings[contactsFound] = [];
            settings[maxWidth] = options[maxWidth] || Consts.DEFAULT_INPUT_MAXWIDTH;
            var inputElement = settings[sInputElement], outputElement = settings[sOutputElement];
            inputElement.onkeydown = privateMethods.inputKeydownListener;
            inputElement.onkeyup = privateMethods.inputKeyupListener;
            if (outputElement) {
                eById('mNote').onfocusin = function () {
                    outputElement.style.display = 'none';
                };
            }
        },
        setWidth: function (width) {
            settings[maxWidth] = width;
        },
        insertData: function (data) {
            settings[dataSet] = settings[dataSet].concat(data);
        },
        addEmailsToList: function (data) {
            var emailUserWpr = document.getElementsByClassName('emailUserWpr')[0],
                singleEmailUserWpr = document.getElementsByClassName('singleEmailUserWpr')[0];

            if (data.length === 1) { //message from tooltip or user page
                cd.appendData(singleEmailUserWpr, 'singleUserEmailTemplate', data, 'afterbegin', true);
                singleEmailUserWpr.style.display = 'block';
                emailUserWpr.style.display = 'none';
                document.querySelector('.invite .popupHeader').textContent = jsResources.SendDirect;                
                return;
            }

            if (data.length > maxMembers) {
                privateMethods.addEmailList(data);

            } else {
                for (var i = 0, l = data.length; i < l; i++) {
                    privateMethods.addEmail({ id: data[i].id, name: data[i].name}, true);
                }
            }
            privateMethods.calculateInputWidth();
        },
        attemptValidate: function () {
            var inputElementValue = settings[sInputElement].value, contact,
                selectedList = settings[sSelectedList].querySelectorAll('.invalidEmail .emailText');

            for (var i = 0, l = selectedList.length; i < l; i++) {

                contact = privateMethods.findContact(selectedList[i].textContent, settings[dataSet]);
                if (contact) {
                    privateMethods.addEmail({ id: contact.id, name: contact.name }, true);
                }

                selectedList[i].parentElement.removeChild(selectedList[i]);
            }

            if (inputElementValue.length > 0 && cd.validateEmail(inputElementValue)) {
                privateMethods.addEmail({ id: inputElementValue, name: inputElementValue }, true);
            }
        },
        calculateContainerWidth: function () {
            privateMethods.changeContainerWidth(false);
        },

        clear: function () {
            settings[sOutputElement].style.display = 'none';
            document.getElementsByClassName('emailUserWpr')[0].style.display = 'block';
            document.getElementsByClassName('singleEmailUserWpr')[0].style.display = 'none';
            document.querySelector('.invite .popupHeader').textContent = jsResources.ShareBy;
            cd.removeChildren(settings[sSelectedList]);
            cd.removeChildren(settings[sOutputElement]);
            settings[contactsFound] = [];
            settings[sEmailSelectedList] = [];
            settings[sInputElement].value = '';

        }
    };
    var privateMethods = {
        inputKeydownListener: function (e) {
            //var outputElement = settings[sOutputElement],
             var   inputValue = settings[sInputElement].value,
                selectedList = settings[sSelectedList];

            if (e.keyCode === 188 || e.keyCode === 13 || e.keyCode === 186) { // , ENTER ;
                e.preventDefault();
                return false;
            }
            if (e.keyCode === 8 && inputValue === '') { //if backspace and value is empty delete last email
                privateMethods.removeEmail(selectedList.lastElementChild);
                e.preventDefault();
                return false;
            }
            if (e.keyCode === 9 && inputValue !== '') {
                e.preventDefault();
                return false;
            }


        },
        inputKeyupListener: function (e) {
            var outputElement = settings[sOutputElement], inputElement = settings[sInputElement],
                 inputValue = inputElement.value, contact, contacts = settings[contactsFound];
            if (e.keyCode === 188 || e.keyCode === 13 || e.keyCode === 186 || e.keyCode === 9) { // , ENTER ; TAB 

                if (inputValue.length > 0) {
                    contact = privateMethods.findContact(inputValue, contacts);
                    if (contact) {
                        privateMethods.addEmail({ id: contact.id, name: contact.name }, true);
                    } else {
                        privateMethods.addEmail({ id: inputValue, name: inputValue }, cd.validateEmail(inputValue));
                    }
                }
                privateMethods.calculateInputWidth();
            } else {

                contacts = privateMethods.searchItems(inputValue);
                if (outputElement) {
                    if (contacts.length > 0) {
                        outputElement.style.display = 'block';
                        outputElement.onclick = privateMethods.listItemClick;
                        privateMethods.appendContacts(contacts);

                    } else {
                        outputElement.onclick = null;
                        outputElement.style.display = 'none';
                    }
                    settings[contactsFound] = contacts;
                }
            }
        },
        listItemClick: function (e) {
            var listItem = e.target;
            if (!listItem || listItem.classList.contains('emailMenu'))
                return;
            while (listItem.nodeName !== 'LI') {
                listItem = listItem.parentElement;
            }

            var display = listItem.getElementsByClassName(eName)[0].textContent, id = listItem.getElementsByClassName(eAddress)[0].textContent;

            privateMethods.addEmail({ id: id, name: display }, true);
            privateMethods.calculateInputWidth();
        },
        appendContacts: function (contacts) {

            var outputElement = settings[sOutputElement];
            cd.appendData(outputElement, emailMenuItemTemplate, contacts, beforeEnd, true);
            privateMethods.loadImages(); //Replace available images with default

            //Hide address without emails
            var emails = outputElement.getElementsByClassName(eAddress);
            for (var i = 0, l = emails.length; i < l; i++) {
                if (emails[i].textContent.indexOf('@') === -1) {
                    emails[i].style.display = 'none';
                }

            }


        },
        addEmail: function (item, valid) {
            var selectedList = settings[sSelectedList], inputElement = settings[sInputElement], outputElement = settings[sOutputElement], emailSelectedList = settings[sEmailSelectedList],  lastElement;

            if (emailSelectedList.indexOf(item.id) > -1) {//email  already exist
                inputElement.value = inputElement.value.slice(0, -1);
                cd.notification('Email already exists');
                return;
            }

            cd.appendData(selectedList, emailSelectedItemTemplate, { id: item.id, name: item.name }, beforeEnd, false);

            if (valid) {
                emailSelectedList.push(item.id);
            } else {
                lastElement = selectedList.lastElementChild;
                lastElement.classList.add('invalidEmail');
            }

            if (!selectedList.onclick) {
                selectedList.onclick = privateMethods.removeEditEmailClick;
            }

            inputElement.value = '';
            if (outputElement) {
                outputElement.style.display = "none";
            }
            if (window.clipboardData) {
                setTimeout(function () { inputElement.focus(); }, 10);
            } else {
                inputElement.focus();
            }

            //privateMethods.calculateInputWidth();

        },
        addEmailList: function (data) {
            var selectedList = settings[sSelectedList],
                guid = cd.guid().slice(0, 5),
                ids = data.map(function (item) {
                    return item.id;
                });
            var object = { name: data.length + ' members', ids: ids, guid: guid };
            cd.appendData(selectedList, 'multiUserEmailTemplate', object, beforeEnd, false);
            selectedList.onclick = privateMethods.removeEditEmailClick;

            //privateMethods.calculateInputWidth();

        },
        findContact: function (input, output) {
            var contact;
            for (var i = 0, l = output.length, found = false; i < l && !found; i++) {
                contact = output[i];
                if (contact.name.toLowerCase().indexOf(input.toLowerCase()) > -1) {
                    found = true;
                }
            }
            return contact;

        },
        editInput: function (emailItem) {
            var inputElement = settings[sInputElement];

            if (!emailItem)
                return;

            inputElement.value = emailItem.textContent;
            privateMethods.removeEmail(emailItem);
            inputElement.focus();
        },
        calculateInputWidth: function () {
            var inputElement = settings[sInputElement], selectedList = settings[sSelectedList], emails = selectedList.getElementsByClassName('emailItem'), width = parseInt(settings[sSelectedList].parentElement.style.width, 10),
                calculadtedWidth = 0, container = inputElement.parentElement;
            inputElement.style.display = 'none';
            for (var i = 0, l = emails.length; i < l; i++) {
                var bottom = (container.offsetHeight + container.offsetTop) - (emails[i].offsetTop + emails[i].offsetHeight),
                    marginBottom = parseInt(window.getComputedStyle(emails[i], null).getPropertyValue('margin-bottom'), 10);
                if (bottom === marginBottom) {
                    calculadtedWidth += (emails[i].offsetWidth + Consts.ITEM_MARGIN_LEFT);
                }
            }
            var inputElementWidth = width - calculadtedWidth;
            inputElement.style.width = (inputElementWidth < Consts.DEFAULT_INPUT_MININPUT ? Consts.DEFAULT_INPUT_MININPUT : (inputElementWidth - 4 * emails.length)) + 'px';
            inputElement.style.display = 'inline-block';
        },
        removeEmail: function (emailItem) {
            var inputElement = settings[sInputElement], emailSelectedList = settings[sEmailSelectedList], selectedList = settings[sSelectedList], dataId, index;
            if (!emailItem) {
                return;
            }
            dataId = emailItem.getAttribute('data-id');
            index = emailSelectedList.indexOf(dataId);
            selectedList.removeChild(emailItem);
            if (selectedList.children.length === 0) {
                selectedList.onclick = null;
            }
            privateMethods.calculateInputWidth();
            inputElement.focus();
            if (index > -1) {
                emailSelectedList.splice(index, 1);
                return true;
            }
            return false;

        },
        removeEditEmailClick: function (e) {
            var target = e.target, emailItem;
            if (!target)
                return;
            emailItem = target;
            while (emailItem && emailItem.nodeName !== 'LI') {
                emailItem = emailItem.parentNode;
            }
            if (target.classList.contains('removeItem')) {
                privateMethods.removeEmail(emailItem);
            } else {
                if (emailItem.classList.contains('emailItem') && emailItem.classList.contains('invalidEmail')) {
                    privateMethods.editInput(emailItem);
                }
            }
        },
        loadImages: function () {
            //loop through list items get the image link and apply it to the src tag
            var outputElement = settings[sOutputElement], listImages = outputElement.getElementsByClassName(eImg), i = 0, onErrorFunc = privateMethods.imageLoadFail;
            (function loadImage(listImages, i) {
                setTimeout(function () {
                    var img = listImages[i];
                    if (!img) {
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

        },
        imageLoadFail: function (e) {
            var image = e.target;
            image.onerror = null;
            image.src = '/Images/user-gmail-pic.jpg';
        },
        searchItems: function (value) {
            var index, currentIndex = 0, contacts = settings[dataSet],
                contact, foundList = [];
            for (var i = 0, l = contacts.length; i < l ; i++) {
                contact = contacts[i];
                while (value && (index = contact.name.toLowerCase().indexOf(value.toLowerCase(), currentIndex)) > -1) {
                    contact.highlightName = contact.name.insert(index, '<mark class="boldPart">');
                    currentIndex = index + 23 + value.length;
                    contact.highlightName = contact.highlightName.insert(currentIndex, '</mark>');

                    foundList.push(contact);
                    currentIndex = currentIndex + 7;
                }
                currentIndex = 0;
            }

            privateMethods.sortList(foundList, value);

            return foundList;
        },
        sortList: function (foundList) {
            var value = settings[sInputElement].value.toLowerCase();
            foundList.sort(function (a, b) {
                return cd.sortMembersByName(a.name, b.name, value);
            });

            foundList.length = foundList.length > settings[contactsToDisplay] ? settings[contactsToDisplay] : foundList.length;

        },
        changeContainerWidth: function (gConnectVisible) {
            var gConnect = document.getElementsByClassName('gConnect')[0],
                gConnectLength = parseInt(window.getComputedStyle(gConnect, null).getPropertyValue('width'), 10) + 5,//5 is position:right
                toLabel = document.querySelector('.emailUser .inputLabel'),
                toLabelWidth = parseInt(window.getComputedStyle(toLabel, null).getPropertyValue('width'), 10),
                //containerWidth = parseInt(window.getComputedStyle(settings[sSelectedList].parentElement).getPropertyValue('width', null), 10),
                newWidth;

            if (gConnectVisible) {
                newWidth = settings[maxWidth] - gConnectLength - toLabelWidth;
            } else {
                newWidth = settings[maxWidth] - toLabelWidth - 5;//5 is position:right of gconnect;
            }

            settings[sSelectedList].parentElement.style.width = newWidth + 'px';
            privateMethods.calculateInputWidth();
        }
    };
    cd.pubsub.subscribe('gAuthFail', function () {
        if (settings[sInputElement]) { //if autocomplete loaded
            privateMethods.changeContainerWidth(true);
        }
    });

    cd.pubsub.subscribe('gAuthSuccess', function () {
        if (settings[sInputElement]) { //if autocomplete loaded
            privateMethods.changeContainerWidth(false);
        }
    });

    cd.autocomplete2 = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            console.log('Method ' + method + ' does not exist on autocomplete');
        }
    };
}(cd, JsResources));
