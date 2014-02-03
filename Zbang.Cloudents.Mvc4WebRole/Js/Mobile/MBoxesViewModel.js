(function (cd) {
    function Box(data) {
        "use strict";
        data = data || {};
        var self = this;
        self.uid = data.id;
        self.boxPicture = data.boxPicture || '/images/EmptyState/my_default3.png';
        self.name = data.name;
        self.itemCount = data.itemCount;
        self.commentCount = data.commentCount;
        self.boxUrl = data.url;
        self.courseCode = data.courseCode;
        self.professor = data.professor;
    }
    cd.Box = Box;
})(cd);

(function ($, ko, cd, dataContext, ZboxResources) {
    "use strict";
    if (!cd.register()) {
        return;
    }
    cd.loadModel('dashboard', 'DashboardContext', registerKODashboard);

    function registerKODashboard() {
        ko.applyBindings(new BoxesViewModel(), document.getElementById('dashboard'));
    }
    function BoxesViewModel() {
        var self = this, /*page = 0,*/ LASTMODIFIED = 'LastModified', NAME = 'Name';//, paggingneed = true;

        self.orderBy = ko.observable(LASTMODIFIED);
        self.loaded = ko.observable(false);

        self.boxes = ko.observableArray([]);
        self.isSearch = ko.observable(false);
        self.searchTerm = ko.observable('');

        
        $('#dashLibImg').attr('src', $('#menu_libImg').attr('src'));


        cd.pubsub.subscribe('dash_boxes', function () {
            clearBoard();
            self.isSearch(false);
            boxesList();
        });

        cd.pubsub.subscribe('dash_search', function (data) {
            //data.query
            clearBoard();
            //we need to maintain the hash
            location.hash = 'search';
            self.isSearch(true);
            self.searchTerm(data.query);
            search(data);
        });

        function search(data) {
            self.loaded(false);
            dataContext.sDashboard({
                data: data,
                success: function (result) {
                    generateModel(result);
                },
                always: function () {
                    self.loaded(true);
                }
            });
        }

        function boxesList() {
            self.loaded(false);
            //var loader = cd.renderLoading($('#BoxList'));
            dataContext.dashboard({
                //data: { pageNumber: page, order: self.orderBy() },
                success: function (data) {
                    cd.pubsub.publish('dashSideD', { friend: data.friends, wall: data.wall });
                    generateModel({ boxes: data.boxes });
                },
                always: function () {
                    self.loaded(true);
                    //loader();
                }
            });
           
        }
        function generateModel(data) {
            var boxes = data.boxes;
            cd.pubsub.publish('dashboard_load');
            if (!boxes.length) {
                //paggingneed = false;
                return;
            }
            var arr = self.boxes();
            for (var i = 0, length = boxes.length; i < length; i++) {
                var box = new cd.Box(boxes[i]);
                if (ko.utils.arrayIndexOf(arr, box) !== -1) {
                    continue;
                }
                arr.push(box);
            }
            self.boxes(arr);
        }
        function clearBoard() {
            self.boxes([]);
            //paggingneed = true;
            //page = 0;
        }
        //cd.registerScroll(function () {
        //    if (paggingneed && self.loaded()) {
        //        //page++;
        //        boxesList();
        //    }
        //},$('#Dashboard'));

        //#region sort
        self.sortName = function () {
            self.orderBy(NAME);
            clearBoard();
            boxesList();
        };
        self.sortDate = function () {
            self.orderBy(LASTMODIFIED);
            clearBoard();
            boxesList();
        };
        //#endregion


        //#region search
        cd.pubsub.subscribe('trig_search_boxes', function (data) {
            cd.pubsub.publish('nav', 'Dashboard/Search/' + encodeURIComponent(data));
        });
        cd.pubsub.subscribe('close_search_boxes', function () {
            cd.pubsub.publish('nav', 'Dashboard');
        });
        //self.search = function (f) {
        //    var $dash_searchInp = $('#dash_searchInp');
        //    $('#dash_searchInp').hide();
        //    document.getElementById('dash_seachIcon').setAttribute('type', 'button');
        //    var inputVal = $dash_searchInp.val();
        //    $dash_searchInp.val('');
        //    cd.pubsub.publish('nav', 'Dashboard/Search/' + encodeURIComponent(inputVal));
        //};
        
        //$('#dash_seachIcon').click(function (e) {
        //    if (this.getAttribute('type') === 'button') {
        //        e.preventDefault();
        //        $('#dash_searchInp').show().focus();
        //        this.setAttribute('type', 'submit');
        //    }
        //});

        self.emptyStateSearch = ko.computed(function () {
            return self.loaded() && !self.boxes().length && self.isSearch();
        });


        //#endregion

        //#region createbox
        $('#createBoxDialog').submit(function (e) {
            e.preventDefault();
            var $form = $(this);
            if (!$form.valid || $form.valid()) {
                var boxName, data = $form.serializeArray();
                $.each(data, function (i, fd) {
                    if (fd.name === "BoxName") {
                        boxName = fd.value;
                        return false;
                    }
                });
                var exists = ko.utils.arrayFirst(self.boxes(), function (i) {
                    return i.name === boxName;
                });
                if (exists) {
                    cd.displayErrors($form, ZboxResources.BoxExists);
                    return false;
                }
                createBox(data);
            }

        });
        function createBox(data) {
            dataContext.createBox({
                data: data,
                success: function (result) {
                    var box = new cd.Box(result);
                    cd.resetForm($('#createBoxDialog'));
                    cd.pubsub.publish('nav', box.boxUrl);
                },
                error: function (msg) {
                    cd.notification(msg[0].Value[0]);
                }
            });
        }
        //#endregion
    }

})(jQuery, ko, cd, cd.data, ZboxResources);