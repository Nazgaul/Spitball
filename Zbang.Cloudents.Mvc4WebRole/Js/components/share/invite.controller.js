(function () {
    angular.module('app').controller('inviteController', invite);

    invite.$inject = ['googleService', 'shareService', '$scope', '$stateParams', '$location', '$anchorScroll'];

    function invite(googleService, shareService, $scope, $stateParams, $location, $anchorScroll) {
        var self = this;
        self.querySearch = querySearch;
        self.allContacts = [];// loadContacts();
        self.contacts = [];
        self.filterSelected = true;

        var googleContact = [];


        self.importGoogleContract = importGoogleContacts;
        self.contactSelected = contactSelected;
        self.sendInvite = sendInvite;

        self.closeInvite = function () {
            self.contacts = [];
            changeTab(true);
            $scope.$emit('close_invite');
        }
        self.inBox = $stateParams.boxId ? true : false;
        self.inMail = true;

        self.mail = function () { changeTab(true); }
        self.system = function () { changeTab(false); }



        $scope.$on("open_invite", function () {
            $location.hash('invite');
            $anchorScroll();
            googleService.initGApi().then(function () {
                if (googleService.isAuthenticated()) {
                    getGoogleContacts();
                    return;
                }
            });
        });


        function changeTab(isMail) {
            if (isMail) {
                self.allContacts = googleContact;
                self.inMail = true;
            } else {
                self.inMail = false;
                getSystemUsers('');
            }
        }

        /**
         * Search for contacts.
         */
        function querySearch(query) {
            if (!self.inMail) {
                return getSystemUsers(query);
            } else {
                var results = query ?
                    self.allContacts.filter(createFilterFor(query)) : [];
                if (!results.length) {
                    results.push({ name: query, email: query, image: '/images/user.svg' });
                }
                return results;
            }
        }
        /**
         * Create filter function for a query string
         */

        function contactSelected(contact) {
            self.contacts.push(contact);
            $scope.$$childHead.$mdContactChipsCtrl.searchText = '';
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
                googleService.checkAuth(false).then(function () {
                    getGoogleContacts();
                });
            });
        }

        function getGoogleContacts() {
            googleService.contacts().then(function (contacts) {
                self.allContacts = googleContact = contacts.map(function (c) {
                    return {
                        name: c.name,
                        email: c.id,
                        image: c.image,
                        _lowername: c.name.toLowerCase()
                    }
                });


            });
        }

        function getSystemUsers(term) {

            shareService.users(term, $stateParams.boxId, 0).then(function (response) {

                self.allContacts = response.map(function (c) {
                    return {
                        name: c.name,
                        email: c.id,
                        image: c.image ||  '/images/user.svg' ,
                        _lowername: c.name.toLowerCase()
                    }
                });

            });
        }

        function sendInvite() {
            var contacts = self.contacts.map(function (c) {
                return c.email;
            });
            if (self.inBox) {
                shareService.inviteToBox(contacts, $stateParams.boxId).then(function () {
                    self.closeInvite();
                });
            } else {
                shareService.inviteToSystem(contacts).then(function () {
                    self.closeInvite();
                });
            }
        }

    }
})()