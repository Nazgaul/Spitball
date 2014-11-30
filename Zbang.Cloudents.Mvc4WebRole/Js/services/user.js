mUser.factory('sUser', [
    'ajaxService',
   
    function (ajaxService) {
        function buildPath(path) {
            return '/user/' + path + '/';
        }
        return {
            friends: function (data) {
                return ajaxService.get(buildPath('friends'), data);
            },
            minProfile: function (data) {
                return ajaxService.get(buildPath('minprofile'), data);
            },         
            adminFriends: function (data) {
                return ajaxService.get(buildPath('adminfriends'), data);
               
            },
            boxes: function (data) {
                return ajaxService.get(buildPath('boxes'), data);
              
            },
            invites: function (data) {
                return ajaxService.get(buildPath('ownedinvites'), data);
            },
            activity: function (data) {
                return ajaxService.get(buildPath('activity'), data);
            },
            departments: function (data) {
                return ajaxService.get(buildPath('adminboxes'), data);
            },
            notification: function () {
                return ajaxService.get(buildPath('notification'));
            }
    };
}
]);
