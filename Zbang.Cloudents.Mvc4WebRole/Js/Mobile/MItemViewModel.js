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
    var boxid = parseInt(location.pathname.split('/')[3],10),
        itemid = parseInt(location.pathname.split('/')[5],10),
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
            data: { id: boxid, pageNumber: 0 },
            success: function (data) {
                for (var i = 0, length = data.length; i < length; i++) {
                    itemsinBox.push(data[i].id);
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

    var lastScrollLeft = 0;
    window.onscroll = function () {
        var documentScrollLeft = document.body.scrollLeft;
        if (lastScrollLeft != documentScrollLeft) {        
            lastScrollLeft = documentScrollLeft;
            return;
        }

        if (document.body.scrollTop >= document.body.clientHeight - window.innerHeight - 50) {
            getPreview();

        }
    };
    function resetView() {
        itemContext.innerHTML = '';
    }
    window.addEventListener("orientationchange", function () {
        var iframe = document.querySelector('.youtubeframe');        
        iframe.height = window.innerHeight - 50;
        iframe.width = window.innerWidth;
    }, false);

    function getItem() {
        dataContext.getItem({
            data: { BoxId: boxid, itemId: itemid, uniName: location.pathname.split('/')[2] },
            success: function (data) {
                document.getElementById('backToBox').setAttribute('href', data.boxUrl);
                blobName = data.blob;
                    loaded = true;
                    getPreview();
            }
        });
    }
    function getPreview() {
        var index = 0;
        if (blobName && loaded && !document.getElementsByClassName('previewFailed').length) {
            loaded = false;
            //var loader = cd.renderLoading($itemContent);
            dataContext.preview({
                data: { blobName: blobName, index: index, id: itemid, width: screen.width, height: screen.height, boxId: boxid },
                success: function (retVal) {
                    if (retVal.preview.trim()) {
                        loaded = true;
                    }
                    index++;
                    var $preview = $(retVal.preview);
                    $preview[0].width = window.innerWidth;
                    $preview[0].height = window.innerHeight - 50;                    
                                        
                    itemContext.insertAdjacentHTML('beforeend', $preview[0].outerHTML);
                    //itemContext.innerHTML += retVal;
                    //self.preview(self.preview() + retVal);
                    //loader();
                }
            });

        }
    }

})(jQuery, cd.data);