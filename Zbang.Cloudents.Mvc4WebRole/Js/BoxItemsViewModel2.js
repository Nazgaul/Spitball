(function ($, dataContext, ko, cd, ZboxResources, analytics) {
    "use strict";

    if (window.scriptLoaded.isLoaded('bivm')) {
        return;
    }

    cd.loadModel('box', 'BoxContext', registerKoBoxItems);

    function registerKoBoxItems() {
        ko.applyBindings(new BoxItemViewModel(), $('#box_items')[0]);
    }

    function BoxItemViewModel() {
        function BaseItem(data) {
            data = data || {};
            var that = this;
            that.name = data.name || '';
            that.uid = data.id;
            that.type = data.type;
            that.numOfViews = data.numOfViews || 0;
            that.commentsCount = data.commentsCount || 0;
            that.ownerId = data.ownerId;
            that.ownerName = data.owner;
            that.ownerUrl = data.userUrl;
            that.rate = 69 / 5 * data.rate;

            that.date = new Date(data.date).getTime();
            that.isNew = ko.observable(data.isNew || false);
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
                         that.ownerId === cd.userDetail().nId);
                });
            that.itemUrl = data.url;// + '?r=box';
        }
        function Quiz(data) {
            var that = this;
            BaseItem.call(that, data);
            that.publish = data.publish;
            that.noPreview = ' noPreview';
            that.description = data.description || '';
        }
        function Item(data) {
            var that = this;
            BaseItem.call(that, data);
            that.thumbnailUrl = data.thumbnail;
            that.numOfDownloads = data.numOfDownloads || 0;
            that.download = data.downloadUrl;
            that.extension = cd.getExtension(data.name, data.type);
            that.extensionColor = cd.getExtensionColor(data.name, data.type);
            that.sponsored = data.sponsored || false;
            that.linkUrl = data.linkUrl || '';

          
            if (data.description) {
                that.description = data.description;
                that.noPreview = '';
            } else {
                that.noPreview = ' noPreview';//space is needed
                that.description = '';
            }
        }

        var self = this, boxid, current = 0, //countOfItems = 0,
            tab;
        self.items = ko.observableArray([]);
        self.permission = ko.observable('none');
        self.loaded = ko.observable(false);
        self.loadedAnimation = ko.observable(false);

        self.currentView = ko.observable('BoxItemsThumbViewTemplate');

        self.viewMode = function (item) {
            if (item.type.toLowerCase() === 'quiz') {
                if (self.currentView() === 'BoxItemsThumbViewTemplate') {
                    return 'QuizItemsThumbViewTemplate';
                }

                return 'QuizItemsListViewTempalte';
            }
            return self.currentView();
        }

        cd.pubsub.subscribe('perm', function (d) {
            self.permission(d);
        });


        cd.pubsub.subscribe('box', function (data) {
            boxid = parseInt(data.id, 10);
            current = 0;
            tab = null;
            self.permission('none');
            getItems();
        });
        cd.pubsub.subscribe('itemTab', function (d) {
            if (typeof d === 'string') {
                tab = d;
            } else {
                tab = null;
            }

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
                    generateModel(result);
                    if (cd.register()) {
                        cd.pubsub.publish('getUpdates');
                        cd.newUpdates.deleteAll(boxid);
                    }                    
                },
                always: function () {
                    self.loaded(true);
                }
            });
        }
        cd.pubsub.subscribe('updates', function (updates) {
            var box = updates[boxid],
                items, item;

            if (!box) {
                return;
            }

            items = box.items;

            if (!items) {
                return;
            }

            for (var i = 0, l = self.items().length; i < l; i++) {
                item = self.items()[i];
                item.isNew(items.indexOf(item.uid) > -1);
            }

        });

        function generateModel(data) {
            var mapped = [], sponsored = false;
            for (var i = 0, l = data.length; i < l; i++) {
                if (data[i].type.toLowerCase() === 'quiz') {
                    if (tab ||self.manageTab()) {                        
                        continue;
                    }

                    mapped.push(new Quiz(data[i]));                    
                    continue;

                }
                if (data[i].sponsored) {
                    sponsored = true;
                }
                mapped.push(new Item(data[i]));

            }
            mapped.sort(sort);
            if (sponsored) {
                document.getElementById('BoxItemList').classList.add('sponsored');                
            } else {
                document.getElementById('BoxItemList').classList.remove('sponsored');
            }

            var tt = new TrackTiming('Box Items', 'Render time of items');
            tt.startTime();
            self.items(mapped);
            cd.loadImages(document.getElementById('BoxItemList'));
            tt.endTime();
            tt.send();
            self.loadedAnimation(true);

            var firstTimeBox = document.getElementById('firstTime').getAttribute('data-boxf');
            if (firstTimeBox === 'true') {
                cd.pubsub.publish('tooltipGuide', {
                    guideId: 'quiz'      
                });
                document.getElementById('firstTime').removeAttribute('data-boxf');
            }
        }

        function sort(a, b) {
            if (a.sponsored) {
                return -1;
            }
            if (b.sponsored) {
                return 1;
            }
            if (a.date < b.date) {
                return 1
            } else {
                return -1;
            }
            if (a.name < b.name) {
                return 1;
            }
            else {
                return -1;
            }
            return 0;
        }

        //#region addItem
        self.addItem = function () {
            if (!cd.register()) {
                cd.pubsub.publish('register');
                return;
            }
            cd.google.register(true);
            cd.pubsub.publish('upload');
        };

        cd.pubsub.subscribe('addItem', function (d) {
            if (d.boxid !== boxid) {
                return;
            }

            try {
                var newItem;

                if (d.type.toLowerCase() === 'quiz') {
                    newItem = new Quiz(d);
                } else {
                    newItem = new Item(d);
                }

                var x = ko.utils.arrayFirst(self.items(), function (i) {
                    return i.uid === newItem.uid;
                });

                if (newItem.type.toLowerCase() === 'quiz') {
                    if (x) {
                        newItem.date = x.date;
                        self.items.remove(x);

                    }
                    self.items.unshift(newItem);
                    self.items.sort(sort);
                    return;
                }

                if (x) { //other type than quiz
                    return;
                }
                self.items.unshift(newItem);
                self.items.sort(sort);
                cd.pubsub.publish('clear_cache');
                cd.loadImages(document.getElementById('BoxItemList'));

            } catch (e) {
                console.log(e);
            }
        });

        cd.pubsub.subscribe('addedItem', function (d) {
            if (d.boxid === boxid) {
                d.item.boxid = d.boxid;
                d.item.isNew = true;
                cd.pubsub.publish('addItem', d.item);
            }
            cd.newUpdates.addUpdate({ itemId: d.item.id, boxId: d.boxid });
        });
        //#endregion

        self.emptyState = ko.computed(function () {
            return self.loaded() && !self.items().length;
        });

        //#region remove item

        self.removeQuiz = function (quiz) {
            if (quiz.ownerId !== cd.userDetail().nId) {
                cd.notification(JsResources.DontHavePermissionToDelete + ' ' + quiz.type.toLowerCase());
                return;
            }

            var quizName = quiz.name || 'quiz draft';
            cd.confirm(JsResources.SureYouWantToDelete + ' ' + quizName + "?",
                            function () {
                                self.items.remove(quiz);
                                cd.pubsub.publish('deleteQuiz', quiz.uid);
                                //countOfItems--;
                                dataContext.quizDelete({
                                    data: { id: quiz.uid },
                                    success: function () {
                                        cd.pubsub.publish('removeItemNotification', { itemid: quiz.uid, boxid: boxid });
                                    },
                                    error: function () {
                                        self.items.push(quiz);
                                    }
                                });

                            }, null);

        };


        self.removeItem = function (item) {
            if (!cd.deleteAllow(self.permission(), item.ownerId)) {
                cd.notification(JsResources.DontHavePermissionToDelete + ' ' + item.type);
                return;
            }
            cd.confirm(JsResources.SureYouWantToDelete + ' ' + item.name + "?",
                function () {
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

                }, null);
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
                if (element.classList.contains('quizItem')) {
                    return;
                }
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
            setView('BoxItemsThumbViewTemplate', document.getElementById('thumbViewBtn'));
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
        self.quizSlct = function (quiz) {
            if (self.manageTab()) {
                quiz.isCheck(!quiz.isCheck());
                return false;
            }

            if (!quiz.publish) {
                //var el = document.getElementById('quizCreate');
                //var scope = angular.element(el).scope();
                //scope.$apply(function () {
                //    scope.initQuiz({ boxId: boxid, boxName: document.getElementById('box_Name').textContent, quizId: quiz.uid });
                //});
                
                cd.pubsub.publish('initQuiz', { boxId: boxid, boxName: document.getElementById('box_Name').textContent, quizId: quiz.uid });
                return false;
            }

            if (quiz.isNew()) {
                quiz.isNew(false);
                cd.newUpdates.deleteUpdate({ type: 'quizzes', boxId: boxid, id: quiz.uid });
            }

            $('#quiz').remove();  //fix for quiz navigation
            cd.pubsub.publish('enterItem');
            return true;
        }

        self.itmSlct = function (item) {
            if (self.manageTab()) {
                item.isCheck(!item.isCheck());
                return false;
            }

            //remove the new tag
            if (item.isNew()) {
                item.isNew(false);
                cd.newUpdates.deleteUpdate({ type: 'items', boxId: boxid, id: item.uid });
            }

            cd.pubsub.publish('enterItem');
            return true;

        };


        $('.boxItemsViewToggle').click(function (e) {
            var type = e.target.getAttribute('data-template');
            setView(type, e.target);
        });
        //Analytics
        $('#BoxItemList').on('click', 'a.downloadBtn', function (e) {
            var data = ko.dataFor(e.target);
            cd.pubsub.publish('item_Download', { id: data.uid });
            analytics.trackEvent('Box', 'Download', 'The number of downloads made on box view');
        });


        $('#addQuiz').click(function (e) {
            if (!cd.register()) {
                cd.pubsub.publish('register');
                return;
            }

            cd.pubsub.publish('initQuiz', { boxId: boxid, boxName: document.getElementById('box_Name').textContent });
            
            //var el = document.getElementById('quizCreate');
            //var scope = angular.element(el).scope();
            //scope.$apply(function () {
            //    scope.initQuiz({ boxId: boxid, boxName: document.getElementById('box_Name').textContent });
            //});

            //angular.element('#quizCreate').scope().$broadcast('initQuiz')
        });

        $('#BoxItemList').hoverIntent({
            over: function (e) {
                var item = ko.dataFor(this), html;

                if (cd.getElementPosition(this).top - $(window).scrollTop() < 132) {//132 header+ topbar
                    return;
                }
                if (item.type.toLowerCase() === 'file') {
                    html = cd.attachTemplateToData('boxItemTooltipTemplate', item);
                }

                else if (item.type.toLowerCase() === 'quiz') {
                    if (!item.publish) {
                        return;
                    }
                    html = cd.attachTemplateToData('boxQuizTooltipTemplate', item);
                }
                if (!this.querySelector('.boxItemTt')) {
                    this.insertAdjacentHTML('afterbegin', html);
                }
                var tooltip = this.querySelector('.boxItemTt');
                if (item.type.toLowerCase() === 'link') {
                    $(tooltip).addClass('ttLink').find('.ttDetail').remove();
                }

                $(tooltip).fadeIn(300);
                cd.parseTimeString($(tooltip).find('[data-time]'));
            },
            out: function () {
                $('.boxItemTt').remove();
            },
            selector: 'li.boxItem',
            timeout: 100,
            interval: 400
        });

        cd.pubsub.subscribe('clearTooltip', function () {
            $('.boxItemTt').remove();
        });

        function setView(view, element) {
            if (self.currentView() === view) {
                return;
            }

            $('.boxItemsViewToggle').removeClass('currentState');

            element.classList.add('currentState');
            self.currentView(view);
            cd.loadImages(document.getElementById('BoxItemList'));
        }

    }
})(jQuery, cd.data, ko, cd, JsResources, cd.analytics);
