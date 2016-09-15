var app;
(function (app) {
    "use strict";
    var defer, serverCall = false;
    var Dashboard = (function () {
        function Dashboard($q, ajaxService2, realtimeFactotry, userUpdatesService, $rootScope) {
            var _this = this;
            this.$q = $q;
            this.ajaxService2 = ajaxService2;
            this.realtimeFactotry = realtimeFactotry;
            this.userUpdatesService = userUpdatesService;
            this.$rootScope = $rootScope;
            this.boxes = null;
            defer = $q.defer();
            $rootScope.$on("remove-box", function (e, arg) {
                arg = parseInt(arg, 10);
                var box = _this.boxes.find(function (v) { return (v.id === arg); });
                if (box) {
                    var index = _this.boxes.indexOf(box);
                    _this.boxes.splice(index, 1);
                }
            });
            $rootScope.$on("refresh-boxes", function () {
                _this.boxes = null;
                defer = $q.defer();
            });
        }
        Dashboard.prototype.getBoxes = function () {
            var _this = this;
            if (this.boxes) {
                return this.$q.when(this.boxes);
            }
            if (!serverCall) {
                serverCall = true;
                this.ajaxService2.get("dashboard/boxlist/")
                    .then(function (response) {
                    serverCall = false;
                    _this.realtimeFactotry.assingBoxes(response.map(function (val) { return val.id; }));
                    _this.boxes = response;
                    for (var i = 0; i < _this.boxes.length; i++) {
                        (function (box) {
                            _this.userUpdatesService.updatesNum(box.id).then(function (val) {
                                box.updates = val;
                            });
                        })(_this.boxes[i]);
                    }
                    defer.resolve(_this.boxes);
                });
            }
            return defer.promise;
        };
        Dashboard.prototype.getUniversityMeta = function (universityId) {
            return this.ajaxService2.get('dashboard/university', { universityId: universityId }, 'university');
        };
        ;
        Dashboard.prototype.createPrivateBox = function (boxName) {
            return this.ajaxService2.post('dashboard/create', { boxName: boxName });
        };
        ;
        Dashboard.prototype.leaderboard = function () {
            return this.ajaxService2.get('dashboard/leaderboard');
        };
        ;
        Dashboard.prototype.recommended = function () {
            return this.ajaxService2.get('dashboard/recommendedcourses');
        };
        Dashboard.$inject = ["$q", "ajaxService2", "realtimeFactory", "userUpdatesService", "$rootScope"];
        return Dashboard;
    }());
    angular.module("app.dashboard").service("dashboardService", Dashboard);
})(app || (app = {}));
