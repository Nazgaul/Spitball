var app;
(function (app) {
    "use strict";
    var stateName = "searchinfo";
    var tabs = ["doc", "course", "quiz"];
    var page = 0, needToBringMore = true;
    var SearchController = (function () {
        function SearchController($scope, dashboardService, $location, $state, $stateParams, analytics, searchService, itemThumbnailService, $q) {
            var _this = this;
            this.$scope = $scope;
            this.dashboardService = dashboardService;
            this.$location = $location;
            this.$state = $state;
            this.$stateParams = $stateParams;
            this.analytics = analytics;
            this.searchService = searchService;
            this.itemThumbnailService = itemThumbnailService;
            this.$q = $q;
            this.noResults = false;
            this.result = [];
            if (tabs.indexOf($stateParams["t"]) === -1) {
                this.$state.go(stateName, { q: this.$stateParams["q"], t: tabs[0] });
            }
            this.tab = $stateParams["t"];
            this.doQuery(false);
            $scope.$watchCollection(function () { return [$state.params["q"], $state.params["t"]]; }, function (newParams, oldParams) {
                if (newParams === oldParams) {
                    return;
                }
                analytics.trackPage($location.url(), "Search");
                _this.tab = newParams[1];
                _this.doQuery(false);
            });
        }
        SearchController.prototype.back = function () {
            var appController = this.$scope["app"];
            appController.back("/dashboard/");
        };
        SearchController.prototype.univeristyClick = function () {
            var _this = this;
            this.dashboardService.getUniversityMeta()
                .then(function (response) {
                _this.$location.path(response.url);
            });
        };
        SearchController.prototype.changeTab = function (tab) {
            if (tabs.indexOf(tab) === -1) {
                return;
            }
            if (this.$state.params["t"] === tab) {
                return;
            }
            this.$state.go(stateName, { q: this.$stateParams["q"], t: tab });
        };
        SearchController.prototype.myPagingFunction = function () {
            return this.doQuery(true);
        };
        SearchController.prototype.doQuery = function (needToAppend) {
            if (!needToAppend) {
                page = 0;
                needToBringMore = true;
            }
            if (!needToBringMore) {
                return this.$q.when();
            }
            switch (this.tab) {
                case tabs[0]:
                    return this.getItems(needToAppend);
                case tabs[2]:
                    return this.getQuizzes(needToAppend);
                default:
                    return this.getBoxes(needToAppend);
            }
        };
        SearchController.prototype.getBoxes = function (needToAppend) {
            var _this = this;
            return this.searchService.searchBox(this.$state.params["q"], page)
                .then(function (response) {
                _this.noResults = false;
                if (needToAppend) {
                    _this.result = _this.result.concat(response);
                }
                else {
                    _this.result = response;
                }
                if (!response.length && page === 0) {
                    needToBringMore = false;
                    _this.noResults = true;
                }
                page++;
            });
        };
        SearchController.prototype.getItems = function (needToAppend) {
            var _this = this;
            return this.searchService.searchItems(this.$state.params["q"], page)
                .then(function (response) {
                _this.noResults = false;
                angular.forEach(response, function (value) {
                    var retVal = _this.itemThumbnailService.assignValue(value.source);
                    value.thumbnail = retVal.thumbnail;
                    value.nameExtension = value.name.replace(/\.[^/.]+$/, "");
                });
                if (needToAppend) {
                    _this.result = _this.result.concat(response);
                }
                else {
                    _this.result = response;
                }
                if (!response.length && page === 0) {
                    needToBringMore = false;
                    _this.noResults = true;
                }
                page++;
            });
        };
        SearchController.prototype.getQuizzes = function (needToAppend) {
            var _this = this;
            return this.searchService.searchQuizzes(this.$state.params["q"], page)
                .then(function (response) {
                _this.noResults = false;
                for (var j = 0; j < response.length; j++) {
                    response[j].publish = true;
                }
                if (needToAppend) {
                    _this.result = _this.result.concat(response);
                }
                else {
                    _this.result = response;
                }
                if (!response.length && page === 0) {
                    needToBringMore = false;
                    _this.noResults = true;
                }
                page++;
            });
        };
        SearchController.$inject = ["$scope", "dashboardService", "$location",
            "$state", "$stateParams", "Analytics", "searchService", "itemThumbnailService", "$q"];
        return SearchController;
    }());
    angular.module("app.search").controller("SearchController", SearchController);
})(app || (app = {}));
