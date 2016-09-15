module app {
    "use strict";

    export interface IDashboardService {
        getBoxes(): angular.IPromise<any>;
        getUniversityMeta(universityId?: number): angular.IPromise<any>;
        createPrivateBox(boxName: string): angular.IPromise<any>;
        leaderboard(): angular.IPromise<any>;
        recommended(): angular.IPromise<any>;
        boxes: any;
    }

    var defer: angular.IDeferred<any>, serverCall = false;

    class Dashboard implements IDashboardService {
        static $inject = ["$q", "ajaxService2", "realtimeFactory", "userUpdatesService", "$rootScope"];
        constructor(private $q: angular.IQService,
            private ajaxService2: IAjaxService2,
            private realtimeFactotry: IRealtimeFactory,
            private userUpdatesService: IUserUpdatesService,
            private $rootScope: angular.IRootScopeService) {
            defer = $q.defer();

            $rootScope.$on("remove-box", (e, arg) => {
                arg = parseInt(arg, 10);
                var box = this.boxes.find(v => (v.id === arg));
                if (box) {
                    var index = this.boxes.indexOf(box);
                    this.boxes.splice(index, 1);
                }
            });
            $rootScope.$on("refresh-boxes", () => {
                this.boxes = null;
                defer = $q.defer();
            });
        }
        boxes = null;
        getBoxes() {
            if (this.boxes) {
                //defer.resolve(this.boxes);
                return this.$q.when(this.boxes);
            }
            if (!serverCall) {
                serverCall = true;
                this.ajaxService2.get("dashboard/boxlist/")
                    .then((response: Array<any>) => {
                        serverCall = false;
                        this.realtimeFactotry.assingBoxes(response.map(val => val.id));
                        this.boxes = response;
                        for (let i = 0; i < this.boxes.length; i++) {
                            (box => {
                                this.userUpdatesService.updatesNum(box.id).then(val => {
                                    box.updates = val;
                                });
                            })(this.boxes[i]);
                        }
                        defer.resolve(this.boxes);
                    });
            }
            return defer.promise;
        }
        getUniversityMeta(universityId?: number) {
            return this.ajaxService2.get('dashboard/university', { universityId: universityId }, 'university');
        };
        createPrivateBox(boxName: string) {
            return this.ajaxService2.post('dashboard/create', { boxName: boxName });
        };
        leaderboard() {
            return this.ajaxService2.get('dashboard/leaderboard');
        };

        recommended() {
            return this.ajaxService2.get('dashboard/recommendedcourses');
        }
    }
    angular.module("app.dashboard").service("dashboardService", Dashboard);
}

//(function () {
//    angular.module('app.dashboard').service('dashboardService', dashboard);
//    dashboard.$inject = ['$q', 'ajaxService', 'userUpdatesService', '$rootScope', 'ajaxService2', 'realtimeFactotry'];

//    function dashboard($q, ajaxservice, userUpdatesService, $rootScope, ajaxService2, realtimeFactotry) {
//        var d = this,
//            serverCall = false,
//            defer = $q.defer();
//        d.boxes = null;
//        d.getBoxes = function () {
//            if (d.boxes) {
//                defer.resolve(d.boxes);
//                return defer.promise;
//            }
//            if (!serverCall) {
//                serverCall = true;
//                ajaxservice.get('dashboard/boxlist').then(function (response) {
//                    serverCall = false;

//                    realtimeFactotry.assingBoxes(response.map(function (val) {
//                        return val.id;
//                    }));
//                    d.boxes = response;
//                    for (var i = 0; i < d.boxes.length; i++) {
//                        (function (box) {
//                            userUpdatesService.updatesNum(box.id).then(function (val) {
//                                box.updates = val;
//                            });
//                        })(d.boxes[i]);
//                    }
//                    defer.resolve(d.boxes);
//                });
//            }
//            return defer.promise;
//        }
//        $rootScope.$on('remove-box', function (e, arg) {
//            arg = parseInt(arg, 10);
//            var box = d.boxes.find(function (v) {
//                return v.id === arg;
//            });
//            if (box) {
//                var index = d.boxes.indexOf(box);
//                d.boxes.splice(index, 1);
//            }
//        });
//        $rootScope.$on('refresh-boxes', function () {
//            d.boxes = null;
//            defer = $q.defer();
//        });

//        d.getUniversityMeta = function (universityId) {
//            return ajaxService2.get('dashboard/university', { universityId: universityId }, 'university');
//        }


//        d.createPrivateBox = function (boxName) {
//            return ajaxservice.post('dashboard/create', { boxName: boxName });
//        }

//        d.leaderboard = function () {
//            return ajaxservice.get('dashboard/leaderboard');
//        }

//        d.recommended = recommended;

//        function recommended() {
//            return ajaxservice.get('dashboard/recommendedcourses');
//        }
//    }
//})();