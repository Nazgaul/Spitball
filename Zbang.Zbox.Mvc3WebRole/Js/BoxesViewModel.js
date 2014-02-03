function Box(data) {
    "use strict";
    var self = this;

    self.uid = data.Uid;
    self.boxPicture = data.BoxPicture ? data.BoxPicture : '/images/EmptyState/mybox.png';

    self.name = data.Name;
    //self.privacySettings = $.trim(data.PrivacySettings);
    self.updateTime = new Date(parseInt(data.UpdateTime.replace("/Date(", "").replace(")/", ""), 10)); // used for sort
    //self.userType = ko.observable(data.UserType);

    self.itemCount = data.ItemCount;
    self.membersCount = data.MembersCount;
    self.commentCount = data.CommentCount;
    //self.owner = data.Owner;

    //self.addboxshow = function () {
    //    return self.userType() === 'invite' || self.userType() === 'none';
    //};
    //self.removeboxshow = function () {
    //    return self.userType() === 'subscribe' || self.userType() === 'owner';
    //};
    //self.removeBoxTxt = function () {
    //    switch (self.userType()) {
    //        case ('subscribe'): return ZboxResources.Unfollow;
    //        case ('owner'): return ZboxResources.Delete;
    //    }
    //};
    //self.boxActivity = function () {
    //    switch (self.userType()) {
    //        case ('none'): return ZboxResources.Follow;
    //        case ('invite'): return ZboxResources.Follow;

    //    }
    //};
    self.boxUrl = "/Box?BoxUid=" + self.uid;
}

(function (mmc, $) {
    "use strict";

    (function () {
        $(window).bind('dashboard', function () {
            $(window).unbind('dashboard');
            ko.applyBindings(new BoxesViewModel(), document.getElementById('Boxeslistsection'));
        });
    })();

    function BoxesViewModel() {
        var self = this,
         cache = {},
         page = 0;

        self.boxes = ko.observableArray([]);
        self.countofboxes = ko.observable();

        self.moreResultExists = ko.observable(true);

        //var elementPerRender = 6;
        //var elementIndex = 0;
        //self.changeLayoutAfterRender = function (element, b) {
        //    $(element[1]).hide().delay(150 * elementIndex).fadeIn(700, 'easeInOutQuad');
        //    elementIndex++;
        //    if (elementIndex === elementPerRender) {
        //        elementIndex = 0;
        //        var elem = $('#BoxList').find('li:nth-child(3n):first');

        //        if (elem.css('margin-right') === '0px' && elem.css('margin-left') === '0px') {
        //            return;
        //        }
        //        $('#BoxList').find('li:nth-child(3n)').css({ 'margin-left': '0px', 'margin-right': '0px' });
        //    }
        //};

        //self.removeBox = function (box) {
        //    deleteBox(box);
        //};

        //self.boxAction = function (box) {
        //    var request = new ZboxAjaxRequest({
        //        url: "/Share/SubscribeToBox",
        //        data: { BoxUid: box.uid },
        //        success: function () {
        //            box.userType('subscribe');
        //        }
        //    });
        //    request.Post();
        //};

        self.moreboxes = function () {
            page++;
            boxesList();
        };


        function sort(b1, b2) {
            if (b1.updateTime > b2.updateTime) return -1;
            return 1;
        }
        mmc.updateHandler('createbox', function (d) {
            var box = new Box(d);
            self.boxes.push(box);
            var boxescount = self.countofboxes();
            self.countofboxes(++boxescount);
            self.boxes.sort(sort);
        });


        //function GenerateConfirm(box) {
        //    if (box.userType() === 'subscribe') {
        //        //case (1): return ZboxResources.SureToDecline0.format(box.name);
        //        //    break;
        //        return ZboxResources.SureToUnfollow0.format(box.name);
        //    }
        //    return ZboxResources.SureToDelete0.format(box.name);
        //}
        //function deleteBox(box) {
        //    if (!mmc.confirm(GenerateConfirm(box))) {
        //        return;
        //    }
        //    var boxescount = self.countofboxes();
        //    self.boxes.remove(box);
        //    self.countofboxes(--boxescount);
        //    var request = new ZboxAjaxRequest({
        //        url: "/Dashboard/DeleteBox",
        //        data: { BoxUid: box.uid, userType: box.userType() },
        //        error: function () {
        //            self.boxes.push(box);
        //            self.countofboxes(++boxescount);
        //            self.boxes.sort(sort);
        //        }
        //    });
        //    request.Post();
        //}
        boxesList();
        function boxesList() {
            var term = ko.toJSON({ pageNumber: page });
            if (term in cache) {
                generateModel(cache[term]);
                return;
            }
            $.ajaxRequest.postJson({
                url: "/Dashboard/BoxList",
                data: term,
                done: function (data) {
                    cache[term] = data;
                    generateModel(data);
                }
            });
        }

        function generateModel(data) {
            if (page === 0) {
                self.countofboxes(data.count);
                self.boxes([]);
            }
            var boxes = data.boxes;
            var arr = self.boxes();
            for (var i = 0; i < boxes.length; i++) {
                var box = new Box(boxes[i]);
                if (ko.utils.arrayIndexOf(arr, box) !== -1) {
                    //if (arr.indexOf(box) !== -1) {
                    continue;
                }
                arr.push(box);
            }
            self.boxes(arr);
            //var mappedBoxes = $.map(data.boxes, function (box) { return new Box(box); });

            //self.boxes.push.apply(self.boxes, mappedBoxes);

            if (self.boxes().length === self.countofboxes()) {
                self.moreResultExists(false);
            }
            else {
                self.moreResultExists(true);
            }
        }

        //#region createBox

        self.addBox = function () {
            createBoxDialog.dialog('show');
        };
        var createBoxDialog = $('#createBoxDialog').dialog({
            submitCallBack: function (url, data) {
                CreateBox(url, data);
            }
        });
        function CreateBox(url, data) {
            $.ajaxRequest.post({
                url: url,
                data: data,
                done: function (data) {
                    self.boxes.push(new Box(data));
                    self.countofboxes(self.countofboxes() + 1);
                },
                error: function (err) {
                    console.log(err);

                }
            });
        }
        //#endregion

        //function registerEvents() {

        //$(window).bind('hashchange', function (e) {
        //    if (e.fragment !== '' && e.fragment.match(/my/i) === null) {
        //        return;
        //    }
        //    //window.location.hash.match(/my/i) !== null
        //    //var f = $.trim(e.fragment.toLowerCase());
        //    //if (f !== '' && f !== mmc.hashConst.My) {
        //        //return;
        //    //}
        //    $('#main').children('div').hide();
        //    $('#My').show();

        //    $('#btnLib, #btnMy').hide();
        //    $('#btnMy').show();

        //    boxesList();
        //});
    }
}(window.mmc = window.mmc || {}, jQuery));