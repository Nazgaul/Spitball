//function Box(data) { //temp fix
//    "use strict";

//    var self = this;
//    self.uid = data.id;
//    self.boxPicture = data.boxPicture || '/images/emptyState/my_default3.png';
//    self.name = data.name;
//    self.itemCount = data.itemCount;
//    self.membersCount = data.membersCount;
//    self.commentCount = data.commentCount;
//    self.boxUrl = data.url;// + '?r=dashboard';
//    self.userType = ko.observable(data.userType);
//    self.courseCode = data.courseCode;
//    self.professor = data.professor;
//    self.boxType = data.boxType;
//    self.numOfUpdates = ko.observable(0);
//    self.removeBoxTitle = isDeleteOrUnfollow(self);
//    function isDeleteOrUnfollow(box) {
//        // <summary></summary>
//        // <param name="box" type="Box"></param>
//        var retVal = false;
//        if (box.userType() === 'none') {
//            return;
//        }
//        if (box.userType() === 'owner') {
//            retVal = true;
//        }
//        if (box.membersCount <= 2 && box.commentCount < 2 && !box.itemCount) {
//            retVal = true;
//        }
//        return retVal ? JsResources.Delete : JsResources.LeaveGroup;
//    }
//    self.removeBoxConfirm = confirmDeleteOrUnfollow(self);
//    function confirmDeleteOrUnfollow(box) {
//        // <summary></summary>
//        // <param name="box" type="Box"></param>

//        if (box.userType() === 'none') {
//            return;
//        }
//        if (box.userType() === 'owner') {
//            return JsResources.SureYouWantTo + ' ' + JsResources.ToDeleteBox;
//        }
//        if (box.membersCount <= 2 && box.commentCount < 2 && !box.itemCount) {
//            return 'You have created an empty course, if you unfollow this course it will be deleted. Do you want to delete the course?';
//        }
//        return JsResources.SureYouWantTo + ' ' + JsResources.ToLeaveGroup;
//    }
//}

//(function ($, dataContext, ko, cd, zboxResources, analytics) {
//    "use strict";

//    if (window.scriptLoaded.isLoaded('bvm')) {
//        return;
//    }

//    cd.loadModel('box', 'BoxContext', registerKoBox);

//    function registerKoBox() {
//        ko.applyBindings(new BoxViewModel(), document.getElementById('box'));
//    }

//    var $window = $(window), $boxFilters, $createFolder;
//    function BoxViewModel() {
//        var self = this, MAXNUMOFMEMBERS = 11;

//        self.boxid = '';
//        self.name = ko.observable(); // use

//        self.membersLength = ko.observable();
//        self.image = ko.observable(); //use
//        self.ownerName = ko.observable();
//        self.ownerId = ko.observable();
//        self.boxtype = ko.observable();
//        self.courseId = ko.observable();//use
//        self.proffer = ko.observable();//use
//        self.privacySetting = ko.observable();
//        self.noOfItems = ko.observable(5);
//        self.noOfComments = ko.observable(5);
//        self.members = ko.observableArray([]);
//        self.copyLink = ko.observable(cd.location());
//        self.parent = ko.observable(new ParentElem());
//        self.userType = ko.observable();
//        self.tabs = ko.observableArray([]);        
//        self.backVisible = ko.computed(function () {
//            return self.parent().url;
//        });
//        self.currentTab = ko.observable(null);

//        self.follow = ko.computed(function () {
//            return (self.userType() === 'none' || self.userType() === 'invite') && cd.register();
//        });

//        self.boxTypeClass = ko.computed(function () {
//            return self.boxtype() === 'box' ? 'private' : 'academic';
//        });
//        self.notification = null;


//        function Member(data) {
//            data = data || {};
//            var that = this;
//            that.name = data.name;
//            that.image = data.image;
//            that.id = data.uid;
//            that.url = data.url;// + '?r=box&s=sidebar';
//            that.isOwner = data.uid === self.ownerId;
//        }
//        function Tab(data) {
//            var selfTab = this;
//            selfTab.id = data.id;
//            selfTab.name = ko.observable(data.name);
//            selfTab.isSelected = ko.computed(function () {
//                return self.currentTab() === selfTab.id;
//            }, this);
//            selfTab.isAction = ko.computed(function () {
//                return selfTab.isSelected() && cd.register();
//            }, this);
//        }
//        function ParentElem() {
//            var that = this;
//            var backData = cd.prevLinkData('box');
//            if (!backData) {
//                that.name = zboxResources.Dashboard;
//                that.url = '/dashboard/';
//                return;
//            }

