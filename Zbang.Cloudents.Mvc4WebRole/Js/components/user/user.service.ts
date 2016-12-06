﻿module app {
    'use strict';
    export interface IUserService {
        getDetails(userid:number): angular.IPromise<any>;
        boxes(userid: number, page: number): angular.IPromise<any>;
        friends(userid: number, page: number): angular.IPromise<any>;
        files(userid: number, page: number): angular.IPromise<any>;
        feed(userid: number, page: number): angular.IPromise<any>;
        quiz(userid: number, page: number): angular.IPromise<any>;
        getNotification(): angular.IPromise<any>;
        gamificationBoard(): angular.IPromise<any>;
        levels(userid: number): angular.IPromise<any>;
    }
    class UserService implements IUserService {
        static $inject = ['ajaxService2'];

        constructor(private ajaxService2: IAjaxService2) {
            
        }
        getDetails  (userid) {
            return this.ajaxService2.get("/user/profilestats/", { id: userid });
        }
        boxes  (userid, page) {
            return this.ajaxService2.get("/user/boxes/", { id: userid, page: page });
        }

        friends (userid, page) {
            return this.ajaxService2.get("/user/friends/", { id: userid, page: page });
        }
        files  (userid, page) {
            return this.ajaxService2.get("/user/items/", { id: userid, page: page });
        }
        feed  (userid, page) {
            return this.ajaxService2.get("/user/comment/", { id: userid, page: page });
        }
        quiz  (userid, page) {
            return this.ajaxService2.get("/user/quiz/", { id: userid, page: page });
        }
        getNotification  () {
            return this.ajaxService2.get("/share/notifications/");
        }

        gamificationBoard() {
            return this.ajaxService2.get("/user/gamificationboard");
        }
        levels(userid: number): angular.IPromise<any> {
            return this.ajaxService2.get("/user/levels");
        }
    }
    angular.module("app.user").service("userService", UserService);

}