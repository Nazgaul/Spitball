(function ($, ko, cd, dataContext) {

    "use strict";
    if (window.scriptLoaded.isLoaded('mL')) {
        return;
    }

    cd.loadModel('library', 'LibraryContext', registerKOLibrary);

    function registerKOLibrary() {
        var $LibraryContent = $('#libraryContent');
        if (!$LibraryContent.length) {
            return;
        }
        ko.applyBindings(new LibraryViewModel(), document.getElementById('library'));
    }

    function LibraryViewModel() {
        function Node(data) {
            data = data || {};
            this.id = data.id;
            this.name = data.name;
            this.color = data.color;
            this.url = '/library/' + this.id + '/' + encodeURIComponent(this.name);
        }
        function LibraryBox(data) {
            data = data || {};
            var that = this;
            cd.Box.call(that, data);
        }

        var self = this, /*page = 0,*/ libraryId = '',
           LASTMODIFIED = 'LastModified', NAME = 'Name';
       //     paggingNeed = true;

        self.courses = ko.observableArray([]);
        self.department = ko.observableArray([]);

        self.loaded = ko.observable(false);
        self.nodevisible = ko.computed(function () {
            if ($('#univeristyName').data('id') !== $('#userName').data('id')) {
                return false;
            }
            if (!self.loaded() || self.isSearch()) {
                return false;
            }
            if (self.department().length) {
                return true;
            }
            if (!self.courses().length) {
                return true;
            }
           
            return false;
        }, self);
        self.boxvisible = ko.computed(function () {
            if (!self.loaded() || self.isSearch()) {
                return false;
            }
            if (!getLibraryId()) {
                return false;
            }
            if (self.courses().length) {
                return true;
            }
            if (!self.department().length) {
                return true;
            }
            return false;
        });


        self.orderBy = ko.observable(LASTMODIFIED);

       

        self.isSearch = ko.observable(false);
        self.searchTerm = ko.observable('');

        //self.orderByVisible = ko.computed(function () {
        //    if (!self.loaded()) {
        //        return false;
        //    }
        //    return state.courses;

        //});
        function getLibraryId() {
            return libraryId;

        }


        cd.pubsub.subscribe('lib_nodes', function (data) {
            libraryId = data.id;
            clearBoard();
            self.isSearch(false);
            libraryList();
        });
        cd.pubsub.subscribe('lib_search', function (data) {
            //data.query
            clearBoard();
       
            //we need to maintain the hash
            location.hash = 'search';
            self.isSearch(true);
            self.searchTerm(data.query);
            search(data);
        });

        function clearBoard() {
            self.courses([]);
            self.department([]);
            //state.clearState();
            //page = 0;
            //paggingNeed = true;
        }

        function libraryList() {
            self.loaded(false);
            dataContext.library({
                data: { section: getLibraryId()},// page: page, order: self.orderBy() },
                success: function (result) {
                    //paggingNeed = false;
                    if (result.nodes && result.nodes.length) {
                        generateModelDepartment(result.nodes);
                    }
                    if (result.boxes && result.boxes.length) {
                        generateModelCourses(result.boxes);
                    }
                    cd.pubsub.publish('lib_load'/*, actions*/);
                },
                always: function () {
                    self.loaded(true);

                }
            });



        }
        function generateModelDepartment(data) {
            var mapped = $.map(data, function (d) { return new Node(d); });
            self.department.push.apply(self.department, mapped);
            //paggingNeed = true;
        }
        function generateModelCourses(data) {
            var mapped = $.map(data, function (d) { return new LibraryBox(d); });
            self.courses.push.apply(self.courses, mapped);
            //paggingNeed = true;
        }
        //#region sort
        //self.sortName = function () {
        //    self.orderBy(NAME);
        //    clearBoard();
        //    libraryList();
        //};
        //self.sortDate = function () {
        //    self.orderBy(LASTMODIFIED);
        //    clearBoard();
        //    libraryList();
        //};
        //#endregion

        //#region scroll
        //cd.registerScroll(function () {
        //    if (paggingNeed && self.loaded()) {
        //        page++;
        //        libraryList();
        //    }
        //},$('#library'));
        //#endregion
        //#region emptystate
        self.emptyState = ko.computed(function () {
            return self.loaded() && !self.courses().length && !self.department().length && !self.isSearch();
        });
        //#endregion

        //#region search
        function search(data) {
            self.loaded(false);
            dataContext.sLibrary({
                data: data,
                success: function (result) {
                    generateModelCourses(result.Elem);
                    cd.pubsub.publish('lib_load'/*, actions*/);
                },
                always: function () {
                    self.loaded(true);
                }
            });
        }
        cd.pubsub.subscribe('trig_search_library', function (data) {
            cd.pubsub.publish('nav', 'dashboard/Search/' + encodeURIComponent(data));
        });
        cd.pubsub.subscribe('close_search_library', function () {
            cd.pubsub.publish('nav', 'library');
        });
        self.emptyStateSearch = ko.computed(function () {
            return self.loaded() && !self.courses().length && self.isSearch();
        });

        //#endregion

        //#region createBox
        $('#Academic').submit(function (e) {
            e.preventDefault();
            var $form = $(this);
            if (!$form.valid || $form.valid()) {
                var data = $form.serializeArray();
                data.push({ name: 'ParentId', value: getLibraryId() });
                createBox(data);
            }

        });
        function createBox(param) {
            dataContext.createAcademicBox({
                data: param,
                success: function (data) {
                    var librarybox = new LibraryBox(data);
                    cd.pubsub.publish('nav', librarybox.boxUrl);
                },
                error: function (msg) {
                    cd.notification(msg[0].Value[0]);
                }
            });
        }
        //#endregion

        //#region createNode
        $('#cLibNode').submit(function (e) {
            e.preventDefault();
            var $form = $(this);
            if (!$form.valid || $form.valid()) {
                var data = $form.serializeArray();
                data.push({ name: 'ParentId', value: getLibraryId() });
                cd.resetForm($form);
                createNode(data);
            }

        });
        function createNode(name) {
            dataContext.createDepartment({
                data: name,
                success: function (data) {
                    self.department.push(new Node(data));
                    location.hash = '';
                },
                error: function (msg) {
                    cd.notification(msg);
                }
            });
        }
        //#endregion
    }
})(jQuery, ko, cd, cd.data);