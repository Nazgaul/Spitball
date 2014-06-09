/// <reference path="../Views/Library/Index.cshtml" />
(function (cd, dataContext, ko, ZboxResources, analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('l')) {
        return;
    }

    cd.loadModel('library', 'LibraryContext', registerKOLibrary);

    function registerKOLibrary() {
        var $LibraryContent = document.getElementById('libraryContent');
        if (!$LibraryContent) {
            return;
        }
        ko.applyBindings(new LibraryViewModel(), $LibraryContent);
    }
    function LibraryViewModel() {
        var libraryConst = 'library',
        self = this, page = 0,

        libraryId = '';

        function Node(data) {
            data = data || {};
            this.id = data.id;
            this.name = data.name;
            this.template = 'library-node';

            this.url = '/library/' + this.id + '/' + encodeURIComponent(this.name);// + '/?r=library';
        }
        function LibraryBox(data) {
            var that = this;
            Box.call(that, data);
            that.boxUrl = that.boxUrl;//.substring(0, that.boxUrl.indexOf('?'));// + '?r=library'; ///replace dashboard with library
            that.boxFollow = function () {
                return that.userType() === 'invite' || that.userType() === 'none';
            };
            that.boxSubscribed = function () {
                return that.userType() === 'subscribe' || that.userType() === 'owner';
            };
            that.subscribe = function () {
                that.userType('subscribe');
                dataContext.subscribeBox({
                    data: { BoxUid: that.uid }
                });
                cd.postFb(that.name,
                JsResources.IJoined.format(that.name),

                cd.location());
                //analytics.trackEvent('Follow', 'Follow', 'Clicking on follow button, on the departement level');
            };


            that.template = 'library-box';
        }

        self.elements = ko.observableArray([]);
        var paggingNeed = true;

        self.loaded = ko.observable(false);
        self.displayMode = function (elem) {
            return elem.template;
        };
        self.addnode = function () {
            nodeDialog.dialog('show');
        };
        self.addBox = function () {
            academicBoxDialog.dialog('show');
        };
        self.nodevisible = ko.computed(function () {
            if ($('#univeristyName').data('id') !== parseInt(cd.userDetail().id, 10)) {
                return false;
            }
            if (!self.loaded()) {
                return false;
            }
            var elem = self.elements()[0];
            if (elem === undefined) {
                return true;
            }
            if (self.elements()[0].constructor === Node) {
                return true;
            }
            return false;
        }, self);
        self.boxvisible = ko.computed(function () {
            if (!self.loaded()) {
                return false;
            }
            if (!getLibraryId()) {
                return false;
            }
            var elem = self.elements()[0];
            if (elem === undefined) {
                return false; // trigger empty state
            }
            if (self.elements()[0].constructor === LibraryBox) {
                return true;
            }
            return false;
        });
        self.backvisible = ko.computed(function () {
            if (!self.loaded()) {
                return false;
            }

            return getLibraryId();
        }, self);

        self.arrowVisible = ko.observable();

        self.backUrl = ko.observable();

        self.title = ko.observable();
        self.titleShow = ko.computed(
            function () {
                var loaded = self.loaded();
                if (!loaded) {
                    return '';
                }

                if (self.title()) {
                    return self.title();
                }
                return JsResources.TopLevel;
            });

        //#region emptystate

        self.emptyState = ko.computed(function () {
            return self.loaded() && !self.elements().length;
        });

        //#endregion


        cd.pubsub.subscribe('lib_nodes', function (data) {
            refreshScreen(function () {
                libraryId = data.id;
                libraryList();
                $('#lib_NodeName').addClass('hover');
                self.title(decodeURIComponent(data.name || ''));
                var uniName = document.getElementById('univeristyName').textContent;
                if (self.title()) {
                    cd.setTitle('{0} | {1} | Cloudents'.format(uniName, self.title()));
                } else {
                    cd.setTitle('{0} | Cloudents'.format(uniName));
                }
              

            });
        });


        function refreshScreen(action) {
            clearBoard();
            action();
        }

        function clearBoard() {
            try {
                window.scrollTo(0, 0);
            }
            catch(err) {
                console.log(err.message);
            }
            self.elements([]);
            $('#lib_NodeName').show().next('input').remove();
            paggingNeed = true;
            page = 0;
        }

        function libraryList() {
            self.loaded(false);
            var $libraryList = $('#libraryList'), initData = $libraryList.data('data');
            var $libraryContentWrpr = $('#libraryContent .contentWpr');
            if (initData) {
                processData(initData);
                $libraryList.data('data', null).removeAttr('data-data');
                self.loaded(true);
                return;
            }


            var loader = cd.renderLoading($libraryContentWrpr);

            dataContext.library({
                data: { section: getLibraryId(), page: page },
                success: function (result) {
                    paggingNeed = false;
                    processData(result);
                },
                always: function () {
                    self.loaded(true);
                    loader();
                }
            });
            function processData(result) {
                if (result.nodes.elem.length) {
                    generateModel(result.nodes,
                        result.nodes.count, function (data) { return new Node(data); });
                    cd.pubsub.publish('libNodesGen');
                }
                if (result.boxes.elem.length) {
                    generateModel(result.boxes, result.boxes.count, function (data) { return new LibraryBox(data); });
                }
                if (!result.parent) {
                    self.backUrl('/' + libraryConst);
                    self.arrowVisible(getLibraryId() === true);
                }
                else {
                    self.backUrl('/' + libraryConst + '/' + result.parent.id + '/' + result.parent.name);
                }           
                
                cd.pubsub.publish('lib_load');

            }
        }

        function generateModel(data, count, action) {
            var temp = [];
            paggingNeed = true;
            for (var i = 0, length = data.elem.length; i < length; i++) {
                temp.push(action(data.elem[i]));
            }
            self.elements.push.apply(self.elements, temp);//.valueHasMutated();


            //cd.pubsub.publish('lib_load');
        }

        function getLibraryId() {
            return libraryId;
        }

        function createNode(url, name) {
            dataContext.createDepartment({
                data: name,
                success: function (data) {
                    self.elements.push(new Node(data));
                },
                error: function (msg) {
                    cd.notification(msg);
                }
            });
        }



        var nodeDialog = $('#createNodeDialog').dialog({
            submitCallBack: function (url, data) {
                var item = ko.utils.arrayFirst(self.elements(), function (i) {
                    return i.name === data[0].value;
                });
                if (item) {
                    cd.notification(JsResources.ItemExists);
                    return false;
                }
                data.push(pushParentId());
                createNode(url, data);
            }
        });
        function pushParentId() {
            return { name: 'ParentId', value: getLibraryId() };
        }
        function createBox(param, f) {

            dataContext.createAcademicBox({
                data: param,
                success: function (data) {
                    var librarybox = new LibraryBox(data);
                    academicBoxDialog.dialog('hide');
                    cd.resetForm(f);
                    cd.pubsub.publish('nav', librarybox.boxUrl);
                },
                error: function (msg) {
                    cd.displayErrors(f, msg);
                    //cd.notification(msg[0].Value[0]);
                }
            });
        }
        var academicBoxDialog = $('#createAcademicBoxDialog').dialog({
            submitCallBack: function (url, data, form) {
                data.push(pushParentId());
                createBox(data, form);
                return false;
            }
        });
        //empty state
        self.createAcademicBox = function (f) {
            var $f = $(f), d = $f.serializeArray();
            d.push(pushParentId());
            createBox(d, f);
        };


        self.unsubscribe = function (box) {
            var isok = false,
                isDelete = box.userType() === 'owner' || (box.membersCount <= 2 && box.commentCount < 2 && box.itemCount === 0);

            if (isDelete) {
                isok = confirm(JsResources.DeleteCourse);
            }
            else {
                isok = confirm(JsResources.SureYouWant + ' ' + JsResources.ToLeaveGroup);
            }
            if (!isok) {
                return;
            }

            dataContext.removeBox2({
                data: { boxUid: box.uid }
            });
            if (isDelete) {
                self.elements.remove(box);
                return;
            }
            box.userType('none');
        };


        //#region deleteNode
        self.deleteNode = function () {
            var id = getLibraryId();
            dataContext.deleteNode({
                data: { id: id },
                success: function () {
                    cd.pubsub.publish('nav', $('#lib_Back').attr('href'));
                    //location.href = $('#lib_Back').prop('href');
                }
            });
        };
        //self.deleteNodeVisible = ko.computed(function () {
        //    if (!self.loaded()) {
        //        return false;
        //    }
        //    return !self.elements().length;
        //});
        //#endregion

        //#region renameNode
        self.renameNodeVisible = ko.computed(function () {
            if (!self.loaded()) {
                return false;
            }
            if (!getLibraryId()) {
                return false;
            }

            return true;
        });
        $('#lib_NodeRename').click(function () {

            var $this = $(this),
              a = $('#lib_Back').replaceWith(function () {
                  return $('<span id="lib_Back">' + $(this).html() + "</span>");
              });
            $('.boxesHeader').addClass('editName');
            var x = cd.elementToInput($('#lib_NodeName'), function (val) {
                finishProcess(val);
            });

            //  $('#lib_Back').replaceWith('<span>');
            var oldFunc = x.show;
            x.show = function () {
                oldFunc();
                $('#lib_Back').replaceWith(a);
                $('.boxesHeader').removeClass('editName');
                $this.css('display', '');
            };

            $this.hide();
            function finishProcess(val) {
                dataContext.renameNode({
                    data: { Id: getLibraryId(), NewName: val },
                    success: function () {
                        if (window.history) {
                            var elements = ['library', getLibraryId(), x.input.val()];
                            history.pushState(null, '', '/' + elements.join('/'));
                        }

                        //$this.show();

                        x.show();
                        $('#lib_NodeName').text(val);
                    },
                    error: function (msg) {
                        cd.notification(msg);
                        //input.focus();
                    }
                });
            }
        });

        //#endregion


        cd.registerScroll(function () {
            if (paggingNeed && self.loaded()) {
                page++;
                libraryList();
            }
        }, $('#library'));

        //cd.loader.registerFacebook();
        //cd.loader.registerTwitter();

        function facebookLikeBox() {
            //var likeBox = document.getElementById('facebookLikeBox');
            //if (!likeBox) {
            //    return;
            //}
            //var href = likeBox.getAttribute('data-href'), link = likeBox.getAttribute('data-link');
            //var height = Math.floor($(window).height() - $(likeBox).offset().top);

            //var src = link.replace(/{{href}}/i, href).replace(/{{height}}/i, height);

            //likeBox.height = height;
            //likeBox.src = src;
            var likeBox = document.getElementById('facebookLikeBox');
            if (!likeBox) {
                return;
            }
            var href = likeBox.getAttribute('data-href'), link = likeBox.getAttribute('data-link');
            var height = Math.floor($(window).height() - $(likeBox).offset().top - 15);
            

            var src = link.replace(/href=/i, 'href='+ href).replace(/height=/i,'height='+ height);
            height = Math.ceil(height / 10) * 10;
            likeBox.height = height;
            likeBox.src = src;

        }

        cd.pubsub.subscribe('windowChanged', function () {
            if (document.getElementById('library').style.display === 'block') {
                innerScrollLetter();
            }
        });

        cd.pubsub.subscribe('library_show', function () {
            facebookLikeBox();
            innerScrollLetter();
        });
        //cd.pubsub.subscribe('windowChanged', function () {
        //    if (document.getElementById('Library').style.display === 'block') {
        //        innerScrollLib();
        //    }
        //});
        function innerScrollLetter() {
            $('#uniLetter').removeClass('unionFeaturedHeight').attr('height', $(document).height() - 65);

            //    var $libList = $('.libLists');
            //    //cd.innerScroll($libList, $(window).height() - parseInt($libList.css('padding-top').replace('px', ''),10) - $libList.offset().top);
        }

        //analytics
        $('.u-Website').click(function () {
            analytics.trackEvent('Library', 'Go to org website', 'number of clicks on the union website icon');
        });
        $('.u-Fb').click(function () {
            analytics.trackEvent('Library', 'Go to org Facebok page', 'number of clicks on the union facebook page icon');
        });
        analytics.setLibrary($('.unionName').text());


    }
})(cd, cd.data, ko, JsResources, cd.analytics);
