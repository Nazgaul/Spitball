/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../scripts/typings/angular-google-analytics/angular-google-analytics.d.ts" />
/// <reference path="ajaxservice2.ts" />
'use strict';
// ReSharper disable once InconsistentNaming
declare var __insp: any;
declare var googletag: any;

//export interface IUserDetailsFactory {
//    init(): angular.IPromise<IUserData>;
//    get(): IUserData;
//    isAuthenticated(): boolean;
//    setName(first, last): void;
//    setImage(image): void;
//    getUniversity(): number;
//    setUniversity(name, id): angular.IPromise<IUserData>;
//    setTheme(theme): void;
//}


interface IUniversity {
    country: string;
    name: string;
    id: number;
}

interface IUserData {
    id: number;
    name: string;
    image: string;
    sex: number;
    score: number;
    email: string;
    url: string;
    isAdmin: boolean;
    theme: string;
    culture: string;
    unread: number;
    createTime: Date;
    university: IUniversity;
}

module app {
    "use strict";
    export interface IUserDetailsFactory {
        init(): angular.IPromise<IUserData>;
        get(): IUserData;
        isAuthenticated(): boolean;
        setName(first, last): void;
        setImage(image): void;
        getUniversity(): number;
        setUniversity(name, id): angular.IPromise<IUserData>;
        setTheme(theme): void;
    }

    class UserDetails implements IUserDetailsFactory {
        static $inject = ['$rootScope', '$q', 'ajaxService2', 'Analytics'];

        private isLogedIn = false;
        private userData: IUserData;
        private serverCall = false;
        private deferDetails = this.$q.defer();

        constructor(private $rootScope: angular.IRootScopeService, private $q: angular.IQService,
            private ajaxService: IAjaxService2, private analytics: angular.google.analytics.AnalyticsService) {
        }



        setDetails(data) {

            // data = data || {};
            if (data.id) {
                this.isLogedIn = true;
                // ReSharper disable UseOfImplicitGlobalInFunctionScope
                __insp.push(['identify', data.id]);
                // ReSharper restore UseOfImplicitGlobalInFunctionScope
            }
            this.analytics.set('dimension1', data.universityName || null);
            this.analytics.set('dimension2', data.universityCountry || null);
            this.analytics.set('dimension3', data.id || null);
            this.analytics.set('dimension4', data.theme || 'dark');

            //$timeout(() => {
            //    googletag.pubads().setTargeting('gender', data.sex);
            //}, 1000);

            var interval = window.setInterval(() => {
                if (googletag.pubads !== undefined && googletag.pubads) {
                    googletag.pubads().setTargeting('gender', data.sex);
                    googletag.pubads().setTargeting('university', data.universityId);
                    window.clearInterval(interval);
                }
            }, 20);

            this.userData = {
                id: data.id,
                name: data.name,
                image: data.image,
                sex: data.sex,
                score: data.score,
                url: data.url,
                createTime: new Date(data.dateTime),
                isAdmin: data.isAdmin,
                theme: data.theme,
                culture: data.culture,
                email: data.email,
                unread: data.unread,
                university: {
                    country: data.universityCountry, // for google analytics
                    name: data.universityName, // in library page
                    id: data.universityId
                }
            };

        }


        init(): angular.IPromise<IUserData> {

            if (this.userData) {
                this.deferDetails.resolve(this.userData);
                return this.deferDetails.promise;
            }
            if (!this.serverCall) {
                this.serverCall = true;

                this.ajaxService.get('/account/details/', null, 'accountDetail').then(response => {
                    this.setDetails(response);
                    this.deferDetails.resolve(this.userData);
                    this.serverCall = false;
                });
            }
            return this.deferDetails.promise;
        }
        get = () => {
            return this.userData;
        }
        isAuthenticated = () => {
            return this.isLogedIn;
        }
        setName = (first: string, last: string) => {
            this.userData.name = first + " " + last;
            this.$rootScope.$broadcast('userDetailsChange');
        }
        setImage = (image?: string) => {
            if (!image) {
                return;
            }
            this.userData.image = image;
            this.$rootScope.$broadcast('userDetailsChange');
        }
        getUniversity = (): number => {
            return this.userData ? this.userData.university.id : null;
        }
        setUniversity = (): angular.IPromise<IUserData> => {
            this.ajaxService.deleteCacheCategory('accountDetail');
            this.userData = null;
            return this.init();
        }
        setTheme = (theme: string) => {
            this.userData.theme = theme;
        }

