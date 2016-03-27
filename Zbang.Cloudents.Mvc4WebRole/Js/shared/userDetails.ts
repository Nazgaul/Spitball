/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
declare var __insp: any;


interface IUserDetailsFactory {
    init(refresh?): ng.IPromise<IUserData>;
    get(): IUserData;
    isAuthenticated(): boolean;
    setName(first, last): void;
    setImage(image): void;
    setUniversity(name, id): void;
    setTheme(theme): void;
}

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
    culture:string;
    createTime: Date;
    university: IUniversity;
}

(() => {
    angular.module('app').factory('userDetailsFactory', userDetails);
    userDetails.$inject = ['$rootScope', '$filter', '$timeout', '$q', '$http', 'ajaxService', 'Analytics'];
    function userDetails($rootScope, $filter, $timeout, $q, $http, ajaxService, analytics): IUserDetailsFactory {
        "use strict";
        var
            isAuthenticated = false,
            userData: IUserData,
            serverCall = false,
            deferDetails = $q.defer();

        function setDetails(data) {
            // data = data || {};
            if (data.id) {
                isAuthenticated = true;
                // ReSharper disable UseOfImplicitGlobalInFunctionScope
                __insp.push(['identify', data.id]);
                // ReSharper restore UseOfImplicitGlobalInFunctionScope
            }
            analytics.set('dimension1', data.universityName || null);
            analytics.set('dimension2', data.universityCountry || null);
            analytics.set('dimension3', data.id || null);
            analytics.set('dimension4', data.theme || 'dark');
            userData = {
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
                university: {
                    country: data.universityCountry, // for google analytics
                    name: data.universityName, // in library page
                    id: data.universityId
                }
            };

        }

        return {
            init: refresh => {
                if (refresh) {
                    deferDetails = $q.defer();
                    userData = null;
                }
                if (userData) {
                    deferDetails.resolve(userData);
                    return deferDetails.promise;
                }
                if (!serverCall) {
                    serverCall = true;

                    ajaxService.get('/account/details/').then(response => {
                        setDetails(response);
                        deferDetails.resolve(userData);
                        serverCall = false;
                    });
                }
                return deferDetails.promise;
            },
            get: () => userData,



            isAuthenticated: () => isAuthenticated,
            setName: (first, last) => {
                //userData.firstName = first;
                //userData.lastName = last;
                userData.name = first + " " + last;
                $rootScope.$broadcast('userDetailsChange');
            },
            setImage: image => {
                if (!image) {
                    return;
                }
                userData.image = image;
                $rootScope.$broadcast('userDetailsChange');
            },
            setUniversity: (name, id) => {
                userData.university.name = name;
                userData.university.id = id;
                $rootScope.$broadcast('universityChange', userData);
            },
            setTheme: theme => {
                userData.theme = theme;
                //$rootScope.$broadcast('themeChange', userData);
            }
            //updateChange: function () {
            //    $rootScope.$broadcast('userDetailsChange');
            //},

            //getUniversity: function () {
            //    if (_.isEmpty(userData.university)) {
            //        return false;
            //    }
            //    return userData.university;

            //},
            //initDetails: function () {
            //    if (this.isAuthenticated()) {
            //        var defer = $q.defer();
            //        $timeout(function () {
            //            defer.resolve();
            //        });
            //        return defer.promise;
            //    }

            //    var promise = sAccount.details();

            //    promise.then(function (response) {
            //        setDetails(response);
            //    });

            //    return promise;
            //}
        };
    }

})();