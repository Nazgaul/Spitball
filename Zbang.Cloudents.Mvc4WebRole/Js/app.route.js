
(function () {
    angular.module('app').config(
    [
        '$stateProvider', '$locationProvider', '$urlRouterProvider',
    function ($stateProvider, $locationProvider, $urlRouterProvider) {
        $stateProvider
            .state('item', {
                url: '/item/{universityName}/{boxId}/{boxName}/{itemId}/{itemName}/',
                templateUrl: function () {
                    return buildUrl('/item/indexpartial/');
                },

                controller: 'ItemController as i'
            })
            .state('jobs', {
                url: '/jobs/',
                templateUrl: function () {
                    return buildUrl('/home/jobs/');
                }
            }).
            state('blog', {
                url: '/blog/',
                templateUrl: function () {
                    return buildUrl('/home/blog/');
                }
            })
            .state('help', {
                url: '/help/',
                templateUrl: function () {
                    return buildUrl('/home/helppartial/');
                }
            })
             .state('department', {
                 url: '/library/',
                 templateUrl: function () {
                     return buildUrl('/library/indexpartial/');
                 },
                 controller: 'Library as l'
             })
             .state('departmentWithNode', {
                 url: '/library/:nodeId/:nodeName/',
                 templateUrl: function () {
                     return buildUrl('/library/indexpartial/');
                 },
                 controller: 'Library as l'
             })
            .state('search', {
                url: '/search/',
                templateUrl: function () {
                    return buildUrl('/search/indexpartial/');
                },
                controller: 'SearchController as s',
                data : { animateClass: 'searchState' }
            })
            .state('dashboard', {
                url: '/dashboard/',
                templateUrl: function () {
                    return buildUrl('/dashboard/indexpartial/');
                },
                controller: 'Dashboard as d',
                data: { animateClass: 'dashboardState' }
                //onEnter: dashboardRedirect
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