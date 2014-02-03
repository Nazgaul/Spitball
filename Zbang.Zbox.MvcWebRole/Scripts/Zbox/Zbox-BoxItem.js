/// <reference path="/Scripts/jquery-1.6.2-vsdoc.js" />
$(document).ready(function () {

    $("#divFiles").simplyScroll();
    Zbox.BoxItem.registerBoxItemEvents();
});
Zbox.BoxItem = {
    CropFileName: function (FileName) {
        var fileName = FileName;
        if (fileName.length > 10) {
            var fileExtension = /[^.]+$/.exec(FileName);
            fileName = fileName.slice(0, 10);
            fileName += "..." + fileExtension;
        }
        return fileName;
    },

    CropLinkName: function (Link) {
        var link = Link;
        if (link.length > 20) {
            link = link.slice(0, 17);
            link += "...";
        }
        return link;
    },


    isColorboxRel: function (link) {
        return this.iscolorbox(link) ? 'colorboxCombine' : '';
    },

    // at the moment same as boxItem Entry should change when the new seach design kicks in.
    AddSearchBoxItemEntry: function (item, subscriberpermission) {
        var boxItemArray;
        if (item.ItemType == 'file') {
            boxItemArray = Zbox.BoxItem.AddFileEntry(item, subscriberpermission);
        }
        else if (item.ItemType == 'link') {
            boxItemArray = Zbox.BoxItem.AddLinkEntry(item, subscriberpermission);
        }
        else {
            throw "unknown item type: " + item.Type;
        }
        return boxItemArray;

    },

    AddBoxItemEntry: function (item, subscriberpermission, isAppend) {
        var boxItemArray;
        if (item.ItemType == 'file') {
            boxItemArray = Zbox.BoxItem.AddFileEntry(item, subscriberpermission);
        }
        else if (item.ItemType == 'link') {
            boxItemArray = Zbox.BoxItem.AddLinkEntry(item, subscriberpermission);
        }
        else {
            throw "unknown item type: " + item.Type;
        }
        if (isAppend)
            $('#divFiles').prepend(boxItemArray);
        else
            return boxItemArray;

    },
    fileItem: '',
    linkItem: '',
    boxItemDetail: '',

    AddFileEntry: function (file, subscriberpermission) {

        file.colorBoxRel = this.isColorboxRel(file.BlobUrl);
        file.cropFileName = this.CropFileName(file.Name);
        file.fileLength = plupload.formatSize(file.Size);
        file.copyToClipboardData = file.BlobUrl;
        file.removeBoxItem = 'removeFile';

        if (this.fileItem == '') {
            var fileItem = $('#fileEntryTemplate').clone();

            if (boxItemDetail == '') {
                var boxItemDetail = document.getElementById('BoxItemDetailTemplate').innerHTML; // $('#BoxItemDetailTemplate').html();
                this.boxItemDetail = boxItemDetail;
                $('#BoxItemDetailTemplate').remove();
            }
            $(fileItem).find('#LocationBoxItemDetailTemplate1').replaceWith(this.boxItemDetail);

            var menuItemShate = document.getElementById('menuItemShareTemplate').innerHTML;
            $(fileItem).find('#LocationmenuItemShareTemplate1').replaceWith(menuItemShate);


            var copyToClipboard = document.getElementById('BoxItemCopyToClipboardMenuTemplate').innerHTML;
            $(fileItem).find('#LocationCopyToClipboardTemplate').replaceWith(copyToClipboard);


            var actionDelete = $(fileItem).find('#LocationBoxItemActionDeleteTemplate1');
            var BoxItemActionDelete = document.getElementById('BoxItemActionDeleteTemplate').innerHTML;
            $(actionDelete).replaceWith(BoxItemActionDelete);

            this.fileItem = fileItem;
            $('#fileEntryTemplate').remove();
        }

        try {
            return Zbox.changeTemplateText(this.fileItem.html(), file);
        }
        finally {
            file = null;
            fileItem = null;
            boxItemDetail = null;
            copyToClipboard = null;
            BoxItemActionDelete = null;
        }


    },

    AddLinkEntry: function (link, subscriberpermission) {

        link.encodedUrl = encodeURI(link.Url);
        link.cropLinkName = this.CropLinkName(link.Url);
        link.copyToClipboardData = link.Url;
        link.removeBoxItem = 'removeLink';
        link.encodedUrlThumbnail = 'http://api.thumbalizr.com/?api_key=eee81f9d0864559575d601091e93d755&amp;width=100&amp;url=' + link.encodedUrl;

        if (this.linkItem == '') {
            var linkItem = $('#linkEntryTemplate').clone();

            if (boxItemDetail == '') {
                var boxItemDetail = document.getElementById('BoxItemDetailTemplate').innerHTML;
                this.boxItemDetail = boxItemDetail;
                $('#BoxItemDetailTemplate').remove();
            }
            $(linkItem).find('#LocationBoxItemDetailTemplate2').replaceWith(this.boxItemDetail);
            var menuItemShate = document.getElementById('menuItemShareTemplate').innerHTML;
            $(linkItem).find('#LocationmenuItemShareTemplate2').replaceWith(menuItemShate);

            var copyToClipboard = document.getElementById('BoxItemCopyToClipboardMenuTemplate').innerHTML;
            $(linkItem).find('#LocationCopyToClipboardTemplate').replaceWith(copyToClipboard);

            var actionDelete = $(linkItem).find('#LocationBoxItemActionDeleteTemplate2');

            var BoxItemActionDelete = document.getElementById('BoxItemActionDeleteTemplate').innerHTML;
            $(actionDelete).replaceWith(BoxItemActionDelete);

            this.linkItem = linkItem;
            $('#linkEntryTemplate').remove();
        }
        try {
            return Zbox.changeTemplateText(this.linkItem.clone().html(), link);
        }
        finally {
            link = null;

            linkItem = null;
            boxItemDetail = null;
            copyToClipboard = null;
            BoxItemActionDelete = null;
        }

    }
}

