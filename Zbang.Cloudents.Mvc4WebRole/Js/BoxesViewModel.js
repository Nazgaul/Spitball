function Box(data) {
    "use strict";

    var self = this;
    self.uid = data.id;
    self.boxPicture = data.boxPicture || '/images/emptyState/my_default3.png';
    self.name = data.name;
    self.itemCount = data.itemCount;
    self.membersCount = data.membersCount;
    self.commentCount = data.commentCount;
    self.boxUrl = data.url + '?r=dashboard';
    self.userType = ko.observable(data.userType);
    self.courseCode = data.courseCode;
    self.professor = data.professor;
    self.boxType = data.boxType;

    self.removeBoxTitle = isDeleteOrUnfollow(self);
    function isDeleteOrUnfollow(box) {
        /// <summary></summary>
        /// <param name="box" type="Box"></param>
        var retVal = false;
        if (box.userType() === 'none') {
            return;
        }
        if (box.userType() === 'owner') {
            retVal = true;
        }
        if (box.membersCount <= 2 && !box.commentCount && !box.itemCount) {
            retVal = true;
        }
        return retVal ? ZboxResources.Delete : ZboxResources.LeaveGroup;
    }
    self.removeBoxConfirm = confirmDeleteOrUnfollow(self);
    function confirmDeleteOrUnfollow(box) {
        /// <summary></summary>
        /// <param name="box" type="Box"></param>
        var retVal = false;
        if (box.userType() === 'none') {
            return;
        }
        if (box.userType() === 'owner') {
            retVal = true;
        }
        if (box.membersCount <= 2 && !box.commentCount && !box.itemCount) {
            retVal = true;
        }
        return retVal ? ZboxResources.ToDeleteBox : ZboxResources.ToLeaveGroup;
    }
}