//            var page = backData.url, split; //remove the first 
//            if (page.indexOf('dashboard') > -1 || page.indexOf('user') > -1 || page.indexOf('search') > -1) {
//                split = 0;
//            } else { //library 
//                split = 1;
//            }
//            that.name = backData.title.split(' | ')[split]; //first part of the title
//            that.url = backData.url;
//            //if (backData.url.indexOf('dashboard') > -1) {
//            //    that.name = JsResources.Dashboard;
//            //    that.url = backData.url;
//            //    return;
//            //}

//            //if (backData.url.indexOf('user') > -1) {
//            //    that.name  = 'User page';
//            //    that.url = backData.url;
//            //    return;
//            //}

//            //that.id = data.id;
//            //that.name = data.name || JsResources.Dashboard;
//            //that.url = '/library/' + that.id + '/' + encodeURIComponent(that.name);
//            ////that.url = that.id ? '/library/' + that.id + '/' + encodeURIComponent(that.name) + '/' : '/dashboard/';
//        }
//        registerEvents();
//        cd.pubsub.subscribe('box', function (data) {
//            self.boxid = data.id;
//            cd.pubsub.publish('boxGroup', [data.id]);
//            getBox();
//        });

//        function getBox() {
//            var $box = $('#box'), initData = $box.data('data');

//            if (initData) {
//                populateData(initData);
//                $box.data('data', '').removeAttr('data-data');

//                return;
//            }
//            dataContext.getBox({
//                data: { boxuid: self.boxid },
//                success: function (result) {
//                    populateData(result);

//                }
//            });

//            function populateData(result) {
//                var mapped = $.map(result.subscribers, function (d) { return new Member(d); });
//                self.ownerId(result.ownerUid);
//                self.image(result.image || '/images/emptyState/my_default3.png')
//                .name(result.name)
//                .ownerName(result.ownerName)

//                .proffer(result.professorName)
//                .courseId(result.courseId)

//                .userType(result.userType)
//                .privacySetting(result.privacySetting)

//                .boxtype(result.boxType)
//                .parent(new ParentElem())

//                .members(mapped)

//                .noOfItems(result.items)
//                .membersLength(result.members)
//                .noOfComments(result.comments)
//                .copyLink(cd.location());
//                //temp
//                //for (var i = 0; i < 5; i++) {
//                //    self.members.push(new Member(result.Subscribers[0]));
//                //}
//                //endtemp                
//                var mappedTabs = $.map(result.tabs, function (d) { return new Tab(d); });
//                self.tabs(mappedTabs);

//                //dashboard go the invite as well
//                $('#box').find('[data-navigation="invite"]')
//                    .data('d', { boxid: self.boxid, image: self.image(), name: self.name(), boxUrl: cd.location() })
//                    .attr('href', function () {
//                        return $(this).data('link') + self.boxid;
//                    });

//                cd.setTitle('{0} | {1} | Cloudents'.format(self.name(), self.ownerName()));

//                cd.pubsub.publish('box_load', self.boxid);                
//                setTimeout(function () {
//                    if (self.follow()) {
//                        document.getElementById('joinGrpWpr').classList.add('show');
//                    }
//                }, 750);

//                //cd.pubsub.publish('init_clipboard', $('#box_CL'));
//                cd.pubsub.publish('perm', self.userType());
//            }
//        }

//        function addMember(member) {
//            if (self.members().length < MAXNUMOFMEMBERS) {
//                self.members.push(member);
//            }
//        }

//        //#region followbox
//        self.followbox = function () {
//            document.getElementById('joinGrpWpr').classList.add('followed');
//            addMember(new Member({
//                name: cd.userDetail().name,
//                image: cd.userDetail().img,
//                uid: cd.userDetail().id
//            }));
//            self.userType('subscribe');
//            cd.pubsub.publish('removeNotification', self.boxid);
//            cd.pubsub.publish('perm', self.userType());
//            cd.pubsub.publish('dinvite', self.boxid);
//            setTimeout(function () {
//                document.getElementById('joinGrpWpr').classList.remove('show');
//            }, 3300);
//            analytics.trackEvent('Follow', 'Join group', 'Clicking on join group button, on the box level');
//            dataContext.follow({
//                data: { BoxUid: self.boxid }
//            });

