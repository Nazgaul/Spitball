///// <reference path="DataContext.js" />
//(function (ko, $, dataContext, utils, analytics) {
//    "use strict";

//    if (window.scriptLoaded.isLoaded('dash')) {
//        return;
//    }

//    utils.loadModel('dashboard', 'DashboardContext', AsideViewModel);

//    function AsideViewModel() {

//        var dashWall = document.getElementById('dash_Wall'),
//        dashFriends = document.getElementById('dash_Friend'),
//        dashWallTextItem = dashWall.getAttribute('data-itemText'),
//        //dashWallTextQuestion = dashWall.getAttribute('data-questionText'),
//        //dashWallTextAnswer = dashWall.getAttribute('data-answerText'),
//        dashWallTextComment = dashWall.getAttribute('data-commenttext'),
//        activityFeedTemplate = 'activityFeedTemplate';

//        function Activity(data) {
//            data = data || {};
//            var self = this;
//            self.userName = data.userName;
//            self.userUid = data.userId;
//            self.userImg = data.userImage;
//            self.userUrl = data.userUrl;

//            self.boxid = data.boxId;
//            self.boxName = data.boxName;
//            self.boxurl = data.url;// + '?r=dashboard&s=activity'

//            self.textAction = textActionReolver();
//            function textActionReolver() {
//                switch (data.action) {
//                    case 'item': return dashWallTextItem;
//                    case 'question': return dashWallTextComment;
//                    case 'answer': return dashWallTextComment;
//                    default:
//                        return '';

//                }
//            }
//        }
//        cd.pubsub.subscribe('addQuestion', function (newquestionobj) {
//            var q = newquestionobj.question;
//            var a = $('#dashB' + newquestionobj.boxid).find('a');
//            var activityobj = {
//                userName: q.userName,
//                userId: q.userUid,
//                userImage: q.userImage,
//                userUrl: q.userUrl,
//                boxId: newquestionobj.box,
//                boxName: a.attr('title'),
//                url: a.attr('href'),
//                action: 'question'
//            };
//            appendFeedData(activityFeedTemplate, activityobj);
//        });
//        cd.pubsub.subscribe('addAnswer', function (newAnswerObj) {
//            var a = newAnswerObj.answer;
//            var a2 = $('#dashB' + newAnswerObj.box).find('a');
//            var activityobj = {
//                userName: a.userName,
//                userId: a.userId,
//                userImage: a.userImage,
//                userUrl: a.userUrl,
//                boxId: newAnswerObj.box,
//                boxName: a2.attr('title'),
//                url: a2.attr('href'),
//                action: 'answer'
//            };
//            appendFeedData(activityFeedTemplate, activityobj);
//        });
//        cd.pubsub.subscribe('addedItem', function (newItemObj) {
//            var a = newItemObj.item;
//            var a2 = $('#dashB' + newItemObj.boxid).find('a');
//            var activityobj = {
//                userName: a.owner,
//                userId: a.ownerId,
//                userImage: a.ownerImg,
//                userUrl: a.ownerUrl,
//                boxId: newItemObj.boxid,
//                boxName: a2.attr('title'),
//                url: a.url,
//                action: 'item'
//            };
//            appendFeedData(activityFeedTemplate, activityobj);

//        });

//        function UserFriends(data) {
//            var self = this;
//            data = data || {};
//            self.userName = data.name;
//            self.userImage = data.image || '/images/user-pic.png';
//            self.id = data.uid;
//            self.url = data.url;// + '?r=dashboard&s=friends';
//        }



//        utils.pubsub.subscribe('dashSideD', function (d) {
//            mapData({ Wall: d.wall, Friends: d.friend });
//        });
//        function mapData(data) {

//            var mappedObjects = [];
//            //Wall
//            for (var i = 0, l = data.Wall.length; i < l ; i++) {
//                mappedObjects.push(new Activity(data.Wall[i]));
//            }
//            cd.appendData(dashWall, activityFeedTemplate, mappedObjects, 'afterbegin', true);

//            //Friends
//            mappedObjects.length = 0;
//            // we want only 11 friend to show - we can do that on server as well
//            for (var i = 0, l = Math.min(data.Friends.length, 11) ; i < l ; i++) {
//                mappedObjects.push(new UserFriends(data.Friends[i]));
//            }
//            cd.appendData(dashFriends, 'friendsTemplate', mappedObjects, 'afterbegin', function () {
//                $('#dashboard').find('.friendItem:not("[data-action]")').remove(); // this can affect box as well
//            });
//        }

//        function appendFeedData(template, data) {
//            var activity = new Activity(data);
//            cd.appendData(dashWall, template, activity, 'afterbegin', false);
//        }

