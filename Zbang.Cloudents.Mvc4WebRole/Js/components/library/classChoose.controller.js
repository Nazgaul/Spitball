var app;
(function (app) {
    "use strict";
    var Steps;
    (function (Steps) {
        Steps[Steps["Start"] = 1] = "Start";
        Steps[Steps["SearchFirst"] = 2] = "SearchFirst";
        Steps[Steps["SearchFirstComplete"] = 3] = "SearchFirstComplete";
        Steps[Steps["SearchSecond"] = 4] = "SearchSecond";
        Steps[Steps["SearchSecondComplete"] = 5] = "SearchSecondComplete";
        Steps[Steps["SearchMore"] = 6] = "SearchMore";
        Steps[Steps["EmptyState"] = 7] = "EmptyState";
        Steps[Steps["ChooseDepartment"] = 8] = "ChooseDepartment";
        Steps[Steps["CreateClass"] = 9] = "CreateClass";
    })(Steps || (Steps = {}));
    var ClassChoose = (function () {
        function ClassChoose(searchService, $scope) {
            this.searchService = searchService;
            this.$scope = $scope;
            this.step = Steps.Start;
        }
        ClassChoose.prototype.classSearch = function () {
            var formElement = this.$scope["classChooseFrom"];
            if (formElement.$invalid) {
                return;
            }
            if (this.term) {
                this.step = Steps.SearchFirst;
                this.searchService.searchBox(this.term, 0)
                    .then(function (response) {
                    console.log(response);
                });
            }
            else {
                this.step = Steps.Start;
            }
        };
        ClassChoose.$inject = ["searchService", "$scope"];
        return ClassChoose;
    }());
    angular.module("app.library").controller("ClassChoose", ClassChoose);
})(app || (app = {}));