//            cd.postFb(self.name(),
//                zboxResources.IJoined.format(self.name()),
//                cd.location());                     
//        };


//        //#endregion


//        //#region tab

//        self.switchTab = function (currenttab) {
//            if (self.currentTab() === currenttab.id) {
//                return;
//            }
//            //window.location.hash = "Dashboard";
//            self.currentTab(currenttab.id);
//            cd.pubsub.publish('itemTab', currenttab.id);
//        };

//        self.myDrag = function (element) {
//            if (element.nodeType === 1) {
//                $(element).droppable({
//                    accept: ".boxItem:not(.boxAddItem)",
//                    hoverClass: 'dropHover',
//                    drop: function (event, ui) {
//                        if (!checkTabPermission()) {
//                            return;
//                        }
//                        var $this = $(this);
//                        assignItemToTab(ui.draggable[0].id, ko.dataFor(event.target).id);
//                        var $span = $('<span class="drop boldFont">+1</span>');
//                        $this.append($span);
//                        $span.fadeIn(1000, function () {
//                            $span.remove();
//                        });
//                    },
//                    over: function (event, ui) {
//                        ui.helper.css('cursor', 'pointer');
//                    },
//                    tolerance: 'pointer'
//                });
//            }

//        };
//        function assignItemToTab(itemid, tabid) {
//            dataContext.addItemToTab({
//                data: { BoxId: self.boxid, TabId: tabid, ItemId: itemid }
//            });
//        }
//        function checkTabPermission() {
//            if (!cd.register()) {
//                cd.pubsub.publish('register', { action: true });
//                return false;
//            }
//            if (self.follow()) {
//                cd.notification(zboxResources.NeedToFollowBox);
//                return false;
//            }
//            return true;
//        }
//        self.addTab = function () {
//            if (checkTabPermission())
//                createTabDialog.dialog('show');
//        };
//        var createTabDialog = $('#box_createTabDialog').dialog({
//            submitCallBack: function (url, data, form) {
//                var tabName;
//                $.each(data, function (i, fd) {
//                    if (fd.name === "Name") {
//                        tabName = fd.value;
//                        return false;
//                    }
//                });
//                var exists = ko.utils.arrayFirst(self.tabs(), function (i) {
//                    return i.name() === tabName;
//                });
//                if (exists) {
//                    cd.displayErrors(form, zboxResources.TabExists);
//                    return false;
//                }
//                createTab(data);

//            }
//        });
//        function createTab(data) {
//            data.push({ name: 'BoxId', value: self.boxid });
//            dataContext.addTab({
//                data: data,
//                success: function (result) {
//                    analytics.trackEvent('Box', 'New Folder', 'Number of folders opened by users');
//                    var newTab = new Tab(result);
//                    self.tabs.unshift(newTab);
//                    scrollTabs();
//                    $(document.getElementById('box_filters').children[1]).click(); // we do this on 1 because 0 is "All"

//                },
//                error: function (msg) {
//                    if ($.isArray(msg)) {
//                        cd.notification(msg[0].Value[0]);
//                    }
//                }
//            });
//        }

//        self.tabMenu = function (m, e) {
//            if (!checkTabPermission()) {
//                return;
//            }
//            e.stopPropagation();
//            var ul = $(e.target).find('ul').show();
//            $('body').one('click', function () {
//                ul.hide();
//            });
//        };
//        self.tabRename = function (m, e) {
//            var $e = $(e.target);
//            closeMenu($e);
//            var d = cd.contentEditableFunc($e.closest('.current').find('.boxFolderName'), function (val) {
//                dataContext.renameTab({
//                    data: { TabId: m.id, NewName: val, BoxUid: self.boxid },
//                    success: function () {
//                        m.name(val);
//                        d.finish();
//                    },
//                    error: function () {
//                    }
//                });
//            });
//        };
//        self.tabManage = function (m, e) {
//            var $e = $(e.target);
//            closeMenu($e);
//            cd.pubsub.publish('itemTabM', { id: m.id, name: m.name() });
//        };
//        self.tabDelete = function (m, e) {
//            var $e = $(e.target);
//            closeMenu($e);

//            cd.confirm(zboxResources.SureDeleteTab, function () {
//                dataContext.deleteTab({
//                    data: { TabId: m.id, BoxUid: self.boxid },
//                    success: function () {
//                        self.tabs.remove(m);
//                        self.switchTab({});
//                    }
//                });
//            }, null);
//        };

