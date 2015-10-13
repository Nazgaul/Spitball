(function() {
    angular.module('app').controller('inviteController', invite);

    invite.$inject = ['googleService', 'shareService'];

    function invite(googleService, shareService) {
        var self = this;
        self.querySearch = querySearch;
        self.allContacts = [];// loadContacts();
        self.contacts = [];
        self.filterSelected = true;


        self.importGoogleContract = importGoogleContacts;
        self.contactSelected = contactSelected;
        self.sendInvite = sendInvite;
        self.chooseEmail = function() {
            console.log('here');
        }
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

        function contactSelected(contact) {
            self.contacts.push(contact);
        }
        function createFilterFor(query) {
            var lowercaseQuery = angular.lowercase(query);
            return function filterFn(contact) {
                return (contact._lowername.indexOf(lowercaseQuery) != -1);;
            };
        }

        //importGoogleContacts();
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
              

            });
        }

        function sendInvite() {
            shareService.inviteToSystem(self.contacts.map(function(c) {
                return c.email;
            }));
        }
        
    }
})()