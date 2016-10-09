(function () {
    'use strict';
    angular.module('app').controller('inviteController', invite);

    invite.$inject = ['googleService', 'shareService', '$scope', '$stateParams',
        '$anchorScroll', '$timeout', 'resManager'];

    function invite(googleService, shareService, $scope, $stateParams, $anchorScroll, $timeout, resManager) {
        var self = this;
        self.querySearch = querySearch;
        self.allContacts = [];// loadContacts();
        self.contacts = [];
        self.nonFilteredContacts = [];
        self.filterSelected = true;
        var googleContact = [];
        self.filterQuery = '';

        self.state = {
            email: 'e',
            facebook: 'f',
            spitball: 's'
        }

        self.switchTab = switchTab;

        self.importGoogleContract = importGoogleContacts;
        self.fbShare = fbShare;
        self.contactSelected = contactSelected;
        self.toggleContact = toggleContact;
        self.sendInvite = sendInvite;
        self.submitDisabled = false;

        self.closeInvite = function () {
            self.contacts = [];
            $scope.$emit('close_invite');
        }
        self.inBox = $stateParams.boxId ? true : false;
        if (self.inBox) {
            self.title = $scope.b.data.name;
            getSystemUsers();
            self.tab = self.state.spitball;
        } else {
            self.title = resManager.get('siteName');
            self.tab = self.state.email;
        }


        $scope.$on("open_invite", function () {
            $anchorScroll.yOffset = 250;
            if (self.tab === self.state.spitball) {
                getSystemUsers();
            }
            $timeout(function () {
                $anchorScroll('invite');
            });
            googleService.initGApi().then(function () {
                if (googleService.isAuthenticated()) {
                    getGoogleContacts();
                    return;
                }
            });
        });


        function fbShare() {
            window.open('https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(window.location), "pop", "width=600, height=400, scrollbars=no");
        }

        function switchTab(tab) {
            self.tab = tab;
            self.filterQuery = '';
            self.contacts = [];
            switch (self.tab) {
                case self.state.spitball:
                    getSystemUsers();
                    break;
                case self.state.email:
                    self.allContacts = googleContact;
                    self.nonFilteredContacts = self.allContacts;
                    break;
                case self.state.facebook:
                    self.allContacts = [];
                    break;
            }
        }


        /**
         * Search for contacts.
         */
        function querySearch(query) {
            if (self.tab === self.state.spitball) {
                return getSystemUsers(query);
            } else {
                if (query) {
                    self.allContacts = self.nonFilteredContacts.filter(createFilterFor(query));
                } else {
                    self.allContacts = self.nonFilteredContacts;
                }
                return self.allContacts;
            }
        }
        /**
         * Create filter function for a query string
         */

        function contactSelected(contact) {
            self.contacts.push(contact);
            $scope.$$childHead.$mdContactChipsCtrl.searchText = '';
        }

        function toggleContact(contact) {
            var index = self.contacts.indexOf(contact);
            if (index > -1) {
                self.contacts.splice(index, 1);
                contact.isSelected = false;
            }
            else {
                self.contacts.push(contact);
                contact.isSelected = true;
            }
        }

        function createFilterFor(query) {
            var lowercaseQuery = angular.lowercase(query);
            return function filterFn(contact) {
                return (contact._lowername.indexOf(lowercaseQuery) != -1);
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

                self.nonFilteredContacts = self.allContacts;
            });
        }

        function getSystemUsers(term) {

            shareService.users(term, $stateParams.boxId, 0).then(function (response) {

                self.allContacts = response.map(function (c) {
                    return {
                        name: c.name,
                        email: c.id,
                        image: c.image,// ||  '/images/user.svg' ,
                        _lowername: c.name.toLowerCase()
                    }
                });

                self.nonFilteredContacts = self.allContacts;
            });
        }

        function sendInvite() {
            self.submitDisabled = true;
            var contacts = self.contacts.map(function (c) {
                return c.email;
            });
            if (self.inBox) {
                $scope.$emit('follow-box');
                shareService.inviteToBox(contacts, $stateParams.boxId).then(successSend, function (response) {
                    $scope.app.showToaster(response, null, 'warn');
                }).finally(function () {
                    self.submitDisabled = false;
                });
            } else {
                shareService.inviteToSystem(contacts).finally(successSend);
            }
        }

        function successSend() {
            $scope.app.showToaster(resManager.get('toasterInviteComplete'));
            self.closeInvite();

        }

    }
})()