(function () {
    "use strict";

    (function () {
        var x = $('#dash_tabs');
        if (x.length) {
            ko.applyBindings(new tabsViewModel(), x[0]);
        }

    })();

    function tabsViewModel() {
        function tab(data) {
            this.id = data.Id;
            this.name = data.Name;
        }
        var self = this;
        self.tabs = ko.observableArray([]);
        getTabs();

        function getTabs() {
            $.ajaxRequest.post({
                url: "/Dashboard/Tab",
                done: function (data) {
                    if (data.length) {
                        var mapped = $.map(data, function (d) { return new tab(d); });
                        self.tabs(mapped);
                    }
                }
            });
        }

        self.switchTab = function (currenttab) {
            //window.location.hash = "Dashboard";
            cd.pubsub.publish('tabselected', currenttab.id);
        };
        self.addTab = function () {
            createTabDialog.dialog('show');
        };
        var createTabDialog = $('#dash_createTabDialog').dialog({
            submitCallBack: function (url, data, form) {
                var tabName;
                $.each(data, function (i, fd) {
                    if (fd.name === "Name") {
                        tabName = fd.value;
                        return false;
                    }
                });
                var exists = ko.utils.arrayFirst(self.tabs(), function (i) {
                    return i.name === tabName;
                });
                if (exists) {
                    cd.displayErrors(form, 'This tab exits');
                    return false;
                }
                createTab(url, data);
            }
        });
        function createTab(url, data) {
            $.ajaxRequest.post({
                url: url,
                data: data,
                done: function (result) {
                    var newTab = new tab(result);
                    self.tabs.push(newTab);
                },
                error: function (msg) {
                    if ($.isArray(msg)) {
                        cd.notification(msg[0].Value[0]);
                    }
                }
            });
        }
        
        //#region dropable
        self.myPostProcessingLogic = function (element) {
            if (element.nodeType === 1) {
                $(element).droppable({
                    accept: ".box",
                    activeClass: 'dropable',
                    hoverClass: "drop-hover",
                    drop: function (event, ui) {
                        assignBoxToTab(ui.draggable[0].id, ko.dataFor(event.target).id);
                    }
                });
            }

        };
        function assignBoxToTab(boxid, tabid) {
            $.ajaxRequest.postJson({
                url: 'Dashboard/AddBoxToTab',
                data: { BoxId: boxid, TabId: tabid },
                done: function (result) {
                    //var newTab = new tab(result);
                    //self.tabs.push(newTab);
                },
                error: function (msg) {
                    //if ($.isArray(msg)) {
                    //    cd.notification(msg[0].Value[0]);
                    //}
                }
            });
        }
        //#endregion
    }



}
)();