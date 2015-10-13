
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
            .state('quiz', {
                url: '/quiz/{universityName}/{boxId}/{boxName}/{quizId}/{quizName}/',
                templateUrl: function () {
                    return buildUrl('/quiz/indexpartial/');
                },

                controller: 'QuizController as q'
            })
        .state('box', {
            url: '/{boxtype:box|course}/{universityType}/{boxId}/{boxName}/',
            templateUrl: function () {
                return buildUrl('/box/indexpartial/');
            },
            controller: 'BoxController as b',

        }).
              state('box.feed', {
                  templateUrl: function () {
                      return buildUrl('/box/feedpartial/');
                  },
                  url: '#feed',
                  controller: 'FeedController as f'
              }).
             state('box.items', {
                 templateUrl: function () {
                     return buildUrl('/box/itemspartial/');
                 },
                 url: '#items',
                 controller: 'ItemsController as i'
             }).
            state('box.quiz', {
                templateUrl: function () {
                    return buildUrl('/box/quizpartial/');
                },
                url: '#quizzes',
                controller: 'QuizzesController as q'
            }).
            state('box.members', {
                templateUrl: function () {
                    return buildUrl('/box/memberspartial/');
                },
                url: '#members',
                controller: 'MembersController as m'
            }).

            state('jobs', {
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
            .state('user', {
                url: '/user/:userId/:userName/',
                templateUrl: function () {
                    return buildUrl('/user/indexpartial/');
                },
                controller: 'UserController as u'
            })
            //.state('settings', {
            //    url: '/account/settings/',
            //    templateUrl: function () {
            //        return buildUrl('/account/settingpartial/');
            //    },
            //    controller: 'AccountSettingsController as a'
            //})
            //.state('settings.profile', {
            //    url:'#info',
            //    templateUrl: function () {
            //        return buildUrl('/account/info/');
            //    },
            //    controller: 'AccountSettingsInfoController as i'
            //})
            //.state('settings.password', {
            //    url: '#password',
            //    templateUrl: function () {
            //        return buildUrl('/account/password/');
            //    },
            //    controller: 'AccountSettingsPasswordController as p'
            //})
            //.state('settings.notification', {
            //    url: '#notification',
            //    templateUrl: function () {
            //        return buildUrl('/account/notification/');
            //    },
            //    controller: 'AccountSettingsNotificationController as n'
            //})
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
            .state('dashboard', {
                url: '/dashboard/',
                templateUrl: function () {
                    return buildUrl('/dashboard/indexpartial/');
                },
                controller: 'Dashboard as d'
                //onEnter: dashboardRedirect
            });
        //$urlRouterProvider.rule(function ($injector, $location) {

        //    var path = $location.path();
        //    var hasTrailingSlash = path[path.length - 1] === '/';

        //    if (!hasTrailingSlash) {
        //        //if last charcter is a slash, return the same url without the slash  
        //        var newPath = path + '/';
        //        return newPath;
        //    }
        //    return null;
        //});
        //$rootScope.$on('$stateChangeStart', function (event, toState) {
        //    var greeting = toState.data.customData1 + " " + toState.data.customData2;
        //    console.log(greeting);

        //    // Would print "Hello World!" when 'parent' is activated
        //    // Would print "Hello UI-Router!" when 'parent.child' is activated
        //});
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