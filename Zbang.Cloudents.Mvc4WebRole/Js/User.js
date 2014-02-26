(function (cd, pubsub, ko, dataContext, $, analytics) {
    if (window.scriptLoaded.isLoaded('user')) {
        return;
    }

    var eById = document.getElementById.bind(document),
        consts = {
            MAXFILES: 8,
            SHOWMOREFILES: 8,
            MAXQUESTIONS: 3,
            SHOWMOREQUESTIONS: 3,
            MAXANSWERS: 3,
            SHOWMOREANSWERS: 3,
            MINCOMMONBOXESVISIBLE: 3,
            MINFOLLOWBOXESVISIBLE: 6,
            MINCOMMONFRIENDSVISIBLE: 7,
            MINALLFRIENDSVISIBLE: 7,
            MININVITESVISIBLE: 6,
            CUPTAB: 'upTab',
            UPTABS: '.upTabs',
            BOXESINROW: 3,
            INVITESINROW: 2,
            FRIENDSINROW: 7,
            EMPTYBOXPICTURE: '/images/emptyState/my_default3.png',
            EMPTY: 'empty',
            UPINVITELIST: 'upInviteList',
            DATALABEL: 'data-label',
            ADMINSCORE: 1000000,
            MAXMEMBERS: 50
        };



    cd.loadModel('user', 'UserContext', registerKOUser);

    function registerKOUser() {
        ko.applyBindings(new UserViewModel(), document.getElementById('user'));
    }


    function UserViewModel() {
        var upInviteList = eById(consts.UPINVITELIST), upMemberSettings = eById('upMemberSettings'),
             filesCount = eById('filesCount'),
            answersCount = eById('answersCount'), questionsCount = eById('questionsCount'),
            upUsername = eById('upUsername'), upUserImg = eById('upUserImg'),
            upUserSchool = eById('upUserSchool'), membersList = eById('upMembersList'),
            memberBoxList = eById('upMemberBoxList'), upMemberSearch = eById('upMembersSearch'),
            upMbrSetingsSndMsg = eById('upMbrSetingsSndMsg'), deptPopup = eById('deptPopup'),
            upFriendsSection = eById('upFriendsSection'), upCoursesSection = eById('upCoursesSection'),
            upInvitesSection = eById('upInvitesSection'), questionsSection = eById('questionsSection'),
            answersSection = eById('answersSection'), filesSection = eById('filesSection');

        var self = this;

        //#region Models
        function Profile(data) {
            var that = this;
            that.id = data.id;
            that.name = data.name;
            that.image = data.image;
            that.universityName = data.universityName;
            that.score = data.score;
        }

        function Box(data) {
            var that = this;
            that.id = data.id;
            that.name = data.name;
            that.image = data.picture || consts.EMPTYBOXPICTURE;
            that.userType = data.userType;
            that.url = data.url + '?r=user&s=courselist';

            self.followBox = function (box) {
                dataContext.follow({
                    data: { BoxUid: box.id },
                    success: function () {
                        cd.pubsub.publish('nav', box.url);
                    }
                });
            };
        }

        function Friend(data) {
            var that = this;
            that.id = data.uid;
            that.name = data.name;
            that.image = data.largeImage;
            that.url = data.url + '?r=user&s=otheruser'
        }

        function Member(data) {
            var that = this;
            that.id = data.id;
            that.inputId = 'member' + that.id;
            that.name = data.name;
            that.image = data.image;
            that.department = data.department;
            that.joinDate = cd.dateToShow(data.joinDate, '/', true);
            that.selected = ko.observable(false);
        }
        function Department(data) {
            var that = this;
            that.id = data.id;
            that.name = data.name;
            that.year = data.year;
            that.fullname = ko.computed(function () {
                return that.name + ' ' + that.year;
            });
            that.selected = ko.observable(false);
        }

        function Invite(data) {
            var that = this;
            that.userId = data.userid;
            that.userName = data.username;
            that.userImage = data.userImage;
            that.status = data.status ? 'Joined' : 'Pending';
            that.inviteType = data.inviteType;
            that.name = data.boxName || 'Cloudents';
            that.inviteImage = data.boxPicture || '/Images/cloudents1-57x57.png';
            that.boxId = data.boxid;
        }

        function File(data) {
            var that = this;
            that.id = data.id;
            that.boxId = data.boxId;
            that.name = data.name;
            that.image = data.image;
            that.rating = data.rate;
            that.views = data.numOfViews;
            that.url = data.url + '?r=user&s=files';

        }

        function Question(data) {
            var that = this;
            that.boxId = data.boxid;
            that.boxName = data.boxName;
            that.boxImage = data.boxPicutre || consts.EMPTYBOXPICTURE;
            that.answersCount = data.answersCount;
            that.content = data.content.replace(/\n/g, '<br/>');
            that.url = data.url + '?r=user&s=question';
        }

        function Answer(data) {
            var that = this;
            that.qUserId = data.qUserId;
            that.qUserImage = data.qUserImage;
            that.qUserName = data.qUserName;
            that.qContent = data.qContent.replace(/\n/g, '<br/>');
            that.boxId = data.boxid;
            that.boxName = data.boxName;
            that.boxImage = data.boxPicture || consts.EMPTYBOXPICTURE;
            that.content = data.content.replace(/\n/g, '<br/>');
            that.answersCount = data.answersCount;
            that.url = data.url + '?r=user&s=answer';
        }
        //#endregion      

        self.name = ko.observable();
        self.userId = ko.observable();
        self.score = ko.observable();

        self.viewSelf = ko.computed(function () {
            return self.userId() === cd.userDetail().nId;
        });


        //#region Boxes Section        

        self.commonBoxes = ko.observableArray();
        self.followingBoxes = ko.observableArray();

        self.maxFollowingBoxes = ko.observable(consts.MINFOLLOWBOXESVISIBLE);
        self.maxCommonBoxes = ko.observable(consts.MINCOMMONBOXESVISIBLE);

        self.followingBoxesVisible = ko.computed(function () {
            return self.followingBoxes().length > 0;
        });

        self.commonBoxesVisible = ko.computed(function () {
            return self.commonBoxes().length > 0;
        });

        self.emptyBoxes = ko.computed(function () {
            return !(self.followingBoxesVisible() || !self.commonBoxes());
        });

        self.CoursesShowAllVisible = ko.computed(function () {
            var commonBoxesLength = self.commonBoxes().length,
                maxCommonLength = self.maxCommonBoxes(),
                followingBoxesLength = self.followingBoxes().length,
                maxFollowingBoxes = self.maxFollowingBoxes();

            return (commonBoxesLength > consts.MINCOMMONBOXESVISIBLE || followingBoxesLength > consts.MINFOLLOWBOXESVISIBLE) && !self.viewSelf();

        });
        self.CoursesSectionVisible = ko.computed(function () {
            return !self.viewSelf();
        });

        //#endregion 

        //#region Friends Section
        self.allFriends = ko.observableArray();
        self.commonFriends = ko.observableArray();

        self.maxCommonFriends = ko.observable(consts.MINCOMMONFRIENDSVISIBLE);
        self.maxAllFriends = ko.observable(consts.MINALLFRIENDSVISIBLE);

        self.allFriendsVisible = ko.computed(function () {
            return self.allFriends().length > 0;
        });

        self.commonFriendsVisible = ko.computed(function () {
            return self.commonFriends().length > 0;
        });
        self.friendsShowAllVisible = ko.computed(function () {
            var commonFriendsLength = self.commonFriends().length,
                maxCommonFriendsLength = self.maxCommonFriends(),
                allFriendsLength = self.allFriends().length,
                maxAllFriends = self.maxAllFriends();

            return (commonFriendsLength > consts.MINCOMMONFRIENDSVISIBLE || allFriendsLength > consts.MINALLFRIENDSVISIBLE) && !self.membersSectionVisible();
        });
        self.friendSectionVisible = ko.computed(function () {
            return (self.allFriendsVisible() || self.commonFriendsVisible()) && !self.membersSectionVisible();
        });

        //#endregion

        //#region Members Section
        var searchInProgress = false;

        self.members = ko.observableArray();

        self.searchResultMembers = ko.observableArray();

        self.departments = ko.observableArray();

        self.displayMembers = ko.observableArray();

        self.membersLength = ko.computed(function (e) {
            return self.members().length + ' ' + ZboxResources.Members;
        });
        self.membersSectionVisible = ko.computed(function () {
            return self.score() >= consts.ADMINSCORE && self.viewSelf();
        });

        self.membersLoaded = ko.observable(false);

        self.displayMembersFilter = ko.computed(function () {

            var selected = [], current;
            for (var i = 0, l = self.departments().length; i < l; i++) {
                current = self.departments()[i];
                if (current.selected()) {
                    selected.push(current.fullname());
                }

            }
            if (!selected.length) {
                return self.displayMembers();
            }

            return ko.utils.arrayFilter(self.displayMembers(), function (m) {
                return selected.indexOf(m.department) > -1;
            });
        });
        //#endregion

        //#region Invites
        self.invites = ko.observableArray();
        self.invitesLoaded = ko.observable(false);
        self.maxInvites = ko.observable(consts.MININVITESVISIBLE);
        self.invitesEmpty = ko.computed(function () {
            if (self.invitesLoaded()) {
                return self.invites().length === 0 ? consts.EMPTY : '';
            }
        });

        self.invitePending = function (status) {
            return status === 'Pending' ? 'upPending' : '';
        };

        self.inviteStatus = function (status) {
            var dataItem = upInviteList;
            if (status === "Pending") {
                return dataItem.getAttribute('data-up-pending');
            } else {
                return dataItem.getAttribute('data-up-joined');
            }
        };

        self.invitesShowAllVisible = ko.computed(function () {
            return self.invites().length > consts.MININVITESVISIBLE;
        });


        self.reInvite = function (invite, e) {
            e.target.textContent = e.target.getAttribute('data-inv-sent');
            dataContext.inviteBox({
                data: { Recepients: invite.userId, BoxUid: invite.boxId }
            });
        };
        //#endregion 

        self.activityLoaded = ko.observable(false);
        //#region Uploads Files
        self.files = ko.observableArray();
        self.maxFiles = ko.observable(consts.MAXFILES);
        self.filesEmpty = ko.computed(function () {
            if (self.activityLoaded()) {
                return self.files().length === 0 ? consts.EMPTY : '';
            }
        });
        self.fileRating = function (rating) {
            return 67 / 5 * rating + 'px';
        };
        self.filesShowMoreVisible = ko.computed(function () {
            return self.files().length > consts.MAXFILES && self.maxFiles() < self.files().length;
        });

        self.showMoreFiles = function () {
            var current = self.maxFiles();
            current += consts.SHOWMOREFILES;
            self.maxFiles(current);
        };
        //#endregion 

        //#region Uploads Questions
        self.questions = ko.observableArray();
        self.maxQuestions = ko.observable(consts.MAXQUESTIONS);
        self.questionsEmpty = ko.computed(function () {
            if (self.activityLoaded()) {
                return self.questions().length === 0 ? consts.EMPTY : '';
            }

        });

        self.questionsShowMoreVisible = ko.computed(function () {
            return self.questions().length > consts.MAXQUESTIONS && self.maxQuestions() < self.questions().length;
        });

        self.showMoreQuestions = function () {
            var current = self.maxQuestions();
            current += consts.SHOWMOREQUESTIONS;
            self.maxQuestions(current);
        };
        //#endregion 


        //#region Uploads Answers
        self.answers = ko.observableArray();
        self.maxAnswers = ko.observable(consts.MAXANSWERS);
        self.answersEmpty = ko.computed(function () {
            if (self.activityLoaded()) {
                return self.answers().length === 0 ? consts.EMPTY : '';
            }
        });

        self.answersShowMoreVisible = ko.computed(function () {
            return self.answers().length > consts.MAXANSWERS && self.maxAnswers() < self.answers().length;
        });

        self.showMoreAnswers = function () {
            var current = self.maxAnswers();
            current += consts.SHOWMOREANSWERS;
            self.maxAnswers(current);
        };
        //#endregion 

        pubsub.subscribe('user', function (data) {
            clear();
            self.userId(parseInt(cd.getParameterFromUrl(1), 10) || cd.userDetail().nId);
            getInitData();
        });

        pubsub.subscribe('userclear', function (data) {
            clear();
        });

        function clear() {
            pubsub.publish('clearTooltip');

            self.activityLoaded(false)
            .invitesLoaded(false)
            .maxCommonBoxes(consts.MINCOMMONBOXESVISIBLE)
            .maxFollowingBoxes(consts.MINFOLLOWBOXESVISIBLE)
            .maxAllFriends(consts.MINALLFRIENDSVISIBLE)
            .maxCommonFriends(consts.MINCOMMONFRIENDSVISIBLE)

            .maxFiles(consts.MAXFILES)
            .maxQuestions(consts.MAXQUESTIONS)
            .maxAnswers(consts.MAXANSWERS)
            .membersLoaded(false)
            .files([]).answers([]).questions([]).commonBoxes([]).followingBoxes([]).commonFriends([]).allFriends([]).invites([]);


            if (upMemberSettings) {
                upMemberSettings.checked = false;
            }

            $('.upMemberBoxes').hide();
            $('.upTabContent').hide();
            $(consts.UPTABS).removeClass(consts.CUPTAB + '2 ' + consts.CUPTAB + '3').addClass(consts.CUPTAB + '1')
            $('#filesSection').show();
            var cB = eById('coursesShow');
            if (cB) {
                cB.checked = false;
            }
            cB = eById('friendsShow');

            if (cB) {
                cB.checked = false;
            }
            cB = eById('invitesShow');
            if (cB) {
                cB.checked = false;
            }
            var f = filesCount;
            if (f) {
                f.textContent = '' + f.getAttribute(consts.DATALABEL);
            }

            f = answersCount;
            if (f) {
                f.textContent = '' + f.getAttribute(consts.DATALABEL);
            }

            f = questionsCount;
            if (f) {
                f.textContent = '' + f.getAttribute(consts.DATALABEL);
            }


        }

        function getInitData() {
            var user = document.getElementById('user'), firstTime = user.getAttribute('data-firstTime'), userScore;

            if (firstTime) {
                if (upUsername) {
                    var username = upUsername.textContent;
                }
                user.removeAttribute('data-firstTime');
                self.name(username);
                userScore = parseInt(document.getElementById('pointsList').getAttribute('data-score'), 10);
                if (!self.membersSectionVisible()) {
                    populateScore(userScore);
                }
                if (!cd.firstLoad) {
                    cd.setTitle(username + ' | Cloudents');
                }
                self.score(userScore);

                pubsub.publish('clearTooltip');
                pubsub.publish('user_load');
                getOtherData();
                registerEvents();
                return;
            }

            dataContext.minProfile({
                data: { userId: self.userId() },
                success: function (data) {
                    populateProfile(data);
                    getOtherData();
                    registerEvents();
                }
            });

            function populateProfile(data) {
                data = data || {};
                var profile = new Profile(data);
                if (!cd.firstLoad) {
                    cd.setTitle(profile.name + ' | Cloudents');
                }
                self.name(profile.name);

                upUsername.textContent = profile.name;
                upUserImg.src = profile.image;
                upUserSchool.textContent = profile.universityName;
                if (profile.score < consts.ADMINSCORE) {
                    populateScore(profile.score);
                }
                self.score(profile.score);

                pubsub.publish('clearTooltip');
                pubsub.publish('user_load');
            }

            function registerEvents() {
                var sendMessageBtn = eById('upSendMessage'),
                    accountSettingsBtn = eById('upAccountSettings'),
                    userName = upUsername.textContent,
                    userImage = upUserImg.src;

                if (self.viewSelf()) {
                    accountSettingsBtn.onclick = function () {
                        analytics.trackEvent('User Page', 'Edit Account', 'User clicked edit account');
                    };
                    return;
                }

                sendMessageBtn.onclick = function () {
                    cd.pubsub.publish('message', { id: '', data: [{ name: userName, id: self.userId(), userImage: userImage }] });
                    analytics.trackEvent('User Page', 'Edit Account', 'User clicked send a message');
                };
            }

            function populateScore(score) {
                var pointsList = eById('pointsList'),
                    pointsListChildren = pointsList.children,
                    statusPoints = eById('userPts').textContent;
                if (statusPoints > score) {
                    score = statusPoints;
                }

                for (var i = 0, c = 0, l = pointsListChildren.length; i < l ; i++) {
                    pointsListChildren[i].textContent = 0;
                }

                var numAnim = new countUp(0, score, 2);

                numAnim.start(function (score) {
                    if (!score) {
                        return;
                    }
                    var strScore = score.toString();
                    for (var i = 0, c = 0, l = pointsListChildren.length; i < l ; i++) {
                        if (l - i > strScore.length) {
                            continue;
                        }
                        pointsListChildren[i].textContent = strScore[c];
                        c++;
                    }
                });



            }
        }

        function getOtherData() {
            if (!self.membersSectionVisible()) {
                getFriendsData();
            } else {
                getMembersData();
            }

            getBoxesData();
            if (self.viewSelf()) {
                getInvitesData();
            }
            getActivityData();
        }

        function getMembersData() {
            var loader = renderLoad(membersList);
            dataContext.getUpMembers({
                success: function (data) {
                    populateMembers(data);
                }
            }).done(function () {
                dataContext.depList({
                    success: function (data) {
                        var map = data.map(function (dep) {
                            return new Department(dep);
                        });
                        self.departments(map);
                    },
                    always: function () {
                        loader();
                        self.membersLoaded(true);
                    }
                });
            });


            function populateMembers(data) {
                data = data || {};
                var map = data.map(function (member) {
                    return new Member(member);
                });
                self.members(map);

                self.members.sort(function (left, right) {
                    return cd.sortMembersByName(left.name, right.name);
                })
                self.displayMembers(map.slice(0, 50));


                cd.loadImages(membersList);
                registerEvents();

                function registerEvents() {
                    var loader;

                    upMemberSearch.onkeyup = function (e) {
                        var input = eById('upMembersSearch'),
                            term;

                        self.displayMembers([]);
                        $(membersList).scrollTop(0);


                        if (Modernizr.input.placeholder) {
                            term = input.value.toLowerCase().trim();
                        } else {
                            if (input.value === input.getAttribute('placeholder')) {
                                term = '';
                            } else {
                                term = input.value.toLowerCase().trim();
                            }
                        }
                        if (term === '') {
                            searchInProgress = false;
                            self.displayMembers(self.members().slice(0, 50));
                            cd.loadImages(membersList);
                            return;
                        }


                        self.searchResultMembers([]);
                        searchInProgress = true;
                        var name;
                        for (var i = 0, l = self.members().length; i < l; i++) {
                            name = self.members()[i].name.toLowerCase();
                            if (name.indexOf(term) > -1) {
                                self.searchResultMembers.push(self.members()[i]);
                            }
                        }
                        self.searchResultMembers.sort(function (left, right) {
                            return cd.sortMembersByName(left.name, right.name, term);
                        });

                        self.displayMembers.push.apply(self.displayMembers, self.searchResultMembers.slice(0, 50));


                        cd.loadImages(membersList);
                    };

                    var sTimeout, cTimeout, currentItem, loading = false; // for disable popup hide when user move to popup

                    membersList.onscroll = function (e) {
                        if (sTimeout) {
                            clearTimeout(sTimeout);
                        }
                        hideBoxesPopup();
                        var count, list;
                        if (membersList.offsetHeight + membersList.scrollTop >= membersList.scrollHeight) {
                            e.preventDefault();
                            e.stopPropagation();
                            list = searchInProgress ? self.searchResultMembers() : self.members();
                            count = self.displayMembers().length;
                            self.displayMembers.push.apply(self.displayMembers, list.slice(count, count + consts.MAXMEMBERS));
                            cd.loadImages(membersList);
                        }
                    };

                    $(membersList).off('mouseover').on('mouseover', '.courseList', function (e) {
                        var parent = this.parentElement;

                        while (parent.nodeName !== 'LI') {
                            parent = parent.parentElement;
                        }
                        currentItem = parent;

                        if (loading) {
                            return;
                        }
                        loading = true;
                        var member = ko.dataFor(parent);
                        dataContext.getUpMemberBoxes({
                            data: { userId: member.id },
                            success: function (data) {
                                data = data || {};
                                if (currentItem !== parent) {
                                    return;
                                }
                                if (!data.length) {
                                    data = [{ name: JsResources.NoSelected }];
                                }
                                sTimeout = setTimeout(function () {
                                    showMemberBoxList(e.target, data);
                                }, 250);

                            },
                            always: function () {
                                loading = false;
                            }
                        });
                    }).off('mouseleave').on('mouseleave', '.courseList', function (e) {
                        if (sTimeout) {
                            clearTimeout(sTimeout);
                        }
                        cTimeout = setTimeout(function () {
                            hideBoxesPopup();
                        }, 250);
                    });

                    $('.upMemberBoxes').off('mouseover').on('mouseover', function (e) {
                        if (cTimeout) {
                            clearTimeout(cTimeout);
                        }
                    })
                    .off('mouseleave').on('mouseleave', function (e) {
                        if (sTimeout) {
                            clearTimeout(sTimeout);
                        }
                        hideBoxesPopup();
                    });

                    upMemberSettings.onchange = function (e) {
                        var that = this;
                        ko.utils.arrayForEach(self.members(), function (member) {
                            member.selected(that.checked);
                        });

                        toggleMessageBtn(that.checked);
                    };

                    $(membersList).on('change', '.checkbox', function () {
                        if ($(membersList).find('.checkbox:checked').length > 0) {
                            toggleMessageBtn(true);
                            return;
                        }

                        toggleMessageBtn(false);
                    });

                    upMbrSetingsSndMsg.onclick = function () {
                        var selected = [], allCbox = upMemberSettings;

                        if (allCbox.checked) {
                            var arr = searchInProgress ? self.searchResultMembers() : self.members();
                            setTimeout(function () {
                                pubsub.publish('message', { id: '', data: arr });
                            }, 10);
                            return;
                        }
                        var checkboxes = membersList.querySelectorAll('input:checked');
                        for (var i = 0, l = checkboxes.length; i < l; i++) {
                            selected.push(ko.dataFor(checkboxes[i]));
                        }
                        if (selected.length === 1) {
                            pubsub.publish('message', { id: '', data: [{ id: selected[0].id, name: selected[0].name, userImage: selected[0].image }] });
                        } else {
                            //setTimeout(function () {
                            pubsub.publish('message', { id: '', data: selected });
                            //}, 10);
                        }


                    };

                    document.getElementsByClassName('deptCol')[0].onclick = function () {
                        $(deptPopup).toggle();
                    };
                }
            }

            function showMemberBoxList(target, boxes) {
                var parent = memberBoxList.parentElement,
                    pos;
                cd.appendData(memberBoxList, 'upMemberBoxItemTemplate', boxes, 'beforeend', true);
                parent.style.display = 'block';
                var pos = calculatePopupPosition();
                parent.style.left = pos.x + 'px';
                parent.style.top = pos.y + 'px';

                function calculatePopupPosition() {
                    return {
                        x: target.offsetLeft + target.offsetWidth / 2 - parent.offsetWidth / 2 + membersList.scrollLeft
                        ,// - left,
                        y: target.offsetTop - target.offsetHeight - parent.offsetHeight - 5 - membersList.scrollTop //10=margin
                    }
                }
            }
            function hideBoxesPopup() {
                memberBoxList.parentElement.style.display = 'none';
            }

            function toggleMessageBtn(isOn) {
                upMbrSetingsSndMsg.style.display = isOn ? 'inline-block' : 'none';
            }
        }

        function getFriendsData() {
            var loader = renderLoad(upFriendsSection);
            dataContext.getFriends({
                data: { userId: self.userId() },
                success: function (data) {
                    populateFriends(data);
                },
                always: function () {
                    loader();
                }
            });

            function populateFriends(data) {
                data = data || {};

                var result;

                if (!data.user.length) {
                    return;
                }
                result = filterObjects(data.my, data.user, Friend);
                self.commonFriends(result.common);
                self.allFriends(result.all);
                cd.loadImages(upFriendsSection);
                var lengths = getFriendsLength(false);
                setHeight(lengths.commonLength, lengths.allLength);
                registerEvents();

                function registerEvents() {
                    if (self.friendsShowAllVisible()) {
                        eById('friendsShow').onchange = function (e) {
                            if (self.maxCommonFriends() < self.commonFriends().length || self.maxAllFriends() < self.allFriends().length) {
                                self.maxCommonFriends(self.commonFriends().length);
                                self.maxAllFriends(self.allFriends().length);
                                cd.loadImages(upFriendsSection);
                            }

                            lengths = getFriendsLength(this.checked);
                            setHeight(lengths.commonLength, lengths.allLength);
                            analytics.trackEvent('User Page', 'Friends List', 'User clicked ' + (this.checked ? 'show more' : 'show less'));

                        };
                    }
                }
                function getFriendsLength(checked) {
                    var commonLength, allLength;
                    if (checked) {
                        commonLength = self.commonFriends().length;
                        allLength = self.allFriends().length;
                    } else {
                        commonLength = self.commonFriends().length < consts.MINCOMMONFRIENDSVISIBLE ? self.commonFriends().length : consts.MINCOMMONFRIENDSVISIBLE;
                        allLength = self.allFriends().length < consts.MINALLFRIENDSVISIBLE ? self.allFriends().length : consts.MINALLFRIENDSVISIBLE;
                    }

                    return { commonLength: commonLength, allLength: allLength };
                }
                function setHeight(commonLength, allLength) {
                    setContainerHeight(document.querySelector('.upFriends .inCommonList'), '.upFriend', commonLength, consts.FRIENDSINROW);
                    setContainerHeight(document.querySelector('.upFriends .inAllFriendList'), '.upFriend', allLength, consts.FRIENDSINROW);
                }


            }
        }

        function getBoxesData() {
            loader = renderLoad(upCoursesSection);
            dataContext.getUserpageBoxes({
                data: { userId: self.userId() },
                success: function (data) {
                    populateCoursesData(data);
                },
                always: function () {
                    loader();
                }
            });

            function populateCoursesData(data) {
                data = data || {};
                var result;

                result = filterBoxes(data);
                self.commonBoxes(result.common);
                self.followingBoxes(result.all);
                cd.loadImages(upCoursesSection);
                var lengths = getBoxesLength(false);
                setHeight(lengths.commonLength, lengths.followingLength);

                registerEvents();

                function filterBoxes(data) {
                    var box,
                        result = {
                            common: [],
                            all: []
                        };
                    for (var i = 0, l = data.length; i < l; i++) {
                        box = new Box(data[i]);
                        if (box.userType === 'subscribe' || box.userType === 'owner') {
                            result.common.push(box);
                        }
                        else {
                            result.all.push(box);
                        }
                    }

                    return result;
                }

                function registerEvents() {
                    if (self.CoursesShowAllVisible()) {
                        eById('coursesShow').onchange = function (e) {

                            if (self.maxCommonBoxes() < self.commonBoxes().length || self.maxFollowingBoxes() < self.followingBoxes().length) {
                                self.maxCommonBoxes(self.commonBoxes().length);
                                self.maxFollowingBoxes(self.followingBoxes().length);
                                cd.loadImages(upCoursesSection);
                            }

                            lengths = getBoxesLength(this.checked);
                            analytics.trackEvent('User Page', 'Course List', 'User clicked ' + (this.checked ? 'show more' : 'show less'));

                            setHeight(lengths.commonLength, lengths.followingLength);

                        };
                    }

                }

                function getBoxesLength(checked) {
                    var commonLength, followingLength;
                    if (checked) {
                        commonLength = self.commonBoxes().length;
                        followingLength = self.followingBoxes().length;
                    } else {
                        commonLength = self.commonBoxes().length < consts.MINCOMMONBOXESVISIBLE ? self.commonBoxes().length : consts.MINCOMMONBOXESVISIBLE;
                        followingLength = self.followingBoxes().length < consts.MINFOLLOWBOXESVISIBLE ? self.followingBoxes().length : consts.MINFOLLOWBOXESVISIBLE;
                    }

                    return { commonLength: commonLength, followingLength: followingLength };
                }
                function setHeight(commonLength, followingLength) {
                    setContainerHeight(
                             document.querySelector('.upCourses .inCommonList'),
                             '.box', commonLength, consts.BOXESINROW);
                    setContainerHeight(
                        document.querySelector('.upCourses .followingList'),
                            '.box', followingLength, consts.BOXESINROW);

                }
            }


        }

        function getInvitesData() {
            var loader = renderLoad(upInvitesSection);
            dataContext.getUserPageInvites({
                success: function (data) {
                    self.invitesLoaded(true);
                    populateInvitesData(data);
                },
                always: function () {
                    loader();
                }
            });

            function populateInvitesData(data) {
                data = data || {};
                var map = data.map(function (invite) {
                    return new Invite(invite);
                });

                self.invites(map);
                cd.loadImages(upInvitesSection);

                setHeight(getInvitesLength(false));
                registerEvents();

                function registerEvents() {
                    if (self.invitesShowAllVisible()) {
                        eById('invitesShow').onchange = function (e) {

                            if (self.maxInvites() < self.invites().length) {
                                self.maxInvites(self.invites().length);
                                cd.loadImages(upInvitesSection);
                            }


                            setHeight(getInvitesLength(this.checked));
                            analytics.trackEvent('User Page', 'Invite List', 'User clicked ' + (this.checked ? 'show more' : 'show less'));

                        };
                    }
                };
                function getInvitesLength(checked) {
                    var invitesLength;
                    if (checked) {
                        invitesLength = self.invites().length;
                    } else {
                        invitesLength = self.invites().length < consts.MININVITESVISIBLE ? self.invites().length : consts.MININVITESVISIBLE;
                    }
                    return invitesLength;
                }

                function setHeight(invitesLength) {
                    setContainerHeight(upInviteList, '.upInviteItem', invitesLength, consts.INVITESINROW);
                }

            }
        }

        function getActivityData() {
            if (self.activityLoaded()) {
                return;
            }
            var loader = renderLoad(document.querySelector('.upUploads'));

            dataContext.getUserPageActivity({
                data: { userId: self.userId() },
                success: function (data) {
                    self.activityLoaded(true);
                    populateActivity(data.items, File, 'files', 'filesCount');
                    populateActivity(data.questions, Question, 'questions', 'questionsCount');
                    populateActivity(data.answers, Answer, 'answers', 'answersCount')
                    registerGeneralEvents();
                },
                always: function () {
                    loader();
                }
            });

            function populateActivity(data, type, array, countId) {
                data = data || {};
                var map = data.map(function (d) {
                    return new type(d);
                });

                self[array](map);
                var element = eById(countId);
                element.textContent = data.length + ' ' + element.getAttribute(consts.DATALABEL);
            }


        }
        function registerGeneralEvents() {
            $(consts.UPTABS).on('click', 'button', function () {
                $('.upTabContent').hide();
                switch (this.id) {
                    case 'questionsCount':
                        $(consts.UPTABS).removeClass(consts.CUPTAB + '1 ' + consts.CUPTAB + '3').addClass(consts.CUPTAB + '2')
                        $(questionsSection).show();
                        break;
                    case 'answersCount':
                        $(consts.UPTABS).removeClass(consts.CUPTAB + '1 ' + consts.CUPTAB + '2').addClass(consts.CUPTAB + '3')
                        $(answersSection).show();
                        break;
                    default:
                        $(consts.UPTABS).removeClass(consts.CUPTAB + '2 ' + consts.CUPTAB + '3').addClass(consts.CUPTAB + '1')
                        $(filesSection).show();
                }
            });
        }

        function setContainerHeight(list, item, itemsLength, itemsInRow) {
            if (!list) {
                return;
            }

            var item = list.querySelector(item)
            if (!item) {
                list.style.height = '0px';
                return;
            }
            var style = getComputedStyle(item);
            var innerHeight = style.getPropertyValue('height'),
            marginTop = style.getPropertyValue('margin-top'),
            marginBottom = style.getPropertyValue('margin-bottom'),
            borderTop = style.getPropertyValue('border-top-width'),
            borderBottom = style.getPropertyValue('border-bottom-width'),
            paddingTop = style.getPropertyValue('padding-top'),
            paddingBottom = style.getPropertyValue('padding-bottom'),

            itemHeight = parseInt(innerHeight !== '' ? innerHeight : '0', 10) +
                parseInt(marginTop !== '' ? marginTop : '0', 10) +
                parseInt(marginBottom !== '' ? marginBottom : '0', 10) +
                parseInt(borderTop !== '' ? borderTop : '0', 10) +
                parseInt(borderBottom !== '' ? borderBottom : '0', 10) +
                parseInt(paddingTop !== '' ? paddingTop : '0', 10) +
                parseInt(paddingBottom !== '' ? paddingBottom : '0', 10),
                height;

            height = Math.ceil(itemsLength / itemsInRow) * itemHeight;

            list.style.height = height + 'px';
        }

        function filterObjects(array1, array2, objType) {
            var map = {},
                result = {
                    common: [],
                    all: []
                };

            array1.forEach(function (el) {
                var o = new objType(el);
                map[o.id] = o;
            });

            array2.forEach(function (el) {
                var obj = new objType(el);
                if (obj.id in map) {
                    result.common.push(obj);
                } else {
                    result.all.push(obj);
                }
            });

            return result;
        }

        var loaders = [];
        function renderLoad(e) {
            var element = e;
            if (loaders.indexOf(element) > -1) {
                return function () { };
            }

            loaders.push(element);
            var cssLoader = '<div class="smallLoader upLoader"><div class="spinner"></div>',
            imgLoader = '<img class="pageLoaderImg upLoader" src="/images/loader2.gif" />',
            loader;

            if (Modernizr.cssanimations) {
                element.insertAdjacentHTML('beforeend', cssLoader);
            } else {
                element.insertAdjacentHTML('beforeend', imgLoader);
            }

            loader = element.querySelector('.upLoader');

            return function () {
                (function (l) {
                    element.removeChild(l);
                    var index = loaders.indexOf(element);
                    loaders.splice(index, 1);
                })(loader);

            };
        }
    }
})(cd, cd.pubsub, ko, cd.data, jQuery, cd.analytics);