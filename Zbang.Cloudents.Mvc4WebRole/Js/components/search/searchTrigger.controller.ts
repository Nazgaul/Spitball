module app {
    "use strict";
    var searchStateName = "searchinfo";
    class SearchTriggerController {
        // state params is no good because this is no state controller 
        static $inject = ["$scope", "$state"/*, "$mdMedia","resManager"*/];
        term;
        placeholder;
        constructor(private $scope: angular.IScope,
            private $state: angular.ui.IStateService
           // private $mdMedia: angular.material.IMedia,
            //private resManager: IResManager
        ) {
            this.term = $state.params["q"];
            //if ($mdMedia('gt-xs')) {
            //    this.placeholder = resManager.get("Search");
            //} else {
            //    this.placeholder = resManager.get("SearchMobile");
            //}
            $scope.$on("$stateChangeStart",
                (event: angular.IAngularEvent,
                    toState: angular.ui.IState,
                    toParams: spitaball.ISpitballStateParamsService,
                    fromState: angular.ui.IState
                ) => {
                    if (fromState.name === searchStateName && toState.name !== searchStateName) {
                        this.term = "";
                    }
                });
        }
        change() {
            this.search();
        }
        search() {
            const form: angular.IFormController = this.$scope["searchTrigger"];
            if (form.$valid) {
                this.$state.go(searchStateName, { q: this.term, t: this.$state.params["t"] });
            }
        }
    }
    angular.module("app.search").controller("SearchTriggerController", SearchTriggerController);
}

