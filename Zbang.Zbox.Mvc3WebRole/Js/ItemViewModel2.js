(function (mmc, $) {
    "use strict";
    if (mmc.page.item) {
        mmc.item = function () {
            new ItemViewModel();
        }
    }
    function ItemViewModel() {
        var self = this;
        var boxid = Zbox.getParameterByName('BoxUid'), itemid = Zbox.getParameterByName('ItemUid'),
        owner = $('#ownerName').data('uid'), type = $('#itemName').data('type'),
        currentPage = 0, countofItem = 0, items = [], currentPosition = 0, cacheExpire = 20,
        $itempopUp = $('#itempopUp');
        //regularWidth = $('#content').width(), maxWidth = 918;

        if (boxid === '') {
            throw 'boxid cannot be null';
        }
        if (itemid === '') {
            throw 'itemid cannot be null';
        }



        initialItems();
        registerEvents();
        function registerEvents() {
            $('body').keyup(function (e) {
                if (e.keyCode !== 39 && e.keyCode !== 37) {
                    return;
                }
                var target = e.target.tagName;
                if (target === 'INPUT' || target === 'TEXTAREA') {
                    return;
                }
                if (e.keyCode === 37) {
                    loadItem(items[prevItem()]);
                }
                else {
                    loadItem(items[nextItem()]);
                }
            });
            $('#prev').click(function () {
                loadItem(items[prevItem()]);
            });
            $('#next').click(function () {
                loadItem(items[nextItem()]);
            });
            $('#delete').click(function () {
                if (mmc.register) {
                    deleteItem();
                }
            });
            $itempopUp.click(function (e) {
                $('.modal').click();
            });

            $('#zoomClose').click(function () {
                $('.modal').click();
            });

            var cacheEnlarge = {};//TODO: need to put cache
            $('#expand').click(function () {
                var $itemDisplay = $('.itemDisplay');
                if (!$itemDisplay.length) {
                    return;
                }

                var content = $itempopUp.css('display', 'table').find('.content');
                mmc.modal(function () {
                    $itempopUp.hide();
                    content.find('.itemDisplay').remove();
                }, $itempopUp);
                var tagName = $itemDisplay[0].tagName, documentElement = document.documentElement;
                switch (tagName) {
                    case 'IMG': 
                        if ($itemDisplay.width() === 585) {
                            var request = new ZboxAjaxRequest({
                                url: "/Item/Enlarge",
                                data: { ItemUid: itemid, BoxUid: boxid, width: documentElement.clientWidth, height: documentElement.clientHeight },
                                success: function (data) {
                                    content.append(data.Preview);
                                }
                            });
                            request.Post();
                        }
                        else {
                            content.append(document.getElementById('content').innerHTML);
                        }
                        break;
                    case 'IFRAME': 
                        var pageContent = $('#content').html();
                        content.append(pageContent);
                        content.find('iframe').attr({
                            width: documentElement.clientWidth * 0.9,
                            height: documentElement.clientHeight * 0.9
                        }).focus();
                        break;
                    default: 
                        content.append(document.getElementById('content').innerHTML);
                        break;
                    
                }
            });
            $('#edit').click(function () {
                if (mmc.register) {
                    renameItem();
                }
            });
        }
        function renameItem() {
            if (!sec.IsDeleteAllow(owner)) {
                mmc.notification("You can't change name");
                return;
            }
            if (type === 'Link') {
                mmc.notification("Can't change link");
                return;
            }
            var $itemName = $('#itemName'), input = $('<input type="text" class="editName inputText" />');
            var fileNameWihoutExtenstion = $itemName.text().split('.').splice(0, 1)[0];
            input.val(fileNameWihoutExtenstion).width($itemName.width()).height($itemName.height()).insertBefore($itemName);
            $itemName.hide();
            input.focus();

            input.focusout(function () {
                checknewFileName();
            });
            input.keyup(function (e) {
                if (e.keyCode === 13) {
                    checknewFileName();
                }
            });
            function checknewFileName() {
                var fileName = $(input).val();
                if (fileName === fileNameWihoutExtenstion) {
                    finishProcess();
                    return;
                }
                var fileCheck = new RegExp("^[^\\\./:\*\?\"<>\|]{1}[^\\/:\*\?\"<>\|]{0,254}$", "i");
                if (fileCheck.test(fileName)) {
                    renameFileName(fileName);
                }
                else {
                    mmc.notification(ZboxResources.InvalidFilename);
                    finishProcess();
                }
            }
            function renameFileName(newFileName) {
                var request = new ZboxAjaxRequest({
                    url: "/Item/Rename",
                    data: { newFileName: newFileName, ItemUid: itemid },
                    success: function (data) {
                        $itemName.text(data)
                        finishProcess();
                    },
                    error: function (msg) {
                        mmc.notification(msg);
                        finishProcess();
                    }
                });
                request.Post();
            }
            function finishProcess() {
                $itemName.show();
                input.remove();
            }

        }


        function deleteItem() {
            if (!sec.IsDeleteAllow(owner)) {
                mmc.notification(ZboxResources.DontHavePermissionDeleteFile);
                return;
            }
            var request = new ZboxAjaxRequest({
                url: "/Item/Delete",
                data: { ItemUid: itemid, BoxUid: boxid },
                beforeSend: function () {
                    return mmc.confirm(ZboxResources.SureDeleteItem);
                },
                success: function (data) {
                    items.splice(currentPosition, 1);
                    if (items.length === 0) {
                        window.location.href = $('#backToBox').attr('href'); //click doesnt work in chrome
                    }
                    loadItem(items[currentPosition]);
                }
            });
            request.Post();
        }

        function prevItem() {
            if (--currentPosition < 0) {
                currentPosition = items.length - 1;
            }
            return currentPosition;
        }
        function nextItem() {
            if (++currentPosition >= items.length) {
                currentPosition = 0;
            }
            return currentPosition;
        }
        var cache = {};
        function loadItem(item) {
            if (item === itemid) return;

            if (item in cache) {
                if (cache[item].timeExpire > new Date()) {
                    changeLayout(cache[item].data);
                    return;
                }
            }
            var twentyMinutesLater = new Date();
            twentyMinutesLater.setMinutes(twentyMinutesLater.getMinutes() + cacheExpire);

            var request = new ZboxAjaxRequest({
                data: { BoxUid: boxid, ItemUid: item },
                url: "/Item/Load",
                success: function (data) {
                    changeLayout(data);
                    cache[item] = {
                        data: data,
                        timeExpire: twentyMinutesLater
                    };

                }
            });
            request.Post();
        }
        function changeLayout(data) {
            itemid = data.Uid;
            owner = data.OwnerUid;
            type = data.Type;
            $('#itemName').text(data.Name).data('type', data.Type);
            $('#update').text(mmc.dateToShow(new Date(parseInt(data.UpdateTime.replace("/Date(", "").replace(")/", ""), 10))));
            $('#ownerImg').attr('src', data.OwnerImg);
            $('#ownerName').text(data.Owner).data('uid', data.OwnerUid);
            $('#content').html(data.Preview);
            changeDownloadUrl();
            changePrintUrl();
            mmc.commentItem(itemid);
            if (Modernizr.history) {
                window.history.pushState('', document.title, changeUrl(window.location.href));
            }
        }

        function changeDownloadUrl() {
            var $download = $('#download');
            var downloadUrl = $download.attr('href').split('/');
            downloadUrl[downloadUrl.length - 1] = itemid;
            $download.attr('href', downloadUrl.join('/'));
        }
        //TODO - we have 2 class in the same page with same logic.... on expand and on item
        function changePrintUrl() {
            var $print = $('.btnPrint');
            $print.attr('href', changeUrl($print.attr('href')));
        }
        function changeUrl(url) {
            return $.param.querystring(url, { ItemUid: itemid });
        }

        function initialItems() {
            var request = new ZboxAjaxRequest({
                data: { BoxUid: boxid, pageNumber: currentPage },
                url: "/Box/Items",
                success: function (data) {
                    for (var i = 0; i < data.Dto.length; i++) {
                        items.push(data.Dto[i].Uid);
                    }
                    countofItem = data.Count;
                    calculatePosition();
                    $('button:disabled').removeAttr('disabled');
                }
            });
            request.Post();
        }

        function calculatePosition() {
            currentPosition = ko.utils.arrayIndexOf(items, itemid);
        }
    }
}(window.mmc = window.mmc || {}, jQuery));