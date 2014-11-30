//define('box',['app'], function (app) {
mBox.factory('sBox',
    [
     'ajaxService',

    function (ajaxService) {
        function buildPath(path) {
            return '/box/' + path + '/';
        }
        return {
            createPrivate: function (data) {
                return ajaxService.post('/dashboard/create/', data);
            },
            createAcademic: function (data) {
                return ajaxService.post('/library/createbox/', data);
            },
            updateInfo: function (data) {
                return ajaxService.post(buildPath('updateinfo'), data);
            },
            items: function (data) {
                return ajaxService.get(buildPath('items'), data);
            },
            quizes: function (data) {
                return ajaxService.get(buildPath('quizes'), data);
            },
            info: function (data) {
                return ajaxService.get(buildPath('data'), data);
            },
            createTab: function (data) {
                return ajaxService.post(buildPath('createtab'), data);
            },
            deleteTab: function (data) {
                return ajaxService.post(buildPath('deletetab'), data);
            },
            renameTab: function (data) {
                return ajaxService.post(buildPath('renametab'), data);
            },
            addItemsToTab: function (data) {
                return ajaxService.post(buildPath('additemtotab'), data);
            },
            members: function (data) {
                return ajaxService.get(buildPath('members'), data);
            },                    
            removeUser: function (data) {
                return ajaxService.post(buildPath('removeuser'), data);
            },
            remove: function (data) {
                return ajaxService.post(buildPath('delete2'), data);
            },
            notification: function (data) {
                return ajaxService.get(buildPath('getnotification'), data);
            },
            changeNotification: function (data) {
                return ajaxService.post(buildPath('changenotification'), data);
            },
            follow: function (data) {
                return ajaxService.post('/share/subscribetobox/', data);
            },
            tabs: function (data) {
                return ajaxService.get(buildPath('tabs'), data);
            },
            deleteUpdates: function (data) {
                return ajaxService.post(buildPath('deleteupdates'), data,true);
            },
            sideBar: function (data) {
                return ajaxService.get(buildPath('sidebar'), data);
            }
        };
    }
    ]);
//});