(function (mmc, $) {
    "use strict";
    if (mmc.page.box || mmc.page.search) {
        mmc.Item = function (data) {
            var self = this;
            self.commentCount = data.CommentCount;
            self.name = data.Name;

            self.itemSize = Zbox.RenderSizeOfFile(data.Size);

            self.uid = data.Uid;
            self.uploader = data.Owner;
            self.userid = data.OwnerId;
            self.type = data.Type;
            if (self.type === 'File') {
                self.thumbnailUrl = ko.observable(data.Thumbnail);

                self.download = "/D/" + data.BoxUid + "/" + self.uid;
            }
            if (self.type === 'Link') {
                self.thumbnailUrl = data.Thumbnail;
                self.download = data.Name;
            }
            self.itemUrl = "/Item?BoxUid=" + data.BoxUid + "&ItemUid=" + self.uid;

            self.deleteAllow = false;
            if (window.sec) { // happen on search - checking existance of object sec
                self.deleteAllow = sec.IsDeleteAllow(self.userid);
            }
        };
    }
    if (mmc.page.box) {
        mmc.boxItem = function () {
            ko.applyBindings(new BoxItemViewModel(), document.getElementById('items'));
        }
    }
    function BoxItemViewModel() {
        //start of View Model
        var self = this;
        var boxid = Zbox.getParameterByName('BoxUid');
        if (boxid === '') {
            throw 'boxid cannot be null';
        }

        self.items = ko.observableArray([]);

        self.countofItems = ko.observable(0);

        //self.sizeOfBox = ko.computed(function () {
        //    var totalOfSize = 0;
        //    ko.utils.arrayForEach(self.items(), function (item) {
        //        if (item.type === 'file') {
        //            totalOfSize += item.size;
        //        }
        //    });
        //    if (totalOfSize === 0) {
        //        return '';
        //    }

        //    return Zbox.RenderSizeOfFile(totalOfSize);
        //}, self);

        self.countofItemsdisplay = ko.computed(function () {
            if (self.countofItems() === 1) {
                return self.countofItems() + ' ' + ZboxResources.File;
            }
            return self.countofItems() + ' ' + ZboxResources.Files;
        }, self);

        var current = 0;
        var unlock = true;

        self.removeItem = function (item) {
            if (!sec.IsDeleteAllow(item.userid)) {

                mmc.notification(ZboxResources.DontHavePermissionToDelete + item.type);
                return false;
            }
            var answer = confirm(ZboxResources.SureYouWantToDelete + ' ' + item.name + "?");
            if (!answer) {
                return false;
            }
            ko.utils.postJson
            self.items.remove(item);
            self.countofItems(self.countofItems() - 1);
            var request = new ZboxAjaxRequest({
                url: "/Item/Delete",
                data: { ItemUid: item.uid, BoxUid: boxid },
                error: function (e) {
                    self.items.push(item);
                }
            });
            request.Post();
        };
        mmc.updateHandler('addItem', function (d) {
            var newItem = new mmc.Item(d);
            self.items.unshift(newItem);
            self.countofItems(self.countofItems() + 1);
        });
        //mmc.uploadFileComplete = function (item, windowrequest) {
        //    /// <summary>
        //    /// used for upload complete to finish the upload
        //    /// </summary>
        //    //if (window !== windowrequest)
        //    //    throw '';
        //    if (item === null) {
        //        throw 'item is null';
        //    }
        //    if (item.Uid === null) {
        //        throw 'item doesnt have uid';
        //    }
        //    var mitem = ko.utils.arrayFirst(self.items(), function (i) {
        //        return i.uid === item.Uid;
        //    });
        //    if (mitem === null) {
        //        var newItem = new mmc.Item(item);
        //        self.items.unshift(newItem);
        //        self.countofItems(self.countofItems() + 1);
        //    }
        //};
        self.changeLayoutAfterRender = function (element, b) {
            var x = ko.utils.arrayIndexOf(self.items(), b); // need this because of ie8

            if (self.items().length === x + 1) {
                if ($('#BoxItemList').find('li:nth-child(3n):first').css('margin-right') === '0px') {
                    return;
                }
                $('#BoxItemList').find('li:nth-child(3n)').css('margin-right', '0px');
            }
        };

        self.noResult = ko.computed(function () {
            return self.countofItems() === 0;
        }, self);


        initialItems();
        function initialItems() {
            var request = new ZboxAjaxRequest({
                data: { BoxUid: boxid, pageNumber: current },
                url: "/Box/Items",
                success: function (data) {
                    GenerateModel(data.Dto);
                    self.countofItems(data.Count);
                }
            });
            request.Post();
        }

        function GenerateModel(data) {
            var itemtoadd = [];
            for (var j = 0; j < data.length; j++) {
                var item = ko.utils.arrayFirst(self.items(), function (i) {
                    return i.uid === data[j].Uid;
                });
                if (item === null) {
                    item = new mmc.Item(data[j]);
                    itemtoadd.push(item);
                }
            }
            self.items.push.apply(self.items, itemtoadd);
            unlock = true;
        }
        mmc.UpdateBoxItemModel = function (data) {
            for (var j = 0; j < data.length; j++) {
                var item = ko.utils.arrayFirst(self.items(), function (i) {
                    return i.uid === data[j].Uid;
                });
                if (item === null && !data[j].Deleted) {
                    var item = new mmc.Item(data[j]);
                    self.items.unshift(item);
                    self.countofItems(self.countofItems() + 1);
                    continue;
                }
                if (data[j].Deleted && item) {
                    self.items.remove(item);
                    self.countofItems(self.countofItems() - 1);
                    continue;
                }
                if (item) {
                    if (item.type === 'file' && item.thumbnailUrl() !== data[j].ThumbnailBlobUrl) {
                        item.thumbnailUrl(data[j].ThumbnailBlobUrl);
                    }
                }
            }
        };
    }
}(window.mmc = window.mmc || {}, jQuery))