Zbox.BoxItem.loadBoxFiles = function (boxId, pageIndex, permission) {
    /// <summary>
    /// Load Box Items.    
    /// </summary>    
    /// <param name="boxId">Box Id.</param>

    //$('#spanFilesMessage').text('(Loading...)');
    var self = this;
    for (var i = 0; i < ZeroClipboard.clients.length; i++) {
        i.destroy();
    }


    var divFiles = $('#divFiles');
    var loadBoxItemsRequest = new ZboxAjaxRequest({

        beforeSend: function () {
            divFiles.empty();
            self.updateBoxItemCount(0);
            // $('#divFiles').text('[Loading box files...]');
        },
        url: "/Storage/GetBoxItemsPaged?boxId=" + boxId + "&pageIndex=" + pageIndex + "&pageSize=1000",
        success: function (pagedQueryResult) {
            self.loadBoxFilesSucess(pagedQueryResult);
        },
        error: function (error) {
            $('#spanFilesMessage').text(error);
        }
    });

    loadBoxItemsRequest.Get();

};

Zbox.BoxItem.loadBoxFilesSucess = function (pagedQueryResult) {
    $('#divFiles').empty();
    var currentBox = Zbox.Box.boxes.GetCurrentBox();
    if (pagedQueryResult.boxId == currentBox.shortUid)
        this.loadboxItemsOnSuccess(pagedQueryResult.boxItemDtos, currentBox.UserPermission);
};

Zbox.BoxItem.loadboxItemsOnSuccess = function (pagedQueryResult, subscriberpermission) {
    var items = '';
    var l = pagedQueryResult.length;

    for (var i = 0; i < l; i++) {
        items += Zbox.BoxItem.AddBoxItemEntry(pagedQueryResult[i], subscriberpermission);
    }
    var $divFiles = $('#divFiles');
    $divFiles.html(items);

    $('#spanFilesMessage').text('');
    this.UpdateBoxItemRegion();
    var $images = $divFiles.find('.item-thumb').find('img');
    Zbox.LazyLoadOfImages($divFiles, $images);

};
Zbox.BoxItem.registerBoxItemEvents = function () {
    var self = this;
    var $divFilesParent = $('#divFilesParent');
    $divFilesParent.click(function (e) {

        if (Zbox.HasClass(e.target, 'removeFile')) {
            var permission = e.target.getAttribute('data-IsUserAllowed')
            if (permission === 'true') {
                self.deletefile(e.target);
            }
            else {
                Zbox.InsufficentPermission();

            }
            return false;
        }
        else if (Zbox.HasClass(e.target, 'removeLink')) {
            var permission = e.target.getAttribute('data-IsUserAllowed')
            if (permission === 'true') {
                self.deleteLink(e.target);
            }
            else {
                Zbox.InsufficentPermission();
            }

            return false;
        }

        var t = e.target;
        if (e.target.nodeName === 'A' && e.target.children.item(0).nodeName === 'IMG') {
            t = e.target.children.item(0);
        }

        if (t.getAttribute('data-LightBox')) {
            Zbox.LightBox.ShowLightBox(t);
            return false;
        }

    });

    $divFilesParent.find('.simply-scroll-btn-right').click(function () {
        var $divFiles = $("#divFiles");
        var $images = $divFiles.find('.item-thumb').find('img');
        Zbox.LazyLoadOfImages($divFiles, $images);
    });




};

