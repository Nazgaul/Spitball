define('Item', ['Knockout','Pubsub'], function () {
    (function ($, dataContext, ko, cd, JsResources, analytics, Modernizr) {
        "use strict";        

        var consts = {
            item: 'item', dataData: 'data-data', link: 'link', www: 'www', file: 'file', ref: '?r=item&s=items',
            mF: 'mF', fLtrExt: 'fourLetterExtension ', disabled: 'disabled', starWidth: 96 / 5, register: 'register',
            itemLoad: 'item_load', itemClear: 'itemclear', perm: 'perm', imagesQuery: 'img.imageContent', rated: 'rated',
            title: '{0} | {1}.{2} | Cloudents', divWrapper: 'divWrapper', checked: 'checked', fetchRate: 'fetchrate',
            flagItemDialog: 'flagItemDialog', fullscreen: 'fulscrn', submit: ':submit', thirdWindowHeight: $(window).height() / 3,
            firstChild: ':first-child', lastChild: ':last-child', annotation: 'annotation', video: 'video'
        },
        eById = document.getElementById.bind(document);

        cd.pubsub.subscribe('initItem', registerKOItem);        

        function registerKOItem() {
            ko.applyBindings(new ItemViewModel(), document.getElementById(consts.item));
        }

        function ItemViewModel() {
            var self = this;

            //#region variables        
            //elements
            var $itemMoreFiles = $('#item_moreFiles'), $itemPreview = $('#itemPreview'),
                            $previewWrapper = $('#previewWrapper'), previewWrapper = $previewWrapper[0], $commentToggle = $('#commentToggle'),
                            $itemShare = $('#itemShare'), $itemPrint = $('#itemPrint'),
                            $previewFailed = $('.previewFailed'), $pageLoader = $('.pageLoader'), $commentBtn = $('.commentBtn'),
                            $rateContainer = $('#rateContainer'), $rated = $('#item').find('.rated'), $itemRight = $('.itemRight'),
                            $itemPrev = $('#itemPrev'), $itemNext = $('#itemNext'), $itemFulscrn = $('#itemFulscrn'),
                            $body = $('body'), $itemDL = $('#item_DL'), $itemPrint = $('#itemPrint'), $itemP = $('#item_P'),
                            $otakimP = $('#Otakim_P'), $itemFS = $('#item_FS'), $itemCL = $('#item_CL'), $itemSettings = $('#itemSettings'),
                            $itemRenameSave = $('#itemRenameSave'), $itemName = $('#itemName'), $itemRename = $('#item_rename'),
                            $itemRenameCancel = $('#itemRenameCancel'), $rateBubble = $('#rateBubble'), $rateBtn = $('#rateBtn'),
                            $ratePopup = $('#ratePopup'), $commentsNumber = $('.commentsNumber'), $itemMsg = $('#item_msg'),

            //data
            isLtr = $('html').css('direction') === 'ltr', itemType, userType, blobName, annotationList = [],
            boxid, ownerid, otakim, commentShow = false, defferedArray = [], defferedItemShow = new $.Deferred(),
            flagRequest = true, flagPopupEvents = true, firstTime = true, rateMenuOpen = false, choosedRate = false, initialRate = 0, loaded = true,
            $bubbleText = $rateBubble.find('.bubbleText'), ratePopupTimeout, ratedItems;

            //#endregion

            //#region observables

            self.itemid = ko.observable();
            self.ownerName = ko.observable();
            self.ownerImage = ko.observable();
            self.ownerId = ko.observable();
            self.update = ko.observable();
            self.itemName = ko.observable();
            self.boxName = ko.observable();
            self.copyLink = ko.observable();
            self.download = ko.observable();
            self.numberOfDownloads = ko.observable();
            self.numberOfViews = ko.observable();
            self.deleteAllow = ko.observable();
            self.flagAllow = ko.observable();
            self.items = ko.observableArray([]);
            self.extension = ko.observable();
            self.extensionColor = ko.observable();
            self.rate = ko.observable();
            self.boxurl = ko.observable();
            self.uniName = ko.observable();

            //#endregion        

            //#region ko events

            //#region flag an item
            self.flagItem = function () {
                if (!cd.register()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }
                var $flagItemDialog = $('#' + consts.flagItemDialog);

                trackEvent('Flag item');
                if (!$flagItemDialog.length && flagRequest) {
                    flagRequest = false;
                    dataContext.badItemPopUp({
                        success: function (data) {
                            $(eById(consts.item)).append(data);
                            if (flagPopupEvents) {
                                flagPopupEvents = false;
                                registerPopEvent();

                            }
                        },
                        always: function () {
                            flagRequest = true;
                        }
                    });
                }
                else {
                    $flagItemDialog.show();
                }


            };

            function registerPopEvent() {
                var $flagItemDialog = $('#' + consts.flagItemDialog), $flagOther = $('#flag_other');
                $flagItemDialog.find('.closeDialog,.cancel').click(function () {
                    if ($flagItemDialog.find('.requestSent').is(':visible')) {
                        $flagItemDialog.find('.flagItem').toggle();
                    }
                    cd.resetForm($flagItemDialog.find('form'));
                    $flagItemDialog.hide();

                });

                $flagItemDialog.find('input[type="radio"]').click(function () {

                    if (this.id === 'flag_radioOther') {
                        $flagOther.focus();
                        if ($flagOther.val() === '') {
                            $flagItemDialog.find(consts.submit).attr(consts.disabled, consts.disabled);
                        }
                    }
                    else {
                        $flagItemDialog.find(consts.submit).removeAttr(consts.disabled);
                    }

                });
                $flagOther.keyup(function () {
                    if ($flagOther.val() === '') {
                        $flagItemDialog.find(consts.submit).attr(consts.disabled, consts.disabled);
                    }
                    else {
                        $flagItemDialog.find(consts.submit).removeAttr(consts.disabled);
                    }
                });

                $flagItemDialog.find('form').submit(function (e) {
                    e.preventDefault();
                    var $form = $(this);
                    if (!$form.valid || $form.valid()) {
                        if ($('#flag_radioOther').is(':checked') && $flagOther.val() === '') {
                            cd.displayErrors($form, 'You need to write something');
                            return;
                        }
                        var fdata = $form.serializeArray();
                        fdata.push({ name: 'ItemId', value: self.itemid() });
                        dataContext.badItemRequest({
                            data: fdata,
                            success: function () {
                                $flagItemDialog.find('.flagItem').toggle();
                            },
                            error: function (msg) {
                                cd.resetErrors($form);
                                cd.displayErrors($form, msg);
                            }

                        });
                    }
                });
            }
            //#endregion

            self.fullscreen = function () {
                $commentToggle.prop('checked', false).trigger('change');
                $body.addClass(consts.fullscreen);
                $itemFulscrn.prepend($previewWrapper);
                trackEvent('Full screen');

                var $selected = $('.select'), $arrow;
                if (!$selected.length) {
                    return;
                }

                if ($selected.is(consts.firstChild)) {
                    $arrow = $itemPrev;
                }
                else if ($selected.is(consts.lastChild)) {
                    $arrow = $itemNext;
                }

                if ($arrow && $arrow.length > 0) {
                    $arrow.addClass(consts.disabled).attr(consts.disabled, consts.disabled);
                }
            };

            //#region delete an item
            self.deleteItem = function () {
                cd.confirm(JsResources.SureYouWantToDelete + ' ' + self.itemName() + "?", function () {
                    cd.sessionStorageWrapper.clear();
                    dataContext.removeItem({
                        data: { itemId: self.itemid(), BoxUid: boxid },
                        success: function () {
                            cd.pubsub.publish('nav', self.boxurl());
                        }
                    });
                }, null);
            };
            //#endregion

            //#endregion

            //#region models
            function Item(data) {
                var that = this;
                data = data || {};
                that.name = ko.observable(data.name);
                that.uid = ko.observable(data.id);
                that.uploader = data.owner;
                that.userid = data.ownerId;
                that.type = data.type;
                that.itemUrl = data.url;
                that.extension = cd.getExtension(that.name(), that.type);
                that.extensionColor = cd.getExtensionColor(that.name(), that.type);

                that.select = ko.computed(function () {
                    return that.uid() === self.itemid();
                });
                that.nameNoType = ko.computed(function () {
                    return getNameNoExtension(that.name(), data.type.toLowerCase());
                });
            }

            function AnnotationObj(data) {
                data = data || {};
                var that = this;
                that.id = data.id;
                that.imageId = data.imageId;
                that.comment = data.comment.replace(/\n/g, '<br/>');
                that.x = data.x;
                that.y = data.y;
                that.width = data.width;
                that.height = data.height;
                that.date = data.creationDate;
                that.userImage = data.userImage;
                that.userName = data.userName;
                that.uid = data.uid;
            }
            //#endregion

            //#region Get item data
            cd.pubsub.subscribe(consts.item, function (data) {
                self.itemid(parseInt(data.id, 10));

                if (cd.register()) {
                    if (!ratedItems) {
                        ratedItems = JSON.parse(cd.localStorageWrapper.getItem('ratedItems'));
                        if (!ratedItems) {
                            ratedItems = {};
                        }
                        if (!ratedItems[cd.userDetail().nId]) {
                            ratedItems[cd.userDetail().nId] = [];
                        }
                    }
                }

                if (boxid !== data.boxid) {
                    boxid = data.boxid;
                    initialItems();
                }
                if (firstTime) {
                    firstTime = false;
                    registerEvents();
                }

                getItem();
            });

            function initialItems() {
                dataContext.getItems({
                    data: {
                        BoxUid: boxid, pageNumber: 0,
                        uniName: cd.getParameterFromUrl(1),
                        boxName: cd.getParameterFromUrl(3)
                    },
                    success: function (data) {
                        var elems = $.map(data, function (i) {
                            if (i.type.toLowerCase() !== 'quiz') {
                                return new Item(i);
                            }
                        });
                        self.items(elems);
                    }
                });
            }

            function getItem() {
                var itemElement = eById(consts.item),
                    data = itemElement.getAttribute(consts.dataData);

                if (data) {
                    var dataObj = JSON.parse(data, cd.isoDateReviver);
                    itemElement.removeAttribute(consts.dataData);
                    parseData(dataObj);
                    return;
                }

                dataContext.getItem({
                    data: {
                        BoxUid: boxid,
                        itemId: self.itemid(),
                        uniName: cd.getParameterFromUrl(1)
                    },
                    success: function (data2) {
                        parseData(data2);
                    }
                });

                function parseData(data) {
                    data = data || {};

                    var itemPageLoad = new $.Deferred();
                    defferedArray.push(itemPageLoad);

                    var annotation = $('#AddAnotation').hide().detach();

                    annotation.find('textarea').css('height', '');//need to be b4 previewWrapper

                    $itemRight.append(annotation);

                    $previewWrapper[0].innerHTML = '';

                    fillVariables();

                    cd.setTitle(consts.title.format(self.boxName(), self.itemName(), self.extension()));

                    cd.pubsub.publish(consts.itemLoad, null, function () {
                        cd.innerScroll($itemMoreFiles, $(window).height() - $itemMoreFiles.offset().top);
                        itemPageLoad.resolve();
                    });

                    loaded = true;
                    defferedArray.push(getAnnotation());
                    defferedArray.push(getPreview());

                    $.data($rateContainer[0], consts.fetchRate, true);

                    cd.pubsub.publish(consts.perm, data.userType);

                    if ($rated.length) {
                        $rated.toggleClass(consts.rated).text(5 - $rated.index());
                    }

                    function fillVariables() {
                        ownerid = data.ownerUid;
                        userType = data.userType;
                        otakim = data.otakim;
                        blobName = data.blob;
                        itemType = data.type;

                        self.ownerName(data.owner)
                        .ownerImage(data.ownerImg)
                        .ownerId(data.ownerUid)
                        .uniName(data.uniName)
                        .update(cd.dateToShow(data.updateTime))
                        .itemName(data.nameWOExtension || data.name)
                        .numberOfDownloads(data.nDownloads)
                        .numberOfViews(data.numberOfViews || 1)
                        .extension(cd.getExtension(data.name, data.type))
                        .extensionColor(cd.getExtensionColor(data.name, data.type))
                        .copyLink(cd.location())
                        .rate(consts.starWidth * data.rate)
                        .boxurl(data.boxUrl)
                        .boxName(data.boxName)
                        .deleteAllow(cd.deleteAllow(userType, ownerid))
                        .flagAllow(checkFlagAllow(userType));

                        function checkFlagAllow() {
                            if (cd.register() && (ownerid === cd.userDetail().nId)) {
                                return false;
                            }
                            return true;
                        }
                    }
                }
            }

            function getAnnotation() {
                annotationList = [];
                return dataContext.getAnnotation({
                    data: { itemId: self.itemid() },
                    success: function (retVal) {

                        for (var i = 0; i < retVal.length; i++) {
                            annotationList.push(new AnnotationObj(retVal[i]));
                        }
                        if (annotationList.length) {
                            $commentsNumber.text(annotationList.length).show();
                        } else {
                            $commentsNumber.text(0).hide();
                        }
                    }
                });
            }


            function getPreview() {
                if (!loaded) {
                    return;
                }
                if ($previewFailed.length) {
                    return;
                }
                if ($pageLoader.length) {
                    return;
                }
                var imagesNumber = previewWrapper.querySelectorAll(consts.imagesQuery).length;

                if (!(imagesNumber === 0 || imagesNumber > 2)) {
                    return;
                }

                loaded = false;

                if (imagesNumber > 0 && !cd.register()) {
                    cd.pubsub.publish('register', { action: true }); //ask user to register if more than 3 pages
                    return;
                }

                $commentToggle.attr(consts.disabled, consts.disabled);

                renderLoader();

                var parametes = {
                    blobName: blobName,
                    imageNumber: imagesNumber,
                    uid: self.itemid(),
                    width: screen.width,
                    height: screen.height,
                    boxUid: boxid,
                };

                return dataContext.preview({
                    data: parametes,
                    success: function (retVal, query) {
                        if (query.blobName !== blobName) {
                            return;
                        }
                        if (query.imageNumber !== previewWrapper.querySelectorAll(consts.imagesQuery).length) {
                            return;
                        }

                        if ($.trim(retVal.preview)) {
                            loaded = true;
                        }
                        previewWrapper.insertAdjacentHTML('beforeend', retVal.preview);
                        //bug 349 scroll issue in iframe in ipad
                        if (/iPhone|iPod|iPad/.test(navigator.userAgent))
                            $('iframe.iframeContent').wrap(function () {
                                return $('<div />').addClass('iframeContent').css({
                                    overflow: 'auto',
                                    '-webkit-overflow-scrolling': 'touch'
                                });
                            });

                        var images = $previewWrapper.find('img.imageContent,video');

                        var newImages = $previewWrapper.find('img[data-new="true"]');

                        images.first().attr('alt', self.itemName() + '.' + self.extension() + ' | ' + self.boxName());

                        newImages.each(function (i, e) {
                            var y = new $.Deferred();

                            $(e).load(y, function (v) {
                                $.when(defferedItemShow).done(function () {
                                    //IE issue........
                                    window.setTimeout(function () {
                                        initializeCanvas(v);
                                    }, 10);
                                });
                            });

                            defferedArray.push(y);
                        });

                        //rateitempopup
                        $ratePopup.show();
                        if (ratedItems && ratedItems[cd.userDetail().nId]) {
                            if (images.length === 0 || ratedItems[cd.userDetail().nId].indexOf(self.itemid()) > -1) {
                                return;
                            }
                        }

                        ratePopupTimeout = setTimeout(function () {
                            $ratePopup.removeClass('changedItem').addClass('show');

                            if (!cd.register()) {
                                $ratePopup.one('click', '.star', function () {
                                    cd.pubsub.publish('register', { action: true });
                                });
                                return;
                            }

                            $ratePopup.one('click', '.star', function () {
                                $ratePopup.addClass('rated');

                                var $this = $(this),
                                    startWidth = $('.stars .full').width(),
                                    currentRate = 5 - $this.index(),
                                    fakeRate = calculateFakeRate(startWidth, currentRate);
                                getItemRate();
                                self.rate(fakeRate);
                                setItemRate(currentRate);
                                if (ratedItems[cd.userDetail().nId].indexOf(self.itemid()) === -1) {
                                    ratedItems[cd.userDetail().nId].push(self.itemid());
                                    cd.localStorageWrapper.setItem('ratedItems', JSON.stringify(ratedItems));
                                }
                                setTimeout(function () {
                                    $ratePopup.removeClass('show').removeClass('rated');
                                }, 3000);

                            });
                            $ratePopup.one('click', '.closeDialog', function () {
                                if (ratedItems[cd.userDetail().nId].indexOf(self.itemid()) === -1) {
                                    ratedItems[cd.userDetail().nId].push(self.itemid());
                                    cd.localStorageWrapper.setItem('ratedItems', JSON.stringify(ratedItems));
                                }
                                $ratePopup.addClass('changedItem').remove('show');

                            });

                        }, 3000);//3 seocnds
                        //

                        function initializeCanvas(e) {
                            var imgElement = e.target;
                            imgElement.removeAttribute('data-new');
                            var
                            docFragment = document.createDocumentFragment(),
                            parent = imgElement.parentNode,
                            imgWidth = Math.min(imgElement.width, 800),
                            imgHeight = imgElement.height,
                            index = images.index(imgElement),
                            canvas = document.createElement('canvas');


                            canvas.className = consts.annotation;
                            canvas.width = $(parent).width() - (($(parent).width() - imgWidth) / 2) + 4;
                            canvas.height = imgHeight;
                            canvas.id = consts.annotation + index;
                            canvas.style[isLtr ? 'left' : 'right'] = ($(parent).width() - imgWidth) / 2 + 'px';
                            docFragment.appendChild(canvas);

                            if (cd.register()) {

                                var canvas2 = document.createElement('canvas');
                                canvas2.className = "newAnnotation";
                                canvas2.width = imgWidth;
                                canvas2.height = imgHeight;
                                canvas2.title = "Drag to insert comment";
                                canvas2.id = "newAnnotation" + index;
                                canvas2.style[isLtr ? 'left' : 'right'] = ($(parent).width() - imgWidth) / 2 + 'px';
                                docFragment.appendChild(canvas2);
                            }
                            var divElem = document.createElement('div');
                            divElem.className = 'annotationList';
                            divElem.id = 'annotationList' + index;
                            divElem.style.height = imgHeight + 'px';


                            if (imgHeight < 90) {
                                imgElement.style.marginBottom = 10 + 90 - imgHeight + 'px';
                            }

                            docFragment.appendChild(divElem);
                            parent.id = 'itemIndex' + index;
                            parent.appendChild(docFragment);
                            e.data.resolve();
                        }

                        $.when.apply($, defferedArray).done(function () {
                            reProcessAnnotation();
                            $commentToggle.removeAttr(consts.disabled);
                        });

                        isCommentShow();
                    },
                    always: function () {
                        $previewWrapper.find('.pageLoader,.pageLoaderImg,.smallLoader').remove();
                    }

                });

                function isCommentShow() {
                    if (document.getElementsByClassName(consts.divWrapper).length) {
                        $commentBtn.parent().show();
                    }
                    else {
                        $commentBtn.parent().hide();
                    }
                }

                function renderLoader() {
                    var cssLoader, imgLoader;
                    if (imagesNumber === 0) {
                        cssLoader = '<div class="smallLoader"><div class="spinner"></div>';
                        imgLoader = '<img class="pageLoaderImg" src="/images/loader2.gif" />';
                    } else {
                        cssLoader = '<div class="pageLoader pageAnim"></div>';
                        imgLoader = '<img class="pageLoaderImg" src="/images/loader1.gif" />';
                    }
                    if (Modernizr.cssanimations) {
                        $previewWrapper.append(cssLoader);
                    }
                    else {
                        $previewWrapper.append(imgLoader);
                    }
                }
            }

            //#endregion

            //#region Clear data 
            cd.pubsub.subscribe(consts.itemClear, function () {
                $('#AddAnotation').detach();
                $previewWrapper[0].innerHTML = '';
                self.itemName('')
                .update('')
                .ownerImage('')
                .ownerName('')
                .uniName('')
                .itemid('');

                boxid = '';
                ownerid = '';
                annotationList = [];
                defferedArray = [];
                $commentToggle.prop(consts.checked, false).trigger('change');
                $itemShare.prop(consts.checked, false);
                $itemPrint.prop(consts.checked, false);
                commentShow = false;
                $ratePopup.hide().removeClass('show');
                clearTimeout(ratePopupTimeout);
                $rateContainer.find('.star').each(function (i, e) {
                    $(e).removeClass('rated').text(e.id.slice(-1));
                });

            });
            //#endregion

            //#region events
            function registerEvents() {

                generalEvents();
                shareEvents();
                printEvents();
                settingsEvents();
                fullScreenEvents();
                rateEvents();
                initializeAnnotation();

                function generalEvents() {
                    $itemPreview.scroll(function () {
                        if ($itemPreview.scrollTop() >= $itemPreview[0].scrollHeight - $itemPreview.height() - consts.thirdWindowHeight) {
                            getPreview();
                        }
                    });

                    $(document).scroll(function () {
                        if (document.body.className === consts.fullscreen) {
                            if ($(document).scrollTop() >= document.body.scrollHeight - $itemPreview.height() - consts.thirdWindowHeight) {
                                getPreview();
                            }
                        }
                    });

                    cd.pubsub.subscribe('windowChanged', function () {
                        cd.innerScroll($itemMoreFiles, $(window).height() - $itemMoreFiles.offset().top - 10);
                    });

                    cd.pubsub.subscribe('item_show', function () {
                        defferedItemShow.resolve();
                    });

                    $itemMoreFiles.on('click', 'a', function (e) {
                        if (self.itemid() === ko.dataFor(e.target).uid()) {
                            e.preventDefault();
                            return false;
                        }
                        $ratePopup.addClass('changedItem').removeClass('show');
                        clearTimeout(ratePopupTimeout);
                        $rateContainer.find('.star').each(function (i, e1) {
                            $(e1).removeClass('rated').text(e1.id.slice(-1));
                        });

                        $commentToggle.prop('checked', false).trigger('change');


                        trackEvent('move to a different item');
                    });
                    $itemPreview.on("scroll", '', function () {
                        if ($itemPreview.scrollTop() > 0) {
                            $('html').addClass('scrolling');
                        } else {
                            $('html').removeClass('scrolling');
                        }
                    });

                }
                function printEvents() {
                    $itemPrint.change(function () {

                        if (!otakim) {
                            this.checked = false;
                            print();
                            return;
                        }
                    });

                    $itemP.click(function () {

                        print();
                    });

                    $otakimP.click(function () {
                        if (!cd.register()) {
                            cd.pubsub.publish('register', { action: true });
                            return;
                        }
                        if (!checkBoxPermission()) {
                            return;
                        }

                        $('[data-ddcbox]').prop('checked', false).css('visibility', 'hidden');
                        var url = '/item/print/' + '?boxId=' + boxid + '&itemId=' + self.itemid() + '&otakim=true';
                        setTimeout(function () { window.open(url, '_blank'); }, 400);
                        trackEvent('Print otakim');
                    });
                }
                function shareEvents() {
                    $itemDL.click(function () {
                        if (!cd.register()) {
                            cd.pubsub.publish('register', { action: true });
                            return;
                        }
                        //if (!checkBoxPermission()) {
                        //    return;
                        //}


                        var url = '/d/' + boxid + '/' + self.itemid();
                        window.open(url);
                        trackEvent('Download', 'The number of downloads made on item view');
                        cd.pubsub.publish('item_Download', { id: self.itemid() });
                    });



                    $itemFS.click(function () {
                        cd.shareFb(self.copyLink(), //url
                          self.itemName() + '.' + self.extension(), //title
                          self.uniName() ? self.boxName() + ' - ' + self.uniName() : self.boxName(), //caption
                          JsResources.IShared + ' {0}.{1} '.format(self.itemName(), self.extension()) + JsResources.OnCloudents +
                          '<center>&#160;</center><center></center>' + JsResources.CloudentsJoin,
                          null //picture
                          );
                    });

                    $itemCL.click(function (e) {
                        e.preventDefault();
                        this.select();
                    });

                    $itemMsg.click(function () {
                        cd.pubsub.publish('message');
                    });
                }
                function settingsEvents() {
                    $itemSettings.click(function () {
                        if (!cd.register()) {
                            cd.pubsub.publish('register', { action: true });
                            return;
                        }
                        $itemRename.show();
                        $itemName.val(self.itemName()).focus();
                    });

                    $itemRenameSave.click(function (e) {
                        e.preventDefault();
                        var fileName = $itemName.val();
                        var oldFilename = self.itemName();

                        if (checknewFileName()) {
                            saveFileName();
                        } else {
                            cd.notification(JsResources.InvalidFilename);
                            setTimeout(function () {
                                $itemName.focus();
                            }, 50);
                        }


                        function checknewFileName() {
                            var fileCheck = new RegExp("^[^\\\./:\*\?\"<>\|]{1}[^\\/:\*\?\"<>\|]{0,254}$", "i");
                            return fileCheck.test(fileName);
                        }

                        function saveFileName() {
                            $itemRename.hide();

                            dataContext.renameItem({
                                data: { newFileName: fileName, ItemId: self.itemid() },
                                success: function (data) {
                                    //var extension = data.queryString.slice(data.queryString.lastIndexOf('.'), data.queryString.length),
                                    var listItemElement = $('.moreFilesName:contains(' + oldFilename + ')'),
                                       listItem = ko.dataFor(listItemElement[0]),
                                       location = self.copyLink().substring(0, self.copyLink().length - 1);
                                    location = location.substring(0, location.lastIndexOf('/') + 1) + data.queryString + '/';

                                    self.itemName(data.name);
                                    listItem.name(data.name + '.' + self.extension());

                                    self.copyLink(location);
                                    fixHistory(location);
                                    $itemName.val('');
                                },
                                error: function (msg) {
                                    cd.notification(msg);

                                }
                            });
                        }
                        function fixHistory(location) {
                            if (window.history && window.history.replaceState) {
                                cd.historyManager.remove();
                                window.history.replaceState(location, '', location);
                            }
                        }
                    });

                    $itemRenameCancel.click(function () {
                        $itemRename.hide();
                    });

                }
                function fullScreenEvents() {
                    $('#exitFulscrn,#itemModel').click(function () {
                        $body.removeClass(consts.fullscreen);
                        $itemPreview.prepend($previewWrapper);
                        $('#itemPrev,#itemNext').removeClass(consts.disabled).removeAttr(consts.disabled);
                    });

                    $itemPrev.click(prevItem);
                    $itemNext.click(nextItem);

                    $(document).keyup(function (e) {
                        var keyCode = e.keyCode || e.which;
                        if (document.getElementById(consts.item).style.display === 'block') {
                            if (keyCode === 37) {
                                prevItem();
                            }
                            if (keyCode === 39) {
                                nextItem();
                            }
                        }
                    });
                    function prevItem() {
                        if (document.body.className.indexOf(consts.fullscreen) === -1) {
                            return;
                        }

                        trackEvent('fullscreenPrev');
                        $itemNext.removeClass(consts.disabled).removeAttr(consts.disabled);
                        var $item = $('.select').prev('li');
                        $item.find('a').click();
                        if ($item.is(consts.firstChild)) {
                            $(this).addClass(consts.disabled).attr(consts.disabled, consts.disabled);
                        }
                    }

                    function nextItem() {
                        if (document.body.className.indexOf(consts.fullscreen) === -1) {
                            return;
                        }

                        trackEvent('fullscreenNext');
                        $itemPrev.removeClass(consts.disabled).removeAttr(consts.disabled);

                        var $item = $('.select').next('li');
                        $item.find('a').click();;
                        if ($item.is(consts.lastChild)) {
                            $(this).addClass(consts.disabled).attr(consts.disabled, consts.disabled);
                        }
                    }
                }
                function rateEvents() {

                    $rateContainer.click(function (e) {
                        trackEvent('Rate item menu opend', 'User opened the rate menu');
                        e.stopPropagation();

                        if (!cd.register()) {
                            cd.pubsub.publish('register', { action: true });
                            return;
                        }

                        if (rateMenuOpen) {
                            return;
                        }

                        rateMenuOpen = true;

                        if ($.data($rateContainer[0], consts.fetchRate)) {
                            getItemRate();
                        }

                        $rateBtn.toggleClass('clicked');
                        $bubbleText.text($bubbleText.attr('data-step1'));
                    });

                    $rateContainer.on('click', '.star', function (e) {
                        e.stopPropagation();
                        var startWidth = $('.stars .full').width();

                        clearTimeout(ratePopupTimeout);

                        if ($ratePopup.is(':visible')) {
                            setTimeout(function () {
                                $ratePopup.removeClass('show').addClass('rated2');
                            }, 500);
                        } else {
                            $ratePopup.removeClass('show');
                        }


                        if (choosedRate) {
                            return;
                        }

                        var $this = $(this),
                            $rated = $rateBubble.find('.' + consts.rated),
                            index = $this.index(),
                            currentRate = 5 - index;

                        if ($this.index() === $rated.index()) {
                            return; // don't do anything if user selects rating which is already selected
                        }

                        choosedRate = true;

                        setItemRate(currentRate);

                        self.rate(calculateFakeRate(startWidth, currentRate));

                        trackEvent('Rate', 'User rated an item with ' + currentRate + ' stars');

                        toggleStarClass($rated, currentRate);



                        initialRate = currentRate;
                        self.rate();

                        setTimeout(function () {
                            $bubbleText.text($bubbleText.attr('data-step2'));
                            $rateBubble.addClass('closing');
                        }, 1000);
                        setTimeout(function () {
                            $rateBtn.removeClass('clicked');
                            $rateBubble.removeClass('closing');
                            rateMenuOpen = choosedRate = false;
                        }, 2000);
                    });

                    $body.click(function () {
                        if (choosedRate) {
                            return;
                        }
                        $rateBtn.removeClass('clicked');
                        $rateBubble.removeClass('closing');
                        rateMenuOpen = choosedRate = false;
                    });
                }


            }

            //#endregion 

            //#region utilities

            function checkBoxPermission() {
                if (userType === 'none' || userType === 'invite') {
                    cd.notification(JsResources.NeedToFollowBox);
                    return false;
                }

                return true;
            }

            function getNameNoExtension(fileName, type) {
                if (type === consts.link) {
                    return fileName;
                }
                return fileName.substring(0, fileName.lastIndexOf('.'));
            }



            function trackEvent(action, label) {
                analytics.trackEvent('Item', action, label);
            }

            function trackAnnotation(action, label) {
                analytics.trackEvent('Annotation', action, label);
            }

            function print() {
                if (!cd.register()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                //if (!checkBoxPermission()) {
                //    return;
                //}

                var url = '/item/print/' + '?boxId=' + boxid + '&itemId=' + self.itemid();
                var mywindow = window.open(url, '_blank');
                mywindow.onload = function () {
                    mywindow.focus();
                    mywindow.print();
                    mywindow.close();
                };
                trackEvent('Print');
                $itemPrint.prop('checked', false);
            }

            function getItemRate() {
                dataContext.getItemRate({
                    data: { itemId: self.itemid() },
                    success: function (data) {
                        var rate = data.Rate >= 0 ? data.Rate : 0;
                        initialRate = rate;
                        $.data($rateContainer[0], consts.fetchRate, false);
                        var $rated = $rateBubble.find('.' + consts.rated);
                        toggleStarClass($rated, rate);
                    }
                });
            }
            function setItemRate(rate) {
                dataContext.rateItem({
                    data: {
                        itemId: self.itemid(),
                        rate: rate
                    }
                });
            }

            function calculateFakeRate(startWidth, currentRate) {
                var distance = currentRate - startWidth / consts.starWidth,
                    starChange;

                if (startWidth === 0 || initialRate === 0 || self.numberOfViews() === 1) {
                    startWidth = 0;
                    starChange = currentRate;
                }
                else {
                    starChange = distance * Math.random() * (1 / self.numberOfViews());
                }

                return startWidth + starChange * consts.starWidth;

            }

            function toggleStarClass($rated, rate) {
                if (rate === 0)
                    return;
                if ($rated.length) {
                    $rated.toggleClass(consts.rated).text(5 - $rated.index());
                }
                $('#star' + rate).toggleClass(consts.rated).text('');
            }
            //#endregion

            //#region annotations 
            function initializeAnnotation() {

                var rect = {},

                    drag = false,
                    annotationHappen = false,
                    className = 'canvas.newAnnotation';


                $previewWrapper.on('mousedown', className, function (e) {
                    e.stopPropagation();
                    annotationHappen = false;
                    if (e.button === 0) { //left btn
                        var x = $(this).offset();
                        rect.startX = e.pageX - x.left;
                        rect.startY = e.pageY - x.top;
                        rect.w = 0;
                        rect.h = 0;
                        drag = true;
                    }
                })

                .on('mouseup', className, function (e) {
                    e.stopPropagation();
                    if (!drag) {
                        return;
                    }
                    drag = false;
                    if (Math.abs(rect.w * rect.h) > 25) {
                        $commentToggle.prop('checked', true).trigger('change');
                        annotationHappen = true;
                        var $this = $(this);
                        var ctxold = this.getContext('2d');
                        clearRectangle(ctxold);
                        var annotationCanvas = $this.siblings('.annotation');
                        var ctx = annotationCanvas[0].getContext('2d');
                        clearRectangle(ctx);
                        var readViews = $itemPreview[0].querySelectorAll('.readView');
                        for (var i = 0, l = readViews.length; i < l; i++) {
                            readViews[i].classList.add('addAnotationShow');
                        }

                        if (rect.w < 0) {
                            rect.startX += rect.w;
                            rect.w = Math.abs(rect.w);
                        }
                        if (rect.h < 0) {
                            rect.startY += rect.h;
                            rect.h = Math.abs(rect.h);
                        }
                        if (!isLtr) {
                            rect.startX -= $this.width() - annotationCanvas.width();
                        }
                        drawRectangle(ctx, rect.startX, rect.startY, rect.w, rect.h);

                        drawLineToComment(ctx, rect.startX, rect.startY, null, rect.w);
                        addComment(annotationCanvas);
                    }

                })
                .on('mousemove', className, function (e) {
                    if (drag) {
                        var x = $(this).offset();
                        rect.w = (e.pageX - x.left) - rect.startX;
                        rect.h = (e.pageY - x.top) - rect.startY;
                        var ctx = this.getContext('2d');
                        clearRectangle(ctx);
                        drawRectangle(ctx, rect.startX, rect.startY, rect.w, rect.h);
                    }
                });


                //Reply
                //$previewWrapper.on('click', 'input.annnotateAction', function () {
                //    var $this = $(this);
                //    dataContext.replyAnnotation({
                //        data: {
                //            Comment: 'this is a test',
                //            ItemUid: self.itemid(),
                //            ImageId: $this.parents('.readView').data('imageId'),
                //            CommentId: $this.parents('.readView').data('id')
                //        }
                //    })
                //});


                var $AddAnotation = $('#AddAnotation');
                $AddAnotation.find('img').attr('src', cd.userDetail().img);
                $('#annotator').text(cd.userDetail().name);
                $AddAnotation.submit(function (e) {
                    e.preventDefault();
                    if (!checkBoxPermission()) {
                        return;
                    }

                    var subRect = rect;
                    var submitBtn = $(this).find('button[type="submit"]').attr(consts.disabled, consts.disabled);
                    var val = this.querySelector('textarea').value.trim(), _that = $(this);
                    trackAnnotation('annotation Create');
                    dataContext.addAnnotation({
                        data: {
                            Comment: val,
                            X: subRect.startX,
                            Y: subRect.startY,
                            Width: subRect.w,
                            Height: subRect.h,
                            ItemId: self.itemid(),
                            ImageId: _that.data('id')
                        },
                        success: function (retVal) {
                            submitBtn.removeAttr(consts.disabled);
                            annotationList.push(new AnnotationObj({
                                id: retVal,
                                imageId: _that.data('id'),
                                comment: $('<div/>').text(val).html(),
                                x: rect.startX,
                                y: rect.startY,
                                width: rect.w,
                                height: rect.h,
                                creationDate: new Date(), // need to do something
                                userImage: $('#userName').prev().attr('src'),
                                userName: cd.userDetail().name,
                                uid: cd.userDetail().id
                            }));
                            clearAddAnnotation();
                            reProcessAnnotation(commentShow);
                            $commentsNumber.text(annotationList.length).show();
                        },
                        error: function (msg) {
                            msg = msg || {};
                            cd.displayErrors($AddAnotation, msg.error);
                            submitBtn.removeAttr(consts.disabled);
                        }

                    });
                });
                $AddAnotation.find('.btnCancel').click(function () {
                    trackAnnotation('annotationCancel');
                    clearAddAnnotation();
                    reProcessAnnotation(commentShow);
                });

                function addComment(annotationCanvas) {
                    var p = $AddAnotation.detach();
                    cd.resetForm($AddAnotation);
                    annotationCanvas.before(p);
                    $('#annEmptyState').remove();
                    var cssObj = { top: rect.startY - 16 };
                    p.data('id', annotationCanvas[0].id.replace(/\D+/, ''))
                        .css(cssObj).show().find('textarea').val('').focus();

                }

                var isClick = false;
                $previewWrapper.on('mouseenter', '.commentToggle', function () {

                    var $this = $(this),
                    annotation = findAnnotationObjById($this.data('id'));
                    drawAnnotation(annotation);
                    trackAnnotation('hover Annotation Remark');
                })
                .on('click', '.annotation', function () {
                    if (!cd.register()) {
                        cd.pubsub.publish('register', { action: true });
                    }
                })
                .on('mouseleave', '.commentToggle', function () {

                    var $this = $(this),
                    annotation = findAnnotationObjById($this.data('id'));
                    var ctx = getContext(consts.annotation + annotation.imageId);
                    clearRectangle(ctx);
                    renderCommnetByBubble();
                })
                .on('click', '.commentToggle', function (e) {
                    trackAnnotation('click Annotation Remark');

                    isClick = true;
                    e.stopPropagation();
                    $('.commentToggleCk').removeClass('commentToggleCk');
                    var $this = $(this);
                    animationSlide(true);

                    renderCommnetByBubble($this);
                })
                .on('mouseenter', '.readView', function () {
                    trackAnnotation('hover Annotation');
                    if (!commentShow) {
                        return;
                    }

                    var $this = $(this);
                    if ($this.hasClass('addAnotationShow')) {
                        return;
                    }
                    var annotation = findAnnotationObjById($this.data('id')),
                    ctx = getContext('annotation' + annotation.imageId);
                    drawAnnotation(annotation, ctx);
                    drawAnnotationLine(annotation, ctx, $this.position().top + 16);
                })
                .on('mouseleave', '.readView', function () {
                    if (!commentShow) {
                        return;
                    }
                    var $this = $(this);
                    if ($this.hasClass('addAnotationShow')) {
                        return;
                    }
                    var annotation = findAnnotationObjById($this.data('id')),
                    ctx = getContext('annotation' + annotation.imageId);
                    clearRectangle(ctx);

                })
                .on('click', '.show-more', function (e) {
                    trackAnnotation('moreAnnotationClick');
                    e.stopPropagation();
                    var readView = $(this).parents('.readView'),
                    annotation = findAnnotationObjById(readView.data('id'));
                    readView.addClass('moreContinue').find('.annotationTextWpr').addClass('moreState');

                    $('.show-more').hide();

                    var needToInc = readView.height() + readView.position().top - readView.parent().height();
                    if (needToInc > 0) {
                        readView.css('top', Math.max(readView.position().top - needToInc, 0));
                        var ctx = getContext(consts.annotation + annotation.imageId);
                        clearRectangle(ctx);
                        drawAnnotation(annotation, ctx);
                        drawAnnotationLine(annotation, ctx, readView.position().top + 16);
                    }
                    $('.readView').not(readView).addClass('addAnotationShow');
                })
                .on('click', '.deleteBtn', function (e) {
                    trackAnnotation('deleteAnnotationClick');
                    e.stopPropagation();
                    var id = $(this).parents('.readView').data('id'),
                    annotation = findAnnotationObjById(id);

                    var index = annotationList.indexOf(annotation);
                    annotationList.splice(index, 1);
                    reProcessAnnotation(commentShow);
                    if (!annotationList.length) {
                        $commentsNumber.hide();
                    }
                    $commentsNumber.text(annotationList.length).show();

                    dataContext.deleteAnnotation({
                        data: { CommentId: id }
                    });
                });
                $itemPreview.click(function () {
                    if (annotationHappen) {
                        return;
                    }
                    isClick = false;
                    clearAddAnnotation();
                    animationSlide(false);
                    reProcessAnnotation(commentShow);
                });

                function renderCommnetByBubble(bubble) {
                    bubble = bubble || $('.commentToggleCk');
                    isClick = true;

                    if (!bubble.length) {
                        return;
                    }
                    var annotation = findAnnotationObjById(bubble.data('id'));
                    bubble.addClass('commentToggleCk');

                    var ctx = getContext(consts.annotation + annotation.imageId);
                    clearRectangle(ctx);
                    clearComments();
                    drawAnnotation(annotation, ctx);

                    var comment = renderComment(annotation, 0);
                    drawAnnotationLine(annotation, ctx, comment.position().top + 16);
                }
                $('#commentToggleLabel').click(function () {
                    if (commentShow) {
                        trackAnnotation('HideComment');
                    }
                    else {
                        trackAnnotation('ShowComment');
                    }

                });

                $commentToggle.change(function () {
                    commentShow = this.checked;
                    var label = $(this).next()[0];
                    if (commentShow) {
                        label.title = label.getAttribute('data-hidecomments');

                    } else {
                        label.title = label.getAttribute('data-showcomments');
                    }
                    reProcessAnnotation(commentShow);
                    $AddAnotation.hide();

                    animationSlide();
                });
                cd.pubsub.subscribe('windowChanged', function () {
                    animationSlide();
                });
                function animationSlide(isShowComment) {
                    isShowComment = isShowComment || commentShow;
                    var slide = 0;
                    if (!isShowComment) {
                        slide = 0;
                    }
                    else {
                        var annotationListWidth = $('.annotationList').width(), previewWrapperWidth = $previewWrapper.width(),
                            previewWithAnnotationWidth = $('.divWrapper').width() + annotationListWidth;
                        if (previewWrapperWidth - previewWithAnnotationWidth < 0) {
                            slide = ((previewWrapperWidth - $('.divWrapper').width()) / 2) - 10;
                        }
                        else {
                            slide = Math.max((previewWrapperWidth - previewWithAnnotationWidth) / 2, annotationListWidth / 2);
                        }
                    }
                    previewWrapper.style[isLtr ? 'left' : 'right'] = -slide + 'px';
                }
                function clearAddAnnotation() {
                    $('.readView').removeClass('addAnotationShow');
                    $AddAnotation.find('textarea').val('').css('height', '');
                    $AddAnotation.hide();
                }
            }
            function getContext(annotationId) {
                return document.getElementById(annotationId).getContext('2d');
            }
            function drawAnnotation(annotation, ctx) {
                ctx = ctx || getContext(consts.annotation + annotation.imageId);
                drawRectangle(ctx, annotation.x, annotation.y, annotation.width, annotation.height);
            }
            function drawAnnotationLine(annotation, ctx, commentPoistion) {
                ctx = ctx || getContext(consts.annotation + annotation.imageId);
                drawLineToComment(ctx, annotation.x, annotation.y, commentPoistion, annotation.width);
            }
            function findAnnotationObjById(id) {
                var annotation = ko.utils.arrayFirst(annotationList, function (i) {
                    return i.id === id;
                });
                return annotation;
            }
            function clearRectangle(ctx) {
                ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
            }
            function drawRectangle(ctx, x, y, w, h) {
                ctx.fillStyle = "rgba(3,133,200,0.2)";
                ctx.strokeStyle = "#fff";
                ctx.fillRect(x + 0.5, y + 0.5, w, h);
                ctx.strokeRect(x + 0.5, y + 0.5, w, h);

            }
            function drawLineToComment(ctx, startx, starty, endy, width) {
                endy = endy || starty;
                ctx.beginPath();

                ctx.strokeStyle = "#0371c8";
                ctx.lineWidth = 1;
                if (isLtr) {
                    ctx.moveTo(startx, starty + 0.5);
                    ctx.lineTo(ctx.canvas.width - 50, starty + 0.5);
                    ctx.lineTo(ctx.canvas.width, endy + 0.5);
                }
                else {
                    ctx.moveTo(startx + width, starty + 0.5);
                    ctx.lineTo(50, starty + 0.5);
                    ctx.lineTo(0, endy + 0.5);
                }
                ctx.stroke();
            }

            function processAnnotationComments(isShowComment) {
                isShowComment = isShowComment || $commentToggle.prop('checked');
                var nextTop = 0, imgId = 0;
                defferedArray = [];
                if (!isShowComment) {
                    $('.annotationList').hide();
                    processCommentRemark();
                    return;
                }
                annotationList.sort(sort);
                if (!annotationList.length) {
                    var p = $('#annEmptyState').clone().show();
                    $('#annotationList0').append(p).show();
                    return;
                }
                for (var i = 0; i < annotationList.length; i++) {
                    var annotation = annotationList[i],
                    canvas = document.getElementById(consts.annotation + annotation.imageId);

                    if (annotation.imageId > imgId) {
                        imgId = annotation.imageId;
                        nextTop = 0;
                    }
                    var obj = renderComment(annotation, nextTop, canvas);
                    nextTop = obj.position().top + obj.height() + 24;
                }

            }
            function clearComments() {
                $('.annotationList').empty();
            }
            function clearCommentRemark() {
                $('.commentToggle').remove();
            }
            function renderComment(annotation, nextTop, canvas, cropComment) {
                var cPartOfImage = 16;
                var topOfAnnotation = Math.max(annotation.y, nextTop + cPartOfImage);
                canvas = canvas || document.getElementById(consts.annotation + annotation.imageId);
                if (typeof cropComment !== 'boolean') {
                    cropComment = cropComment || true;// || true;
                }
                var cssObj = { top: topOfAnnotation - cPartOfImage };
                var x = $(cd.attachTemplateToData('anotationTmpl', annotation)).css(cssObj).data({
                    id: annotation.id,
                    imageId: annotation.imageId
                });
                if (annotation.uid === cd.userDetail().id) {
                    x.addClass('author');
                }
                $('#annotationList' + annotation.imageId).append(x).show();
                if (cropComment) {
                    if (x.find('.annotationText').height() < x.find('.annotationTextWpr').height()) {
                        x.find('.show-more').remove();
                    }
                    //x.find('.annotationText').dotdotdot({
                    //    after: $(document.createElement('button')).attr('type', 'button').addClass('more').text(JsResources.More),
                    //    height: 55
                    //});
                }
                cd.parseTimeString(x.find('[data-time]'));
                return x;
            }
            function processCommentRemark() {
                var nextTop = 0, imgId = 0;
                annotationList.sort(sort);
                for (var i = 0; i < annotationList.length; i++) {
                    var annotation = annotationList[i],
                    btn = document.createElement('button');
                    if (annotation.imageId > imgId) {
                        imgId = annotation.imageId;
                        nextTop = 0;
                    }

                    var top = Math.max(nextTop, annotation.y + 3);

                    btn.className = 'commentToggle';
                    btn.setAttribute('type', 'button');
                    //btn.type = 'button'; - safari raise error on this
                    btn.style.top = top + 'px';
                    $.data(btn, 'id', annotation.id);
                    var itemIndex = document.getElementById('itemIndex' + annotation.imageId);
                    if (itemIndex) {
                        itemIndex.appendChild(btn);
                    }
                    nextTop = top + 20;
                }
            }
            function sort(c1, c2) {
                if (c1.imageId < c2.imageId) return -1;
                if (c1.imageId > c2.imageId) return 1;
                if (c1.y < c2.y) return -1;
                return 1;
            }

            function reProcessAnnotation(isShowComment) {

                clearComments();
                clearCommentRemark();
                var canvases = document.getElementsByClassName(consts.annotation);
                for (var i = 0; i < canvases.length; i++) {
                    var ctx = canvases[i].getContext('2d');
                    ctx.clearRect(0, 0, canvases[i].width, canvases[i].height);
                }
                processAnnotationComments(isShowComment);
            }

            //#endregion
        }

    })(jQuery, window.cd.data, window.ko, window.cd, window.JsResources, window.cd.analytics, Modernizr);
});