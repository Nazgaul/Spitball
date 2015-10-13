(function() {
    angular.module('app').controller('inviteController', invite);

    invite.$inject = ['googleService'];

    function invite(googleService) {
        var self = this;
        self.querySearch = querySearch;
        self.allContacts = [];// loadContacts();
        self.contacts = [];
        self.filterSelected = true;


        self.importGoogleContract = importGoogleContacts;
        /**
         * Search for contacts.
         */
        function querySearch(query) {
            var results = query ?
                self.allContacts.filter(createFilterFor(query)) : [];
            return results;
        }
        /**
         * Create filter function for a query string
         */
        function createFilterFor(query) {
            var lowercaseQuery = angular.lowercase(query);
            return function filterFn(contact) {
                return (contact._lowername.indexOf(lowercaseQuery) != -1);;
            };
        }

        importGoogleContacts();
        function importGoogleContacts() {
            googleService.initGApi().then(function () {
                if (googleService.isAuthenticated()) {
                    getGoogleContacts();
                    return;
                }
                googleService.checkAuth(true).then(function () {
                    getGoogleContacts();
                });
            });
        }
        
        function getGoogleContacts() {
            googleService.contacts().then(function (contacts) {
                self.allContacts = contacts.map(function(c) {
                    return {
                        name: c.name,
                        email: c.id,
                        image: c.image,
                        _lowername : c.name.toLowerCase()
                    }
                });
                //$scope.friends = $scope.friends.concat(contacts);
                //$scope.sources.google = true;
                //$scope.$broadcast('itemChange');

            });
        }
        function loadContacts() {
            var contacts = [
              'Marina Augustine',
              'Oddr Sarno',
              'Nick Giannopoulos',
              'Narayana Garner',
              'Anita Gros',
              'Megan Smith',
              'Tsvetko Metzger',
              'Hector Simek',
              'Some-guy withalongalastaname',
              'Marina Augustine',
              'Oddr Sarno',
              'Nick Giannopoulos',
              'Narayana Garner',
              'Anita Gros',
              'Megan Smith',
              'Tsvetko Metzger',
              'Hector Simek',
              'Some-guy withalongalastaname'
            ];
            return contacts.map(function (c, index) {
                var cParts = c.split(' ');
                var contact = {
                    name: c,
                    email: cParts[0][0].toLowerCase() + '.' + cParts[1].toLowerCase() + '@example.com',
                    image: 'http://lorempixel.com/50/50/people?' + index
                };
                contact._lowername = contact.name.toLowerCase();
                return contact;
            });
        }
    }
})()