        //    setUniversity: () => {

        //        ajaxService.deleteCacheCategory('accountDetail');
        //        userData = null;

        //        return init();
        //        //userData.university.name = name;
        //        //userData.university.id = id;
        //        //$rootScope.$broadcast('universityChange', userData);
        //    },
        //    setTheme: theme => {
        //        userData.theme = theme;
        //    }
        //return {
        //   // init: initialize,
        //    get: () => userData,
        //    isAuthenticated: () => isAuthenticated,
        //    setName: (first, last) => {
        //        userData.name = first + " " + last;
        //        $rootScope.$broadcast('userDetailsChange');
        //    },
        //    setImage: image => {
        //        if (!image) {
        //            return;
        //        }
        //        userData.image = image;
        //        $rootScope.$broadcast('userDetailsChange');
        //    },
        //    getUniversity: () => {
        //        return userData ? userData.university.id : null;
        //    },
        //    setUniversity: () => {

        //        ajaxService.deleteCacheCategory('accountDetail');
        //        userData = null;

        //        return init();
        //        //userData.university.name = name;
        //        //userData.university.id = id;
        //        //$rootScope.$broadcast('universityChange', userData);
        //    },
        //    setTheme: theme => {
        //        userData.theme = theme;
        //    }
        //};

    }
    angular.module('app').service('userDetailsFactory', UserDetails);
}

//(() => {
//    angular.module('app').factory('userDetailsFactory', userDetails);
//    userDetails.$inject = ['$rootScope', '$q', 'ajaxService2', 'Analytics'];

//    function userDetails($rootScope: angular.IRootScopeService,
//        $q: angular.IQService,
//        ajaxService: IAjaxService2,
//        analytics: angular.google.analytics.AnalyticsService): IUserDetailsFactory {
//        "use strict";
//        var
//            isAuthenticated = false,
//            userData: IUserData,
//            serverCall = false,
//            deferDetails = $q.defer();

//        function setDetails(data) {
//            // data = data || {};
//            if (data.id) {
//                isAuthenticated = true;
//                // ReSharper disable UseOfImplicitGlobalInFunctionScope
//                __insp.push(['identify', data.id]);
//                // ReSharper restore UseOfImplicitGlobalInFunctionScope
//            }
//            analytics.set('dimension1', data.universityName || null);
//            analytics.set('dimension2', data.universityCountry || null);
//            analytics.set('dimension3', data.id || null);
//            analytics.set('dimension4', data.theme || 'dark');

//            //$timeout(() => {
//            //    googletag.pubads().setTargeting('gender', data.sex);
//            //}, 1000);

//            var interval = window.setInterval(() => {
//                    if (googletag.pubads !== undefined && googletag.pubads) {
//                        googletag.pubads().setTargeting('gender', data.sex);
//                        googletag.pubads().setTargeting('university', data.universityId);
//                        window.clearInterval(interval);
//                    }
//                },
//                20);

//            userData = {
//                id: data.id,
//                name: data.name,
//                image: data.image,
//                sex: data.sex,
//                score: data.score,
//                url: data.url,
//                createTime: new Date(data.dateTime),
//                isAdmin: data.isAdmin,
//                theme: data.theme,
//                culture: data.culture,
//                email: data.email,
//                unread: data.unread,
//                university: {
//                    country: data.universityCountry, // for google analytics
//                    name: data.universityName, // in library page
//                    id: data.universityId
//                }
//            };

//        }


//        function init() {
//            if (userData) {
//                deferDetails.resolve(userData);
//                return deferDetails.promise;
//            }
//            if (!serverCall) {
//                serverCall = true;

//                ajaxService.get('/account/details/', null, 'accountDetail')
//                    .then(response => {
//                        setDetails(response);
//                        deferDetails.resolve(userData);
//                        serverCall = false;
//                    });
//            }
//            return deferDetails.promise;
//        }