Zbox.BoxItem.deletefile = function (element) {
    var self = this;
    var boxItem = $(element).parents('div.boxItem');
    var fileId = boxItem.attr('data-itemid');
    var filename = boxItem.find('div.item-label').attr('title');

    var deleteFileRequest = new ZboxAjaxRequest({
        beforeSend: function () {
            $(boxItem).fadeTo(0, 0.5);
        },
        url: '/Storage/DeleteItem',
        data: { ItemId: fileId, boxId: $('#selecteBoxId').val() },
        success: function (data) {
            Zbox.LoadUserStatus();
            boxItem.remove();
            self.UpdateBoxItemRegion();
        },
        error: function (error) {
            $('#spanFilesMessage').text(error);
        }
    });

    var boxName = Zbox.Box.boxes.GetCurrentBox().BoxName; //  $('#selectedBoxName').text();

    Zbox.ShowConfirmDialog({
        title: '<img class="float-left icon-popupAlert" src="/Content/Images/icon-popupAlert.png" /><div class="style80 float-left">Delete File</div><br class="clear" />',
        message: '<div class="style81">You are about to delete the file <span class="italic">"' + filename + '"</span> from the ' + boxName + ' box.</div>' +
                                  '<div class="style81">Are you sure?</div>',
        ok: function () {
            deleteFileRequest.Post();
            return true;
        }
    });
};

Zbox.BoxItem.deleteLink = function (element) {
    var self = this;
    var boxItem = $(element).parents('div.boxItem');
    var linkId = boxItem.attr('data-itemid');
    var deleteLinkRequest = new ZboxAjaxRequest({
        beforeSend: function () {
            $(boxItem).fadeTo(0, 0.5);
        },
        url: '/Storage/DeleteItem',
        data: { ItemId: linkId, boxId: $('#selecteBoxId').val() },
        success: function (data) {
            boxItem.remove();
            self.UpdateBoxItemRegion();
        },
        error: function (error) {
            $('#spanFilesMessage').text(error);
        }
    });
    var linkName = boxItem.find('div.item-label').attr('title');
    var boxName = Zbox.Box.boxes.GetCurrentBox().BoxName;
    Zbox.ShowConfirmDialog({
        title: '<img class="float-left icon-popupAlert" src="/Content/Images/icon-popupAlert.png" /><div class="style80 float-left">Delete Link</div><br class="clear" />',
        message: '<div class="style81">You are about to delete the link  <span class="italic">' + linkName + '</span> from ' + boxName + ' box.</div>' +
                              '<div class="style81">Are you sure?</div>',
        ok: function () {
            deleteLinkRequest.Post();
            return true;
        }
    });

    return false;
};

Zbox.BoxItem.iscolorbox = function (url) {

    var IsFile = /DownloadBoxItem\b/.test(url);

    return !IsFile || /\.(doc|docx|xls|xlsx|ppt|pptx|pps|odt|ods|odp|sxw|sxc|sxi|wpd|pdf|rtf|html|txt|csv|tsv|gif|png|jpg|jpeg|bmp)(?:\?([^#]*))?(?:#(\.*))?$/i.test(url);
};

Zbox.BoxItem.updateBoxItemCount = function (count) {
    $('#boxItemCount').find('label').text(count);
};
//updateCount object with 2 params
Zbox.BoxItem.UpdateBoxItemRegion = function () {
    // $.simplyScroll.fn.ReChangeSimplyScroll($('#divFiles'));
    var $divFiles = $('#divFiles');
    $divFiles.trigger('elementChanged');
    var numberOfItemsInBox = $divFiles.children().length;
    this.updateBoxItemCount(numberOfItemsInBox);
    if (numberOfItemsInBox > 0) {
        $('#downloadAllFiles').button('option', 'disabled', false);
    }
    else {
        $('#downloadAllFiles').button('option', 'disabled', true);
    }
    //Zbox.BoxItem.registerCopyToClipboard();
    Zbox.UpdateScreenTime();
};