(function () {
    angular.module('app.library').controller('LibraryChoose', libraryChoose);

    libraryChoose.$inject = ['libraryService', 'userDetails', '$state', 'facebookService', '$q'];

    function libraryChoose(libraryService, userDetails, $state, facebookService, $q) {
        var self = this, friendsUniversitys = [];
        self.term = '';
        self.universities = [];
        self.search = search;
        self.selectUniversity = selectUniversity;


        var friendPromise = facebookService.getToken().then(function (token) {
            return libraryService.getUniversityByFriends(token);
            //    .then(function (data) {

            //    console.log(data);
            //});
        });


        //var searchPromise = search();

        $q.all([friendPromise, libraryService.getUniversity(self.term)]).then(function (response) {
            friendsUniversitys = response[0];
            assignData(response[1]);
        });
        function search() {
            libraryService.getUniversity(self.term).then(function (response) {
                assignData(response);
            });
        }

        function selectUniversity(university) {
            libraryService.chooseUniversity(university.id).then(function () {
                userDetails.setUniversity(university.name);
                $state.go('department');
            });
        }

        function assignData(response) {
            if (!self.term) {
                response = friendsUniversitys.concat(response);
            }
            var data = [];
            for (var i = 0; i < response.length; i++) {
                if (checkInArray(data, response[i].id)) {
                    continue;
                }
                response[i].extraPeople = Math.max(response[i].numOfUsers - 5, 0);
                for (var j = response[i].userImages.length; j < 5; j++) {
                    response[i].userImages.push('/images/site/user_' + j + '.png');
                }
                data.push(response[i]);
            }



            self.universities = data;
        }

        function checkInArray(arr, id) {
            return arr.find(function (e) {
                return e.id === id;
            });
        }


    }
})();