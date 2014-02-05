(function (cd, pubsub, ko, dataContext, $, analytics) {
    if (window.scriptLoaded.isLoaded('user')) {
        return;
    }

    var eById = document.getElementById.bind(document);

    cd.loadModel('user', 'UserContext', registerKOUser);

    function registerKOUser() {
        ko.applyBindings(new UserViewModel(), document.getElementById('user'));
    }


    function UserViewModel() {
        var self = this;
        var consts = {
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
            UPINVITELIST: 'upInviteList'
        };
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
            that.url = data.url

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
            that.url = data.url;
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
            that.url = data.url;

        }

        function Question(data) {
            var that = this;
            that.boxId = data.boxid;
            that.boxName = data.boxName;
            that.boxImage = data.boxPicutre || consts.EMPTYBOXPICTURE;
            that.answersCount = data.answersCount;
            that.content = data.content.replace(/\n/g, '<br/>');
            that.url = data.url;
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
            that.url = data.url;
        }
        //#endregion      

        self.name = ko.observable();
        self.userId = ko.observable();

        self.viewSelf = ko.computed(function () {
            return self.userId() === cd.userDetail().nId;
        });

        //self.inviteFriends = function (viewModel, e) {
        //    pubsub.publish('nav', e.target.getAttribute('data-href'));
        //};

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
        self.CoursesShowAllVisible = ko.computed(function () {
            var commonBoxesLength = self.commonBoxes().length,
                maxCommonLength = self.maxCommonBoxes(),
                followingBoxesLength = self.followingBoxes().length,
                maxFollowingBoxes = self.maxFollowingBoxes();

            return (commonBoxesLength > consts.MINCOMMONBOXESVISIBLE || followingBoxesLength > consts.MINFOLLOWBOXESVISIBLE) && !self.viewSelf();

        });
        self.CoursesSectionVisible = ko.computed(function () {
            return (self.commonBoxesVisible() || self.followingBoxesVisible()) && !self.viewSelf();
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

            return (commonFriendsLength > consts.MINCOMMONFRIENDSVISIBLE || allFriendsLength > consts.MINALLFRIENDSVISIBLE);
        });
        self.friendSectionVisible = ko.computed(function () {
            return self.allFriendsVisible() || self.commonFriendsVisible();
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
            var dataItem = eById(consts.UPINVITELIST);
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
            getFriendsData();
            getBoxesData();
            if (self.viewSelf()) {
                getInvitesData();
            }
            getActivityData();
        });

        pubsub.subscribe('userclear', function (data) {
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

            .files([]).answers([]).questions([]).commonBoxes([]).followingBoxes([]).commonFriends([]).allFriends([]).invites([]);

            $('.upTabContent').hide();
            $(consts.UPTABS).removeClass(consts.CUPTAB + '2 ' + consts.CUPTAB + '3').addClass(consts.CUPTAB + '1')
            $('#filesSection').show();
            var cB = eById('coursesShow');
            if (cB){
                cB.checked = false;
            }
            cB = eById('friendsShow');

            if (cB){
                cB.checked = false;
            }
            cB = eById('invitesShow');
            if (cB){
                cB.checked = false;
            }          
            var f = eById('filesCount');
            f.textContent = '' + f.getAttribute('data-label');

            f = eById('answersCount');
            f.textContent = '' + f.getAttribute('data-label');
            
            f = eById('questionsCount');
            f.textContent = '' + f.getAttribute('data-label');
        }

        function getInitData() {
            var user = document.getElementById('user'), firstTime = user.getAttribute('data-firstTime');

            if (firstTime) {
                user.removeAttribute('data-firstTime');
                populateScore(parseInt(document.getElementById('pointsList').getAttribute('data-score'), 10));
                registerEvents();                
                return;
            }

            dataContext.minProfile({
                data: { userId: self.userId() },
                success: function (data) {
                    populateProfile(data);                    
                }
            });

            function populateProfile(data) {
                data = data || {};
                var profile = new Profile(data);

                self.name(profile.name);

                eById('upUsername').textContent = profile.name;
                eById('upUserImg').src = profile.image;
                eById('upUserSchool').textContent = profile.universityName;
                populateScore(profile.score);
                registerEvents();
            }

            function registerEvents() { //not sure why this is happen twice in line 322 and 343.
                var sendMessageBtn = eById('upSendMessage'),
                    accountSettingsBtn = eById('upAccountSettings'),
                    userName = eById('upUsername').textContent,
                    userImage = eById('upUserImg').src;

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
                //if (score === 0 && self.viewSelf()) {
                //    eById('pointsWpr').classList.add('empty');
                //    return;
                //}
                var pointsList = eById('pointsList'),
                    pointsListChildren = pointsList.children;
                for (var i = 0, c = 0, l = pointsListChildren.length; i < l ; i++) {
                    pointsListChildren[i].textContent = 0;
                }

                pubsub.publish('user_load');
                pubsub.publish('clearTooltip');

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

        function getFriendsData() {
            var loader = renderLoad(eById('upFriendsSection'));
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

                var lengths = getFriendsLength(false);
                setHeight(lengths.commonLength, lengths.allLength);
                registerEvents();

                function registerEvents() {
                    if (self.friendsShowAllVisible()) {
                        eById('friendsShow').onchange = function (e) {
                            if (self.maxCommonFriends() < self.commonFriends().length || self.maxAllFriends() < self.allFriends().length) {                                
                                self.maxCommonFriends(self.commonFriends().length);                                
                                self.maxAllFriends(self.allFriends().length);                                
                            }

                            lengths = getFriendsLength(this.checked);
                            setHeight(lengths.commonLength, lengths.allLength);

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
                    setContainerHeight(document.querySelector('.upFriends .inCommonList'),'.upFriend', commonLength, consts.FRIENDSINROW);
                    setContainerHeight(document.querySelector('.upFriends .inAllFriendList'),'.upFriend', allLength, consts.FRIENDSINROW);
                }


            }
        }

        function getBoxesData() {
            var loader = renderLoad(eById('upCoursesSection'));
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
                            }

                            lengths = getBoxesLength(this.checked);

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
            var loader = renderLoad(eById('upInvitesSection'));
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
                setHeight(getInvitesLength(false));
                registerEvents();

                function registerEvents() {
                    if (self.invitesShowAllVisible()) {
                        eById('invitesShow').onchange = function (e) {

                            if (self.maxInvites() < self.invites().length) {                              
                                self.maxInvites(self.invites().length);

                            }


                            setHeight(getInvitesLength(this.checked));

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
                    setContainerHeight(
                        eById(consts.UPINVITELIST),
                            '.upInviteItem', invitesLength, consts.INVITESINROW);
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
                element.textContent = data.length + ' ' + element.getAttribute('data-label');
            }


        }
        function registerGeneralEvents() {
            $(consts.UPTABS).on('click', 'button', function () {
                $('.upTabContent').hide();
                switch (this.id) {
                    case 'questionsCount':
                        $(consts.UPTABS).removeClass(consts.CUPTAB + '1 ' + consts.CUPTAB + '3').addClass(consts.CUPTAB + '2')
                        $('#questionsSection').show();
                        break;
                    case 'answersCount':
                        $(consts.UPTABS).removeClass(consts.CUPTAB + '1 ' + consts.CUPTAB + '2').addClass(consts.CUPTAB + '3')
                        $('#answersSection').show();
                        break;
                    default:
                        $(consts.UPTABS).removeClass(consts.CUPTAB + '2 ' + consts.CUPTAB + '3').addClass(consts.CUPTAB + '1')
                        $('#filesSection').show();
                }
            });

            //    // i move the call from the subpub because subpub is not singleton
            //    var options = document.querySelectorAll('.upUploads input[type=radio]'),
            //        prev = null;
            //    for (var i = 0, l = options.length; i < l; i++) {
            //        options[i].onchange = function (e) {
            //            switch (this.id) {
            //                case 'upFiles':
            //                    getFilesData();
            //                    break;
            //                case 'upQuestions':
            //                    getQuestionsData();
            //                    break;
            //                case 'upAnswers':
            //                    getAnswersData();
            //                    break;
            //            }
            //        };
            //    }

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
            console.log($(item).outerHeight(true));
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
                parseInt(marginBottom !== '' ? marginBottom : '0', 10)+
                parseInt(borderTop !== '' ? borderTop : '0', 10)+
                parseInt(borderBottom !== '' ? borderBottom : '0', 10)+
                parseInt(paddingTop !== '' ? paddingTop : '0', 10) +
                parseInt(paddingBottom !== '' ? paddingBottom : '0', 10),
                height;

            //if (itemHeight === 116) {
            //    itemHeight += 4;
            //}
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