//        function closeMenu(elem) {
//            elem.closest('ul').hide();
//        }
//        //#endregion

//        //#region scroll of tabs

//        var scrollCreated = false, increated = false;
//        function scrollTabs() {

//            if (increated) {
//                return;
//            }
//            increated = true;

//            var boxFilterOffset = $window.height() - $boxFilters.offset().top + $window.scrollTop();

//            if ($createFolder.offset().top + 40 > $window.height()) {
//                $createFolder.height(40);
//                if (boxFilterOffset >= 0) {
//                    cd.innerScroll($boxFilters, boxFilterOffset);
//                } else {
//                    $boxFilters.slimScroll({ 'destroy': '' });
//                    scrollCreated = false;

//                }

//                scrollCreated = true;
//                increated = false;
//                return;
//            }
//            if (scrollCreated) {
//                $boxFilters.slimScroll({ 'destroy': '' });
//                //    $createFolder.css({ height: $window.height() - $createFolder.offset().top });
//                scrollCreated = false;
//            }
//            increated = false;

//        }
//        //#endregion
//        //#region event
//        function registerEvents() {
//            $('#showAllMembers').click(function () {
//                analytics.trackEvent('BoxSettings', 'Open - show all members');
//                openBoxSettings('Members');
//            });
//            $('#boxSettings').click(function () {
//                analytics.trackEvent('BoxSettings', 'Open - show settings');
//                openBoxSettings('Settings');
//            });
//            //$('.static').click(function () {
//            document.getElementById('boxInvite').addEventListener('click', function (e) {
//                if (!cd.register()) {
//                    cd.pubsub.publish('register', { action: true });
//                    e.stopPropagation();
//                    e.preventDefault();
//                    return;
//                }
//                if (self.follow()) {
//                    cd.notification(zboxResources.NeedToFollowBox);
//                    e.stopPropagation();
//                    e.preventDefault();
//                    return;
//                }
//            }, true);

//            function openBoxSettings(tabName) {
//                if (!cd.register()) {
//                    cd.pubsub.publish('register', { action: true });
//                    return;
//                }
//                if (self.follow()) {
//                    cd.notification(zboxResources.NeedToFollowBox);
//                    return false;
//                }
//               var data = {
//                    boxUid: self.boxid,
//                    boxType: self.boxtype(),
//                    userType: self.userType(),
//                    image: self.image(),
//                    name: self.name(),
//                    willDelete: function () {
//                        if (self.userType() === 'owner') {
//                            return 1;
//                        }
//                        if (self.members().length <= 2 && self.noOfComments() < 2 && self.noOfItems() === 0) {
//                            return 2;
//                        }
//                        return 0;
//                    }
//                }
//                if (self.boxtype() === 'academic') {
//                    data.professor = self.proffer();
//                    data.courseNumber = self.courseId();
//                } else {
//                    data.boxType = "private";
//                    data.ownerName = self.ownerName();
//                    data.privacy = self.privacySetting();
//                }
//                cd.pubsub.publish('boxSettings', { tabName: tabName, settings: data });

//            }
//            cd.pubsub.subscribe('updateBoxInfo', function (d) {
//                self.name(d.name);
//                self.courseId(d.courseId);
//                self.proffer(d.professor);
//                if (d.privacy) {
//                    self.privacySetting(d.privacy);
//                }

//            });

//            cd.pubsub.subscribe('updateBoxUrl', function (boxName) {
//                var location = self.copyLink().substring(0, self.copyLink().length - 1);
//                    location = location.substring(0, location.lastIndexOf('/') + 1) + boxName + '/';
//                self.copyLink(location);
//                if (window.history) {
//                    cd.historyManager.remove();

//                    window.history.replaceState(location, '', location);
//                }
//            });
//            cd.pubsub.subscribe('addItem', function () {
//                if (self.boxid === '') {
//                    return;
//                }
//                if (self.userType() !== 'none' && self.userType() !== 'invite') {
//                    return;
//                }

//                addMember(new Member({
//                    name: cd.userDetail().name,
//                    image: cd.userDetail().img,
//                    uid: cd.userDetail().id
//                }));
//                self.userType('subscribe');
//                document.getElementById('joinGrpWpr').classList.remove('show');
//                cd.pubsub.publish('perm', self.userType());
//                cd.pubsub.publish('dinvite', self.boxid);
//            });

