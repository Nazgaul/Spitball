(function () {
    angular.module('app').controller('inviteController', invite);

    invite.$inject = ['googleService', 'shareService', '$scope', '$stateParams',
        '$anchorScroll', '$timeout', 'resManager'];

    function invite(googleService, shareService, $scope, $stateParams, $anchorScroll, $timeout, resManager) {
        var self = this;
        self.querySearch = querySearch;
        self.allContacts = [];// loadContacts();
        self.contacts = [];
        self.filterSelected = true;
        var googleContact = [];
        self.filterQuery = '';

        self.state = {
            email: 'e',
            facebook: 'f',
            spitball: 's'
        }

        self.switchTab = switchTab;
        self.tab = self.state.spitball;
        getSystemUsers();

        self.importGoogleContract = importGoogleContacts;
        self.fbShare = fbShare;
        self.contactSelected = contactSelected;
        self.toggleContact = toggleContact;
        self.sendInvite = sendInvite;
        self.submitDisabled = false;

        self.closeInvite = function () {
            self.contacts = [];
            changeTab(true);
            $scope.$emit('close_invite');
        }
        self.inBox = $stateParams.boxId ? true : false;
        if (self.inBox) {
            self.title = $scope.b.data.name;
        } else {
            self.title = resManager.get('siteName');
        }
        self.inMail = true;

        self.mail = function () { changeTab(true); }
        self.system = function () { changeTab(false); }



        $scope.$on("open_invite", function () {
            $anchorScroll.yOffset = 100;
            if (self.state.spitball) {
                getSystemUsers('');
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


        function changeTab(isMail) {
            if (isMail) {
                self.allContacts = googleContact;
                self.inMail = true;
            } else {
                self.inMail = false;
                getSystemUsers('');
            }
        }

        function fbShare() {
            window.open('https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(window.location), "pop", "width=600, height=400, scrollbars=no");
        }

        function switchTab(tab) {
            self.tab = tab;
            self.allContacts = [];
            switch (self.tab) {
                case self.state.spitball:
                    getSystemUsers('');
                    break;
                case self.state.email:
                    self.allContacts = googleContact;
                    break;
                case self.state.facebook:
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
            });
        }

        function sendInvite() {
            if (self.contacts.length) {

            }
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