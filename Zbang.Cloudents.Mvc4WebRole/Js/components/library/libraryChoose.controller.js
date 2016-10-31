(function () {
    "use strict";
    angular.module("app.library").controller("LibraryChoose", libraryChoose);

    libraryChoose.$inject = ["libraryService", "$state", "countryService",
        "userDetailsFactory", "$scope", "resManager", "realtimeFactory", "$location", "$rootScope"];

    function libraryChoose(libraryService, $state, countryService, userDetailsFactory,
        $scope, resManager, realtimeFactory, $location, $rootScope) {
        var self = this, page = 0;
        self.term = '';
        self.universities = [];
        self.universitySearch = universitySearch;
        self.search = search;
        self.searchAutoComplete = searchAutoComplete;
        self.createNewUniversity = createNewUniversity;
        self.selectUniversity = selectUniversity;
        self.createUniversity = false;
        self.needCode = false;
        self.createCancel = createCancel;
        self.countries = [];
        self.paging = paging;
        self.code = {
            userName: userDetailsFactory.get().name
        }

        countryService.getCountries(function (iso, country) {
            self.countries.push({ iso: iso, name: country });
        });
        function createCancel() {
            self.createUniversity = false;
            self.term = '';

        }
        function universitySearch(libraryChooseForm) {
            if (libraryChooseForm.$valid) {
                search();
            } else {
                self.universities = [];
            }
        }

        function search(needPage) {
            needPage = needPage || false;
            if (!needPage) {
                page = 0;
            }
            return libraryService.getUniversity(self.term, page).then(function (response) {
                assignData(response, needPage);
            });
        }
        function paging() {
            page++;
            return search(true);
        }
        function searchAutoComplete(term, myform) {
            myform.name.$setValidity('required', true);
            myform.name.$setValidity('server', true);
            return libraryService.getUniversity(term, 0);
        }

        function selectUniversity(university, myform) {
            self.submitDisabled = true;

            libraryService.chooseUniversity(university.id, self.code.studentId).then(function (response) {
                if (response) {

                    self.needCode = true;
                    self.code.university = university;
                    self.code.closedUniText1 = response.textPopupUpper;
                    self.code.closedUniText2 = response.textPopupLower;
                    return;
                }
                //userDetailsFactory.init(true).then(function () {
                    goToLibrary();
                   
                //});
            }, function (response) {
                myform.studentId.$setValidity('server', false);
                self.error = response;
            }).finally(function () {
                self.submitDisabled = false;
            });;
        }

        function assignData(response, needPage) {
            var data = [];
            for (var i = 0; i < response.length; i++) {
                var uni = response[i];
                if (checkInArray(data, uni.id)) {
                    continue;
                }
                if (!uni.image) {
                    uni.image = 'https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/universityEmptyState.png';
                }
                uni.extraPeople = Math.max(uni.numOfUsers - 5, 0);
                for (var j = uni.userImages.length; j < 5; j++) {
                    uni.userImages.push('/images/site/user_' + j + '.png');
                }
                data.push(uni);
            }
            if (needPage) {
                self.universities = self.universities.concat(data);
                return;
            }
            //if (!data.length) {

            //}
            self.showNoResult = !data.length;
            self.universities = data;
        }
        function createNewUniversity(myform) {

            if (!self.universityName) {
                myform.name.$setValidity('required', false);
                return;
            }
            self.submitDisabled = true;
            libraryService.createUniversity(self.universityName, self.countryCode).then(function (response) {
                $scope.app.showToaster(resManager.get('toasterOpenSchool'));
                //userDetailsFactory.init(true).then(function () {
                
                goToLibrary(response.url);
                //});
            }).finally(function () {
                self.submitDisabled = false;
            });
        }



        function goToLibrary(url) {
            userDetailsFactory.setUniversity().then(function () {
                //bug 5120/
                realtimeFactory.changeUniversity();
                $rootScope.$broadcast('change-university');
                if (!url) {
                    $state.go('classChoose', null, { reload: true });
                    return;
                }
                $location.url(url);
            });
        }

        function checkInArray(arr, id) {
            return arr.find(function (e) {
                return e.id === id;
            });
        }

        //function createUniversity() {

        //}

    }
})();