//        return {
//            init: init,
//            get: () => userData,
//            isAuthenticated: () => isAuthenticated,
//            setName: (first, last) => {
//                userData.name = first + " " + last;
//                $rootScope.$broadcast('userDetailsChange');
//            },
//            setImage: image => {
//                if (!image) {
//                    return;
//                }
//                userData.image = image;
//                $rootScope.$broadcast('userDetailsChange');
//            },
//            getUniversity: () => {
//                return userData ? userData.university.id : null;
//            },
//            setUniversity: () => {

//                ajaxService.deleteCacheCategory('accountDetail');
//                userData = null;

//                return init();
//                //userData.university.name = name;
//                //userData.university.id = id;
//                //$rootScope.$broadcast('universityChange', userData);
//            },
//            setTheme: theme => {
//                userData.theme = theme;
//            }
//        };
//    }

//})();//(() => {
//    angular.module('app').factory('userDetailsFactory', userDetails);
//    userDetails.$inject = ['$rootScope', '$q', 'ajaxService2', 'Analytics'];

//    function userDetails($rootScope: angular.IRootScopeService,
//        $q: angular.IQService,
//        ajaxService: IAjaxService2,
//        analytics: angular.google.analytics.AnalyticsService): IUserDetailsFactory {
//        "use strict";
//        var
//            isAuthenticated = false,
//            userData: IUserData,
//            serverCall = false,
//            deferDetails = $q.defer();

//        function setDetails(data) {
//            // data = data || {};
//            if (data.id) {
//                isAuthenticated = true;
//                // ReSharper disable UseOfImplicitGlobalInFunctionScope
//                __insp.push(['identify', data.id]);
//                // ReSharper restore UseOfImplicitGlobalInFunctionScope
//            }
//            analytics.set('dimension1', data.universityName || null);
//            analytics.set('dimension2', data.universityCountry || null);
//            analytics.set('dimension3', data.id || null);
//            analytics.set('dimension4', data.theme || 'dark');

//            //$timeout(() => {
//            //    googletag.pubads().setTargeting('gender', data.sex);
//            //}, 1000);

//            var interval = window.setInterval(() => {
//                    if (googletag.pubads !== undefined && googletag.pubads) {
//                        googletag.pubads().setTargeting('gender', data.sex);
//                        googletag.pubads().setTargeting('university', data.universityId);
//                        window.clearInterval(interval);
//                    }
//                },
//                20);

//            userData = {
//                id: data.id,
//                name: data.name,
//                image: data.image,
//                sex: data.sex,
//                score: data.score,
//                url: data.url,
//                createTime: new Date(data.dateTime),
//                isAdmin: data.isAdmin,
//                theme: data.theme,
//                culture: data.culture,
//                email: data.email,
//                unread: data.unread,
//                university: {
//                    country: data.universityCountry, // for google analytics
//                    name: data.universityName, // in library page
//                    id: data.universityId
//                }
//            };

//        }


//        function init() {
//            if (userData) {
//                deferDetails.resolve(userData);
//                return deferDetails.promise;
//            }
//            if (!serverCall) {
//                serverCall = true;

//                ajaxService.get('/account/details/', null, 'accountDetail')
//                    .then(response => {
//                        setDetails(response);
//                        deferDetails.resolve(userData);
//                        serverCall = false;
//                    });
//            }
//            return deferDetails.promise;
//        }

//        return {
//            init: init,
//            get: () => userData,
//            isAuthenticated: () => isAuthenticated,
//            setName: (first, last) => {
//                userData.name = first + " " + last;
//                $rootScope.$broadcast('userDetailsChange');
//            },
//            setImage: image => {
//                if (!image) {
//                    return;
//                }
//                userData.image = image;
//                $rootScope.$broadcast('userDetailsChange');
//            },
//            getUniversity: () => {
//                return userData ? userData.university.id : null;
//            },
//            setUniversity: () => {

//                ajaxService.deleteCacheCategory('accountDetail');
//                userData = null;

//                return init();
//                //userData.university.name = name;
//                //userData.university.id = id;
//                //$rootScope.$broadcast('universityChange', userData);
//            },
//            setTheme: theme => {
//                userData.theme = theme;
//            }
//        };
//    }

//})();

