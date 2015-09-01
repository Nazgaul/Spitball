
(function () {
    angular.module('app').config(
    [
        '$stateProvider', '$locationProvider', '$urlRouterProvider',
        function ($stateProvider, $locationProvider, $urlRouterProvider) {


            $stateProvider
                //.state('root', {
                //    abstract: true,
                //    controller: ['userDetails', function (userDetails) {
                //        var root = this;
                //        root.user = {
                //            name: userDetails.getName(),
                //            image: userDetails.getImage(),
                //            isAuthenticated: userDetails.isAuthenticated(),
                //            isAdmin: userDetails.isAdmin()
                //        };
                //    }],
                //    controllerAs: 'root',
                //    template: '<div ui-view></div>',
                //    resolve: {
                //        user: ['userDetails', function (userDetails) {
                //            return userDetails.init();
                //        }]
                //    }
                //}).
                //.state('root.libChoose', {
                //    url: '/library/choose/',
                //    templateUrl: function () {
                //        return buildUrl('/library/choosepartial/');
                //    },
                //    controller: 'LibChooseController as libChoose'
                //})
                .state('dashboard', {
                    url: '/dashboard/',
                    templateUrl: function() {
                        return buildUrl('/dashboard/indexpartial/');
                    },
                    controller: 'Dashboard as d'
                    //onEnter: dashboardRedirect
                });      
            $urlRouterProvider.rule(function ($injector, $location) {

                var path = $location.path();
                var hasTrailingSlash = path[path.length - 1] === '/';

                if (!hasTrailingSlash) {
                    //if last charcter is a slash, return the same url without the slash  
                    var newPath = path + '/';
                    return newPath;
                }
                return null;
            });
            function buildUrl(path) {
                return path + '?lang=' + getCookie('l2') + '&version=' + window.version;

                function getCookie(cname) {
                    var name = cname + "=";
                    var ca = document.cookie.split(';');
                    for (var i = 0; i < ca.length; i++) {
                        var c = ca[i];
                        while (c.charAt(0) == ' ') c = c.substring(1);
                        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
                    }
                    return "";
                }
            }
        }


    ]);
})();