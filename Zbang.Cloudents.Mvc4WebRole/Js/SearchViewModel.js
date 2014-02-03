function SearchViewModel() {
    function baseSearch() {
        var self = this;
        self.changeName = function (markName) {
            try {
                var reg = new RegExp(query, 'gi');
                var location = markName.search(reg);
                var start = markName.substring(0, location);
                var word = markName.substring(location, location + query.length);
                var end = markName.substring(location + query.length, markName.length);

                return start + '<mark>' + word + '</mark>' + end;

            } catch (e) {
                return markName;
            }
        };
    }
    function Box(data) {
        "use strict";
        var self = this;
        self.uid = data.Uid;
        self.boxPicture = data.BoxPicture ? data.BoxPicture : '/images/EmptyState/my_default.png';
        self.name = data.Name;
        self.updateTime = new Date(parseInt(data.UpdateTime.replace("/Date(", "").replace(")/", ""), 10)); // used for sort
        self.itemCount = data.ItemCount;
        self.membersCount = data.MembersCount;
        self.commentCount = data.CommentCount;
        self.boxUrl = "/Box?BoxUid=" + self.uid;
    }
    function SearchItem(data) {
        var self = this;
        baseSearch.call(self);
        mmc.Item.call(self, data);
        self.deleteAllow = false;
    }

    function SearchBox(data) {
        var self = this;
        baseSearch.call(self);
        Box.call(self, data);
        self.editable = false;
    }

    var self = this;
    var query = Zbox.getParameterByName('query');
    self.itemQuery = {
        array: [],
        page: 0,
        moreResult: ko.observable(true),
        thumbViewName: 'items-thumb-template'
    };

    self.boxQuery = {
        array: [],
        page: 0,
        moreResult: ko.observable(true),
        thumbViewName: 'boxes-template'
    };
    
    self.datatoDisplay = ko.observableArray([]);
    var unlock = true;

    self.typeFilter = ko.observable('Box');
    self.changeFilter = function (data) {
        self.datatoDisplay([]);
        self.typeFilter(data);
        var display = viewData();
        self.datatoDisplay.push.apply(self.datatoDisplay, display.array);
        if (!display.array.length) {
            doQuery();
        }
    };
    //self.viewType = ko.observable(Zbox.getItem('boxesView', 'List'));
   
    self.displayMode = function () {
        var display = viewData();
        return display.thumbViewName;
    };
    self.resultFound = ko.computed(function () {
        var display = viewData();
        return display.array.length === 0 && !display.moreResult();
    }, self);



    registerEvents();
    doQuery();

    function viewData(type) {
        if (type === undefined) {
            type = self.typeFilter();
        }
        switch (type) {
            case 'Box':
                return self.boxQuery;
            default:
                return self.itemQuery;
        }
    }


    function doQuery() {
        var queryparam = viewData();
        if (!queryparam.moreResult()) {
            return;
        }
        var request = new ZboxAjaxRequest({
            data: { query: query, pageNumber: queryparam.page, type: self.typeFilter() },
            url: "/Home/DoSearch",
            beforeSend: function () {
                $('div.loading').show();
            },
            success: function (data) {
                generateModel(data);
                queryparam.page++;
            },
            error: function () { 
                $('#noRes').show();
            },
            complete: function () {
                $('div.loading').hide();
                unlock = true;
            }
        });
        request.Post();
    }

    function generateModel(data) {

        var queryDisplay = viewData();
        if (data.length === 0) {
            queryDisplay.moreResult(false);
        }
        for (var j = 0; j < data.length; j++) {
            var exists = ko.utils.arrayFirst(self.itemQuery.array, function (i) {
                return i.uid === data[j].Uid;
            });
            if (exists === null) {
                var elem;
                switch (self.typeFilter()) {
                    case 'Box':
                        elem = new SearchBox(data[j]);
                        break;
                    default:
                        elem = new SearchItem(data[j]);
                        break;

                }
                queryDisplay.array.push(elem);
            }
        }
        self.datatoDisplay.push.apply(self.datatoDisplay, queryDisplay.array);
    }

    function registerEvents() {
        //Resize.MainContentHeightChange();
        var queryDisplay = viewData();
        $(window).scroll(function () {
            if ($(this).scrollTop() + $(this).height() > ($(this).find('ul').height() * 0.75)) {
                if (queryDisplay.moreResult() && unlock) {
                    unlock = false;
                    doQuery();
                }
            }
        });
    }

}