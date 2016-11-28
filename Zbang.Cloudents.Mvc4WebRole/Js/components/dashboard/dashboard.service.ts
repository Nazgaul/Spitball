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

    //var defer: angular.IDeferred<any>, serverCall = false;

    class Dashboard implements IDashboardService {
        static $inject = ["$q", "ajaxService2", "realtimeFactory", "userUpdatesService", "$rootScope"];
        private defer: angular.IDeferred<any>;
        private serverCall = false;
        private serverCallUniversityMeta = false;

        private deferUniversityMeta: angular.IDeferred<any>;

        constructor(private $q: angular.IQService,
            private ajaxService2: IAjaxService2,
            private realtimeFactotry: IRealtimeFactory,
            private userUpdatesService: IUserUpdatesService,
            private $rootScope: angular.IRootScopeService) {
            this.defer = $q.defer();
            this.deferUniversityMeta = $q.defer();


            $rootScope.$on("delete-updates",
                (e, arg) => {
                var box = this.boxes.find(v => (v.id === arg));
                if (box) {
                    box.updates = 0;
                }
            });
            $rootScope.$on("remove-box", (e, arg) => {
                arg = parseInt(arg, 10);
                var box = this.boxes.find(v => (v.id === arg));
                if (box) {
                    const index = this.boxes.indexOf(box);
                    this.boxes.splice(index, 1);
                }
            });
            $rootScope.$on("refresh-boxes", () => {
                this.boxes = null;
                this.defer = $q.defer();
            });
            $rootScope.$on("refresh-university", () => {
                this.universityMeta = null;
                this.deferUniversityMeta = $q.defer();
            });
        }
        boxes = null;
        universityMeta = null;
        getBoxes() {
            if (this.boxes) {
                //defer.resolve(this.boxes);
                return this.$q.when(this.boxes);
            }
            if (!this.serverCall) {
                this.serverCall = true;
                this.ajaxService2.get("dashboard/boxlist/")
                    .then((response: Array<any>) => {
                        this.serverCall = false;
                        this.realtimeFactotry.assingBoxes(response.map(val => val.id));
                        this.boxes = response;
                        for (let i = 0; i < this.boxes.length; i++) {
                            (box => {
                                this.userUpdatesService.updatesNum(box.id).then(val => {
                                    box.updates = val;
                                });
                            })(this.boxes[i]);
                        }
                        this.defer.resolve(this.boxes);
                    });
            }
            return this.defer.promise;
        }
        getUniversityMeta(universityId?: number) {
            if (this.universityMeta) {
                return this.$q.when(this.universityMeta);
            }
            if (!this.serverCallUniversityMeta) {
                this.serverCallUniversityMeta = true;
                this.ajaxService2.get('dashboard/university', { universityId: universityId }, 'university')
                    .then(response => {
                        this.universityMeta = response;
                        this.deferUniversityMeta.resolve(this.universityMeta);
                    });
            }
            return this.deferUniversityMeta.promise;
            //return this.ajaxService2.get('dashboard/university', { universityId: universityId }, 'university');
        };
        createPrivateBox(boxName: string) {
            return this.ajaxService2.post("/dashboard/create/", { boxName: boxName });
        };
        leaderboard() {
            return this.ajaxService2.get("/dashboard/leaderboard/");
        };

        recommended() {
            return this.ajaxService2.get("/dashboard/recommendedcourses/");
        }
    }
    angular.module("app.dashboard").service("dashboardService", Dashboard);
}