//        $(dashWall).on('click', '[data-navigation="Box"]', function () {
//            analytics.trackEvent('Dashboard', 'Activity feed', 'Number of Clicks on users activity ');
//        });
//        var h3Height, offset;
//        utils.pubsub.subscribe('dashboard_show', function () {
//            initHeight();
//            innerScrollWallDashboard();
//        });
//        function initHeight() {
//            if (!h3Height) {
//                h3Height = $('.sectionHeader').outerHeight(true);
//                offset = $(dashWall).offset().top;
//            }
//        }
//        utils.pubsub.subscribe('windowChanged', function () {
//            if (document.getElementById('dashboard').style.display === 'block') {
//                initHeight();
//                innerScrollWallDashboard();
//            }
//        });

//        var scrollCreated = false;
//        function innerScrollWallDashboard() {

//            var height = ($(window).height() - $(dashWall).offset().top);
//            utils.innerScroll($(dashWall), height);
//            //         if (!scrollCreated) {

//            //var x = $(dashWall).parent().height(height);
//            //    /$(x).mCustomScrollbar();
//            //}            
//            //analytics.trackEvent('Activity Items', 'Scroll Created');

//        }


//        analytics.setLibrary($('.uniText').text());

//        $('#dash_Product').click(function () {
//            var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : screen.left;
//            var dualScreenTop = window.screenTop != undefined ? window.screenTop : screen.top;

//            var w = 935, h = 600,
//             left = (screen.width / 2) - (w / 2) + dualScreenLeft,
//             top = (screen.height / 2) - (h / 2) + dualScreenTop;
//            analytics.trackEvent('Dashboard', 'AdvertismentClick');
//            window.open(this.getAttribute('data-url'), '', 'height=' + h + ',menubar=0,status=0,toolbar=0,scrollbars=1,width=' + w + ',left=' + left + ',top=' + top + '');
//        });

//        (function () {
//            var dashShowFriends = document.getElementById('dash_showfriends'),
//                membersList = dashShowFriends.getElementsByClassName('membersList')[0],
//                //sendAMsgBtm = document.getElementById('frndPopSndMsg'),
//                friends = [], MINHEIGHT = 420, MAXHEIGHT = 600;

//            function Friend(data) {
//                var _self = this;
//                _self.id = data.uid;
//                _self.name = data.name;
//                _self.image = data.image;
//                //_self.identifier = cd.guid();
//                _self.url = data.url;// + '?r=dashboard&s=members';
//            }
//            document.getElementById('showUserFriends').onclick = function (e) {
//                if (friends.length) {
//                    changeFriendsPopupView(true);
//                    return;
//                }
//                dataContext.getFriends({
//                    success: function (data) {
//                        //use in pseudo element
//                        dashShowFriends.querySelector('.membersCount').setAttribute('data-count', data.my.length);
//                        for (var i = 0, l = data.my.length; i < l; i++) {
//                            friends.push(new Friend(data.my[i]));
//                        }
//                        friends.sort(function (a, b) {
//                            return cd.sortMembersByName(a.name, b.name);
//                        });
//                        appendMembers(friends);

//                        changeFriendsPopupView(true);
//                    }
//                });
//            };
//            document.getElementById('closeShowFriends').onclick = function (e) {
//                changeFriendsPopupView(false, true);
//            };


//            //$(membersList).on('change', '.checkbox', function (e) {
//            //    var checkboxes = dashShowFriends.querySelectorAll('.memberItem input:checked');
//            //    if (checkboxes.length) {
//            //        sendAMsgBtm.style.display = 'inline-block';
//            //        return;
//            //    }
//            //    sendAMsgBtm.style.display = 'none';


//            //});


//            $(membersList).on('click', '.inviteUserBtn', function (e) {

//                var id = getListItem(e.target).id,
//                friend = $.data(document.getElementById(id));
//                triggerMessagePopup([{ id: friend.id, name: friend.name, userImage:friend.image }]);

//            });


//            //document.getElementById('friendsSettings').addEventListener('change', function () {
//            //    var checkboxes = membersList.getElementsByClassName('checkbox'),
//            //        state = this.checked;
//            //    for (var i = 0, l = checkboxes.length; i < l; i++) {
//            //        checkboxes[i].checked = state;
//            //    }

//            //    $(checkboxes).trigger('change');
//            //}, true);

//            //sendAMsgBtm.onclick = function () {
//            //    var checkboxes = membersList.querySelectorAll('.memberItem input:checked'), memberItem,
//            //        selectedMembers = [];
//            //    if (!checkboxes.length) {
//            //        return;
//            //    }
//            //    for (var i = 0, l = checkboxes.length; i < l; i++) {
//            //        memberItem = $.data(document.getElementById(checkboxes[i].id));
//            //        selectedMembers.push({ id: memberItem.id, name: memberItem.name, userImage:memberItem.image });

//            //    }

//            //    triggerMessagePopup(selectedMembers);
//            //}

//            document.getElementById('friendsSearch').onkeyup = function () {
//                searchMembers(this.value);
//            }