//            cd.pubsub.subscribe('enterItem', function () {
//                if (!self.follow()) {
//                    return;
//                }
//                self.followbox();
//            });
//            //cd.pubsub.subscribe('boxclear', revertSettings);
//            //function revertSettings() {
//            //    for (var i = 0; i < contentEditable.length; i++) {
//            //        contentEditable[i].finish();
//            //    }
//            //    contentEditable = [];
//            //    self.name.valueHasMutated();
//            //    self.courseId.valueHasMutated();
//            //    self.proffer.valueHasMutated();
//            //    $('#BoxTopWpr').removeClass('boxEdit');
//            //    //$('#box_Owner').show();
//            //}
//            //function updateBoxInfo() {
//            //    self.name($('#box_Name').text());
//            //    self.courseId($('#box_Course').text());
//            //    self.proffer($('#box_Prof').text());
//            //    revertSettings();
//            //    dataContext.updateBoxInfo({
//            //        data: { boxuid: self.boxid, name: self.name(), courseCode: self.courseId(), professor: self.proffer() }
//            //    });
//            //    if (self.userType() === 3) {
//            //        self.privacySetting($('#box_privacy').find('select').val());
//            //        dataContext.chngPrvcySttings({
//            //            data: {
//            //                boxUid: self.boxid,
//            //                privacy: self.privacySetting()
//            //            }
//            //        });
//            //    }
//            //    dataContext.changeNotification({
//            //        data: {
//            //            boxUid: self.boxid,
//            //            notification: $('#box_notification').find('select').val()
//            //        },
//            //    });
//            //}            
//            $('#box_FS').click(function () {
//                cd.shareFb(cd.location(), //url
//                    self.name(), //title
//                    self.boxtype() === 'academic' ? self.name() + ' - ' + self.ownerName() : self.name(), //caption
//                    zboxResources.IShared + ' ' + self.name() + ' ' + zboxResources.OnCloudents + '<center>&#160;</center><center></center>' + zboxResources.CloudentsJoin,
//                    null //picture
//                   );
//            });

//            $('#box_CL').click(function () {
//                this.select();
//            });

//            $('#box_msg').click(function () {
//                cd.pubsub.publish('message',
//                    { text: zboxResources.FindThisInteresting + '\n\u200e"' + self.name() + '" - ' + cd.location() }
//                    );
//            });

//            $('.boxActions').find('.backWpr').click(function () {
//                cd.newUpdates.deleteLocalUpdates(self.boxid);
//            });

//            //cd.menu($('#box_CL'),$('#boxShareMenu'), function () {
//            //    cd.pubsub.publish('init_clipboard', $('#box_CL'));
//            //}, function () { cd.pubsub.publish('destroy_clipboard'); });
//            //            $('.box_CL').click(function () {cd.pubsub.publish('init_clipboard', $('#box_CL')) });

//            //send a message
//            //$(document).on('click', '.membersAll .msg', function () {
//            //    cd.pubsub.publish('message', [{ name: $(this).data('name'), id: $(this).data('uid') }]);
//            //});

//            cd.pubsub.subscribe('windowChanged', function () {
//                if (document.getElementById('box').style.display === 'block') {
//                    scrollTabs();

//                    $createFolder.css({ height: $window.height() - $createFolder.offset().top });
//                }
//            });
//            cd.pubsub.subscribe('box_show', function () {
//                //ie issue
//                $createFolder = $('.createFolder');
//                $boxFilters = $('#box_filters');
//                $createFolder.css({ height: $window.height() - $createFolder.offset().top });
//                scrollTabs();
//            });


//        }
//        //#endregion




//        //#region clear
//        cd.pubsub.subscribe('boxclear', function () {

//            self.boxid = '';
//            self.name('');
//            self.image('');
//            self.ownerName('');
//            self.ownerId('');
//            self.boxtype('');
//            self.courseId('');
//            self.proffer('');
//            self.privacySetting('');
//            self.members([]);
//            self.noOfItems = ko.observable(5);
//            self.noOfComments = ko.observable(5);
//            //self.userType('none');
//            self.tabs([]);
//            document.getElementById('joinGrpWpr').classList.remove('followed');
//            document.getElementById('joinGrpWpr').classList.remove('show');
//            //cd.pubsub.publish('destroy_clipboard', $('#box_CL'));
//            self.currentTab(null);
//            $('#boxShare').prop('checked', false);
//            document.getElementById('BoxItemList').classList.remove('sponsored');
//        });
//        //#endregion
//    }
//})(jQuery, cd.data, ko, cd, JsResources, cd.analytics);