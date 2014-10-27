mUser.factory('sUser', [
    'ajaxService',
   
    function (ajaxService) {
        function buildPath(path) {
            return '/user/' + path + '/';
        }
        return {
            friends: function (data) {
                return ajaxService.get(buildPath('Friends'), data);
            },
            minProfile: function (data) {
                return ajaxService.post(buildPath('Link'), data);
                var dfd = $q.defer();
                $http.get(User + 'MinProfile/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },         
            adminFriends: function (data) {
                return ajaxService.get(buildPath('AdminFriends'), data);
               
            },
            boxes: function (data) {
                return ajaxService.get(buildPath('Boxes'), data);
              
            },
            invites: function (data) {
                return ajaxService.get(buildPath('OwnedInvites'), data);
            },
            activity: function (data) {
                return ajaxService.get(buildPath('Activity'), data);
            },
            departments: function (data) {
                return ajaxService.get(buildPath('AdminBoxes'), data);
            }
    };
}
]);