//            cd.pubsub.subscribe('dashboardclear', function () {
//                changeFriendsPopupView(false, true);
//            });
//            function appendMembers(friendsToShow) {
//                cd.appendData(membersList, 'friendItemTemplate', friendsToShow, 'beforeend', true);
//                calculatePopupHeight();
//                setScroll();
//                for (var i = 0, l = friendsToShow.length; i < l; i++) {
//                    $.data(document.getElementById('dashfriend' + friendsToShow[i].id), friendsToShow[i]);
//                }
//            }

//            cd.pubsub.subscribe('windowChanged', function () {
//                var $popup = $('#dash_showfriends .boxSettings');
//                if ($popup.length && $popup.is(':visible')) {
//                    calculatePopupHeight();
//                    setScroll();
//                }
//            });

//            function setScroll() {
//                var $popup = $('#dash_showfriends .boxSettings'),
//                    $scrollElem = $('#membersListDashboard'),
//                   maxHeight = parseInt($scrollElem.css('max-height'), 10);

//                if (!$popup.is(':visible')) {
//                    return;
//                }


//                var margin = $('.popupHeader').height() +
//                            $('.popupTopBar').outerHeight() +
//                            $('.membersTop').outerHeight(),

//                    height = $popup.height() - margin;

//                if (maxHeight >= height) {//+1 fix because of different calculations
//                    $scrollElem.height(height);
//                    cd.innerScroll($scrollElem, height);
//                    if (Modernizr.touch) {
//                        return;//no need to continue for touch devices
//                    }
//                    var scrollDirection = window.getComputedStyle(document.getElementsByTagName('html')[0], null).getPropertyValue('direction') == 'ltr' ? 'right' : 'left',//rtl
//                        scrollBar = document.querySelector('.membersContent .slimScrollBar'),
//                        scrollDiv = document.querySelector('.membersContent .slimScrollDiv'),
//                        scrollRail = document.querySelector('.membersContent .slimScrollRail');

//                    scrollDiv.style.overflow = 'visible';
//                    // scrollBar.style.removeProperty('right'); scrollBar.style.removeProperty('left'); //clean direction
//                    scrollBar.style[scrollDirection] = '-12px';
//                    scrollRail.style[scrollDirection] = '-12px';
//                }
//            }
//            function calculatePopupHeight() {
//                var $popup = $('#dash_showfriends .boxSettings'),
//                  $scrollElem = $('#membersListDashboard');

//                if (!($popup.length && $popup.is(':visible'))) {
//                    return;
//                }

//                if ($popup[0].querySelector('.slimScrollDiv')) {
//                    $scrollElem.slimScroll({ 'destroy': '' });
//                }
//                $scrollElem.css('height', '');
//                $popup.css({ height: '', top: '' });

//                var popupHeight = $popup.outerHeight(),
//                    windowHeight = window.innerHeight;

//                if (popupHeight < MINHEIGHT) {
//                    $popup.height(MINHEIGHT);
//                    popupHeight = MINHEIGHT;
//                }
//                if (popupHeight > MAXHEIGHT) {
//                    $popup.height(MAXHEIGHT);
//                    popupHeight = MAXHEIGHT;
//                }
//                if (popupHeight > windowHeight) {
//                    $popup.height(windowHeight);
//                    popupHeight = windowHeight;
//                }
//                if (popupHeight + $popup.offset().top > windowHeight) {
//                    var top = (windowHeight - $popup.outerHeight()) / 2;
//                    $popup.css('top', top > 0 ? top : 0);
//                }

//            }

//            function searchMembers(value) {
//                var foundList = [];

//                for (var i = 0, l = friends.length; i < l ; i++) {
//                    if (friends[i].name.toLowerCase().indexOf(value.toLowerCase()) > -1) {
//                        foundList.push(friends[i]);
//                    }
//                }
//                foundList.sort(function (a, b) {
//                    return cd.sortMembersByName(a.name, b.name, value);
//                });
//                appendMembers(foundList);
//            }

//            function triggerMessagePopup(friends) {
//                changeFriendsPopupView(false, false);
//                cd.pubsub.publish('messageFromPopup', { id: dashShowFriends.id, data: friends });
//            }

//            function getListItem(target) {
//                while (target.nodeName !== 'LI') {
//                    target = target.parentElement;
//                }
//                return target;
//            }

//            function changeFriendsPopupView(isShow, clearInput) {
//                if (!isShow) {
//                    //clear all
//                    dashShowFriends.style.display = 'none';
//                    //document.getElementById('friendsSettings').style.display = 'none'
//                    //document.getElementById('frndPopSndMsg').style.display = 'none';

//                    if (clearInput) {
//                        document.getElementById('friendsSearch').value = '';
//                    }

//                    //var checkboxes = dashShowFriends.querySelectorAll('input:checked');
//                    //for (var i = 0, l = checkboxes.length ; i < l; i++) {
//                    //    checkboxes[i].checked = false;
//                    //}
//                    return;
//                }

//                appendMembers(friends);
//                dashShowFriends.style.display = 'block';
//                calculatePopupHeight();
//                setScroll();
//            }
//        }());
//    }

//}(ko, jQuery, cd.data, cd, cd.analytics));