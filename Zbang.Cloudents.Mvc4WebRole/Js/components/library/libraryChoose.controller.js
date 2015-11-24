(function () {
    angular.module('app.library').controller('LibraryChoose', libraryChoose);

    libraryChoose.$inject = ['libraryService', 'userDetails', '$state', 'countryService', 'universitySuggest', 'universityInit'];

    function libraryChoose(libraryService, userDetails, $state, countryService, universitySuggest, universityInit) {
        var self = this;
        self.term = '';
        self.universities = [];
        self.search = search;
        self.searchAutoComplete = searchAutoComplete;
        self.createNewUniversity = createNewUniversity;
        self.selectUniversity = selectUniversity;
        self.createUniversity = false;
        self.needCode = false;
        self.countries = [];

        self.code = {}
        userDetails.get().then(function (response) {
            self.code.userName = response.name;
        });

        countryService.getCountries(function (iso, country) {
            self.countries.push({ iso: iso, name: country });
        });
        assignData(universityInit);

        function search() {
            libraryService.getUniversity(self.term).then(function (response) {
                assignData(response);
            });
        }
        function searchAutoComplete(term) {
            return libraryService.getUniversity(term);
        }

        function selectUniversity(university) {
            libraryService.chooseUniversity(university.id, self.code.studentId).then(function (response) {
                if (response) {

                    self.needCode = true;
                    self.code.university = university;
                    self.code.closedUniText1 = response.textPopupUpper;
                    self.code.closedUniText2 = response.textPopupLower;
                    return;
                }

                goToLibrary(university.name, university.id);
            });
        }

        function assignData(response) {
            if (!self.term) {
                response = universitySuggest.concat(response);
            }
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
            self.universities = data;
        }
        function createNewUniversity() {

            libraryService.createUniversity(self.universityName, self.countryCode).then(function (response) {
                goToLibrary(self.universityName, response.id);
            });
        }

        function goToLibrary(universityName, id) {
            userDetails.setUniversity(universityName, id);
            $state.go('department');
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




