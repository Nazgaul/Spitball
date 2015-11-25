
(function () {
    angular.module('app').config(config);
    config.$inject = ['$stateProvider'];
    function config($stateProvider) {


        $stateProvider
            .state('root', {
                abstract: true,
                resolve: {
                    user: [
                        'userDetails', function (userDetails) {
                            return userDetails.init();
                        }
                    ]
                },
                template: '<div ui-view></div>'
            })

        
        .state('jobs', {
            parent: 'root',
            url: '/jobs/',
            templateUrl: function () {
                return buildUrl('/home/jobs/');
            }
        }).
        state('blog', {
            parent: 'root',
            url: '/blog/',
            templateUrl: function () {
                return buildUrl('/home/blog/');
            }
        })
        .state('help', {
            parent: 'root',
            url: '/help/',
            templateUrl: function () {
                return buildUrl('/home/helppartial/');
            }
        })
        .state('item', {
            parent: 'root',
            url: '/item/{universityName}/{boxId}/{boxName}/{itemId}/{itemName}/',
            templateUrl: function () {
                return buildUrl('/item/indexpartial/');
            },

            controller: 'ItemController as i'
        })


        .state('search', {
            parent: 'root',
            url: '/search/',
            templateUrl: function () {
                return buildUrl('/search/indexpartial/');
            },
            controller: 'SearchController as s',
            data: { animateClass: 'searchState' }
        })
        .state('dashboard', {
            parent: 'root',
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

        //$urlRouterProvider.rule(function ($injector, $location) {
        //    var path = $location.url();

        //    // check to see if the path already has a slash where it should be
        //    if (path[path.length - 1] === '/' || path.indexOf('/?') > -1) {
        //        return;
        //    }

        //    if (path.indexOf('?') > -1) {
        //        return path.replace('?', '/?');
        //    }

        //    return path + '/';
        //});
    }



})();