(function (cd, dataContext, ko, ZboxResources, analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('bsvm')) {
        return;
    }
    cd.loadModel('dashboard', 'DashboardContext', registerKODashboard);

    function registerKODashboard() {
        ko.applyBindings(new BoxesViewModel(), document.getElementById('dash_Boxes'));
    }

    function BoxesViewModel() {
        var self = this,
        issearch = false,
        $privateBoxDialog = $('#privateBoxDialog'),
        $dash_SearchQuery = $('#g_searchQ');


        self.loaded = ko.observable(false);
        self.loadedAnimation = ko.observable(false);
        self.boxes = ko.observableArray([]);


        //self.boxes.subscribe(function (newValue) {
        //    console.log(newValue);
        //});


        self.academicBoxes = ko.computed(function () {
            return ko.utils.arrayFilter(self.boxes(), function (b) {
                return b.boxType === 'academic';
            });
        });
        self.privateBoxes = ko.computed(function () {
            return ko.utils.arrayFilter(self.boxes(), function (b) {
                return  b.boxType === 'box' || issearch;
            }
            );
        });


        cd.pubsub.subscribe('dash_search', function (data) {
            //clearBoard();
            issearch = true;
            $dash_SearchQuery.val(data.query);
            search(data);
        });
        cd.pubsub.subscribe('dash_boxes', function () {
            clearBoardWithSearch();
            boxesList();
        });


        //function clearBoard() {
        //    self.boxes([]);
        //}

        function clearBoardWithSearch() {
            //clearBoard();
            issearch = false;
            $dash_SearchQuery.val('').trigger('change'); //cant remember why i need trigger - maybe placeholder issue (ie9)
        }

        function search(data) {
            self.loaded(false).loadedAnimation(false);
            
            var $boxList = $('#BoxList'), initData = $boxList.data('data');
            if (initData) {
                
                $boxList.removeAttr('data-data').data('data', '');
                cd.pubsub.publish('dashSideD', { friend: initData.friends, wall: initData.wall });
            }
            dataContext.sDashboard({
                data: data,
                success: function (result) {
                    generateModel( result.boxes );
                },
                always: function () {
                    $dash_SearchQuery.next().removeAttr('disabled');
                    self.loaded(true);
                }
            });
        }
        function boxesList() {
            self.loaded(false).loadedAnimation(false);

            var $boxList = $('#BoxList'), initData = $boxList.data('data');
            if (initData) {
                $boxList.removeAttr('data-data').data('data', '');
                generateModel(initData.boxes);
                cd.pubsub.publish('dashSideD', { friend: initData.friends, wall: initData.wall });
                self.loaded(true);
                return;
            }
            var loader = cd.renderLoading(('body'));
            dataContext.dashboard({
                success: function (data) {
                    generateModel(data.boxes);
                    cd.pubsub.publish('dashSideD', { friend: data.friends, wall: data.wall });

                },
                always: function () {
                    loader();
                    self.loaded(true);
                }
            });
        }

        function generateModel(data) {
            var boxes = data;
            
            self.boxes([]);
            if (!cd.firstLoad){
                document.title = JsResources.Dashboard + ' | Cloudents';
            }
            cd.pubsub.publish('dashboard_load');
            if (!boxes.length) {
                self.loadedAnimation(true);
                return;
            }
            var arr = self.boxes(), arrids = [];

            for (var i = 0, length = boxes.length; i < length; i++) {
                var box = new Box(boxes[i]);
                if (ko.utils.arrayIndexOf(arr, box) !== -1) {
                    continue;
                }
                arr.push(box);
                arrids.push(box.uid);
            }
            //$('#BoxList').append(cd.attachTemplateToData('firstTemplate', ko.utils.unwrapObservable(self.boxes)));
            var tt = new TrackTiming('Dashboard', 'Render time of boxes');
            tt.startTime();
            self.boxes(arr);
            cd.loadImages(document.getElementById('dash_right'));
            tt.endTime();
            tt.send();
            cd.pubsub.publish('boxGroup', arrids);
            self.loadedAnimation(true);
        }

        

        self.emptyState = ko.computed(function () {
            return self.loaded() && !self.boxes().length && !issearch;
        });
        
        
        self.removeBox = function (box) {
            /// <summary></summary>
            /// <param name="box" type="Box"></param>

            //if (!confirm(ZboxResources.SureYouWant0ThisBox.format(box.removeBoxTitle))) {
            if (!confirm(ZboxResources.SureYouWant +  ' ' + box.removeBoxConfirm)) {
                
                return;
            }
            analytics.trackEvent('Follow', 'Unfollow', 'Clicking on the  box "x" mark on dashboard level');
            self.boxes.remove(box);
            dataContext.removeBox2({
                data: { boxUid: box.uid }
            });
        };

        var emptyText = '';
        self.mptySrchStt = ko.computed(function () {
            var x = self.loaded() && !self.boxes().length && issearch;
            if (x) {
                var $Lib = $('#dash_seaEmpy');
                if (!emptyText) {
                    emptyText = $Lib.text();
                }
                $Lib.text(emptyText.format($dash_SearchQuery.val()));
            }
            return x;
        });

        self.boxvisible = ko.computed(function () {
            if (self.loaded() && issearch) {
                return false;
            }
            return true;
        });

        self.titleShow = ko.computed(
            function () {
                var loaded = self.loaded();
                if (!loaded) {
                    return '';
                }
                if (issearch && self.boxes().length) {
                    return '\u200F' + self.boxes().length + '\u200F ' + ZboxResources.SearchResults + ' \u200E“' + $dash_SearchQuery.val() + '”\u200E';
                }
                return ZboxResources.CoursesFollow;
            });

        //#region createBox
        var firstTime = true;
        self.addBox = function () {
            //createBoxDialog.dialog('show');
            $privateBoxDialog.show().find('.inputText').focus();

            if (firstTime) {
                registerEvents();
                firstTime = false;
            }
            function registerEvents() {
                var $form = $('#createBoxDialog');
                $privateBoxDialog.find('.closeDialog,.cancel,a').click(function () {
                    cd.resetForm($form);
                    $privateBoxDialog.hide();
                });
                $form.submit(function (e) {
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
                            return i.name.toLowerCase() === boxName.toLowerCase();
                        });
                        if (exists) {
                            cd.displayErrors($form, ZboxResources.BoxExists);
                            return false;
                        }
                        createBox(data);
                    }

                });
            }

        };
        var isSubmit = false;
        function createBox(data) {
            if (isSubmit) {
                return;
            }
            analytics.trackEvent('Dashboard', 'Create privte box', 'Number of clicks on "create" after writing down the box name');
            isSubmit = true;
            dataContext.createBox({
                data: data,
                success: function (result) {
                    var box = new Box(result);
                    cd.resetForm($('#createBoxDialog'));
                    $privateBoxDialog.hide();
                    $('#BoxName').val('');
                    isSubmit = false;
                    cd.pubsub.publish('nav', box.boxUrl + '?r=dashboard');
                },
                error: function (msg) {
                    isSubmit = false;
                    cd.notification(msg[0].Value[0]);
                }
            });
        }
        //#endregion



        //var _private = cd._private = cd._private || {}

        //console.log(_private);
        //$(function () {
        //    cd._seal();
        //    console.log(_private);
        //})



    }

})(window.cd, cd.data, ko, ZboxResources, cd.analytics);