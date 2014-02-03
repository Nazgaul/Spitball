(function ($, dataContext) {
    "use strict";
    if (window.scriptLoaded.isLoaded('mItvm')) {
        return;
    }
    if (!String.prototype.trim) {
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, '');
        };
    }
    var boxid = location.pathname.split('/')[3],
        itemid = location.pathname.split('/')[5],
        itemsinBox = [],
        index = 0,
        blobName = '',
    loaded = false,
    prevItem = document.getElementById('prevItem'),
    nextItem = document.getElementById('nextItem'),
    downloadItem = document.getElementById('download'),
    downloadUrl = downloadItem.getAttribute('data-url'),
    itemContext = document.getElementById('item_content');



    initialItems();
    getItem();

    function initialItems() {
        //we can get the data from box before
        dataContext.getItems({
            data: { BoxUid: boxid, pageNumber: 0 },
            success: function (data) {
                for (var i = 0, length = data.dto.length; i < length; i++) {
                    itemsinBox.push(data.dto[i].id);
                }
                calculatePosition();
                prevItem.removeAttribute('disabled');
                nextItem.removeAttribute('disabled');
            }
        });

        function calculatePosition() {
            index = itemsinBox.indexOf(itemid);
        }
    }
    prevItem.onclick = function () {
        if (!index) {
            return;
        }
        itemid = itemsinBox[--index];
        resetView();
        getItem();
    };
    nextItem.onclick = function () {
        if (index === itemsinBox.length - 1) {
            return;
        }
        itemid = itemsinBox[++index];
        resetView();
        getItem();
    };
    downloadItem.onclick = function () {
        downloadItem.setAttribute('href', '/d/' +  boxid + '/' + itemid);
    };
    window.onscroll = function () {
        if (document.body.scrollTop >= document.body.clientHeight - window.innerHeight - 50) {
            getPreview();

        }
    };
    function resetView() {
        itemContext.innerHTML = '';
    }
    

    function getItem() {
        dataContext.getItem({
            data: { BoxUid: boxid, itemId: itemid, uniName: location.pathname.split('/')[2] },
            success: function (data) {
                document.getElementById('backToBox').setAttribute('href', data.boxUrl);
                blobName = data.blob;
                    loaded = true;
                    getPreview();
            }
        });
    }
    function getPreview() {
        var images = itemContext.getElementsByTagName('img').length;
        //var images = $('.previewWrapper').find('img').length;
        if (blobName && loaded && !document.getElementsByClassName('previewFailed').length) {
            loaded = false;
            //var loader = cd.renderLoading($itemContent);
            dataContext.preview({
                data: { blobName: blobName, imageNumber: images, uid: itemid, width: screen.width, height: screen.height, boxUid: boxid },
                success: function (retVal) {
                    if (retVal.preview.trim()) {
                        loaded = true;
                    }
                    itemContext.insertAdjacentHTML('beforeend', retVal.preview);
                    //itemContext.innerHTML += retVal;
                    //self.preview(self.preview() + retVal);
                    //loader();
                }
            });

        }
    }

})(jQuery, cd.data);