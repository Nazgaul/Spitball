(function (ko, dataContext, cd) {   
    "use strict";
    if (window.scriptLoaded.isLoaded('mBivm')) {
        return;
    }

    cd.loadModel('box', 'BoxContext', registerKoBoxItems);
    function registerKoBoxItems() {
        ko.applyBindings(new BoxItemViewModel(), $('#box_items')[0]);
    }
    function BoxItemViewModel() {

        function Item(data) {
            var that = this;
            that.name = data.name;
            that.uid = data.id;
            that.uploader = data.owner;
            //that.userid = data.OwnerId;
            that.type = data.type;
            if (that.type === 'File') {
                that.thumbnailUrl = ko.observable(data.thumbnail);
                
            }
            if (that.type === 'Link') {
                that.thumbnailUrl = data.thumbnail;
            }
            that.itemUrl = data.url;
            
        }


        var self = this, boxid, current = 0, countOfItems = 0, tab;
        self.items = ko.observableArray([]);
      
        self.itemsCount = ko.computed(function () {
            var x = self.items().length;
            $('#box_items_count').text(x);
            return x;
        });
       
        self.permission = ko.observable(0);
        self.loaded = ko.observable(false);

        
        cd.pubsub.subscribe('box', function (data) {
            boxid = data.id;
            current = 0;
            self.permission(0);
            getItems();
            cd.pubsub.publish('upload', { id: boxid });
        });
        cd.pubsub.subscribe('perm', function (d) {
            self.permission(d);
        });

        function getItems() {
            self.loaded(false);
            dataContext.getItems({
                data: {
                    BoxUid: boxid, pageNumber: current, tab: tab,
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
            var mapped = $.map(data, function (d) { return new Item(d); });
            self.items(mapped);
        }
        cd.pubsub.subscribe('addItem', function (d) {
            var newItem = new Item(d);
            self.items.unshift(newItem);
            //countOfItems++;
        });
        self.emptyState = ko.computed(function () {
            return self.loaded() && !self.items().length;
        });

        cd.pubsub.subscribe('itemTab', function (d) {
            tab = d;
            current = 0;
            getItems();

        });
        //self.addItem = function () {
        //    if (!cd.register()) {
        //        cd.pubsub.publish('register');
        //        return;
        //    }
        //    //cd.pubsub.publish('upload', { id: boxid, name: $('#box_Name').text() });
        //};
    }
})(ko, cd.data, cd);