(function () {
    "use strict";
    (function () {
        cd.pubsub.subscribe('library', function () {
            ko.applyBindings(new LibraryViewModel(), document.getElementById('Library'));
            $(window).unbind('library.singleton');
        }, 'library.singleton');
        $(window).bind('library.singleton', function () {
            ko.applyBindings(new LibraryViewModel(), document.getElementById('Library'));
            $(window).unbind('library.singleton');
        });
    })();
    function LibraryViewModel() {
        var libraryConst = 'Library',
        self = this, cache = {};
        function Node(data) {
            var self = this;
            data = data || {};
            self.id = data.Id;
            self.name = data.Name;
            self.color = data.Color;
            self.template = 'library-node';


        }
        function LibraryBox(data) {
            var self = this;
            Box.call(self, data);
            self.userType = ko.observable(data.UserType);
            self.boxFollow = function () {
                return self.userType() === 'invite' || self.userType() === 'none';
            };
            self.boxSubscribed = function () {
                return self.userType() === 'subscribe' || self.userType() === 'owner';
            };
            self.subscribe = function () {
                $.ajaxRequest.post({
                    url: "/Share/SubscribeToBox",
                    data: { BoxUid: self.uid },
                    done: function () {
                        self.userType('subscribe');
                    }
                });
            };
            self.template = 'library-box';
        }

        self.elements = ko.observableArray([]);
        self.count = ko.computed({
            read: function () {
                if (!self.loaded()) {
                    return '';
                }
                var count = self.elements().length;
                if (count === 0) {
                    return '';
                }
                if (self.elements()[0] instanceof LibraryBox) {
                    return ''
                }
                if (count === 1) {
                    return '1 item';
                }
                return count + ' items';
            }
            , deferEvaluation: true
        }, self);
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
            var elem = self.elements()[0];
            if (elem === undefined) {
                return true;
            }
            if (self.elements()[0].constructor === LibraryBox) {
                return true;
            }
            return false;
        }, self);
        self.backvisible = ko.computed(function () {
            if (!self.loaded()) {
                return false;
            }
            return getLibraryId() !== '';
        }, self);

        self.backUrl = ko.observable();

        self.title = ko.observable(cd.getParameterByName('name'));
        self.titleShow = ko.computed(
            function () {
                if (!self.loaded() || !self.elements().length) {
                    return '';
                }
                if (self.elements()[0] instanceof Node)
                {
                    return '';
                }
                return self.title();
            }
        ,self);
            

        $(window).bind('library', function (e) {
            LibraryList(getLibraryId());
            GetParent(getLibraryId());
            self.title(cd.getParameterByName('name'));
        });

        LibraryList(getLibraryId());
        GetParent(getLibraryId());

        function LibraryList(section) {
            $.ajaxRequest.get({
                beforeSend: function () {
                    self.loaded(false);
                    self.elements([]);
                },
                url: "/" + libraryConst + "/Nodes",
                data: { section: section },
                done: function (data) {

                    if (data.Nodes.length) {
                        generateModel(data.Nodes, function (data) { return new Node(data); });
                    }
                    if (data.Boxes.length) {
                        generateModel(data.Boxes, function (data) { return new LibraryBox(data); });
                    }
                },
                always: function () {
                    self.loaded(true);
                }
            });
        }
        function GetParent(section) {
            if (section === undefined || section === '') {
                return;
            }
            $.ajaxRequest.get({
                url: "/" + libraryConst + "/ParentNode",
                data: { section: section },
                done: function (data) {
                    if (data == null) {
                        self.backUrl('/' + libraryConst);
                    }
                    else {
                        self.backUrl('/' + libraryConst + '?section=' + data.Id + '&name=' + data.Name);
                    }
                }
            });

        }
        function generateModel(data, action) {
            var temp = [];
            for (var i = 0, length = data.length; i < length; i++) {
                var node = action(data[i]);
                temp.push(node);
            }
            self.elements(temp);
        }

        function getLibraryId() {
            return cd.getParameterByName('section');
        }

        function CreateNode(url, name) {
            $.ajaxRequest.post({
                url: url,
                data: name,
                done: function (data) {
                    self.elements.push(new Node(data));
                }
            });
        }
        self.search = function (form, e) {
            var $form = $(form);
            if (!$form.valid || $form.valid()) {
                Search($form.serializeArray());
            }
        }
        function Search(data) {
            $.ajaxRequest.get({
                url: '/Library/Search',
                data: data,
                done: function (data) {
                    generateModel(data, function (data) { return new LibraryBox(data); });
                }
            });
        }

        var nodeDialog = $('#createNodeDialog').dialog({
            submitCallBack: function (url, data) {
                data.push(pushParentId());
                CreateNode(url, data);
            }
        });
        function pushParentId() {
            return { name: 'ParentId', value: getLibraryId() };
        }
        function CreateBox(url, param) {
            $.ajaxRequest.post({
                url: url,
                data: param,
                done: function (data) {
                    self.elements.push(new LibraryBox(data));
                }
            });
        }
        var academicBoxDialog = $('#createAcademicBoxDialog').dialog({
            submitCallBack: function (url, data) {
                data.push(pushParentId());
                CreateBox(url, data);
            }
        });
    }
})();
