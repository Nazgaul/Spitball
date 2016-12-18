module app {
    "use strict";
    // enum TabState {
    //    Doc = 1,
    //    Course = 2,
    //    Quiz = 3
    // }
    var stateName = "searchinfo";
    var tabs = ["doc", "course", "quiz","flashcard"];
    var page = 0, needToBringMore = true;
    class SearchController {

        static $inject = ["$scope", "dashboardService", "$location",
            "$state", "$stateParams", "Analytics", "searchService", "itemThumbnailService", "$q"];
        tab: string;
        noResults = false;
        result = [];
        constructor(private $scope: angular.IScope,
            private dashboardService: IDashboardService,
            private $location: angular.ILocationService,
            private $state: angular.ui.IStateService,
            private $stateParams: angular.ui.IStateParamsService,
            private analytics: IAnalytics,
            private searchService: ISearchService,
            private itemThumbnailService: IItemThumbnailService,
            private $q: angular.IQService) {

            if (tabs.indexOf($stateParams["t"]) === -1) {
                this.$state.go(stateName, { q: this.$stateParams["q"], t: tabs[0] });
            }
            this.tab = $stateParams["t"];

            this.doQuery(false);
            $scope.$watchCollection(() => [$state.params["q"], $state.params["t"]],
                (newParams, oldParams) => {
                    if (newParams === oldParams) {
                        return;
                    }
                    analytics.trackPage($location.url(), "Search");
                    this.tab = newParams[1];
                    this.doQuery(false);
                });

        }
        back() {
            const appController: IAppController = this.$scope["app"];
            appController.back("/dashboard/");
        }

        univeristyClick() {
            this.dashboardService.getUniversityMeta()
                .then(response => {
                    this.$location.path(response.url);
                });
        }
        changeTab(tab) {
            // self.tab = tab;
            if (tabs.indexOf(tab) === -1) {
                return;
            }
            if (this.$state.params["t"] === tab) {
                return;
            }
            this.$state.go(stateName, { q: this.$stateParams["q"], t: tab });
        }
        myPagingFunction() {
            return this.doQuery(true);
        }

        private doQuery(needToAppend: boolean) {
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
                case tabs[3]:
                    return this.getFlashcards(needToAppend);
                case tabs[2]:
                    return this.getQuizzes(needToAppend);
                default:
                    return this.getBoxes(needToAppend);
            }
        }

        private getBoxes(needToAppend: boolean) {
            return this.searchService.searchBox(this.$state.params["q"], page)
                .then(response => {
                    this.noResults = false;
                    if (needToAppend) {
                        this.result = this.result.concat(response);
                    } else {
                        this.result = response;
                    }
                    if (!response.length && page === 0) {
                        needToBringMore = false;
                        this.noResults = true;
                    }
                    page++;
                });
        }

        private getItems(needToAppend: boolean) {
            return this.searchService.searchItems(this.$state.params["q"], page)
                .then(response => {
                    this.noResults = false;
                    angular.forEach(response,
                        value => {
                            var retVal = this.itemThumbnailService.assignValue(value.source);
                            value.thumbnail = retVal.thumbnail;
                            value.nameExtension = value.name.replace(/\.[^/.]+$/, "");
                        });
                    if (needToAppend) {
                        this.result = this.result.concat(response);
                    } else {
                        this.result = response;
                    }
                    if (!response.length && page === 0) {
                        needToBringMore = false;
                        this.noResults = true;
                    }
                    page++;
                });
        }

        private getQuizzes(needToAppend: boolean) {

            return this.searchService.searchQuizzes(this.$state.params["q"], page)
                .then(response => {
                    this.noResults = false;
                    for (let j = 0; j < response.length; j++) {
                        response[j].publish = true;
                    }
                    if (needToAppend) {
                        this.result = this.result.concat(response);
                    } else {
                        this.result = response;
                    }
                    if (!response.length && page === 0) {
                        needToBringMore = false;
                        this.noResults = true;
                    }
                    page++;
                });
        }
        private getFlashcards(needToAppend: boolean) {

            return this.searchService.searchFlashcards(this.$state.params["q"], page)
                .then(response => {
                    this.noResults = false;
                    for (let j = 0; j < response.length; j++) {
                        const flashcard = response[j];
                        console.log(flashcard);
                        flashcard.publish = true;
                        flashcard.url = this.$state.href("flashcard",
                            {
                                universityType: flashcard.uniName,
                                boxId: flashcard.boxId,
                                boxName: flashcard.boxName,
                                id: flashcard.id,
                                name: flashcard.name
                            });
                    }
                    if (needToAppend) {
                        this.result = this.result.concat(response);
                    } else {
                        this.result = response;
                    }
                    if (!response.length && page === 0) {
                        needToBringMore = false;
                        this.noResults = true;
                    }
                    page++;
                });
        }
    }
    angular.module("app.search").controller("SearchController", SearchController);
}