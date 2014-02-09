﻿(function ($, dataContext, ko, cd, ZboxResources, analytics) {
    "use strict";

    if (window.scriptLoaded.isLoaded('bivm')) {
        return;
    }

    cd.loadModel('box', 'BoxContext', registerKoBoxItems);

    function registerKoBoxItems() {
        ko.applyBindings(new BoxItemViewModel(), $('#box_items')[0]);
    }

    function BoxItemViewModel() {
        function Item(data) {
            data = data || {};
            var that = this;
            that.name = data.name;
            that.uid = data.id;
            //that.uploader = data.Owner;
            that.userid = data.ownerId;
            that.type = data.type;
            that.numOfViews = data.numOfViews;
            that.rate = 69 / 5 * data.rate;
            if (that.type === 'File') {
                that.thumbnailUrl = data.thumbnail;
            }
            if (that.type === 'Link') {
                that.thumbnailUrl = data.thumbnail;

            }
            that.download = "/d/" + boxid + "/" + that.uid;
            that.tabId = ko.observable(data.tabId);
            that.isCheck = ko.computed({
                read: function () {
                    return that.tabId() === self.manageTab();
                },
                write: function (value) {
                    if (value) {
                        that.tabId(self.manageTab());
                    }
                    else {
                        that.tabId(null);
                    }
                }
            });
            that.deleteAllow = ko.computed(
                function () {
                    return (self.permission() === 'subscribe' || self.permission() === 'owner') && (self.permission() === 'owner' ||
                         that.userid === cd.userDetail().nId);
                });
            that.itemUrl = data.url + '?r=box';
        }

        var self = this, boxid, current = 0, //countOfItems = 0,
            tab;
        self.items = ko.observableArray([]);
        self.permission = ko.observable('none');
        self.loaded = ko.observable(false);
        self.loadedAnimation = ko.observable(false);

        cd.pubsub.subscribe('perm', function (d) {
            self.permission(d);
        });


        cd.pubsub.subscribe('box', function (data) {
            boxid = data.id;
            current = 0;
            tab = null;
            self.permission('none');
            getItems();
        });
        cd.pubsub.subscribe('itemTab', function (d) {
            tab = d;
            current = 0;
            self.manageTab('');
            getItems();

        });
        cd.pubsub.subscribe('boxclear', function () {
            self.manageTab('');
            self.loaded(false).loadedAnimation(false).items([]);
        });


        function getItems() {
            self.loaded(false).loadedAnimation(false);
            dataContext.getItems({
                data: {
                    BoxUid: boxid,
                    pageNumber: current, tab: tab,
                    uniName: cd.getParameterFromUrl(1),
                    boxName: cd.getParameterFromUrl(3)
                },
                success: function (result) {
                    generateModel(result.dto);
                },
                always: function () {
                    self.loaded(true);
                }
            });
        }

        function generateModel(data) {

            var mapped = [];
            for (var i = 0, l = data.length; i < l; i++) {
                mapped.push(new Item(data[i]));
            }
            var tt = new TrackTiming('Box Items', 'Render time of items');
            tt.startTime();
            self.items(mapped);
            cd.loadImages(document.getElementById('BoxItemList'));
            tt.endTime();
            tt.send();
            self.loadedAnimation(true);                       
        }      

        //#region addItem
        self.addItem = function () {
            if (!cd.register()) {
                cd.pubsub.publish('register');
                return;
            }
            cd.pubsub.publish('upload');
        };
        cd.pubsub.subscribe('addItem', function (d) {
            try {
                var newItem = new Item(d);
                var x = ko.utils.arrayFirst(self.items(), function (i) {
                    return i.uid === newItem.uid;
                });
                if (x) {
                    return;
                }
                self.items.unshift(newItem);
                cd.pubsub.publish('clear_cache');
                //self.loadedAnimation(true);
                cd.loadImages(document.getElementById('BoxItemList'));
            } catch (e) {
            }


        });
        cd.pubsub.subscribe('addedItem', function (d) {
            if (d.boxid === boxid) {
                cd.pubsub.publish('addItem', d.item);
            }            
        });
        //#endregion


        self.emptyState = ko.computed(function () {
            return self.loaded() && !self.items().length;
        });

        //#region remove item
        self.removeItem = function (item) {
            if (!cd.deleteAllow(self.permission(), item.userid.toString())) { //userid is an int we need a string
                cd.notification(ZboxResources.DontHavePermissionToDelete + ' ' + item.type);
                return false;
            }
            if (!confirm(ZboxResources.SureYouWantToDelete + ' ' + item.name + "?")) {
                return false;
            }
            self.items.remove(item);
            //countOfItems--;
            dataContext.removeItem({
                data: { itemId: item.uid, BoxUid: boxid },
                success: function () {
                    cd.pubsub.publish('removeItemNotification', { itemid: item.uid, boxid: boxid });
                },
                error: function () {
                    self.items.push(item);
                }
            });
        };
        cd.pubsub.subscribe('removeItem', function (id) {
            var x = ko.utils.arrayFirst(self.items(), function (i) {
                return i.uid === id;
            });
            self.items.remove(x);
        });
        //#endregion

        var cursorlocation = { top: 0 };
        cursorlocation[$('html').css('direction') === 'ltr' ? 'left' : 'right'] = 0;
        //#region draggable
        self.edrag = function (element) {
            if (element.nodeType === 1) {
                $(element).draggable({
                    cursor: "pointer",
                    //delay: 100,
                    cursorAt: { top: 0, left: 0 },
                    helper: function (event) {
                        var item = ko.dataFor(event.target);
                        item.name = item.name.slice(0, 20);
                        return $("<div class='draggable'>" + item.name + "</div>");
                    }

                });
            }

        };
        //#endregion



        //#region manage tab
        self.manageTab = ko.observable('');

        cd.pubsub.subscribe('itemTabM', function (d) {
            current = 0;
            tab = null;
            self.manageTab(d.id);
            getItems();
        });

        self.manageSave = function () {
            //We can do that better...
            var x = ko.utils.arrayFilter(self.items(), function (i) {
                return i.tabId() === self.manageTab();
            });
            var uids = ko.utils.arrayMap(x, function (item) {
                return item.uid;
            });
            cd.pubsub.publish('clear_cache'); //aync problem
            dataContext.addItemToTab({
                data: { BoxId: boxid, TabId: self.manageTab(), ItemId: uids, nDelete: true },
                success: function () {
                    cd.pubsub.publish('itemTab', self.manageTab());
                }
            });

        };
        self.managedecline = function () {
            cd.pubsub.publish('itemTab', self.manageTab());
        };
        //#endregion
        //if user in manage mode the click on item trigger the checkbox and not go to item view
        self.itmSlct = function (item) {
            if (self.manageTab()) {
                item.isCheck(!item.isCheck());
                return false;
            }
            return true;
        };
        //Analytics
        $('#BoxItemList').on('click', 'a.downloadBtn', function (e) {
            var data = ko.dataFor(e.target);
            cd.pubsub.publish('item_Download', { id: data.uid });
            analytics.trackEvent('Box', 'Download', 'The number of downloads made on box view');
        });
    }
})(jQuery, cd.data, ko, cd, ZboxResources, cd.analytics);