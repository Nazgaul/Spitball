(function () {
    angular.module('app.library').controller('LibraryChoose', libraryChoose);

    libraryChoose.$inject = ['libraryService'];

    function libraryChoose(libraryService) {
        var self = this;
        self.term = '';
        self.universities = [];
        self.search = search;
        self.selectUniversity = selectUniversity;


        search();
        function search() {
            libraryService.getUniversity(self.term).then(function (response) {
                for (var i = 0; i < response.length; i++) {
                    response[i].extraPeople = Math.max(response[i].numOfUsers - 5, 0);
                    for (var j = response[i].userImages.length; j < 5; j++) {
                        response[i].userImages.push('/images/site/user_' + j + '.png');
                    }
                }
                self.universities = response;
            });
        }

        function selectUniversity(university) {
            console.log(university);
        }


    }
})();