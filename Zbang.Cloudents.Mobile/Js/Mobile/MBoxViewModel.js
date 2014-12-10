(function (cd, ko, dataContext) {
    if (window.scriptLoaded.isLoaded('mBovm')) {
        return;
    }
    cd.loadModel('box', 'BoxContext', registerKoBox);

    function registerKoBox() {
        ko.applyBindings(new BoxViewModel(), $('#box')[0]);
    }
    function BoxViewModel() {
        var self = this, boxloadFirstTime = true, $boxelements = $('#box_items,#box_Comment');

        self.boxid = ko.observable();
        self.userType = ko.observable(3);


        self.name = ko.observable();
        self.image = ko.observable();
        self.ownerName = ko.observable();
        self.ownerId = ko.observable();
        self.parent = ko.observable(new parentElem());

        self.members = ko.observableArray([]);
        self.membersC = ko.observable();
        self.noOfItems = ko.observable(5);
        self.noOfComments = ko.observable(5);
        self.proffesor = ko.observable();
        self.courseId = ko.observable();
        self.privacySetting = ko.observable();
        self.boxtype = ko.observable();

        self.subMemerbs = ko.computed(function () {
            return self.membersC() - self.members().length;
        });
        //self.tabs = ko.observableArray([]);
        self.boxDesc = ko.computed(function () {
            if (self.boxtype() === 'academic') {
                var courseid = self.courseId() || '';
                var professor = self.proffesor() || '';

                if (!courseid.length) {
                    return professor;
                }
                if (!professor.length) {
                    return courseid;
                }
                return courseid + ' | ' + professor;
            }
            return self.ownerName();

        });

        function member(data) {
            data = data || {};
            var that = this;
            that.name = data.name;
            that.image = data.image;
            that.id = data.uid;
            that.isOwner = data.uid === self.ownerId();
        }


        function parentElem(data) {
            var that = this;
            that.name = 'Dashboard';
            that.url = '/dashboard';
        }

        cd.pubsub.subscribe('box', function (data) {
            self.boxid(parseInt(data.id, 10));
            if (boxloadFirstTime) {
                boxloadFirstTime = false;
                //registerEvents();
            }
            if (!$boxelements.hasClass('boxElementvisible')) {
                $('#box_items').addClass('boxElementvisible');
            }
            //location.hash = 'box_items';
            window.scrollTo(0, 1); // safari like to scroll to location
            GetBox();
        });

        function GetBox() {
            var $box = $('#box'), initData = $box.data('data');
            if (initData) {
                populateData(initData);
                $box.data('data', '').removeAttr('data-data');
                return;
            }
            dataContext.getBox({
                data: { id: self.boxid() },
                success: function (result) {
                    populateData(result);
                }
            });
            function populateData(result) {
                self.image(result.image || '/images/EmptyState/my_default3.png');
                self.name(result.name);
                self.ownerName(result.ownerName);
                self.ownerId(result.ownerUid);
                self.proffesor(result.professorName);
                self.membersC(Math.max(result.members, result.subscribers.length));
                self.courseId(result.courseId);
                self.userType(result.userType);
                self.privacySetting(result.privacySetting);
                self.boxtype(result.sboxType);
                self.parent(new parentElem(result));
                var mapped = $.map(result.subscribers, function (d) { return new member(d); });

                //self.members(mapped.slice(0, 4));
                self.members(mapped);
                self.noOfItems(result.Items);

                self.noOfComments(result.comments);
                //for (var i = 0; i < 6; i++) {
                //    self.members.push(new member(result.Subscribers[0]));
                //}
                ////self.membersC(10);

                //var mappedTabs = $.map(result.Tabs, function (d) { return new tab(d); });
                //self.tabs(mappedTabs);
                cd.pubsub.publish('box_load', { name: result.name, url: self.parent().url });
                cd.pubsub.publish('perm', self.userType());

                populateTabs(result.tabs);





            }
        }

        //#region tabs
        var tabSelect = document.getElementById('boxTabs');
        function populateTabs(tabs) {
            for (var i = 1, length = tabSelect.length; i < length; i++) {
                tabSelect.remove(i);
            }
            for (var j = 0, length2 = tabs.length; j < length2; j++) {
                var opt = document.createElement("option");
                opt.value = tabs[j].id;
                opt.text = tabs[j].name;


                tabSelect.add(opt);
            }
        }
        tabSelect.onchange = function () {
            cd.pubsub.publish('itemTab', tabSelect.value);
        };
        //#endregion

        $('.tabToggle').click(function (e) {
            var $target = $(e.currentTarget);
            var trg = $('#' + $target.data('elem'));
            if (trg.hasClass('boxElementvisible')) {
                return;
            }
            $boxelements.removeClass('boxElementvisible');
            trg.addClass('boxElementvisible');
        });
        //cd.pubsub.subscribe('nav_hash', function (hash) {
        //    if (typeof hash === 'string' && hash === '' && $('#Box').is(':visible')) {
        //       //location.hash = 'box_items'; // bring back the default view
        //    }

        //    switch (hash) {
        //        case 'showitems':
        //            $('#Box').addClass('showItems');
        //            break;
        //        case 'showcomments':
        //            $('#Box').addClass('showComments');
        //            break;
        //        default:
        //            $('#Box').removeClass('showItems showComments');
        //            break;
        //    }
        //});

        //#region followbox
        self.follow = ko.computed(function () {
            return self.userType() < 2 && cd.register();
        });

        self.followbox = function () {
            self.userType(2);
            //var $userName = $('#userName');
            //addMember(new member({
            //    Name: $userName.text(),
            //    Image: $('#userDetails').find('img').prop('src'),
            //    Uid: $userName.data('id')
            //}));
            cd.pubsub.publish('perm', self.userType());
            cd.pubsub.publish('dinvite', self.boxid());
            dataContext.follow({
                data: { BoxUid: self.boxid() }
            });
        };
        //#endregion

        //#region settings
        $('#settings_bname').focusout(function () {
            dataContext.updateBoxInfo({
                data: { boxuid: self.boxid(), name: self.name(), courseCode: self.courseId(), professor: self.proffesor() }
            });
        });
        var $settings_bnotification = $('#settings_bnotification');
        cd.pubsub.subscribe('box_settings', function () {
            dataContext.getNotification({
                data: { boxUid: self.boxid() },
                success: function (data) {
                    $settings_bnotification.val(data);
                    //createNotificationDialog.find('select').val(data);
                }
            });
        });
        $settings_bnotification.change(function () {
            dataContext.changeNotification({
                data: {
                    boxUid: self.boxid(),
                    notification: $(this).val()
                },
            });
        });
        function isDeleteOrUnfollow() {
            if (self.userType() === 3) {
                return true;
            }
            if (self.members().length < 2 && !self.noOfComments() && !self.noOfItems()) {
                return true;
            }
            return false;
        }
        self.removeB = ko.computed(function () {
            var retVal = isDeleteOrUnfollow();
            return retVal ? JsResources.Delete : JsResources.Unfollow;
        });
        self.removeBA = function () {
            if (!confirm(JsResources.SureYouWant0ThisBox.format(self.removeB()))) {
                return;
            }
            dataContext.removeBox2({
                data: { id: self.boxid() },
                success: function () { cd.pubsub.publish('nav', '/'); }
            });

        };
        //#endregion

        //#region invite
        self.invite = function (f, e) {
            /// <param name="e" type="Event"></param>
            if (!cd.register()) {
                cd.unregisterAction(this);
                e.stopPropagation();
                return;
            }
            if (self.follow()) {
                cd.notification(JsResources.NeedToFollowBox);
                e.stopPropagation();
                return;
            }
            cd.pubsub.publish('invite', { id: self.boxid(), name: self.name(), privacy: self.privacySetting() === 'AnyoneWithUrl' });
            return true; // to continue propgration
        };
        //#endregion
    }
})(cd, ko, cd.data);