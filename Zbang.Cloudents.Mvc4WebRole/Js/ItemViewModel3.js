/// <reference path="../Views/Item/Index.cshtml" />
/// <reference path="../Views/Item/_PreviewImage.cshtml" />
(function ($, dataContext, ko, cd, ZboxResources, analytics, Modernizr) {
    "use strict";


    var cDISABLED = 'disabled';
    if (window.scriptLoaded.isLoaded('ivm')) {
        return;
    }

    cd.loadModel('item', 'ItemContext', registerKOItem);


    //var itemPreviewGuid;

    function registerKOItem() {
        ko.applyBindings(new ItemViewModel(), document.getElementById('item'));

    }

    function ItemViewModel() {
        var self = this, boxid,
            $itemMoreFiles = $('#item_moreFiles'),
            ownerid,
            blobName = '',
            firstTime = true,
            loaded = true,
            otakim = false,
            annotationList = [],
            previewWrapper = document.getElementById('previewWrapper'),
            $previewWrapper = $(previewWrapper),
            $itemPreview = $('#itemPreview'),
            itemType, userType,
            CAnnotation = 'annotation',
            isLtr = $('html').css('direction') === 'ltr',
            /*We need that to excecute annotation when everything is complete*/
            defferedArray = [],
            defferedItemShow = new $.Deferred(),
            commentShow = false;




        function Item(data) {
            var that = this;
            data = data || {};
            that.name = data.name;
            that.uid = ko.observable(data.id);
            that.uploader = data.owner;
            that.userid = data.ownerId;
            that.type = data.type;
            that.itemUrl = data.url + '?r=item&s=items';
            that.select = ko.computed(function () {
                return that.uid() === self.itemid();
            });
            that.nameNoType = ko.computed(function () {
                var name = that.name;
                return data.type === 'File' ? name.substring(0, name.indexOf('.')) : name;
            });
            that.extension = getExtension(that.name, that.type);
            that.extensionColor = getExtensionColor(that.name, that.type);
        }
        function getExtensionColor(filename, type) {
            var prefix = 'mF';
            if (type === 'Link') {
                return prefix + 'link';
            }
            var cssClass = '';
            var extension = getExtension(filename, type);
            if (extension.length > 3) {
                cssClass += 'fourLetterExtension ';
            }
            return cssClass += prefix + extension.toLowerCase();
        }
        function getExtension(filename, type) {
            if (type === 'Link') {
                return 'www';
            }
            var a = filename.split(".");
            if (a.length === 1 || (a[0] === "" && a.length === 2)) {
                return "";
            }
            return a.pop().substring(0, 4);
        }

        cd.pubsub.subscribe('item', function (data) {
            self.itemid(parseInt(data.id, 10));
            if (boxid !== data.boxid) {
                boxid = data.boxid;
                initialItems();
            }
            if (firstTime) {
                registerEvents();
            }
            getItem();
        });

        cd.pubsub.subscribe('itemclear', function () {

            previewWrapper.innerHTML = '';
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
            $('#commentToggle').prop('checked', false).trigger('change');
            //cd.pubsub.publish('destroy_clipboard', $('#item_CL'));
            commentShow = false;
            document.getElementById('itemShare').checked = false;
            document.getElementById('itemPrint').checked = false;
            //$('.printDD').removeClass('showOtakim');
        });

        function getItem() {
            var data = document.getElementById('item').getAttribute('data-data');
            if (data) {
                var dataObj = JSON.parse(data, cd.isoDateReviver);
                document.getElementById('item').removeAttribute('data-data');
                onSuccess(dataObj);
                return;
            }

            dataContext.getItem({
                data: {
                    BoxUid: boxid,
                    itemId: self.itemid(),
                    uniName: cd.getParameterFromUrl(1)
                    //width: screen.width,
                    //height: screen.height
                },
                success: function (data) {
                    onSuccess(data);

                }
            });

            function onSuccess(data) {
                changeLayout(data);
                ownerid = data.ownerUid;
                userType = data.userType;
                otakim = data.otakim;
                self.deleteAllow(checkDeleteAllow(data.userType));
                if (!cd.firstLoad) {
                    cd.setTitle('{0} | {1}.{2} | Cloudents'.format(self.boxName(), self.itemName(), self.extension()));
                }
                var itemPageLoad = new $.Deferred();
                defferedArray.push(itemPageLoad);
                cd.pubsub.publish('item_load', null, function () {
                    cd.innerScroll($itemMoreFiles, $(window).height() - $itemMoreFiles.offset().top);
                    itemPageLoad.resolve();
                });

                loaded = true;
                defferedArray.push(getAnnotation());
                defferedArray.push(getPreview());


                cd.pubsub.publish('perm', data.userType);
            }
        }
        self.itemid = ko.observable();
        self.ownerName = ko.observable();
        self.ownerImage = ko.observable();
        self.ownerId = ko.observable();
        self.update = ko.observable();
        self.itemName = ko.observable();
        self.boxName = ko.observable();
        self.copyLink = ko.observable();
        self.download = ko.observable();
        self.numberOfViews = ko.observable();
        self.deleteAllow = ko.observable();
        self.items = ko.observableArray([]);
        self.extension = ko.observable();
        self.extensionColor = ko.observable();
        self.rate = ko.observable();
        self.boxurl = ko.observable();
        self.uniName = ko.observable();

        self.deleteItem = function () {
            if (!confirm(ZboxResources.SureYouWantToDelete + ' ' + self.itemName() + "?")) {
                return;
            }
            cd.sessionStorageWrapper.clear();
            dataContext.removeItem({
                data: { itemId: self.itemid(), BoxUid: boxid },
                success: function () {
                    cd.pubsub.publish('nav', self.boxurl());
                }
            });
           
        }
        function changeLayout(data) {

            var annotation = $('#AddAnotation').hide().detach();
            annotation.find('textarea').css('height', ''); //need to be b4 previewWrapper
            $('.itemRight').append(annotation);
            previewWrapper.innerHTML = '';
            $('#item_down').text(data.NDownloads || '0'); //bug 965

            self.ownerName(data.owner)
            .ownerImage(data.ownerImg)
            .ownerId(data.ownerUid)
            .uniName(data.uniName)
            .update(cd.dateToShow(data.updateTime))
            .itemName(data.nameWOExtension || data.name)            
            .numberOfViews(data.numberOfViews || 1);
            blobName = data.blob;
            itemType = data.type;
            self.extension(getExtension(data.name, itemType))
            .extensionColor(getExtensionColor(data.name, itemType))
            .copyLink(cd.location())
            .rate(69 / 5 * data.rate);
            $('#commentToggle').prop('checked', false).trigger('change');
            var $rated = $('.rated');
            if ($rated.length) {
                $rated.toggleClass('rated').text(5 - $rated.index());
            }
            $.data($('#rateContainer')[0], 'fetchrate', true);
            //cd.pubsub.publish('init_clipboard', $('#item_CL'));

            //back button
            var prevData = cd.prevLinkData('item');
            if (prevData) {
                self.boxurl(prevData.url)
                    .boxName(prevData.title.split(' | ')[0]); //takes search from Search | * | Cloudents
            } else {
                self.boxurl(data.boxUrl).boxName(data.boxName)
            }
        }
        function checkDeleteAllow(userType) {
            if (userType < 2) {
                return false;
            }

            if (!(userType > 1 && userType === 3 || ownerid === cd.userDetail().nId)) {
                return false;
            }
            return true;
        }

        //function putPreview(data) {
        //    previewWrapper.insertAdjacentHTML('beforeend', data);
        //    //self.preview(data);
        //    isCommentShow();
        //    // self.fullscreenVisible(fullscreenCheck());
        //}

        function initialItems() {
            self.items([]);
            //we can get the data from box before
            dataContext.getItems({
                data: {
                    BoxUid: boxid, pageNumber: 0,
                    uniName: cd.getParameterFromUrl(1),
                    boxName: cd.getParameterFromUrl(3)
                },
                success: function (data) {
                    var elems = $.map(data.dto, function (i) { return new Item(i); });
                    self.items.push.apply(self.items, elems);
                }
            });
        }

        function isCommentShow() {
            if (document.getElementsByClassName('divWrapper').length) {
                $('.commentBtn').parent().show();
            }
            else {
                $('.commentBtn').parent().hide();
            }
        }
        //function fullscreenCheck() {
        //    var $elem = $('.previewWrapper');
        //    if ($elem.find('.previewFailed').length) {
        //        return false;
        //    }
        //    var elem = $elem[0];
        //    if (elem === undefined) {
        //        return false;
        //    }

        //    //return Modernizr.fullscreen;
        //    return true;
        //}
        // self.fullscreenVisible = ko.observable(false);
        self.fullscreen = function () {
            $('#commentToggle').prop('checked', false).trigger('change');
            $('body').addClass('fulscrn');
            //$('#itemFulscrn').addClass('show');
            $previewWrapper.prependTo('#itemFulscrn');
            trackEvent('Full screen');

            var $selected = $('.select'), $arrow;
            if (!$selected.length) {
                return;
            }


            if ($selected.is(':first-child')) {
                $arrow = $('#itemPrev');
            }
            else if ($selected.is(':last-child')) {
                $arrow = $('#itemNext');
            }

            if ($arrow.length > 0) {
                $arrow.addClass(cDISABLED).attr(cDISABLED, cDISABLED);
            }
        };
        self.renameItem = function (m, e) {
            if (!cd.register()) {
                cd.pubsub.publish('register');
                return;
            }
            var d = cd.contentEditableFunc(e.target, checknewFileName);


            function checknewFileName(fileName) {

                var fileCheck = new RegExp("^[^\\\./:\*\?\"<>\|]{1}[^\\/:\*\?\"<>\|]{0,254}$", "i");
                if (fileCheck.test(fileName)) {
                    dataContext.renameItem({
                        data: { newFileName: fileName, ItemId: self.itemid() },
                        success: function (data) {
                            self.itemName(data);

                            d.finish();
                        },
                        error: function (msg) {
                            cd.notification(msg);

                        }
                    });
                }
                else {
                    cd.notification(ZboxResources.InvalidFilename);
                }
            }
        };

        var processLike = false;

        function trackEvent(action, label) {
            analytics.trackEvent('Item', action, label);
        }
        function trackAnnotation(action, label) {
            analytics.trackEvent('Annotation', action, label);
        }


        function registerEvents() {
            firstTime = false;
            $.data($('#rateContainer')[0], 'fetchrate', true);

            //download
            document.getElementById('item_DL').addEventListener('click', function () {
                if (!cd.register()) {
                    cd.unregisterAction(this);
                    return;
                }
                //$('#item_DL').click(function () {
                var url = '/d/' + boxid + '/' + self.itemid();
                window.open(url);
                trackEvent('Download', 'The number of downloads made on iten view');
                cd.pubsub.publish('item_Download', { id: self.itemid() });

            });
            $('#itemPrint').change(function (e) {
                if (!otakim) {
                    this.checked = false;
                    print();
                    return;
                }
                //$('.printDD').addClass('showOtakim');
            });
            $('#item_P').click(function () {
                print();
            })
            function print() {
                if (!cd.register()) {
                    cd.unregisterAction(this);
                    return;
                }
                //if (itemType === 'Link') {
                //    window.open(self.itemName());
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
                $('#itemPrint').prop('checked', false)
            }
            $('#Otakim_P').click(function () {
                if (!cd.register()) {
                    cd.unregisterAction(this);
                    return;
                }
                var url = '/item/print/' + '?boxId=' + boxid + '&itemId=' + self.itemid() + '&otakim=true';
                var mywindow = window.open(url, '_blank');
                trackEvent('Print otakim');
            })

            document.getElementById('item_FS').addEventListener('click', function () {
                cd.shareFb(cd.location(), //url
                    self.itemName() + '.' + self.extension(), //title
                    self.uniName() ? self.boxName() + ' - ' + self.uniName() : self.boxName(), //caption
                    JsResources.IShared + ' {0}.{1} '.format(self.itemName(), self.extension()) + JsResources.OnCloudents + '<center>&#160;</center><center></center>' + JsResources.CloudentsJoin,
                    null //picture
                    );
            });


            document.getElementById('item_CL').addEventListener('click', function (e) {
                e.preventDefault();
                this.select();
            });

            $('#item_msg').click(function () {
                cd.pubsub.publish('message',
                    { text: ZboxResources.FindThisInteresting + '\n\u200e“' + self.itemName() + '” - ' + cd.location() }
                    );
            });

            var x = $(window).height() / 3;
            $itemPreview.scroll(function () {
                if ($itemPreview.scrollTop() >= $itemPreview[0].scrollHeight - $itemPreview.height() - x) {
                    getPreview();
                }

            });
            cd.pubsub.subscribe('windowChanged', function () {
                //put the 10 px dont know why atm
                cd.innerScroll($itemMoreFiles, $(window).height() - $itemMoreFiles.offset().top - 10);
            });
            initializeAnnotation();

            cd.pubsub.subscribe('item_show', function () {
                defferedItemShow.resolve();
            });

            //fullscreen
            $('#exitFulscrn,#itemModel').click(function () {
                $('body').removeClass('fulscrn');
                //  $previewWrapper
                $('#previewWrapper').prependTo('#itemPreview');
                $('#itemPrev,#itemNext').removeClass(cDISABLED).removeAttr(cDISABLED);
            });
            $('#itemPrev').click(function () {
                prevItem();
            });
            $('#itemNext').click(function () {
                nextItem()
            });

            $(document).keyup(function (e) {
                var keyCode = e.keyCode || e.which;
                if (document.getElementById('item').style.display === 'block') {
                    if (keyCode === 37) {
                        prevItem();
                    }
                    if (keyCode === 39) {
                        nextItem();
                    }
                }
            });
            function prevItem() {
                trackEvent('fullscreenPrev');
                $('#itemNext').removeClass(cDISABLED).removeAttr(cDISABLED);
                var $item = $('.select').prev('li');
                $item.find('a').click();
                if ($item.is(':first-child')) {
                    $(this).addClass(cDISABLED).attr(cDISABLED, cDISABLED);
                }
            }

            function nextItem() {
                trackEvent('fullscreenNext');
                $('#itemPrev').removeClass(cDISABLED).removeAttr(cDISABLED);

                var $item = $('.select').next('li');
                $item.find('a').click();;
                if ($item.is(':last-child')) {
                    $(this).addClass(cDISABLED).attr(cDISABLED, cDISABLED);
                }
            }

            $(document).scroll(function () {
                if (document.body.className === 'fulscrn') {
                    if ($(document).scrollTop() >= document.body.scrollHeight - $itemPreview.height() - x) {
                        getPreview();
                    }
                }
            });

            $('#item_moreFiles').on('click', 'a', function (e) {
                if (self.itemid() === ko.dataFor(e.target).uid()) {
                    e.preventDefault();
                    return false;
                }
                trackEvent('move to a different item');
            });

            //Rate system start
            var $bubble = $('#rateBubble'), $bubbleText = $bubble.find('.bubbleText'), $rateBtn = $('#rateBtn'), rated = 'rated', menuOpened = false, choosedRate = false, initialRate = 0, $rateContainer = $('#rateContainer');
            $rateContainer.click(function (e) {
                trackEvent('Rate item menu opend', 'User opened the rate menu');
                e.stopPropagation();

                if (!cd.register()) {
                    cd.unregisterAction(this);
                    return;
                }

                if (menuOpened)
                    return;

                menuOpened = true;

                if ($.data($rateContainer[0], 'fetchrate'))
                    dataContext.getItemRate({
                        data: { itemId: self.itemid() },
                        success: function (data) {
                            var rate = data.Rate >= 0 ? data.Rate : 0;
                            initialRate = rate;
                            $.data($rateContainer[0], 'fetchrate', false);
                            var $rated = $bubble.find('.' + rated);
                            toggleStarClass($bubble, $rated, rated, rate);
                        }
                    });

                $rateBtn.toggleClass('clicked');
                $bubbleText.text($bubbleText.attr('data-step1'));
            });

            $('.star').click(function (e) {
                e.stopPropagation();
                if (choosedRate)
                    return;
                trackEvent('Rate', 'User rated an item');
                choosedRate = true;
                var $this = $(this);
                var $rated = $bubble.find('.' + rated);
                var currentRate = 5 - $this.index();
                if ($this.index() === $rated.index()) {
                    return; // don't do anything if user selects rating which is already selected
                }

                //                $.data($rateContainer[0], 'lastrate', $this.index());


                toggleStarClass($bubble, $rated, rated, currentRate);

                dataContext.rateItem({
                    data: {
                        itemId: self.itemid(),
                        rate: currentRate
                    }
                });

                var startWidth = $('.stars .full').width(), clickWidth = (5 - $this.index()), distance = clickWidth - startWidth / 13.8;
                var starChange;
                //if (startWidth === 0 && self.numberOfViews() <=1)
                if (startWidth === 0 || initialRate === 0) {
                    startWidth = 0;
                    starChange = currentRate;
                }
                else
                    starChange = distance * Math.random() * (1 / self.numberOfViews());



                self.rate(startWidth + starChange * 13.8);

                setTimeout(function () {
                    $bubbleText.text($bubbleText.attr('data-step2'));
                    $bubble.addClass('closing');
                }, 1000);
                setTimeout(function () {
                    $rateBtn.removeClass('clicked');
                    $bubble.removeClass('closing');
                    menuOpened = choosedRate = false;
                }, 2000);
            });

            $('body').click(function (e) {
                if (choosedRate)
                    return;
                $rateBtn.removeClass('clicked');
                $bubble.removeClass('closing');
                menuOpened = choosedRate = false;
            });
        }

        function toggleStarClass($bubble, $rated, rated, rate) {
            if (rate === 0)
                return;
            if ($rated.length) {
                $rated.toggleClass(rated).text(5 - $rated.index());
            }
            $('#star' + rate).toggleClass(rated).text('');
        }
        function getPreview() {
            if (!loaded) {
                return;
            }
            if ($('.previewFailed').length) {
                return;
            }
            if ($('.pageLoader').length) {
                return;
            }
            var imagesNumber = previewWrapper.querySelectorAll('img.imageContent').length;

            if (!(imagesNumber === 0 || imagesNumber > 2)) {
                return;
            }

            loaded = false;
            if (imagesNumber > 0 && !cd.register()) {
                cd.unregisterAction();
                return;
            }
            $('#commentToggle').attr(cDISABLED, cDISABLED);

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
            var parametes = {
                blobName: blobName,
                imageNumber: imagesNumber,
                uid: self.itemid(),
                width: screen.width,// $previewWrapper.width(), this could be 0
                height: screen.height,// $previewWrapper.height(),this could be 0
                boxUid: boxid,
            };
            return dataContext.preview({
                data: parametes,
                success: function (retVal, query) {
                    if (query.blobName !== blobName) {
                        return;
                    }
                    if (query.imageNumber !== previewWrapper.querySelectorAll('img.imageContent').length) {
                        return;
                    }

                    if ($.trim(retVal.preview)) {
                        loaded = true;
                    }
                    previewWrapper.insertAdjacentHTML('beforeend', retVal.preview);
                    //bug 349 scroll issue in iframe in ipad
                    if (/iPhone|iPod|iPad/.test(navigator.userAgent))
                        $('iframe.iframeContent').wrap(function () {
                            var $this = $(this);
                            return $('<div />').addClass('iframeContent').css({
                                overflow: 'auto',
                                '-webkit-overflow-scrolling': 'touch'
                            });
                        });

                    var images = $previewWrapper.find('img.imageContent');

                    var newImages = $previewWrapper.find('img[data-new="true"]');

                    newImages.each(function (i, e) {
                        var y = new $.Deferred();
                        $(e).load(y, function (e) {

                            $.when(defferedItemShow).done(function () {
                                initializeCanvas(e);
                            });
                        });

                        defferedArray.push(y);
                    });
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

                        canvas.className = CAnnotation;
                        canvas.width = $(parent).width() - (($(parent).width() - imgWidth) / 2) + 4;
                        canvas.height = imgHeight;
                        canvas.id = CAnnotation + index;
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
                        //  processAnnotationComments();
                        reProcessAnnotation();
                        $('#commentToggle').removeAttr(cDISABLED);
                    });
                    // no need to this every time
                    // self.fullscreenVisible(fullscreenCheck());
                    isCommentShow();
                },
                always: function () {
                    //loader.remove doesnt work
                    $previewWrapper.find('.pageLoader,.pageLoaderImg,.smallLoader').remove();
                }

            });

            //}
        }

        var request = true;
        self.flagItem = function () {
            if (!cd.register()) {
                cd.pubsub.publish('register');
                return;
            }
            trackEvent('Flag item');
            var $flagItemDialog = $('#flagItemDialog');

            if (!$flagItemDialog.length && request) {
                request = false;
                dataContext.badItemPopUp({
                    success: function (data) {
                        $('#item').append(data);

                        registerPopEvent();

                    }
                });
            }
            else {
                $flagItemDialog.show();
            }
        };

        function registerPopEvent() {
            var $flagItemDialog = $('#flagItemDialog'), $flagOther = $('#flag_other');
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
                    $flagItemDialog.find(':submit').attr(cDISABLED, cDISABLED);
                }
                else {
                    $flagItemDialog.find(':submit').removeAttr(cDISABLED);
                }

            });
            $flagOther.keyup(function () {
                if ($flagOther.val() === '') {
                    $flagItemDialog.find(':submit').attr(cDISABLED, cDISABLED);
                }
                else {
                    $flagItemDialog.find(':submit').removeAttr(cDISABLED);
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
                    fdata.push({ name: 'ItemUid', value: self.itemid() });
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
                    $('#commentToggle').prop('checked', true).trigger('change');
                    annotationHappen = true;
                    var $this = $(this);
                    var ctxold = this.getContext('2d');
                    clearRectangle(ctxold);
                    var annotationCanvas = $this.siblings('.annotation');
                    var ctx = annotationCanvas[0].getContext('2d');
                    //drawAnnotation(ctx);
                    clearRectangle(ctx);
                    $('.readView').addClass('addAnotationShow');

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
                    //if (rect.w < 0) {
                    //    Math.abs(rect.w);
                    //    rect.startX = rect.startX - rect.w;
                    //}
                    //Math.abs(rect.w);
                    //Math.abs(rect.h);
                    //rect.startX = rect.startX - rect.w;
                    //rect.startY = rect.startY - rect.h;

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
            //$AddAnotation.find('textarea').elastic();
            $AddAnotation.find('img').attr('src', cd.userDetail().img);
            $('#annotator').text(cd.userDetail().name);
            $AddAnotation.submit(function (e) {
                e.preventDefault();
                if (userType === 'none' || userType === 'invite') {
                    cd.notification(ZboxResources.NeedToFollowBox);
                    return;
                }
                var subRect = rect;
                var submitBtn = $(this).find('button[type="submit"]').attr(cDISABLED, cDISABLED);
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
                        submitBtn.removeAttr(cDISABLED);
                        annotationList.push(new AnnotationObj({
                            Id: retVal,
                            ImageId: _that.data('id'),
                            Comment: $('<div/>').text(val).html(),
                            X: rect.startX,
                            Y: rect.startY,
                            Width: rect.w,
                            Height: rect.h,
                            CreationDate: new Date(), // need to do something
                            UserImage: $('#userName').prev().attr('src'),
                            UserName: cd.userDetail().name,
                            Uid: cd.userDetail().id
                        }));
                        clearAddAnnotation();
                        reProcessAnnotation(commentShow);
                    },
                    error: function (msg) {
                        msg = msg || {};
                        cd.displayErrors($AddAnotation, msg.error);
                        submitBtn.removeAttr(cDISABLED);
                    }

                });
            });
            $AddAnotation.find('.btnCancel').click(function () {
                trackAnnotation('annotationCancel');
                clearAddAnnotation();
                reProcessAnnotation(commentShow);
                //$previewWrapper.removeClass('annotateState');
                //$('.annotationList').removeClass('wide');

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
                //if (isClick) {
                //    return;
                //}
                var $this = $(this),
                annotation = findAnnotationObjById($this.data('id'));
                drawAnnotation(annotation);
                trackAnnotation('hover Annotation Remark');
            })
            .on('click', '.annotation', function () {
                if (!cd.register()) {
                    cd.pubsub.publish('register');
                }
            })
            .on('mouseleave', '.commentToggle', function () {
                //if (isClick) {
                //    return;
                //}
                var $this = $(this),
                annotation = findAnnotationObjById($this.data('id'));
                var ctx = getContext(CAnnotation + annotation.imageId);
                clearRectangle(ctx);
                renderCommnetByBubble();
            })
            .on('click', '.commentToggle', function (e) {
                trackAnnotation('click Annotation Remark');
                //$previewWrapper.addClass('annotateState');
                //$('.annotationList').addClass('wide');
                isClick = true;
                e.stopPropagation();
                $('.commentToggleCk').removeClass('commentToggleCk');
                var $this = $(this);
                animationSlide(true);
                //annotation = findAnnotationObjById($this.data('id'));
                //$this.addClass('commentToggleCk');

                //var ctx = getContext(CAnnotation + annotation.imageId);
                //clearRectangle(ctx);
                //clearComments();
                //drawAnnotation(annotation, ctx);

                //var comment = renderComment(annotation, 0);
                //drawAnnotationLine(annotation, ctx, comment.position().top + 16);
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
                //readView.find('.show-less').show();
                //readView.find('.annotationText').trigger('destroy.dot').empty().text(annotation.comment);
                var needToInc = readView.height() + readView.position().top - readView.parent().height();
                if (needToInc > 0) {
                    readView.css('top', Math.max(readView.position().top - needToInc, 0));
                    var ctx = getContext(CAnnotation + annotation.imageId);
                    clearRectangle(ctx);
                    drawAnnotation(annotation, ctx);
                    drawAnnotationLine(annotation, ctx, readView.position().top + 16);
                }
                $('.readView').not(readView).addClass('addAnotationShow');
            })
                //.on('click')
            .on('click', '.deleteBtn', function (e) {
                trackAnnotation('deleteAnnotationClick');
                e.stopPropagation();
                var id = $(this).parents('.readView').data('id'),
                annotation = findAnnotationObjById(id);

                var index = annotationList.indexOf(annotation);
                annotationList.splice(index, 1);
                reProcessAnnotation(commentShow);
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
                //$previewWrapper.removeClass('annotateState');
                //$('.annotationList').removeClass('wide');
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

                var ctx = getContext(CAnnotation + annotation.imageId);
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
            $('#commentToggle').change(function () {
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
            ctx = ctx || getContext(CAnnotation + annotation.imageId);
            drawRectangle(ctx, annotation.x, annotation.y, annotation.width, annotation.height);
        }
        function drawAnnotationLine(annotation, ctx, commentPoistion) {
            ctx = ctx || getContext(CAnnotation + annotation.imageId);
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

        function AnnotationObj(data) {
            data = data || {};
            var _that = this;
            _that.id = data.Id;
            _that.imageId = data.ImageId;
            _that.comment = data.Comment.replace(/\n/g, '<br/>');
            _that.x = data.X;
            _that.y = data.Y;
            //_that.replies = $.map(data.Replies || {}, function (reply) { return new AnnotationObj(reply) });
            _that.width = data.Width;
            _that.height = data.Height;
            if (!(data.CreationDate instanceof Date)) {
                data.CreationDate = new Date(parseInt(data.CreationDate.replace("/Date(", "").replace(")/", ""), 10));
            }
            _that.date = cd.dateToShow(data.CreationDate);
            _that.userImage = data.UserImage;
            _that.userName = data.UserName;
            _that.uid = data.Uid;
            //_that.temp = data.temp || false;
        }

        function getAnnotation() {
            annotationList = [];
            return dataContext.getAnnotation({
                data: { itemId: self.itemid() },
                success: function (retVal) {

                    for (var i = 0; i < retVal.length; i++) {
                        annotationList.push(new AnnotationObj(retVal[i]));
                    }
                }
            });
        }
        function processAnnotationComments(isShowComment) {
            isShowComment = isShowComment || $('#commentToggle').prop('checked');
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
                canvas = document.getElementById(CAnnotation + annotation.imageId);

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
            canvas = canvas || document.getElementById(CAnnotation + annotation.imageId);
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
                //    after: $(document.createElement('button')).attr('type', 'button').addClass('more').text(ZboxResources.More),
                //    height: 55
                //});
            }
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
            var canvases = document.getElementsByClassName(CAnnotation);
            for (var i = 0; i < canvases.length; i++) {
                var ctx = canvases[i].getContext('2d');
                ctx.clearRect(0, 0, canvases[i].width, canvases[i].height);
            }
            processAnnotationComments(isShowComment);
        }
    }
})(jQuery, window.cd.data, window.ko, window.cd, window.ZboxResources, window.cd.analytics, Modernizr);