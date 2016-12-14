/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../scripts/typings/angular-google-analytics/angular-google-analytics.d.ts" />
/// <reference path="ajaxservice2.ts" />
// ReSharper disable once InconsistentNaming
declare var __insp: any;
//declare var googletag: any;


interface IUniversity {
    country: string;
    name: string;
    id: number;
}

interface IUserData {
    id: number;
    name: string;
    image: string;
    score: number;
    email: string;
    isAdmin: boolean;
    badges: number;
    culture: string;
    createTime: Date;
    university: IUniversity;

    levelName: string;
    nextLevel: number;
}

module app {
    "use strict";
    export interface IUserDetailsFactory {
        init(): angular.IPromise<IUserData>;
        get(): IUserData;
        isAuthenticated(): boolean;
        setName(first: string, last: string): void;
        setImage(image: string): void;
        getUniversity(): number;
        setUniversity(name: string, id: number): angular.IPromise<IUserData>;
    }

    class UserDetails implements IUserDetailsFactory {
        static $inject = ["$rootScope", "$q", "ajaxService2", "Analytics", "$timeout", "$interval"];

        private isLogedIn = false;
        private userData: IUserData;
        private serverCall = false;
        private deferDetails = this.$q.defer();

        constructor(private $rootScope: angular.IRootScopeService, private $q: angular.IQService,
            private ajaxService: IAjaxService2,
            private analytics: angular.google.analytics.AnalyticsService,
            private $timeout: angular.ITimeoutService,
            private $interval: angular.IIntervalService) {
        }



        private setDetails(data: any) {
            if (data.id) {
                this.isLogedIn = true;
                // ReSharper disable UseOfImplicitGlobalInFunctionScope
                __insp.push(["identify", data.id]);
                // ReSharper restore UseOfImplicitGlobalInFunctionScope
            }
            this.$timeout(() => {
                //analytics doesnt work without timeout
                this.analytics.set("dimension1", data.universityName || "null"); // analytics doesnt support null
                this.analytics.set("dimension2", data.universityCountry || "null"); // analytics doesnt support null
                this.analytics.set("dimension3", data.id || "null"); // analytics doesnt support null
            });


            this.userData = {
                id: data.id,
                name: data.name,
                image: data.image,
                score: data.score,
                createTime: new Date(data.dateTime),
                isAdmin: data.isAdmin,
                culture: data.culture,
                badges: data.badges,
                email: data.email,
                levelName: data.levelName,
                nextLevel: data.nextLevel,
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

                this.ajaxService.get("/account/details/", null, "accountDetail").then((response: Object) => {
                    this.setDetails(response);
                    this.deferDetails.resolve(this.userData);
                    this.serverCall = false;
                });
            }
            return this.deferDetails.promise;
        }
        get = () => {
            return this.userData;
        };
        isAuthenticated = (): boolean => {
            return this.isLogedIn;
        };
        setName = (first: string, last: string) => {
            this.userData.name = first + " " + last;
            this.$rootScope.$broadcast("userDetailsChange");
        };
        setImage = (image?: string) => {
            if (!image) {
                return;
            }
            this.userData.image = image;
            this.$rootScope.$broadcast("userDetailsChange");
        };
        getUniversity = (): number => {
            return this.userData ? this.userData.university.id : null;
        };
        setUniversity = (): angular.IPromise<IUserData> => {
            this.ajaxService.deleteCacheCategory("accountDetail");
            this.$rootScope.$broadcast("refresh-university");
            this.userData = null;
            this.deferDetails = this.$q.defer();
            return this.init();
        };
    }
    angular.module("app").service("userDetailsFactory", UserDetails);
}
