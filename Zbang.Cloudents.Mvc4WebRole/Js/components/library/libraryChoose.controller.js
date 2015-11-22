(function() {
    angular.module('app.library').controller('LibraryChoose', libraryChoose);

    libraryChoose.$inject = ['libraryService'];

    function libraryChoose(libraryService) {
        var self = this;
        self.term = '';
        self.search = search;


        search();
        function search() {
            libraryService.getUniversity(self.term).then(function (response) {
                console.log(response);
            });
        }


    }
})();