module app {
    "use strict";
    export interface IUserUpdatesService {
        updatesNum(boxid: number): angular.IPromise<number>;
        boxUpdates(boxid: number): angular.IPromise<Object>;
        deleteUpdates(boxId: number): void;

    }

    class UserUpdatesService implements IUserUpdatesService {
        static $inject = ["ajaxService2", "$q", "userDetailsFactory", "$rootScope", "$window", "$stateParams"];

        private data: Array<any>;
        private deferred: angular.IDeferred<any>;
        private allUpdates = {};
        constructor(private ajaxService: IAjaxService2, private $q: angular.IQService,
            private userDetailsFactory: IUserDetailsFactory,
            private $rootScope: angular.IRootScopeService, private $window: angular.IWindowService,
            private $stateParams: spitaball.ISpitballStateParamsService) {

            this.deferred = $q.defer();
            userDetailsFactory.init()
                .then((userData: IUserData) => {
                    if (userData.id)
                        //if (userData.university.id) {
                        this.getUpdates();
                    //} else {
                    //need to resolve this
                    // this.deferred.resolve();
                    //}
                });

            $rootScope.$on("$stateChangeSuccess", (event: angular.IAngularEvent, toState:
                angular.ui.IState, toParams: spitaball.ISpitballStateParamsService,
                fromState: angular.ui.IState,
                fromParams: spitaball.ISpitballStateParamsService) => {
                if (fromState.parent === "box") {
                    this.deleteUpdates(fromParams.boxId);
                }
            });
            //$window.onbeforeunload = () => {
            //    var boxId = $stateParams.boxId;
            //    if (boxId) {
            //        this.deleteFromServer(boxId);
            //    }
            //};

        }
        private getUpdates() {
            this.ajaxService.get("/account/updates/").then((response2: Array<any>) => {
                this.data = response2;
                for (let i = 0; i < response2.length; i++) {
                    const currBox = this.allUpdates[response2[i].boxId] || {};
                    if (response2[i].answerId) {
                        currBox[response2[i].answerId] = true;
                    }
                    else if (response2[i].questionId) {
                        currBox[response2[i].questionId] = true;
                    }
                    this.allUpdates[response2[i].boxId] = currBox;

                }
                this.deferred.resolve();
            }).catch(() => {
                this.deferred.reject();
            });
        }

        deleteUpdates(boxId: number) {
            this.updatesNum(boxId).then((length: number) => {
                if (!length) {
                    return;
                }
                this.deleteFromServer(boxId);
                delete this.allUpdates[boxId];
                this.$rootScope.$broadcast("delete-updates", boxId);
            });
        }
        deleteFromServer(boxId: number) {
            if (!this.userDetailsFactory.isAuthenticated()) {
                return;
            }
            this.ajaxService.post("/box/deleteupdates/", {
                boxId: boxId
            });
        }



        updatesNum(boxid: number): angular.IPromise<number> {
            var q = this.$q.defer<number>();

            this.boxUpdates(boxid).then(() => {
                q.resolve(this.allUpdates[boxid] ? Object.keys(this.allUpdates[boxid]).length : 0);
            });

            return q.promise;
        }

        boxUpdates(boxid: number) {
            const promise = this.deferred.promise;

            var q = this.$q.defer();

            promise.then(() => {
                q.resolve(this.allUpdates[boxid]);
            });
            return q.promise;

        }
    }
    angular.module('app.user').service('userUpdatesService', UserUpdatesService);
}


