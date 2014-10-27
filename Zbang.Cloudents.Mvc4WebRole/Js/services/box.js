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
                return ajaxService.post('/Dashboard/Create/', data);
            },
            createAcademic: function (data) {
                return ajaxService.post('/Library/CreateBox/', data);
            },
            updateInfo: function (data) {
                return ajaxService.post(buildPath('UpdateInfo'), data);
            },
            items: function (data) {
                return ajaxService.get(buildPath('Items'), data);
            },
            quizes: function (data) {
                return ajaxService.get(buildPath('Quizes'), data);
            },
            info: function (data) {
                return ajaxService.get(buildPath('Data'), data);
            },
            createTab: function (data) {
                return ajaxService.post(buildPath('CreateTab'), data);
            },
            deleteTab: function (data) {
                return ajaxService.post(buildPath('DeleteTab'), data);
            },
            renameTab: function (data) {
                return ajaxService.post(buildPath('RenameTab'), data);
            },
            addItemsToTab: function (data) {
                return ajaxService.post(buildPath('AddItemToTab'), data);
            },
            members: function (data) {
                return ajaxService.get(buildPath('Members'), data);
            },
           
            invite: function (data) {
                return ajaxService.post('/Share/InviteBox/', data);
            },
            removeUser: function (data) {
                return ajaxService.post(buildPath('RemoveUser'), data);
            },
            remove: function (data) {
                return ajaxService.post(buildPath('Delete2'), data);
            },
            notification: function (data) {
                return ajaxService.get(buildPath('GetNotification'), data);
            },
            follow: function (data) {
                return ajaxService.post('/Share/SubscribeToBox/', data);
            },
            tabs: function (data) {
                return ajaxService.get(buildPath('Tabs'), data);
            },
            deleteUpdates: function (data) {
                return ajaxService.post(buildPath('DeleteUpdates'), data,true);
            }
        };
    }
    ]);
//});