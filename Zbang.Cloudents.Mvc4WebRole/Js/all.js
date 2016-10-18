var app;
(function (app) {
    "use strict";
    angular.module("app").config(config);
    config.$inject = ["$controllerProvider", "$locationProvider", "$provide",
        "$httpProvider", "$compileProvider", "$animateProvider",
        "$mdAriaProvider", "$mdIconProvider", "$sceDelegateProvider", "$mdThemingProvider"];
    function config($controllerProvider, $locationProvider, $provide, $httpProvider, $compileProvider, $animateProvider, $mdAriaProvider, $mdIconProvider, $sceDelegateProvider, $mdThemingProvider) {
        $controllerProvider.allowGlobals();
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
        $provide.factory("requestinterceptor", [function () { return ({
                "request": function (c) {
                    return c;
                },
                "response": function (response) {
                    return response;
                },
                "responseError": function (response) {
                    switch (response.status) {
                        case 400:
                        case 412:
                            alert("Spitball has updated, refreshing page");
                            window.location.reload(true);
                            break;
                        case 401:
                            window.open("/", "_self");
                            break;
                        case 403:
                            window.open("/error/membersonly/?returnUrl=" + encodeURIComponent(location.href), "_self");
                            break;
                        case 404:
                            window.open("/error/notfound/", "_self");
                            break;
                        case 500:
                            window.open("/error/", "_self");
                            break;
                        default:
                            break;
                    }
                    return response;
                }
            }); }]);
        $httpProvider.interceptors.push("requestinterceptor");
        $compileProvider.debugInfoEnabled(false);
        $animateProvider.classNameFilter(/angular-animate/);
        $mdThemingProvider.disableTheming();
        $mdAriaProvider.disableWarnings();
        $sceDelegateProvider.resourceUrlWhitelist([
            "self",
            (window["cdnPath"] + "/**")
        ]);
        $mdIconProvider
            .iconSet("t", append("/images/site/icons.svg"))
            .iconSet("i", append("/images/site/itemIcons.svg"))
            .iconSet("u", append("/images/site/uploadIcons.svg"))
            .iconSet("lc", append("/images/site/libChooseIcons.svg"))
            .iconSet("b", append("/images/site/box-icons.svg"))
            .iconSet("q", append("/images/site/quizIcons.svg"))
            .iconSet("p", append("/images/site/profileIcons.svg"));
        function append(str) {
            return (window["cdnPath"] || "") + str + "?" + window["version"];
        }
    }
})(app || (app = {}));
(function () {
    angular.module("app").config(config);
    config.$inject = ['AnalyticsProvider'];
    function config(analyticsProvider) {
        var analyticsObj = {
            'cookieDomain': 'spitball.co',
            'alwaysSendReferrer': true
        };
        if (window['id'] && window['id'] > 0) {
            analyticsObj['userId'] = window['id'];
        }
        analyticsProvider.setAccount({
            tracker: 'UA-9850006-3',
            fields: analyticsObj
        });
        analyticsProvider.trackUrlParams(true);
        analyticsProvider.setPageEvent("$stateChangeSuccess");
        analyticsProvider.delayScriptTag(true);
    }
    angular.module("app").run(anylticsRun);
    anylticsRun.$inject = ["Analytics", "$document"];
    function anylticsRun(analytics, $document) {
        $document.ready(function () {
            analytics.createAnalyticsScriptTag();
        });
    }
    ;
})();
(function () {
    angular.module("app").run(config);
    config.$inject = ["timeAgo"];
    function config(timeAgo) {
        if (document.documentElement.lang === "he") {
            timeAgo.settings.overrideLang = "he_IL";
        }
        var threeDays = 60 * 60 * 24 * 3;
        timeAgo.settings.fullDateAfterSeconds = threeDays;
        timeAgo.settings.refreshMillis = 15000;
    }
})();
(function () {
    angular.module("app").config(config);
    config.$inject = ['ScrollBarsProvider'];
    function config(ScrollBarsProvider) {
        ScrollBarsProvider.defaults = {
            scrollInertia: 400,
            scrollButtons: false,
            theme: "dark-thin",
            autoHideScrollbar: true
        };
    }
    ;
})();
var app;
(function (app) {
    "use strict";
    var AppController = (function () {
        function AppController($rootScope, $location, userDetails, $mdToast, $document, $mdMenu, resManager, cacheFactory, sbHistory, $state, $window) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$location = $location;
            this.userDetails = userDetails;
            this.$mdToast = $mdToast;
            this.$document = $document;
            this.$mdMenu = $mdMenu;
            this.resManager = resManager;
            this.cacheFactory = cacheFactory;
            this.sbHistory = sbHistory;
            this.$state = $state;
            this.$window = $window;
            this.back = function (defaultUrl) {
                var element = _this.sbHistory.popElement();
                if (!element) {
                    _this.$location.url(defaultUrl);
                    return;
                }
                _this.$rootScope.$broadcast("from-back");
                _this.$state.go(element.name, element.params);
            };
            this.logOut = function () {
                _this.cacheFactory.clearAll();
                Intercom("shutdown");
            };
            this.toggleMenu = function () {
                _this.$rootScope.$broadcast("open-menu");
            };
            this.showToaster = function (text, parentId, theme) {
                var element = _this.$document.find("header")[0];
                if (parentId) {
                    element = _this.$document[0].querySelector("#" + parentId);
                }
                var toaster = _this.$mdToast.simple()
                    .textContent(text)
                    .position("top right")
                    .parent(element)
                    .hideDelay(2000);
                toaster.toastClass("angular-animate");
                _this.$mdToast.show(toaster);
            };
            this.openMenu = function ($mdOpenMenu, ev) {
                if (!_this.userDetails.isAuthenticated()) {
                    _this.$rootScope.$broadcast("show-unregisterd-box");
                    return;
                }
                $mdOpenMenu(ev);
            };
            this.showBoxAd = false;
            $rootScope.$on("$stateChangeSuccess", function (event, toState) {
                _this.showBoxAd = toState.parent === "box";
                var path = $location.path(), absUrl = $location.absUrl(), virtualUrl = absUrl.substring(absUrl.indexOf(path));
                window["dataLayer"].push({ event: "virtualPageView", virtualUrl: virtualUrl });
                __insp.push(["virtualPage"]);
            });
            $rootScope.$on("$stateChangeError", function (event, toState, toParams, fromState, fromParams, error) {
                console.error(error);
            });
            $rootScope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {
                if (!fromState.name) {
                    return;
                }
                if (toState.name === "user" && toParams.userId === 22886) {
                    event.preventDefault();
                    $rootScope.$broadcast("state-change-start-prevent");
                }
                $mdMenu.hide();
                $mdToast.hide();
                $rootScope.$broadcast("close-menu");
                $rootScope.$broadcast("close-collapse");
                var toStateName = toState.name;
                if (toStateName !== "searchinfo") {
                    $rootScope.$broadcast("search-close");
                }
                if (fromParams.boxId && toParams.boxId) {
                    if (fromParams.boxId === toParams.boxId && toStateName === "box"
                        && fromState.name.startsWith("box")) {
                        event.preventDefault();
                        $rootScope.$broadcast("state-change-start-prevent");
                    }
                }
                if (toStateName === "settings" && fromState.name.startsWith("settings")) {
                    event.preventDefault();
                    $rootScope.$broadcast("state-change-start-prevent");
                }
                if (!userDetails.isAuthenticated()) {
                    return;
                }
                var details = userDetails.get();
                if (details.university.id) {
                    document.title = resManager.get("siteName");
                    return;
                }
                var userWithNoUniversityState = "universityChoose";
                if (toStateName !== userWithNoUniversityState) {
                    $rootScope.$broadcast("state-change-start-prevent");
                    event.preventDefault();
                }
            });
        }
        AppController.prototype.resetForm = function (myform) {
            myform.$setPristine();
            myform.$setUntouched();
        };
        ;
        AppController.$inject = ["$rootScope", "$location",
            "userDetailsFactory", "$mdToast", "$document", "$mdMenu", "resManager",
            "CacheFactory",
            "sbHistory", "$state", "$window"];
        return AppController;
    }());
    angular.module("app").controller("AppController", AppController);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var boxId;
    var BoxController = (function () {
        function BoxController($state, $stateParams, boxData, $scope, $rootScope, user, resManager, boxService, ajaxService2, $timeout, $window, userUpdatesService) {
            var _this = this;
            this.$state = $state;
            this.$stateParams = $stateParams;
            this.boxData = boxData;
            this.$scope = $scope;
            this.$rootScope = $rootScope;
            this.user = user;
            this.resManager = resManager;
            this.boxService = boxService;
            this.ajaxService2 = ajaxService2;
            this.$timeout = $timeout;
            this.$window = $window;
            this.userUpdatesService = userUpdatesService;
            if ($state.current.name === "box") {
                $state.go("box.feed", $stateParams, { location: "replace" });
            }
            boxId = $stateParams.boxId;
            this.data = boxData;
            this.showLeaderboard = this.isAcademic = boxData.boxType === "academic" || boxData.boxType === "academicClosed";
            this.needFollow = boxData.userType === "invite" || boxData.userType === "none";
            this.canInvite = boxData.boxType !== "academicClosed" && this.isAcademic || (boxData.userType === "owner" && !this.isAcademic);
            this.canShare = boxData.boxType !== "academicClosed" && this.isAcademic && !this.needFollow;
            this.owner = boxData.userType === "owner";
            $scope.$on("close_invite", function () {
                _this.inviteOpen = false;
            });
            $scope.$on("follow-box", function () {
                _this.followBox();
            });
            $scope.$on("close-collapse", function () {
                _this.inviteOpen = false;
                _this.settingsOpen = false;
            });
            $scope.$on("hide-leader-board", function () {
                _this.showLeaderboard = false;
            });
            $window.onbeforeunload = function () {
                if (!_this.user.id) {
                    return;
                }
                userUpdatesService.deleteUpdates(boxId);
            };
        }
        BoxController.prototype.follow = function () {
            var _this = this;
            if (!this.user.id) {
                this.$rootScope.$broadcast("show-unregisterd-box");
                return;
            }
            var appController = this.$scope["app"];
            appController.showToaster(this.resManager.get("toasterFollowBox"));
            this.boxService.follow(boxId).then(function () {
                _this.followBox();
            });
        };
        BoxController.prototype.followBox = function () {
            this.needFollow = false;
            this.$rootScope.$broadcast("refresh-boxes");
        };
        BoxController.prototype.updateBox = function (updateBoxForm) {
            var _this = this;
            if (this.settings.needFollow) {
                this.boxService.unfollow(boxId).then(function () {
                    _this.$rootScope.$broadcast("remove-box", boxId);
                    _this.$state.go("dashboard");
                });
                return;
            }
            var needToSave = false;
            angular.forEach(updateBoxForm, function (value, key) {
                if (key[0] === "$")
                    return;
                if (!needToSave) {
                    needToSave = !value.$pristine;
                }
            });
            if (needToSave) {
                this.data.name = this.settings.name;
                this.data.privacySetting = this.settings.privacy;
                if (this.isAcademic) {
                    this.data.courseId = this.settings.courseId;
                    this.data.professorName = this.settings.professorName;
                }
                this.settings.submitDisabled = true;
                this.boxService.updateBox(boxId, this.data.name, this.settings.courseId, this.settings.professorName, this.settings.privacy, this.settings.notificationSettings).then(function (response) {
                    _this.settingsOpen = false;
                    _this.$stateParams["boxName"] = response.queryString;
                    var appController = _this.$scope["app"];
                    appController.showToaster(_this.resManager.get("toasterBoxSettings"));
                    _this.$state.go('box.feed', _this.$stateParams, { location: "replace" });
                }).finally(function () {
                    _this.settings.submitDisabled = false;
                });
            }
        };
        BoxController.prototype.inviteToBox = function () {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            if (this.inviteOpen) {
                this.$rootScope.$broadcast("close-collapse");
                this.inviteOpen = false;
            }
            else {
                this.$rootScope.$broadcast("close-collapse");
                this.inviteOpen = true;
                this.$scope.$broadcast("open_invite");
            }
        };
        BoxController.prototype.closeCollapse = function () {
            this.$rootScope.$broadcast("close-collapse");
        };
        BoxController.prototype.isActiveState = function (state) {
            return state === this.$state.current.name;
        };
        BoxController.prototype.inviteExpand = function () {
            var _this = this;
            if (this.html) {
                return;
            }
            return this.ajaxService2.getHtml("/share/invitedialog/").then(function (response) {
                _this.html = response;
                _this.$timeout(function () {
                    _this.$scope.$broadcast("open_invite");
                });
            });
        };
        BoxController.prototype.toggleSettings = function () {
            var _this = this;
            if (this.needFollow) {
                return;
            }
            if (this.settingsHtml) {
                if (this.settingsOpen) {
                    this.settingsOpen = false;
                }
                else {
                    this.settingsOpen = true;
                }
                return;
            }
            return this.ajaxService2.getHtml('/box/boxsettings/').then(function (response) {
                _this.settingsHtml = response;
                _this.$timeout(function () {
                    _this.$rootScope.$broadcast('close-collapse');
                    _this.settingsOpen = true;
                    _this.settings = _this.settings || {};
                    _this.settings.name = _this.data.name;
                    _this.settings.needFollow = _this.needFollow;
                    _this.settings.submitDisabled = false;
                    if (_this.isAcademic) {
                        _this.settings.courseId = _this.data.courseId;
                        _this.settings.professorName = _this.data.professorName;
                    }
                    else if (_this.owner) {
                        _this.settings.privacy = _this.data.privacySetting;
                    }
                    if (!_this.settings.notificationSettings) {
                        _this.boxService.notification(boxId).then(function (response2) {
                            _this.settings.notificationSettings = response2;
                        });
                    }
                });
            });
        };
        BoxController.prototype.canDelete = function (userId) {
            if (this.user.isAdmin || this.user.id === userId) {
                return true;
            }
            return false;
        };
        BoxController.$inject = ["$state", "$stateParams", "boxData", "$scope",
            "$rootScope", "user", "resManager", "boxService", "ajaxService2", "$timeout", "$window", "userUpdatesService"];
        return BoxController;
    }());
    angular.module('app.box').controller('BoxController', BoxController);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var BoxService = (function () {
        function BoxService(ajaxService2) {
            this.ajaxService2 = ajaxService2;
        }
        BoxService.prototype.getBox = function (boxid) {
            return this.ajaxService2.get('/box/data/', { id: boxid }, "boxData");
        };
        ;
        BoxService.prototype.getFeed = function (boxid, top, skip) {
            return this.ajaxService2.get('/qna/', { id: boxid, top: top, skip: skip });
        };
        ;
        BoxService.prototype.getReplies = function (boxid, commentId, replyId) {
            return this.ajaxService2.get('/qna/replies/', { boxid: boxid, id: commentId, replyId: replyId });
        };
        BoxService.prototype.leaderBoard = function (boxid) {
            return this.ajaxService2.get('/box/leaderboard/', { id: boxid });
        };
        BoxService.prototype.getRecommended = function (boxid) {
            return this.ajaxService2.get('/box/recommended/', { id: boxid });
        };
        ;
        BoxService.prototype.items = function (boxId, tabId, page) {
            return this.ajaxService2.get('/box/items/', { id: boxId, tabId: tabId, page: page }, "boxItems");
        };
        ;
        BoxService.prototype.getTabs = function (boxid) {
            return this.ajaxService2.get('/box/tabs/', { id: boxid });
        };
        ;
        BoxService.prototype.addItemToTab = function (boxId, tabId, itemId) {
            return this.ajaxService2.post('/box/additemtotab/', {
                boxId: boxId,
                tabId: tabId,
                itemId: itemId
            }, "boxItems");
        };
        BoxService.prototype.createTab = function (name, boxId) {
            return this.ajaxService2.post('/box/createtab/', {
                name: name,
                boxId: boxId
            });
        };
        BoxService.prototype.renameTab = function (tabId, name, boxId) {
            return this.ajaxService2.post('/box/renametab/', {
                tabId: tabId,
                name: name,
                boxId: boxId
            });
        };
        BoxService.prototype.deleteTab = function (tabId, boxId) {
            return this.ajaxService2.post('/box/deletetab/', {
                tabId: tabId,
                boxId: boxId
            }, "boxItems");
        };
        BoxService.prototype.deleteItem = function (itemId) {
            return this.ajaxService2.post('/item/delete/', {
                itemId: itemId
            }, "boxItems");
        };
        ;
        BoxService.prototype.filterItem = function (term, boxId, page) {
            return this.ajaxService2.get('/search/iteminbox/', {
                term: term,
                boxId: boxId,
                page: page
            });
        };
        ;
        BoxService.prototype.getMembers = function (boxid) {
            return this.ajaxService2.get('/box/members/', { boxId: boxid });
        };
        ;
        BoxService.prototype.getQuizzes = function (boxid) {
            return this.ajaxService2.get('/box/quizes/', { id: boxid });
        };
        ;
        BoxService.prototype.postComment = function (content, boxId, files, anonymously) {
            return this.ajaxService2.post('/qna/addcomment/', { content: content, boxId: boxId, files: files, anonymously: anonymously });
        };
        BoxService.prototype.deleteComment = function (commentId, boxId) {
            return this.ajaxService2.post('/qna/deletecomment/', {
                questionId: commentId,
                boxId: boxId
            });
        };
        BoxService.prototype.postReply = function (content, boxId, commentId, files) {
            return this.ajaxService2.post('/qna/addreply/', {
                content: content,
                boxId: boxId,
                commentId: commentId,
                files: files
            });
        };
        BoxService.prototype.deleteReply = function (postId, boxId) {
            return this.ajaxService2.post('/qna/deletereply/', {
                answerId: postId,
                boxId: boxId
            });
        };
        BoxService.prototype.likeComment = function (postId, boxId) {
            return this.ajaxService2.post('/qna/likecomment/', {
                commentId: postId,
                boxId: boxId
            });
        };
        BoxService.prototype.likeReply = function (replyId, boxId) {
            return this.ajaxService2.post('/qna/likereply/', {
                replyId: replyId, boxId: boxId
            });
        };
        BoxService.prototype.commentLikes = function (postId, boxId) {
            return this.ajaxService2.get('/qna/commentlikes/', {
                id: postId,
                boxId: boxId
            });
        };
        BoxService.prototype.replyLikes = function (replyId, boxId) {
            return this.ajaxService2.get('/qna/replylikes/', {
                id: replyId,
                boxId: boxId
            });
        };
        BoxService.prototype.follow = function (boxId) {
            return this.ajaxService2.post('/share/subscribetobox/', {
                boxId: boxId
            }, "boxData");
        };
        BoxService.prototype.notification = function (boxId) {
            return this.ajaxService2.get('/box/getnotification/', {
                boxId: boxId
            });
        };
        BoxService.prototype.unfollow = function (boxId) {
            return this.ajaxService2.post('/box/delete/', {
                id: boxId
            }, "boxData");
        };
        BoxService.prototype.updateBox = function (boxId, name, course, professor, privacy, notification) {
            return this.ajaxService2.post('/box/updateinfo/', {
                id: boxId,
                name: name,
                courseCode: course,
                professor: professor,
                boxPrivacy: privacy,
                notification: notification
            }, "boxData");
        };
        BoxService.prototype.feedLikes = function (boxId) {
            return this.ajaxService2.get("/qna/Likes/", {
                id: boxId
            });
        };
        BoxService.$inject = ["ajaxService2"];
        return BoxService;
    }());
    angular.module("app.box").service("boxService", BoxService);
})(app || (app = {}));
var app;
(function (app) {
    'use strict';
    var page = 0, needToBringMore = true, disablePaging = false;
    function resetParams() {
        page = 0;
        needToBringMore = true;
        disablePaging = false;
    }
    var ItemsController = (function () {
        function ItemsController(boxService, $stateParams, $rootScope, itemThumbnailService, $mdDialog, $scope, user, $q, resManager, $state, $window, $timeout) {
            var _this = this;
            this.boxService = boxService;
            this.$stateParams = $stateParams;
            this.$rootScope = $rootScope;
            this.itemThumbnailService = itemThumbnailService;
            this.$mdDialog = $mdDialog;
            this.$scope = $scope;
            this.user = user;
            this.$q = $q;
            this.resManager = resManager;
            this.$state = $state;
            this.$window = $window;
            this.$timeout = $timeout;
            this.items = [];
            this.uploadShow = true;
            this.buildItem = function (value) {
                value.downloadLink = value.url + 'download/';
                var retVal = _this.itemThumbnailService.assignValue(value.source);
                value.thumbnail = retVal.thumbnail;
                value.nameExtension = value.name.replace(/\.[^/.]+$/, "");
            };
            resetParams();
            $scope["stateParams"] = $stateParams;
            if ($stateParams["tabId"] && $stateParams["q"]) {
                $state.go("box.items", { tabId: $stateParams["tabId"], q: null });
                return;
            }
            if ($stateParams["tabId"]) {
                this.getItems().then(function () {
                    _this.scrollToPosition();
                });
            }
            else if ($stateParams["q"]) {
                this.getFilter().then(function () {
                    ;
                    _this.scrollToPosition();
                });
            }
            else {
                this.getItems().then(function () {
                    ;
                    _this.scrollToPosition();
                });
            }
            $rootScope.$on('disablePaging', function () {
                disablePaging = true;
            });
            $rootScope.$on('enablePaging', function () {
                disablePaging = false;
            });
            $scope.$on('update-thumbnail', function (e, args) {
                var item = _this.items.find(function (x) { return (x.id === args); });
                if (item) {
                    item.thumbnail += '&1=1';
                }
            });
            $rootScope.$on('item_upload', function (event, response2) {
                var self = _this;
                if (angular.isArray(response2)) {
                    for (var j = 0; j < response2.length; j++) {
                        pushItem(response2[j]);
                    }
                    return;
                }
                pushItem(response2);
                function pushItem(response) {
                    if (!response) {
                        return;
                    }
                    if (response.boxId !== $stateParams.boxId) {
                        return;
                    }
                    if (response.item.tabId !== $stateParams["tabId"]) {
                        return;
                    }
                    self.followBox();
                    var item = response.item;
                    self.buildItem(item);
                    self.items.unshift(item);
                }
            });
            $rootScope.$on('close_upload', function () {
                _this.uploadShow = true;
            });
            $rootScope.$on('item_delete', function (e, itemId) {
                var item = _this.items.findIndex(function (x) { return (x.id === itemId); });
                if (item >= 0) {
                    _this.items.splice(item, 1);
                }
            });
            $scope.$watchCollection(function () {
                return [$state.params["tabId"], $state.params["q"]];
            }, function (newParams, oldParams) {
                if ($state.current.name !== 'box.items') {
                    return;
                }
                if (newParams[0] !== oldParams[0]) {
                    if ($stateParams["tabId"] && $stateParams["q"]) {
                        $state.go('box.items', { tabId: $stateParams["tabId"], q: null });
                        return;
                    }
                    resetParams();
                    _this.getItems();
                }
                if (newParams[1] !== oldParams[1]) {
                    resetParams();
                    _this.getFilter();
                }
            });
        }
        ItemsController.prototype.myPagingFunction = function () {
            if (this.term) {
                return this.$q.when();
            }
            return this.getItems();
        };
        ;
        ItemsController.prototype.followBox = function () {
            this.$scope.$emit('follow-box');
        };
        ItemsController.prototype.removeItemFromTab = function (item) {
            this.boxService.addItemToTab(this.$stateParams.boxId, null, item.id);
            this.$scope.$broadcast('tab-item-remove');
            var index = this.items.indexOf(item);
            this.items.splice(index, 1);
        };
        ItemsController.prototype.addItemToTab = function ($data, tab) {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            var item = this.items.findIndex(function (x) { return (x.id === $data.id); });
            if (item >= 0) {
                this.items.splice(item, 1);
            }
            tab.count++;
            this.followBox();
            this.boxService.addItemToTab(this.$stateParams.boxId, tab.id, $data.id);
        };
        ItemsController.prototype.openUpload = function () {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            this.$rootScope.$broadcast('open_upload', this.$stateParams["tabId"]);
            this.uploadShow = false;
        };
        ItemsController.prototype.deleteItem = function (ev, item) {
            var _this = this;
            disablePaging = true;
            var confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('deleteItem'))
                .targetEvent(ev)
                .ok(this.resManager.get('dialogOk'))
                .cancel(this.resManager.get('dialogCancel'));
            this.$mdDialog.show(confirm).then(function () {
                var index = _this.items.indexOf(item);
                _this.boxService.deleteItem(item.id).then(function () {
                    _this.$scope.$broadcast('tab-item-remove');
                    _this.items.splice(index, 1);
                });
            }).finally(function () {
                disablePaging = false;
            });
        };
        ItemsController.prototype.getItems = function () {
            var _this = this;
            if (!needToBringMore) {
                return this.$q.when();
            }
            if (disablePaging) {
                return this.$q.when();
            }
            return this.boxService.items(this.$stateParams.boxId, this.$stateParams["tabId"], page).then(function (response) {
                angular.forEach(response, _this.buildItem);
                if (page > 0) {
                    _this.items = _this.items.concat(response);
                }
                else {
                    _this.items = response;
                }
                if (!response.length) {
                    needToBringMore = false;
                }
                page++;
            });
        };
        ItemsController.prototype.filter = function () {
            if (!this.term) {
                this.getItems();
            }
            this.$state.go('box.items', { tabId: null, q: this.term });
        };
        ItemsController.prototype.getFilter = function () {
            var _this = this;
            this.term = this.$stateParams["q"];
            return this.boxService.filterItem(this.$stateParams["q"], this.$stateParams.boxId, 0).then(function (response) {
                angular.forEach(response, _this.buildItem);
                _this.items = response;
            });
        };
        ItemsController.prototype.scrollToPosition = function () {
            var _this = this;
            var yOffsetParam = this.$stateParams["pageYOffset"];
            if (yOffsetParam) {
                this.$timeout(function () {
                    _this.$window.scrollTo(0, yOffsetParam);
                });
            }
        };
        ItemsController.$inject = ['boxService', '$stateParams', '$rootScope',
            'itemThumbnailService', '$mdDialog',
            '$scope', 'user', '$q', 'resManager', '$state', "$window", "$timeout"];
        return ItemsController;
    }());
    angular.module('app.box.items').controller('ItemsController', ItemsController);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var States;
    (function (States) {
        States[States["UserList"] = 1] = "UserList";
        States[States["Chat"] = 3] = "Chat";
    })(States || (States = {}));
    var timeoutvalidate;
    var ChatController = (function () {
        function ChatController($scope, $timeout, $stateParams, realtimeFactory) {
            var _this = this;
            this.$scope = $scope;
            this.$timeout = $timeout;
            this.$stateParams = $stateParams;
            this.realtimeFactory = realtimeFactory;
            this.state = States.UserList;
            this.connected = false;
            this.scrollSetting = {
                scrollbarPosition: "outside",
                scrollInertia: 50
            };
            timeoutvalidate = null;
            $scope.$watch(realtimeFactory.isConnected, function (newValue, oldValue) {
                if (newValue === false) {
                    timeoutvalidate = _this.$timeout(function () {
                        _this.connected = false;
                        $scope.$applyAsync();
                    }, 50);
                }
                else {
                    if (timeoutvalidate) {
                        _this.$timeout.cancel(timeoutvalidate);
                    }
                    _this.connected = true;
                    $scope.$applyAsync();
                }
            });
            $scope.$on("open-chat-user", function (e, args) {
                _this.state = States.Chat;
                _this.$scope.$broadcast("go-chat", args);
            });
            $scope.$on("go-chat", function (e, args) {
                _this.state = States.Chat;
                _this.$timeout(function () {
                    _this.$scope.$broadcast("go-conversation", args);
                });
            });
            if ($stateParams["conversationData"]) {
                this.state = States.Chat;
                this.$scope.$broadcast("go-chat", $stateParams["conversationData"]);
            }
        }
        ChatController.prototype.backFromChat = function () {
            this.state = States.UserList;
        };
        ChatController.$inject = ["$scope", "$timeout", "$stateParams", "realtimeFactory"];
        return ChatController;
    }());
    angular.module("app.chat").controller("ChatController", ChatController);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var unreadCount = 0;
    var ChatBus = (function () {
        function ChatBus(ajaxService) {
            this.ajaxService = ajaxService;
            this.setUnread = function (count) {
                unreadCount = count;
            };
            this.getUnread = function () {
                return unreadCount;
            };
        }
        ChatBus.prototype.getUnreadFromServer = function () {
            var _this = this;
            return this.ajaxService.get("/chat/unread")
                .then(function (response) {
                _this.setUnread(response);
            });
        };
        ChatBus.prototype.messages = function (q, page) {
            return this.ajaxService.get("/chat/conversation", { q: q, page: page });
        };
        ChatBus.prototype.chat = function (id, userIds, dateTime, top) {
            return this.ajaxService.get("/chat/messages", {
                chatRoom: id,
                userIds: userIds,
                startTime: dateTime,
                top: top
            });
        };
        ChatBus.prototype.preview = function (blob, i) {
            return this.ajaxService.get("/chat/Preview", {
                blobName: blob,
                index: i
            });
        };
        ChatBus.prototype.read = function (id) {
            return this.ajaxService.post("chat/markread", {
                chatRoom: id
            });
        };
        ChatBus.factory = function () {
            var factory = function (ajaxService2) {
                return new ChatBus(ajaxService2);
            };
            factory["$inject"] = ["ajaxService2"];
            return factory;
        };
        return ChatBus;
    }());
    angular
        .module("app.chat")
        .factory("chatBus", ChatBus.factory());
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var ChatTimeAgo = (function () {
        function ChatTimeAgo(timeAgo, nowTime) {
            var _this = this;
            this.timeAgo = timeAgo;
            this.nowTime = nowTime;
            this.scope = {
                fromTime: "@",
                format: "@"
            };
            this.restrict = "EA";
            this.link = function (scope, element) {
                var threeDaysInMilliseconds = 2.592e+8;
                var fromTime;
                scope.$watch('fromTime', function () {
                    fromTime = _this.timeAgo.parse(scope.fromTime);
                });
                var unregister = scope.$watch(function () {
                    return _this.nowTime() - fromTime;
                }, function (value) {
                    if (value > threeDaysInMilliseconds) {
                        element.text('');
                        unregister();
                        return;
                    }
                    element.text(_this.timeAgo.inWords(value, fromTime, scope.format));
                });
            };
        }
        ChatTimeAgo.factory = function () {
            var directive = function (timeAgo, nowTime) {
                return new ChatTimeAgo(timeAgo, nowTime);
            };
            directive["$inject"] = ["timeAgo", "nowTime"];
            return directive;
        };
        return ChatTimeAgo;
    }());
    angular
        .module("app.chat")
        .directive("chatTimeAgo", ChatTimeAgo.factory());
})(app || (app = {}));
;
var app;
(function (app) {
    "use strict";
    var page = 0;
    var ChatUsers = (function () {
        function ChatUsers(chatBus, userDetailsFactory, $timeout, $rootScope, $scope, notificationService, resManager) {
            var _this = this;
            this.chatBus = chatBus;
            this.userDetailsFactory = userDetailsFactory;
            this.$timeout = $timeout;
            this.$rootScope = $rootScope;
            this.$scope = $scope;
            this.notificationService = notificationService;
            this.resManager = resManager;
            this.focusSearch = false;
            this.users = [];
            this.usersPaging = function () {
                page++;
                _this.search(_this.term, true);
            };
            this.search();
            $scope.$on("hub-status", function (e, args) {
                var user = _this.users.find(function (f) { return (f.id === args.userId); });
                if (!user) {
                    return;
                }
                user.lastSeen = new Date().toISOString();
                user.online = args.online;
                $scope.$applyAsync();
            });
            $scope.$on("refresh-boxes", function () {
                _this.search().then(function () {
                    $scope.$applyAsync();
                });
            });
            $scope.$on("remove-box", function () {
                _this.search().then(function () {
                    $scope.$applyAsync();
                });
            });
            $scope.$on("hub-chat", function (e, args) {
                var self = _this;
                if (!self.users.length) {
                    _this.search();
                    notificationService.send(resManager.get('toasterChatMessage'), args.message, null, onNotificationClick);
                    self.updateUnread();
                    $scope.$applyAsync();
                    return;
                }
                var user = self.getConversationPartner(args.chatRoom);
                if (user) {
                    notificationService.send(user.name, args.message, user.image, onNotificationClick);
                    user.unread++;
                    self.updateUnread();
                    $scope.$applyAsync();
                    return;
                }
                user = self.users.find(function (f) { return (f.id === args.user); });
                if (!user) {
                    notificationService.send(resManager.get('toasterChatMessage'), args.message, null, onNotificationClick);
                    self.search();
                    return;
                }
                notificationService.send(user.name, args.message, user.image);
                user.unread++;
                user.conversation = args.id;
                self.updateUnread();
                $scope.$applyAsync();
                function onNotificationClick() {
                    var partner = self.getConversationPartner(args.chatRoom);
                    self.chat(partner);
                }
            });
        }
        ChatUsers.prototype.getConversationPartner = function (chatRoomId) {
            return this.users.find(function (f) { return (f.conversation === chatRoomId); });
        };
        ChatUsers.prototype.search = function (term, loadNextPage) {
            var _this = this;
            if (!loadNextPage) {
                page = 0;
            }
            if (!term) {
                this.term = '';
            }
            return this.chatBus.messages(term, page).then(function (response) {
                if (loadNextPage) {
                    _this.users = _this.makeUniqueAndRemoveMySelf(_this.users.concat(response));
                }
                else {
                    page = 0;
                    _this.users = _this.makeUniqueAndRemoveMySelf(response);
                }
                if (!Modernizr.cssscrollbar) {
                    _this.$scope["c"].updateScrollbar2("update");
                }
                _this.updateUnread();
            });
        };
        ChatUsers.prototype.expandSearch = function () {
            this.$rootScope.$broadcast("expandChat");
            this.focusSearch = true;
        };
        ChatUsers.prototype.makeUniqueAndRemoveMySelf = function (array) {
            var flags = [];
            var output = [];
            var l = array.length;
            var i;
            for (i = 0; i < l; i++) {
                if (array[i].id === this.userDetailsFactory.get().id)
                    continue;
                if (flags[array[i].id])
                    continue;
                flags[array[i].id] = true;
                output.push(array[i]);
            }
            return output;
        };
        ChatUsers.prototype.updateUnread = function () {
            var _this = this;
            if (this.users) {
                var x = 0;
                for (var i = 0; i < this.users.length; i++) {
                    x += this.users[i].unread || 0;
                }
                this.$timeout(function () {
                    _this.chatBus.setUnread(x);
                });
            }
        };
        ChatUsers.prototype.chat = function (user) {
            if (user.unread) {
                user.unread = 0;
                this.chatBus.read(user.conversation);
                this.updateUnread();
            }
            this.$rootScope.$broadcast("expandChat");
            this.$scope.$emit("go-chat", user);
        };
        ChatUsers.$inject = ["chatBus", "userDetailsFactory", "$timeout",
            "$rootScope", "$scope", "notificationService", "resManager"];
        return ChatUsers;
    }());
    angular.module("app.chat").controller("chatUsers", ChatUsers);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var chunkSize = 50;
    var Conversation = (function () {
        function Conversation($scope, chatBus, userDetailsFactory, $timeout, itemThumbnailService, realtimeFactory, $uiViewScroll, routerHelper, $mdDialog) {
            var _this = this;
            this.$scope = $scope;
            this.chatBus = chatBus;
            this.userDetailsFactory = userDetailsFactory;
            this.$timeout = $timeout;
            this.itemThumbnailService = itemThumbnailService;
            this.realtimeFactory = realtimeFactory;
            this.$uiViewScroll = $uiViewScroll;
            this.routerHelper = routerHelper;
            this.$mdDialog = $mdDialog;
            this.lastPage = false;
            this.messages = [];
            this.upload = {
                url: "/upload/chatfile/",
                options: {
                    chunk_size: "3mb"
                },
                callbacks: {
                    filesAdded: function (uploader) {
                        _this.$timeout(function () {
                            uploader.start();
                        }, 1);
                    },
                    beforeUpload: function (up, file) {
                        up.settings.multipart_params = {
                            fileName: file.name,
                            fileSize: file.size,
                            users: [_this.userChat.id]
                        };
                    },
                    fileUploaded: function (uploader, file, response) {
                        var obj = JSON.parse(response.response);
                        if (obj.success) {
                            _this.messages.push({
                                time: new Date().toISOString(),
                                partner: false,
                                blob: obj.payload,
                                thumb: _this.itemThumbnailService.getChat(obj.payload)
                            });
                            _this.realtimeFactory.sendMsg(_this.userChat.id, null, _this.userChat.conversation, obj.payload);
                        }
                    }
                }
            };
            $scope.$on("go-conversation", function (e, args) {
                _this.conversation(args);
            });
            $scope.$on("preview-ready", function (e, args) {
                var message = _this.messages.find(function (f) { return (f.blob === args); });
                if (message) {
                    message.thumb += '&1=1';
                }
            });
            $scope.$on("hub-chat", function (e, args) {
                if (args.userId !== userDetailsFactory.get().id) {
                    if (!_this.userChat) {
                        return;
                    }
                    if (!_this.userChat.conversation) {
                        _this.userChat.conversation = args.chatRoom;
                    }
                    var messages = _this.messages.filter(function (message) { return (message.text === args.message); });
                    var attachments = _this.messages.filter(function (message) { return (message.blob === args.blob); });
                    if (!args.blob && (!messages.length || messages[messages.length - 1].time < new Date(+new Date() - 60000).toISOString())) {
                        _this.messages.push({
                            text: args.message,
                            time: new Date().toISOString(),
                            partner: false
                        });
                    }
                    if (args.blob && !attachments.length) {
                        _this.messages.push({
                            blob: args.blob,
                            time: new Date().toISOString(),
                            thumb: itemThumbnailService.getChat(args.blob),
                            partner: false
                        });
                    }
                    _this.$timeout(function () {
                        var unread = _this.chatBus.getUnread();
                        _this.chatBus.setUnread(--unread);
                    });
                    $scope.$apply();
                    _this.scrollToBotton();
                    return;
                }
                if (_this.userChat && _this.userChat.conversation === args.chatRoom) {
                    if (args.blob) {
                        args.thumb = itemThumbnailService.getChat(args.blob);
                    }
                    _this.messages.push({
                        text: args.message,
                        time: new Date().toISOString(),
                        partner: true,
                        blob: args.blob,
                        thumb: args.thumb
                    });
                    _this.$timeout(function () {
                        var unread = _this.chatBus.getUnread();
                        _this.chatBus.setUnread(--unread);
                    });
                    $scope.$apply();
                    _this.chatBus.read(_this.userChat.conversation);
                    _this.scrollToBotton();
                    return;
                }
            });
        }
        Conversation.prototype.conversation = function (userChat) {
            var _this = this;
            this.userChat = userChat;
            this.chatBus.chat(this.userChat.conversation, [this.userChat.id, this.userDetailsFactory.get().id], null, chunkSize).then(function (response) {
                _this.messages = _this.handleChatMessages(response);
                _this.scrollToBotton();
                if (response.length < chunkSize) {
                    _this.lastPage = true;
                }
            });
        };
        Conversation.prototype.send = function () {
            if (!this.newText) {
                return;
            }
            this.messages.push({
                text: this.newText,
                time: new Date().toISOString(),
                partner: false
            });
            this.realtimeFactory.sendMsg(this.userChat.id, this.newText, this.userChat.conversation);
            this.newText = "";
        };
        Conversation.prototype.handleChatMessages = function (response) {
            response.reverse();
            for (var i = 0; i < response.length; i++) {
                response[i].partner = response[i].userId !== this.userDetailsFactory.get().id;
                if (response[i].blob) {
                    response[i].thumb = this.itemThumbnailService.getChat(response[i].blob);
                }
            }
            return response;
        };
        Conversation.prototype.scrollToBotton = function () {
            var _this = this;
            this.$timeout(function () {
                _this.$scope.$broadcast("chat-scroll");
            });
        };
        Conversation.prototype.loadMoreMessages = function () {
            var _this = this;
            var firstMessage = this.messages[0];
            if (!firstMessage.id) {
                return;
            }
            this.chatBus.chat(this.userChat.conversation, [this.userChat.id, this.userDetailsFactory.get().id], this.messages[0].time, chunkSize).then(function (response) {
                _this.messages = _this.handleChatMessages(response).concat(_this.messages);
                _this.$timeout(function () {
                    _this.$uiViewScroll(angular.element("#chatMessage_" + firstMessage.id));
                });
                if (response.length < chunkSize) {
                    _this.lastPage = true;
                }
            });
        };
        Conversation.prototype.dialog = function (blob, ev) {
            var _this = this;
            this.$scope.$broadcast("disablePaging");
            this.$mdDialog.show({
                controller: "previewController",
                controllerAs: "lc",
                templateUrl: this.routerHelper.buildUrl("/chat/previewdialog/"),
                parent: angular.element(document.body),
                targetEvent: ev,
                clickOutsideToClose: true,
                resolve: {
                    doc: function () { return _this.chatBus.preview(blob, 0); },
                    blob: function () { return blob; }
                },
                fullscreen: true
            }).finally(function () {
                _this.$scope.$broadcast("enablePaging");
            });
        };
        Conversation.$inject = ["$scope", "chatBus", "userDetailsFactory",
            "$timeout", "itemThumbnailService", "realtimeFactory",
            "$uiViewScroll", "routerHelper", "$mdDialog"];
        return Conversation;
    }());
    angular.module("app.chat").controller("conversation", Conversation);
})(app || (app = {}));
var app;
(function (app) {
    "use scrict";
    var Guid = (function () {
        function Guid() {
        }
        Guid.newGuid = function () {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        };
        return Guid;
    }());
    app.Guid = Guid;
    var connectionStatus = false;
    var RealTimeFactory = (function () {
        function RealTimeFactory(Hub, $rootScope, ajaxService) {
            var _this = this;
            this.Hub = Hub;
            this.$rootScope = $rootScope;
            this.ajaxService = ajaxService;
            this.commands = [];
            this.changeStatus = function (isConnected) {
                connectionStatus = isConnected;
                _this.$rootScope.$applyAsync();
            };
            this.isConnected = function () {
                return connectionStatus;
            };
            this.boxIds = [];
            this.assingBoxes = function (boxIds) {
                if (!boxIds) {
                    return;
                }
                if (!angular.isArray(boxIds)) {
                    boxIds = [boxIds];
                    _this.boxIds = _this.boxIds.concat(boxIds);
                }
                if (_this.canSend) {
                    _this.hub.invoke('enterBoxes', boxIds);
                }
                else {
                    _this.commands.push(function () {
                        _this.assingBoxes.apply(_this, [boxIds]);
                    });
                }
            };
            this.hub = new Hub('spitballHub', {
                rootPath: (dChat || 'https://connect.spitball.co') + '/s',
                listeners: {
                    chat: function (message, chatRoom, userId, blob) {
                        $rootScope.$broadcast("hub-chat", {
                            message: message,
                            chatRoom: chatRoom,
                            userId: userId,
                            blob: blob
                        });
                    },
                    online: function (userId) {
                        $rootScope.$broadcast('hub-status', {
                            userId: userId,
                            online: true
                        });
                    },
                    offline: function (userId) {
                        $rootScope.$broadcast('hub-status', {
                            userId: userId,
                            online: false
                        });
                    },
                    updateImage: function (blob) {
                        $rootScope.$broadcast('preview-ready', blob);
                    },
                    updateThumbnail: function (itemId) {
                        $rootScope.$broadcast('update-thumbnail', itemId);
                    },
                    echo: function (i) {
                    }
                },
                errorHandler: function (error) {
                    ajaxService.logError('signalr', 'errorHandler', error);
                },
                methods: ['send', 'changeUniversity', 'enterBoxes'],
                stateChanged: function (state) {
                    switch (state.newState) {
                        case $.signalR.connectionState.connecting:
                            _this.canSend = false;
                            _this.changeStatus(false);
                            break;
                        case $.signalR.connectionState.connected:
                            _this.canSend = true;
                            for (var i = 0; i < _this.commands.length; i++) {
                                _this.commands[i]();
                            }
                            _this.commands = [];
                            _this.changeStatus(true);
                            break;
                        case $.signalR.connectionState.reconnecting:
                            _this.canSend = false;
                            ajaxService.logError('signalr', 'reconnecting');
                            _this.changeStatus(false);
                            break;
                        case $.signalR.connectionState.disconnected:
                            _this.canSend = false;
                            _this.changeStatus(false);
                            connectionStatus = false;
                            break;
                    }
                }
            });
            $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams) {
                if (toParams.boxId) {
                    if (_this.boxIds.indexOf(toParams.boxId) === -1) {
                        _this.assingBoxes(toParams.boxId);
                    }
                }
            });
            this.hub.connection.disconnected(function () {
                setTimeout(function () {
                    _this.hub.connection.start();
                }, 5000);
            });
        }
        RealTimeFactory.prototype.sendMsg = function (userId, message, conversationId, blob) {
            this.hub.invoke('send', userId, message, conversationId, blob);
        };
        RealTimeFactory.prototype.changeUniversity = function () {
            this.hub.invoke('changeUniversity');
        };
        ;
        RealTimeFactory.$inject = ["Hub", "$rootScope", "ajaxService2"];
        return RealTimeFactory;
    }());
    angular.module('app.chat').service('realtimeFactory', RealTimeFactory);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var PreviewController = (function () {
        function PreviewController($mdDialog, doc, blob, $sce) {
            this.$mdDialog = $mdDialog;
            this.downloadLink = "/chat/download/?blobName=" + blob;
            if (!doc || !doc.viewName) {
                this.view = 'preview-faild.html';
            }
            else {
                this.items = doc.content;
                if (doc.viewName === 'Text') {
                    this.items[0] = $sce.trustAsResourceUrl(this.items[0]);
                }
                this.view = "chat-" + doc.viewName + ".html";
            }
        }
        PreviewController.prototype.close = function () {
            this.$mdDialog.hide();
        };
        PreviewController.$inject = ['$mdDialog', 'doc', 'blob', '$sce'];
        return PreviewController;
    }());
    angular.module('app.chat').controller('previewController', PreviewController);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var AppRun = (function () {
        function AppRun(routerHelper) {
            this.routerHelper = routerHelper;
            routerHelper.configureStates(getStates());
            function getStates() {
                return [
                    {
                        state: "dashboard",
                        config: {
                            url: "/dashboard/",
                            controller: "Dashboard as d",
                            data: { animateClass: "dashboard" },
                            resolve: {
                                boxes: ["dashboardService", function (dashboardService) { return dashboardService.getBoxes(); }]
                            }
                        },
                        templateUrl: "/dashboard/indexpartial/"
                    }];
            }
        }
        AppRun.factory = function () {
            var factory = function (routerHelper) {
                return new AppRun(routerHelper);
            };
            factory["$inject"] = ["routerHelper"];
            return factory;
        };
        return AppRun;
    }());
    angular.module("app.dashboard").run(AppRun.factory());
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var defer, serverCall = false;
    var Dashboard = (function () {
        function Dashboard($q, ajaxService2, realtimeFactotry, userUpdatesService, $rootScope) {
            var _this = this;
            this.$q = $q;
            this.ajaxService2 = ajaxService2;
            this.realtimeFactotry = realtimeFactotry;
            this.userUpdatesService = userUpdatesService;
            this.$rootScope = $rootScope;
            this.boxes = null;
            defer = $q.defer();
            $rootScope.$on("delete-updates", function (e, arg) {
                var box = _this.boxes.find(function (v) { return (v.id === arg); });
                if (box) {
                    box.updates = 0;
                }
            });
            $rootScope.$on("remove-box", function (e, arg) {
                arg = parseInt(arg, 10);
                var box = _this.boxes.find(function (v) { return (v.id === arg); });
                if (box) {
                    var index = _this.boxes.indexOf(box);
                    _this.boxes.splice(index, 1);
                }
            });
            $rootScope.$on("refresh-boxes", function () {
                _this.boxes = null;
                defer = $q.defer();
            });
        }
        Dashboard.prototype.getBoxes = function () {
            var _this = this;
            if (this.boxes) {
                return this.$q.when(this.boxes);
            }
            if (!serverCall) {
                serverCall = true;
                this.ajaxService2.get("dashboard/boxlist/")
                    .then(function (response) {
                    serverCall = false;
                    _this.realtimeFactotry.assingBoxes(response.map(function (val) { return val.id; }));
                    _this.boxes = response;
                    for (var i = 0; i < _this.boxes.length; i++) {
                        (function (box) {
                            _this.userUpdatesService.updatesNum(box.id).then(function (val) {
                                box.updates = val;
                            });
                        })(_this.boxes[i]);
                    }
                    defer.resolve(_this.boxes);
                });
            }
            return defer.promise;
        };
        Dashboard.prototype.getUniversityMeta = function (universityId) {
            return this.ajaxService2.get('dashboard/university', { universityId: universityId }, 'university');
        };
        ;
        Dashboard.prototype.createPrivateBox = function (boxName) {
            return this.ajaxService2.post('dashboard/create', { boxName: boxName });
        };
        ;
        Dashboard.prototype.leaderboard = function () {
            return this.ajaxService2.get('dashboard/leaderboard');
        };
        ;
        Dashboard.prototype.recommended = function () {
            return this.ajaxService2.get('dashboard/recommendedcourses');
        };
        Dashboard.$inject = ["$q", "ajaxService2", "realtimeFactory", "userUpdatesService", "$rootScope"];
        return Dashboard;
    }());
    angular.module("app.dashboard").service("dashboardService", Dashboard);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var ValidQuestion;
    (function (ValidQuestion) {
        ValidQuestion[ValidQuestion["AnswerNeedText"] = 1] = "AnswerNeedText";
        ValidQuestion[ValidQuestion["QuestionNeedText"] = 2] = "QuestionNeedText";
        ValidQuestion[ValidQuestion["QuestionCorrectAnswer"] = 3] = "QuestionCorrectAnswer";
        ValidQuestion[ValidQuestion["QuestionMoreAnswer"] = 4] = "QuestionMoreAnswer";
        ValidQuestion[ValidQuestion["EmptyQuestion"] = 5] = "EmptyQuestion";
        ValidQuestion[ValidQuestion["Ok"] = 6] = "Ok";
    })(ValidQuestion || (ValidQuestion = {}));
    var quizId;
    var saveInProgress = false;
    var canNavigateBack = false;
    function finishUpdate() {
        saveInProgress = false;
    }
    var QuizData = (function () {
        function QuizData() {
            this.questions = [new Question()];
        }
        QuizData.prototype.completeData = function () {
            if (!this.questions.length) {
                this.questions.push(new Question());
            }
        };
        QuizData.prototype.deserialize = function (input) {
            this.questions = [];
            this.id = input.id;
            this.name = input.name;
            for (var i = 0; i < input.questions.length; i++) {
                this.questions.push(new Question().deserialize(input.questions[i]));
            }
            this.completeData();
            return this;
        };
        return QuizData;
    }());
    var Question = (function () {
        function Question() {
            this.answers = [new Answer(), new Answer()];
        }
        Question.prototype.completeQuestion = function () {
            for (var i = this.answers.length; i < 2; i++) {
                this.answers.push(new Answer());
            }
        };
        Question.prototype.addAnswer = function () {
            this.answers.push(new Answer());
        };
        Question.prototype.removeAnswer = function (index) {
            this.answers.splice(index, 1);
        };
        Question.prototype.validQuestion = function () {
            var emptyQuestion = true;
            var retVal = [];
            if (!this.text) {
                retVal.push(ValidQuestion.QuestionNeedText);
            }
            else {
                emptyQuestion = false;
            }
            for (var j = 0; j < this.answers.length; j++) {
                var valid = this.answers[j].validAnswer();
                if (valid === ValidQuestion.AnswerNeedText) {
                    retVal.push(valid);
                }
                else {
                    emptyQuestion = false;
                }
            }
            if (this.correctAnswer == null) {
                retVal.push(ValidQuestion.QuestionCorrectAnswer);
            }
            else {
                emptyQuestion = false;
            }
            if (!retVal.length) {
                return ValidQuestion.Ok;
            }
            if (emptyQuestion) {
                return ValidQuestion.EmptyQuestion;
            }
            return retVal[0];
        };
        Question.prototype.deserialize = function (input) {
            this.answers = [];
            this.id = input.id;
            this.text = input.text;
            for (var i = 0; i < input.answers.length; i++) {
                this.answers.push(new Answer().deserialize(input.answers[i]));
            }
            if (angular.isNumber(input.correctAnswer)) {
                this.correctAnswer = input.correctAnswer;
            }
            else {
                this.correctAnswer = this.answers.findIndex(function (x) { return x.id === input.correctAnswer; });
            }
            this.completeQuestion();
            return this;
        };
        return Question;
    }());
    app.Question = Question;
    var Answer = (function () {
        function Answer() {
        }
        Answer.prototype.validAnswer = function () {
            if (this.text) {
                return ValidQuestion.Ok;
            }
            return ValidQuestion.AnswerNeedText;
        };
        Answer.prototype.deserialize = function (input) {
            this.id = input.id;
            this.text = input.text;
            return this;
        };
        return Answer;
    }());
    var QuizCreateController = (function () {
        function QuizCreateController($mdDialog, $state, $stateParams, $scope, quizService, quizData, resManager, $q, $window) {
            var _this = this;
            this.$mdDialog = $mdDialog;
            this.$state = $state;
            this.$stateParams = $stateParams;
            this.$scope = $scope;
            this.quizService = quizService;
            this.quizData = quizData;
            this.resManager = resManager;
            this.$q = $q;
            this.$window = $window;
            this.quizNameDisabled = true;
            this.submitDisabled = false;
            this.newName = function () {
                var self = _this;
                _this.$scope.$watch(function () { return _this.quizData.name; }, function (newVal, oldVal) {
                    if (newVal === oldVal) {
                        return;
                    }
                    var form = self.$scope["quizName"];
                    if (!form.$valid) {
                        console.log('her');
                        return;
                    }
                    form.$setPristine();
                    if (!saveInProgress) {
                        submitQuizName();
                        function finishSaveName() {
                            finishUpdate();
                            if (!form.$submitted) {
                                submitQuizName();
                            }
                        }
                        function submitQuizName() {
                            form.$setSubmitted();
                            saveInProgress = true;
                            if (quizId) {
                                self.quizService.updateQuiz(quizId, newVal).finally(finishSaveName);
                            }
                            else {
                                self.createQuiz(newVal).finally(finishSaveName);
                            }
                        }
                    }
                });
            };
            this.navigateBackToBox = function () {
                _this.$state.go("box.quiz", {
                    boxtype: _this.$stateParams["boxtype"],
                    universityType: _this.$stateParams["universityType"],
                    boxId: _this.$stateParams.boxId,
                    boxName: _this.$stateParams["boxName"]
                });
            };
            this.boxName = $stateParams["name"] || $stateParams["boxName"];
            quizId = $stateParams["quizid"];
            this.newName();
            if (quizData) {
                this.quizData = new QuizData().deserialize(quizData);
            }
            else {
                this.quizData = new QuizData();
            }
            $window.onbeforeunload = function () {
                for (var i = 0; i < _this.quizData.questions.length; i++) {
                    var question = _this.quizData.questions[i];
                    if (!question.id && question.validQuestion() !== ValidQuestion.EmptyQuestion) {
                        return _this.resManager.get("quizLeaveTitle");
                    }
                }
            };
            $scope.$on("$destroy", function () {
                $window.onbeforeunload = undefined;
            });
            $scope.$on("$stateChangeStart", function (event) {
                if (canNavigateBack) {
                    return;
                }
                if (!canNavigate()) {
                    event.preventDefault();
                    $scope.$emit("state-change-start-prevent");
                }
            });
            function canNavigate() {
                for (var i = 0; i < this.quizData.questions.length; i++) {
                    var question = this.quizData.questions[i];
                    if (!question.id) {
                        if (!confirm("Are you sure you want to leave this page?")) {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        QuizCreateController.prototype.createQuiz = function (name) {
            var _this = this;
            var self = this;
            return self.quizService.createQuiz(self.$stateParams.boxId, name).then(function (response) {
                quizId = response;
                _this.$state.go("quizCreate", {
                    boxtype: self.$stateParams["boxtype"],
                    universityType: self.$stateParams["universityType"],
                    boxId: self.$stateParams.boxId,
                    boxName: self.$stateParams["boxName"],
                    quizid: response,
                    name: self.$stateParams["name"]
                });
            });
        };
        QuizCreateController.prototype.addQuestion = function () {
            var _this = this;
            var self = this;
            var $qArray = [this.$q.when()];
            var promiseCreateQuiz = this.$q.when();
            if (!quizId) {
                promiseCreateQuiz = this.createQuiz(this.quizData.name);
            }
            promiseCreateQuiz.then(function () {
                for (var i = 0; i < _this.quizData.questions.length; i++) {
                    var question = _this.quizData.questions[i];
                    var validQuestion = question.validQuestion();
                    if (validQuestion !== ValidQuestion.Ok) {
                        _this.showQuestionErrors(validQuestion, i);
                        return;
                    }
                    if (validQuestion === ValidQuestion.Ok && !question.id) {
                        (function (question, index) {
                            $qArray.push(self.quizService.createQuestion(quizId, question)
                                .then(function (response) {
                                self.quizData.questions[index] = new Question().deserialize(response);
                            }));
                        })(question, i);
                    }
                }
                _this.$q.when($qArray)
                    .then(function () {
                    _this.quizData.questions.push(new Question());
                });
            });
        };
        QuizCreateController.prototype.showQuestionErrors = function (valid, index) {
            var form = this.$scope["createQuestions"];
            form['valids_' + index].$setValidity('server', false);
            switch (valid) {
                case ValidQuestion.AnswerNeedText:
                    this.error = this.resManager.get("quizCreateNeedAnswerText");
                    break;
                case ValidQuestion.QuestionNeedText:
                case ValidQuestion.EmptyQuestion:
                    this.error = this.resManager.get("quizCreateNeedQuestionText");
                    break;
                case ValidQuestion.QuestionCorrectAnswer:
                    this.error = this.resManager.get("quizCreateCorrectAnswer");
                    break;
                case ValidQuestion.QuestionMoreAnswer:
                    this.error = this.resManager.get("quizCreateNeedAnswers");
                    break;
                default:
            }
        };
        QuizCreateController.prototype.removeQuestionFromArray = function (question) {
            var index = this.quizData.questions.indexOf(question);
            this.quizData.questions.splice(index, 1);
        };
        QuizCreateController.prototype.deleteQuestion = function (question) {
            if (question.id) {
                this.quizService.deleteQuestion(question.id);
            }
            this.removeQuestionFromArray(question);
        };
        QuizCreateController.prototype.editQuestion = function (question) {
            var canEdit = true;
            var i = this.quizData.questions.length;
            while (i--) {
                var question_1 = this.quizData.questions[i];
                var validQuestion = question_1.validQuestion();
                if (validQuestion === ValidQuestion.Ok) {
                    continue;
                }
                if (validQuestion === ValidQuestion.EmptyQuestion) {
                    this.removeQuestionFromArray(question_1);
                    continue;
                }
                this.showQuestionErrors(validQuestion, i);
                canEdit = false;
                break;
            }
            if (canEdit) {
                if (question.id) {
                    this.quizService.deleteQuestion(question.id);
                }
                angular.forEach(question.answers, function (a) {
                    a.id = null;
                });
                question.id = null;
            }
        };
        QuizCreateController.prototype.close = function (ev) {
            var _this = this;
            canNavigateBack = true;
            if (!quizId) {
                this.navigateBackToBox();
                return;
            }
            var confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('quizLeaveTitle'))
                .textContent(this.resManager.get('quizLeaveContent'))
                .targetEvent(ev)
                .ok(this.resManager.get('quizDelete'))
                .cancel(this.resManager.get('quizSaveAsDraft'));
            this.$mdDialog.show(confirm).then(function () {
                _this.quizService.deleteQuiz(quizId).then(_this.navigateBackToBox);
            }, function () {
                var $qArray = [_this.$q.when()], self = _this;
                for (var i = 0; i < _this.quizData.questions.length; i++) {
                    var question = _this.quizData.questions[i];
                    if (question.validQuestion() === ValidQuestion.EmptyQuestion) {
                        continue;
                    }
                    if (!question.id) {
                        (function (question) {
                            $qArray.push(self.quizService.createQuestion(quizId, question));
                        })(question);
                    }
                }
                _this.$q.when($qArray)
                    .then(_this.navigateBackToBox);
            });
        };
        QuizCreateController.prototype.template = function (question) {
            if (question.id && question.validQuestion() === ValidQuestion.Ok) {
                return 'quiz-question-template.html';
            }
            return 'create-quiz-question-template.html';
        };
        QuizCreateController.prototype.publish = function () {
            var _this = this;
            if (!this.quizData.name) {
                var form = this.$scope['quizName'];
                form["name"].$setValidity("required", false);
                return;
            }
            var $qArray = [this.$q.when()];
            for (var i = 0; i < this.quizData.questions.length; i++) {
                var question = this.quizData.questions[i];
                var validQuestion = question.validQuestion();
                if (validQuestion !== ValidQuestion.Ok) {
                    this.showQuestionErrors(validQuestion, i);
                    return;
                }
                if (validQuestion === ValidQuestion.Ok && !question.id) {
                    $qArray.push(this.quizService.createQuestion(quizId, question));
                }
            }
            this.submitDisabled = true;
            this.$q.when($qArray)
                .then(function () {
                _this.quizService.publish(quizId)
                    .then(function () {
                    canNavigateBack = true;
                    _this.navigateBackToBox();
                })
                    .finally(function () {
                    _this.submitDisabled = false;
                })
                    .catch(function () { });
            });
        };
        QuizCreateController.$inject = ["$mdDialog", "$state", "$stateParams", "$scope",
            "quizService", "quizData", "resManager", "$q", "$window"];
        return QuizCreateController;
    }());
    angular.module('app.quiz.create').controller('QuizCreateController', QuizCreateController);
    console.log('here');
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var QuizService = (function () {
        function QuizService(ajaxService) {
            this.ajaxService = ajaxService;
        }
        QuizService.prototype.getQuiz = function (boxId, quizId) {
            return this.ajaxService.get("/quiz/data/", { boxId: boxId, quizId: quizId });
        };
        QuizService.prototype.saveAnswers = function (data) {
            return this.ajaxService.post('/quiz/saveAnswers', data);
        };
        QuizService.prototype.getDiscussion = function (data) {
            return this.ajaxService.get('/quiz/discussion', data);
        };
        QuizService.prototype.createDiscussion = function (data) {
            return this.ajaxService.post('/quiz/creatediscussion', data);
        };
        QuizService.prototype.removeDiscussion = function (data) {
            return this.ajaxService.post('/quiz/deletediscussion', data);
        };
        QuizService.prototype.getNumberOfSolvers = function (data) {
            return this.ajaxService.get('/quiz/numberofsolvers', data);
        };
        QuizService.prototype.draft = function (quizId) {
            return this.ajaxService.get('/quiz/draft/', { quizId: quizId });
        };
        QuizService.prototype.createQuiz = function (boxId, name) {
            return this.ajaxService.post('/quiz/create/', {
                boxId: boxId,
                name: name
            });
        };
        QuizService.prototype.updateQuiz = function (id, name) {
            return this.ajaxService.post('/quiz/update/', {
                id: id,
                name: name
            });
        };
        QuizService.prototype.createQuestion = function (quizId, question) {
            return this.ajaxService.post('/quiz/createquestion/', {
                quizId: quizId,
                model: question
            });
        };
        QuizService.prototype.updateQuestion = function (questionId, text) {
            return this.ajaxService.post('/quiz/updatequestion/', {
                id: questionId,
                text: text
            });
        };
        QuizService.prototype.deleteQuestion = function (questionId) {
            return this.ajaxService.post('/quiz/deletequestion/', {
                id: questionId
            });
        };
        QuizService.prototype.publish = function (quizId) {
            return this.ajaxService.post('/quiz/save/', {
                quizId: quizId
            });
        };
        QuizService.prototype.deleteQuiz = function (quizId) {
            return this.ajaxService.post('/quiz/delete/', {
                id: quizId
            });
        };
        QuizService.$inject = ["ajaxService2"];
        return QuizService;
    }());
    angular.module("app.quiz").service('quizService', QuizService);
})(app || (app = {}));
(function () {
    angular.module('app.quiz.create', ['textAngular', 'app.ajaxservice']);
})();
(function () {
    "use strict";
    angular.module('textAngular').config(config);
    config.$inject = ['$provide'];
    function config($provide) {
        $provide.decorator('taOptions', ['taRegisterTool', '$delegate', '$q', '$stateParams',
            function (taRegisterTool, taOptions, $q, $stateParams) {
                taOptions.forceTextAngularSanitize = false;
                var buttons = [['fontUp', 'fontDown'],
                    ['bold', 'italics', 'underline'],
                    ['justifyLeft', 'justifyCenter', 'justifyRight'],
                    ['ol', 'ul'],
                    ['insertImage'],
                    ['redo', 'undo']];
                if (Modernizr.inputtypes.color) {
                    buttons[1].push('color');
                }
                taOptions.toolbar = buttons;
                taOptions.defaultFileDropHandler = function (file, insertAction) {
                    var dfd = $q.defer();
                    var client = new XMLHttpRequest();
                    client.onreadystatechange = function () {
                        if (client.readyState === 4 && client.status === 200) {
                            var response = JSON.parse(client.response);
                            if (!response.success) {
                                alert('Error');
                                return;
                            }
                            insertAction('insertImage', response.payload, true);
                            dfd.resolve();
                        }
                    };
                    var formData = new FormData();
                    formData.append(file.name, file);
                    formData.append("boxId", $stateParams.boxId);
                    client.open("POST", "/upload/quizimage/", true);
                    client.send(formData);
                    return dfd.promise;
                };
                return taOptions;
            }]);
    }
})();
var app;
(function (app) {
    "use strict";
    var stateName = "searchinfo";
    var tabs = ["doc", "course", "quiz"];
    var page = 0, needToBringMore = true;
    var SearchController = (function () {
        function SearchController($scope, dashboardService, $location, $state, $stateParams, analytics, searchService, itemThumbnailService, $q) {
            var _this = this;
            this.$scope = $scope;
            this.dashboardService = dashboardService;
            this.$location = $location;
            this.$state = $state;
            this.$stateParams = $stateParams;
            this.analytics = analytics;
            this.searchService = searchService;
            this.itemThumbnailService = itemThumbnailService;
            this.$q = $q;
            this.noResults = false;
            this.result = [];
            if (tabs.indexOf($stateParams["t"]) === -1) {
                this.$state.go(stateName, { q: this.$stateParams["q"], t: tabs[0] });
            }
            this.tab = $stateParams["t"];
            this.doQuery(false);
            $scope.$watchCollection(function () { return [$state.params["q"], $state.params["t"]]; }, function (newParams, oldParams) {
                if (newParams === oldParams) {
                    return;
                }
                analytics.trackPage($location.url(), "Search");
                _this.tab = newParams[1];
                _this.doQuery(false);
            });
        }
        SearchController.prototype.back = function () {
            var appController = this.$scope["app"];
            appController.back("/dashboard/");
        };
        SearchController.prototype.univeristyClick = function () {
            var _this = this;
            this.dashboardService.getUniversityMeta()
                .then(function (response) {
                _this.$location.path(response.url);
            });
        };
        SearchController.prototype.changeTab = function (tab) {
            if (tabs.indexOf(tab) === -1) {
                return;
            }
            if (this.$state.params["t"] === tab) {
                return;
            }
            this.$state.go(stateName, { q: this.$stateParams["q"], t: tab });
        };
        SearchController.prototype.myPagingFunction = function () {
            return this.doQuery(true);
        };
        SearchController.prototype.doQuery = function (needToAppend) {
            if (!needToAppend) {
                page = 0;
                needToBringMore = true;
            }
            if (!needToBringMore) {
                return this.$q.when();
            }
            switch (this.tab) {
                case tabs[0]:
                    return this.getItems(needToAppend);
                case tabs[2]:
                    return this.getQuizzes(needToAppend);
                default:
                    return this.getBoxes(needToAppend);
            }
        };
        SearchController.prototype.getBoxes = function (needToAppend) {
            var _this = this;
            return this.searchService.searchBox(this.$state.params["q"], page)
                .then(function (response) {
                _this.noResults = false;
                if (needToAppend) {
                    _this.result = _this.result.concat(response);
                }
                else {
                    _this.result = response;
                }
                if (!response.length && page === 0) {
                    needToBringMore = false;
                    _this.noResults = true;
                }
                page++;
            });
        };
        SearchController.prototype.getItems = function (needToAppend) {
            var _this = this;
            return this.searchService.searchItems(this.$state.params["q"], page)
                .then(function (response) {
                _this.noResults = false;
                angular.forEach(response, function (value) {
                    var retVal = _this.itemThumbnailService.assignValue(value.source);
                    value.thumbnail = retVal.thumbnail;
                    value.nameExtension = value.name.replace(/\.[^/.]+$/, "");
                });
                if (needToAppend) {
                    _this.result = _this.result.concat(response);
                }
                else {
                    _this.result = response;
                }
                if (!response.length && page === 0) {
                    needToBringMore = false;
                    _this.noResults = true;
                }
                page++;
            });
        };
        SearchController.prototype.getQuizzes = function (needToAppend) {
            var _this = this;
            return this.searchService.searchQuizzes(this.$state.params["q"], page)
                .then(function (response) {
                _this.noResults = false;
                for (var j = 0; j < response.length; j++) {
                    response[j].publish = true;
                }
                if (needToAppend) {
                    _this.result = _this.result.concat(response);
                }
                else {
                    _this.result = response;
                }
                if (!response.length && page === 0) {
                    needToBringMore = false;
                    _this.noResults = true;
                }
                page++;
            });
        };
        SearchController.$inject = ["$scope", "dashboardService", "$location",
            "$state", "$stateParams", "Analytics", "searchService", "itemThumbnailService", "$q"];
        return SearchController;
    }());
    angular.module("app.search").controller("SearchController", SearchController);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var SearchService = (function () {
        function SearchService(ajaxService2) {
            this.ajaxService2 = ajaxService2;
        }
        SearchService.prototype.searchBox = function (term, page) {
            return this.ajaxService2.get("/search/boxes/", { q: term, page: page }, "searchBox", "search");
        };
        SearchService.prototype.searchItems = function (term, page) {
            return this.ajaxService2.get("/search/items/", { q: term, page: page }, "searchItem", "search");
        };
        SearchService.prototype.searchQuizzes = function (term, page) {
            return this.ajaxService2.get("/search/quizzes/", { q: term, page: page }, "searchQuiz", "search");
        };
        SearchService.$inject = ["ajaxService2"];
        return SearchService;
    }());
    angular.module("app.search").service("searchService", SearchService);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var searchStateName = "searchinfo";
    var SearchTriggerController = (function () {
        function SearchTriggerController($scope, $state) {
            var _this = this;
            this.$scope = $scope;
            this.$state = $state;
            this.term = $state.params["q"];
            $scope.$on("$stateChangeStart", function (event, toState, toParams, fromState) {
                if (fromState.name === searchStateName && toState.name !== searchStateName) {
                    _this.term = "";
                }
            });
        }
        SearchTriggerController.prototype.change = function () {
            this.search();
        };
        SearchTriggerController.prototype.search = function () {
            var form = this.$scope["searchTrigger"];
            if (form.$valid) {
                this.$state.go(searchStateName, { q: this.term, t: this.$state.params["t"] });
            }
        };
        SearchTriggerController.$inject = ["$scope", "$state"];
        return SearchTriggerController;
    }());
    angular.module("app.search").controller("SearchTriggerController", SearchTriggerController);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var UploadWrapper = (function () {
        function UploadWrapper(ajaxService, $scope, $rootScope) {
            var _this = this;
            this.ajaxService = ajaxService;
            this.$scope = $scope;
            this.$rootScope = $rootScope;
            $scope.$on('open_upload', function () {
                $rootScope.$broadcast('close-collapse');
                _this.open = true;
            });
            $scope.$on("close_upload", function () {
                _this.open = false;
            });
        }
        UploadWrapper.prototype.uploadOpen = function () {
            var _this = this;
            if (this.html) {
                return;
            }
            return this.ajaxService.getHtml('/item/uploaddialog/').then(function (response) {
                _this.html = response;
            });
        };
        UploadWrapper.prototype.uploadCollapsed = function () {
            this.$scope.$broadcast("uploadCollapsed");
        };
        UploadWrapper.$inject = ["ajaxService2", "$scope", "$rootScope"];
        return UploadWrapper;
    }());
    angular.module("app").controller("UploadWrapper", UploadWrapper);
})(app || (app = {}));
(function () {
    angular.module('app').directive('compileHtml', compileHtml);
    compileHtml.$inject = ['$sce', '$parse', '$compile'];
    function compileHtml($sce, $parse, $compile) {
        return {
            link: function (scope, element, attr) {
                var parsed = $parse(attr.compileHtml);
                function getStringValue() {
                    return (parsed(scope) || '').toString();
                }
                scope.$watch(getStringValue, function () {
                    var el = $compile(parsed(scope) || '')(scope);
                    element.empty();
                    element.append(el);
                });
            }
        };
    }
})();
var app;
(function (app) {
    "use strict";
    var UserUpdatesService = (function () {
        function UserUpdatesService(ajaxService, $q, userDetailsFactory, $rootScope, $window, $stateParams) {
            var _this = this;
            this.ajaxService = ajaxService;
            this.$q = $q;
            this.userDetailsFactory = userDetailsFactory;
            this.$rootScope = $rootScope;
            this.$window = $window;
            this.$stateParams = $stateParams;
            this.allUpdates = {};
            this.deferred = $q.defer();
            userDetailsFactory.init()
                .then(function (userData) {
                if (userData.id)
                    _this.getUpdates();
            });
            $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
                if (fromState.parent === "box") {
                    _this.deleteUpdates(fromParams.boxId);
                }
            });
        }
        UserUpdatesService.prototype.getUpdates = function () {
            var _this = this;
            this.ajaxService.get("/account/updates/").then(function (response2) {
                _this.data = response2;
                for (var i = 0; i < response2.length; i++) {
                    var currBox = _this.allUpdates[response2[i].boxId] || {};
                    if (response2[i].answerId) {
                        currBox[response2[i].answerId] = true;
                    }
                    else if (response2[i].questionId) {
                        currBox[response2[i].questionId] = true;
                    }
                    _this.allUpdates[response2[i].boxId] = currBox;
                }
                _this.deferred.resolve();
            }).catch(function () {
                _this.deferred.reject();
            });
        };
        UserUpdatesService.prototype.deleteUpdates = function (boxId) {
            var _this = this;
            this.updatesNum(boxId).then(function (length) {
                if (!length) {
                    return;
                }
                _this.deleteFromServer(boxId);
                delete _this.allUpdates[boxId];
                _this.$rootScope.$broadcast("delete-updates", boxId);
            });
        };
        UserUpdatesService.prototype.deleteFromServer = function (boxId) {
            if (!this.userDetailsFactory.isAuthenticated()) {
                return;
            }
            this.ajaxService.post("/box/deleteupdates/", {
                boxId: boxId
            });
        };
        UserUpdatesService.prototype.updatesNum = function (boxid) {
            var _this = this;
            var q = this.$q.defer();
            this.boxUpdates(boxid).then(function () {
                q.resolve(_this.allUpdates[boxid] ? Object.keys(_this.allUpdates[boxid]).length : 0);
            });
            return q.promise;
        };
        UserUpdatesService.prototype.boxUpdates = function (boxid) {
            var _this = this;
            var promise = this.deferred.promise;
            var q = this.$q.defer();
            promise.then(function () {
                q.resolve(_this.allUpdates[boxid]);
            });
            return q.promise;
        };
        UserUpdatesService.$inject = ["ajaxService2", "$q", "userDetailsFactory", "$rootScope", "$window", "$stateParams"];
        return UserUpdatesService;
    }());
    angular.module('app.user').service('userUpdatesService', UserUpdatesService);
})(app || (app = {}));
var app;
(function (app) {
    var HideChatOnMobile = (function () {
        function HideChatOnMobile($mdMedia) {
            var _this = this;
            this.$mdMedia = $mdMedia;
            this.restrict = 'A';
            this.link = function (scope, element, attrs) {
                if (_this.$mdMedia('xs')) {
                    element.on(attrs['hideChatOnMobile'], function () {
                        $('html').removeClass('expanded-chat');
                    });
                }
            };
        }
        HideChatOnMobile.factory = function () {
            var directive = function ($mdMedia) {
                return new HideChatOnMobile($mdMedia);
            };
            directive['$inject'] = ['$mdMedia'];
            return directive;
        };
        return HideChatOnMobile;
    }());
    angular
        .module("app.chat")
        .directive("hideChatOnMobile", HideChatOnMobile.factory());
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var ToggleChat = (function () {
        function ToggleChat(chatBus, $mdMedia, userDetailsFactory, $rootScope, $state) {
            var _this = this;
            this.chatBus = chatBus;
            this.$mdMedia = $mdMedia;
            this.userDetailsFactory = userDetailsFactory;
            this.$rootScope = $rootScope;
            this.$state = $state;
            this.restrict = "A";
            this.link = function (scope, element) {
                var $html = $("html");
                var className = "expanded-chat";
                var hide = "hide";
                if (!_this.userDetailsFactory.getUniversity()) {
                    element.addClass(hide);
                }
                _this.$rootScope.$on("change-university", function () {
                    if (_this.userDetailsFactory.getUniversity()) {
                        element.removeClass(hide);
                    }
                });
                element.on("click", function () {
                    if (_this.$mdMedia("xs")) {
                        if (_this.$state.current.name === "chat") {
                            scope["app"].back("/dashboard/");
                            return;
                        }
                        _this.$state.go("chat");
                    }
                    else {
                        $html.toggleClass(className);
                    }
                });
                scope.$on("expandChat", function () {
                    $html.addClass(className);
                });
                if (_this.$mdMedia("gt-sm")) {
                    return;
                }
                _this.chatBus.getUnreadFromServer();
                var counterElem = $(".chat-counter");
                var cleanUpFunc = scope.$watch(_this.chatBus.getUnread, function (value) {
                    if (value > 0) {
                        counterElem.text(value.toString()).show();
                    }
                    else {
                        counterElem.hide();
                    }
                });
                var hubChatListener = scope.$on("hub-chat", function () {
                    var unread = _this.chatBus.getUnread();
                    _this.chatBus.setUnread(++unread);
                    scope.$applyAsync();
                });
                scope.$on("$destroy", function () {
                    cleanUpFunc();
                    hubChatListener();
                });
            };
        }
        ToggleChat.factory = function () {
            var directive = function (chatBus, $mdMedia, userDetailsFactory, $rootScope, $state) {
                return new ToggleChat(chatBus, $mdMedia, userDetailsFactory, $rootScope, $state);
            };
            directive["$inject"] = ["chatBus", "$mdMedia", "userDetailsFactory", "$rootScope", "$state"];
            return directive;
        };
        return ToggleChat;
    }());
    angular
        .module("app.chat")
        .directive("toggleChat", ToggleChat.factory());
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var AppRun = (function () {
        function AppRun(routerHelper) {
            this.routerHelper = routerHelper;
            routerHelper.configureStates(getStates());
            function getStates() {
                return [
                    {
                        state: "user",
                        config: {
                            url: "/user/{userId:int}/:userName/",
                            controller: "UserController as u",
                            resolve: {
                                userData: [
                                    "userService", "$stateParams", function (userService, $stateParams) {
                                        return userService.getDetails($stateParams.userId);
                                    }
                                ]
                            }
                        },
                        templateUrl: "/user/indexpartial/"
                    }
                ];
            }
        }
        AppRun.factory = function () {
            var factory = function (routerHelper) {
                return new AppRun(routerHelper);
            };
            factory["$inject"] = ["routerHelper"];
            return factory;
        };
        return AppRun;
    }());
    angular.module('app.user').run(AppRun.factory());
})(app || (app = {}));
'use strict';
(function () {
    angular.module('app').config(config);
    config.$inject = ['DoubleClickProvider'];
    function config(doubleClickProvider) {
        var topDashboard = 'div-gpt-ad-1461243129238-0', boxMenu = 'div-gpt-ad-1461243129238-1', menu = 'div-gpt-ad-1461243129238-2', topBox = 'div-gpt-ad-1461243129238-3', itemSide = 'div-gpt-ad-1461244713254-0', itemBetween = 'div-gpt-ad-1459950737650-5', topItem = 'div-gpt-ad-1461568291501-0', searchTop = 'div-gpt-ad-1461243129238-6';
        doubleClickProvider.defineSlot('/107474526/Dash_Top_Banner', [[300, 75], [964, 100], [468, 60]], topDashboard)
            .defineSlot('/107474526/Box_Square_Banner', [[234, 60], [220, 200]], boxMenu)
            .defineSlot('/107474526/Box_Top_Banner', [[300, 75], [964, 100], [468, 60]], topBox)
            .defineSlot('/107474526/Dash_Square_Banner', [[234, 60], [220, 200]], menu)
            .defineSlot('/107474526/Item_Side_Banner', [160, 600], itemSide)
            .defineSlot('/107474526/Item_300x250_UTF', [[728, 90], [234, 60], [300, 600]], itemBetween)
            .defineSlot('/107474526/Item_Top_Banner', [[300, 75], [468, 60], [970, 300]], topItem)
            .defineSlot('/107474526/Search_Top_Banner', [[300, 75], [964, 100], [468, 60]], searchTop);
        doubleClickProvider.defineSizeMapping(topDashboard)
            .addSize([1000, 768], [964, 100])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);
        doubleClickProvider.defineSizeMapping(topBox)
            .addSize([1000, 768], [964, 100])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);
        doubleClickProvider.defineSizeMapping(menu)
            .addSize([960, 650], [220, 200])
            .addSize([0, 0], [234, 60]);
        doubleClickProvider.defineSizeMapping(boxMenu)
            .addSize([960, 650], [220, 200])
            .addSize([0, 0], [234, 60]);
        doubleClickProvider.defineSizeMapping(searchTop)
            .addSize([1000, 768], [964, 100])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);
        doubleClickProvider.defineSizeMapping(topItem)
            .addSize([1000, 768], [964, 100])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);
        doubleClickProvider.defineSizeMapping(itemBetween)
            .addSize([1000, 768], [964, 100])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);
        doubleClickProvider.defineSizeMapping(itemSide)
            .addSize([0, 0], [160, 600]);
    }
})();
var app;
(function (app) {
    "use strict";
    var second = 1000, minute = 60 * second, hour = 60 * minute, day = 24 * hour;
    var cancelObjs = {};
    var AjaxService2 = (function () {
        function AjaxService2($http, $q, analytics, cacheFactory, routerHelper) {
            this.$http = $http;
            this.$q = $q;
            this.analytics = analytics;
            this.cacheFactory = cacheFactory;
            this.routerHelper = routerHelper;
            this.cacheCategories = {
                university: {
                    maxAge: 6 * hour
                },
                accountDetail: {
                    maxAge: day
                },
                searchItem: {
                    maxAge: hour
                },
                searchBox: {
                    maxAge: hour
                },
                searchQuiz: {
                    maxAge: hour
                },
                html: {
                    maxAge: 30 * day,
                    storageMode: "localStorage"
                },
                department: {
                    maxAge: 15 * minute
                },
                boxItems: {
                    maxAge: 15 * minute
                },
                boxData: {
                    maxAge: 15 * minute
                },
                itemComment: {
                    maxAge: 15 * minute
                }
            };
            var dChat = window['dChat'];
            if (dChat.indexOf("develop") > -1) {
                this.cacheCategories.html = {
                    maxAge: 1 * second,
                    storageMode: "localStorage"
                };
            }
            for (var cacheKey in this.cacheCategories) {
                this.buildFactoryObject(cacheKey);
            }
        }
        AjaxService2.prototype.buildFactoryObject = function (cacheKey) {
            var dst = {};
            angular.extend(dst, {
                deleteOnExpire: "aggressive",
                maxAge: minute,
                recycleFreq: 15000,
                storageMode: "sessionStorage",
                storagePrefix: version
            }, this.cacheCategories[cacheKey]);
            this.cacheFactory(cacheKey, dst);
        };
        AjaxService2.prototype.deleteCacheCategory = function (category) {
            var dataCache = this.cacheFactory.get(category);
            dataCache.removeAll();
        };
        AjaxService2.prototype.post = function (url, data, category) {
            var _this = this;
            var dfd = this.$q.defer(), startTime = new Date().getTime();
            this.$http.post(this.buildUrl(url), data).then(function (response) {
                var retVal = response.data;
                _this.trackTime(startTime, url, data, "post");
                if (angular.isArray(category)) {
                    category.forEach(function (e) {
                        _this.deleteCacheCategory(e);
                    });
                }
                if (angular.isString(category)) {
                    _this.deleteCacheCategory(category);
                }
                if (!retVal) {
                    _this.logError(url, data, retVal);
                    dfd.reject();
                    return;
                }
                if (retVal.success) {
                    dfd.resolve(retVal.payload);
                    return;
                }
                dfd.reject(retVal.payload);
            }).catch(function (response) {
                dfd.reject(response);
                _this.logError(url, data, response);
            });
            return dfd.promise;
        };
        AjaxService2.prototype.getHtml = function (url) {
            var _this = this;
            var dfd = this.$q.defer();
            url = this.buildUrl(url);
            url = this.routerHelper.buildUrl(url);
            var dataCache = this.cacheFactory.get("html");
            if (dataCache.get(url)) {
                dfd.resolve(dataCache.get(url));
            }
            else {
                var startTime = new Date().getTime();
                this.$http.get(url).then(function (response) {
                    _this.trackTime(startTime, url, "get html", "html");
                    var data = response.data;
                    if (!data) {
                        dfd.reject();
                        return;
                    }
                    dataCache.put(url, data);
                    dfd.resolve(data);
                    return;
                }).catch(function (response) {
                    dfd.reject(response);
                    _this.logError(url, null, response);
                });
            }
            return dfd.promise;
        };
        AjaxService2.prototype.get = function (url, data, category, cancelCategory) {
            var self = this;
            var deferred = this.$q.defer();
            var cacheKey = url + JSON.stringify(data);
            if (category) {
                var dataCache = this.cacheFactory.get(category);
                if (dataCache && dataCache.get(cacheKey)) {
                    deferred.resolve(dataCache.get(cacheKey));
                    return deferred.promise;
                }
            }
            getFromServer(category);
            return deferred.promise;
            function getFromServer(cacheCategory) {
                var startTime = new Date().getTime();
                var getObj = {
                    params: data
                };
                if (cancelCategory) {
                    if (cancelObjs[cancelCategory]) {
                        cancelObjs[cancelCategory].resolve();
                    }
                    cancelObjs[cancelCategory] = self.$q.defer();
                    getObj.timeout = cancelObjs[cancelCategory].promise;
                }
                self.$http.get(self.buildUrl(url), getObj).then(function (response) {
                    var retVal = response.data;
                    self.trackTime(startTime, url, data, "get");
                    if (!retVal) {
                        deferred.reject();
                        return;
                    }
                    if (retVal.success) {
                        if (cacheCategory) {
                            var categoryFactory = self.cacheFactory.get(cacheCategory);
                            categoryFactory.put(cacheKey, retVal.payload);
                        }
                        deferred.resolve(retVal.payload);
                        return;
                    }
                    deferred.reject(retVal.payload);
                }).catch(function (response) {
                    deferred.reject(response);
                    self.logError(url, data, response);
                });
            }
        };
        AjaxService2.prototype.buildUrl = function (url) {
            url = url.toLowerCase();
            if (!url.startsWith("/")) {
                url = "/" + url;
            }
            if (!url.endsWith("/")) {
                url = url + "/";
            }
            ;
            return url;
        };
        AjaxService2.prototype.logError = function (url, data, payload) {
            var log = {
                data: data,
                payload: payload
            };
            console.error("eror ajax", url, data, payload);
            $.ajax({
                type: "POST",
                url: "/error/jslog/",
                contentType: "application/json",
                data: angular.toJson({
                    errorUrl: url,
                    errorMessage: JSON.stringify(log),
                    cause: "ajaxRequest"
                })
            });
        };
        AjaxService2.prototype.trackTime = function (startTime, url, data, type) {
            var timeSpent = new Date().getTime() - startTime;
            this.analytics.trackTimings(url.toLowerCase() !== "/item/preview/" ? "ajax " + type
                : "ajaxPreview", url, timeSpent, JSON.stringify(data));
        };
        AjaxService2.$inject = ["$http", "$q", "Analytics", "CacheFactory", "routerHelper"];
        return AjaxService2;
    }());
    angular.module("app").service("ajaxService2", AjaxService2);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var UserDetails = (function () {
        function UserDetails($rootScope, $q, ajaxService, analytics, $timeout, $interval) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$q = $q;
            this.ajaxService = ajaxService;
            this.analytics = analytics;
            this.$timeout = $timeout;
            this.$interval = $interval;
            this.isLogedIn = false;
            this.serverCall = false;
            this.deferDetails = this.$q.defer();
            this.get = function () {
                return _this.userData;
            };
            this.isAuthenticated = function () {
                return _this.isLogedIn;
            };
            this.setName = function (first, last) {
                _this.userData.name = first + " " + last;
                _this.$rootScope.$broadcast("userDetailsChange");
            };
            this.setImage = function (image) {
                if (!image) {
                    return;
                }
                _this.userData.image = image;
                _this.$rootScope.$broadcast("userDetailsChange");
            };
            this.getUniversity = function () {
                return _this.userData ? _this.userData.university.id : null;
            };
            this.setUniversity = function () {
                _this.ajaxService.deleteCacheCategory("accountDetail");
                _this.userData = null;
                _this.deferDetails = _this.$q.defer();
                return _this.init();
            };
        }
        UserDetails.prototype.setDetails = function (data) {
            var _this = this;
            if (data.id) {
                this.isLogedIn = true;
                __insp.push(["identify", data.id]);
            }
            this.$timeout(function () {
                _this.analytics.set("dimension1", data.universityName || "null");
                _this.analytics.set("dimension2", data.universityCountry || "null");
                _this.analytics.set("dimension3", data.id || "null");
            });
            var interval = this.$interval(function () {
                if (googletag.pubads !== undefined && googletag.pubads) {
                    googletag.pubads().setTargeting("gender", data.sex);
                    googletag.pubads().setTargeting("university", data.universityId);
                    _this.$interval.cancel(interval);
                }
            }, 20);
            this.userData = {
                id: data.id,
                name: data.name,
                image: data.image,
                sex: data.sex,
                score: data.score,
                url: data.url,
                createTime: new Date(data.dateTime),
                isAdmin: data.isAdmin,
                culture: data.culture,
                email: data.email,
                university: {
                    country: data.universityCountry,
                    name: data.universityName,
                    id: data.universityId
                }
            };
        };
        UserDetails.prototype.init = function () {
            var _this = this;
            if (this.userData) {
                this.deferDetails.resolve(this.userData);
                return this.deferDetails.promise;
            }
            if (!this.serverCall) {
                this.serverCall = true;
                this.ajaxService.get("/account/details/", null, "accountDetail").then(function (response) {
                    _this.setDetails(response);
                    _this.deferDetails.resolve(_this.userData);
                    _this.serverCall = false;
                });
            }
            return this.deferDetails.promise;
        };
        UserDetails.$inject = ["$rootScope", "$q", "ajaxService2", "Analytics", "$timeout", "$interval"];
        return UserDetails;
    }());
    angular.module("app").service("userDetailsFactory", UserDetails);
})(app || (app = {}));
var app;
(function (app) {
    'use strict';
    var loaded = false;
    var SideMenu = (function () {
        function SideMenu(user, dashboardService, $location, $scope, $mdSidenav) {
            var _this = this;
            this.user = user;
            this.dashboardService = dashboardService;
            this.$location = $location;
            this.$scope = $scope;
            this.$mdSidenav = $mdSidenav;
            this.courses = [];
            this.privateBoxes = [];
            this.userUrl = this.user.url;
            this.showBoxesNodes = true;
            this.coursesOpen = false;
            this.boxesOpen = false;
            $scope.$on("close-menu", function () {
                $mdSidenav("left").close();
            });
            $scope.$on("open-menu", function () {
                $mdSidenav("left").toggle();
            });
            $scope.$on("remove-box", function (e, arg) {
                arg = parseInt(arg, 10);
                _this.removeElement(_this.courses, arg);
                _this.removeElement(_this.privateBoxes, arg);
            });
            $scope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
                if (fromState.parent === "box") {
                    if (fromParams.boxtype === "box") {
                        var box = _this.privateBoxes.find(function (i) { return (i.id === fromParams.boxId); }) || {};
                        box.updates = 0;
                    }
                    else {
                        var abox = _this.courses.find(function (i) { return (i.id === fromParams.boxId); }) || {};
                        abox.updates = 0;
                    }
                }
            });
            $scope.$watch(function () {
                if (dashboardService.boxes) {
                    return dashboardService.boxes.length;
                }
                return dashboardService.boxes;
            }, function (val) {
                if (angular.isNumber(val)) {
                    if (val > 0) {
                        _this.showBoxesNodes = true;
                    }
                    else {
                        _this.showBoxesNodes = false;
                    }
                    return;
                }
                if (!val) {
                    _this.showBoxesNodes = true;
                }
            });
        }
        SideMenu.prototype.univeristyClick = function () {
            var _this = this;
            this.dashboardService.getUniversityMeta().then(function (response) {
                _this.$location.path(decodeURIComponent(response.url));
            });
        };
        SideMenu.prototype.initOpen = function () {
            if (!loaded) {
                this.getBoxes();
                loaded = true;
            }
            return true;
        };
        SideMenu.prototype.toggleCourses = function () {
            if (!this.initOpen()) {
                return;
            }
            this.coursesOpen = !this.coursesOpen;
            this.boxesOpen = false;
        };
        SideMenu.prototype.toggleBoxes = function () {
            if (!this.initOpen()) {
                return;
            }
            this.coursesOpen = false;
            this.boxesOpen = !this.boxesOpen;
        };
        SideMenu.prototype.isSectionSelected = function (section) {
            return decodeURI(this.$location.url()).startsWith(section);
        };
        SideMenu.prototype.getBoxes = function () {
            var _this = this;
            this.dashboardService.getBoxes().then(function (response2) {
                for (var i = 0; i < response2.length; i++) {
                    var b = response2[i];
                    if (b.boxType.startsWith('academic')) {
                        _this.courses.push(b);
                    }
                    else {
                        _this.privateBoxes.push(b);
                    }
                }
            });
        };
        SideMenu.prototype.removeElement = function (arr, arg) {
            var box = arr.find(function (e) { return (e.id === arg); });
            if (box) {
                var index = arr.indexOf(box);
                arr.splice(index, 1);
            }
        };
        SideMenu.$inject = ["user", "dashboardService", "$location", "$scope", "$mdSidenav"];
        return SideMenu;
    }());
    angular.module("app").controller("SideMenu", SideMenu);
})(app || (app = {}));
'use strict';
(function () {
    angular.module('app').provider('routerHelper', routerHelperProvider);
    routerHelperProvider.$inject = ['$locationProvider', '$stateProvider', '$urlRouterProvider'];
    function routerHelperProvider($locationProvider, $stateProvider, $urlRouterProvider) {
        this.$get = routerHelper;
        $locationProvider.html5Mode(true);
        routerHelper.$inject = ['$state'];
        function routerHelper($state) {
            var hasOtherwise = false;
            var service = {
                configureStates: configureStates,
                getStates: getStates,
                buildUrl: buildUrl
            };
            return service;
            function configureStates(states, otherwisePath) {
                states.forEach(function (state) {
                    if (!state.config.parent) {
                        state.config.parent = 'root';
                    }
                    if (state.templateUrl) {
                        var template = [
                            'ajaxService2', function (ajaxService2) { return ajaxService2
                                .getHtml(state.templateUrl); }
                        ];
                        if (state.config.views) {
                            state.config.views[''] = {
                                templateProvider: template,
                                controller: state.config.controller
                            };
                        }
                        else {
                            state.config.templateProvider = template;
                        }
                    }
                    $stateProvider.state(state.state, state.config);
                });
                if (otherwisePath && !hasOtherwise) {
                    hasOtherwise = true;
                    $urlRouterProvider.otherwise(otherwisePath);
                }
            }
            function getStates() { return $state.get(); }
            function buildUrl(path) {
                return path + '?lang=' + handleLanguage.getLangCookie() + '&version=' + version;
            }
        }
        return this;
    }
})();
var app;
(function (app) {
    "use strict";
    var SbHistory = (function () {
        function SbHistory($rootScope, $window) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$window = $window;
            this.skipState = false;
            this.arr = [];
            this.popElement = function () {
                if (_this.arr.length === 1) {
                    return;
                }
                return _this.arr.pop();
            };
            this.firstState = function () {
                return _this.arr.length === 0;
            };
            this.$rootScope.$on('$stateChangeStart', function (event) {
                _this.pageYOffset = $window.pageYOffset;
            });
            this.$rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
                if (fromState.name === toState.name) {
                    return;
                }
                if (toParams.fromBack) {
                    return;
                }
                if (_this.skipState) {
                    _this.skipState = false;
                    return;
                }
                _this.arr.push({
                    name: fromState.name,
                    params: angular.extend({}, fromParams, { pageYOffset: _this.pageYOffset })
                });
                _this.pageYOffset = 0;
            });
            this.$rootScope.$on('from-back', function () {
                _this.skipState = true;
            });
        }
        SbHistory.$inject = ["$rootScope", "$window"];
        return SbHistory;
    }());
    angular.module('app').service('sbHistory', SbHistory);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var ItemThumbnailService = (function () {
        function ItemThumbnailService() {
            this.logo = "https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/universityEmptyState.png";
        }
        ItemThumbnailService.prototype.get = function (name, width, height) {
            return "https://az779114.vo.msecnd.net/preview/" + encodeURIComponent(name) + ".jpg?width=" + width + "&height=" + height + "&mode=crop&scale=both";
        };
        ItemThumbnailService.prototype.assignValues = function (elements, widthElement, heightElement) {
            for (var i = 0; i < elements.length; i++) {
                if (!elements[i].thumbnail) {
                    var retVal = this.assignValue(elements[i].source, widthElement, heightElement);
                    elements[i].thumbnail = retVal.thumbnail;
                }
            }
            return elements;
        };
        ItemThumbnailService.prototype.assignValue = function (source, widthElement, heightElement) {
            widthElement = widthElement || 300;
            heightElement = heightElement || 424;
            var thumbnail = this.get(source, widthElement, heightElement);
            return {
                thumbnail: thumbnail
            };
        };
        ItemThumbnailService.prototype.getChat = function (name) {
            if (!name) {
                return "";
            }
            return "https://az779114.vo.msecnd.net/preview/chat/" + encodeURIComponent(name) + ".jpg?width=256&height=138&scale=both&mode=crop";
        };
        ItemThumbnailService.prototype.getUniversityPic = function (name, width, height) {
            if (!name) {
                name = "DefaultUni5.jpg";
            }
            return "https://az779114.vo.msecnd.net/universities/cover/" + encodeURIComponent(name) + "?mode=crop&cropxunits=100&cropyunits=100&crop=0,12,0,0&anchor=topcenter&width=" + width + "&height=" + height;
        };
        return ItemThumbnailService;
    }());
    angular.module("app").service("itemThumbnailService", ItemThumbnailService);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var NgSpinnerBar = (function () {
        function NgSpinnerBar($rootScope, $timeout) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$timeout = $timeout;
            this.link = function (scope, element) {
                var hide = "hide";
                _this.$rootScope.$on("$stateChangeStart", function () {
                    element.removeClass(hide);
                });
                _this.$rootScope.$on("$stateChangeSuccess", addClass);
                _this.$rootScope.$on("$stateNotFound", addClass);
                _this.$rootScope.$on("$stateChangeError", addClass);
                _this.$rootScope.$on("state-change-start-prevent", function () {
                    _this.$timeout(addClass, 1);
                });
                function addClass() {
                    element.addClass(hide);
                }
            };
        }
        NgSpinnerBar.factory = function () {
            var directive = function ($rootScope, $timeout) {
                return new NgSpinnerBar($rootScope, $timeout);
            };
            directive["$inject"] = ["$rootScope", "$timeout"];
            return directive;
        };
        return NgSpinnerBar;
    }());
    angular
        .module("app")
        .directive("ngSpinnerBar", NgSpinnerBar.factory());
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var ResManager = (function () {
        function ResManager(ajaxService) {
            this.ajaxService = ajaxService;
            return this;
        }
        ResManager.prototype.get = function (value) {
            var result = '';
            if (!value) {
                return result;
            }
            var resource = window["JsResources"][value];
            if (!resource) {
                this.ajaxService.logError('missing resource', value);
                return result;
            }
            return resource;
        };
        ResManager.$inject = ["ajaxService2"];
        return ResManager;
    }());
    angular.module("app").service("resManager", ResManager);
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var SbScroll = (function () {
        function SbScroll($compile, $mdMedia) {
            var _this = this;
            this.$compile = $compile;
            this.$mdMedia = $mdMedia;
            this.restrict = 'A';
            this.terminal = true;
            this.priority = 1000;
            this.link = function (scope, element, attribute) {
                element.removeAttr('sb-scroll');
                if (_this.$mdMedia('xs')) {
                    element.removeAttr('container');
                }
                if (Modernizr.cssscrollbar) {
                    element.removeAttr("ng-scrollbars-config ng-scrollbars-paging-function ng-scrollbars");
                    _this.$compile(element)(scope);
                    return;
                }
                element.removeAttr("srph-infinite-scroll");
                _this.$compile(element)(scope);
            };
        }
        SbScroll.factory = function () {
            var directive = function ($compile, $mdMedia) {
                return new SbScroll($compile, $mdMedia);
            };
            directive["$inject"] = ["$compile", "$mdMedia"];
            return directive;
        };
        return SbScroll;
    }());
    angular
        .module("app")
        .directive("sbScroll", SbScroll.factory());
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var ChatScrollButtom = (function () {
        function ChatScrollButtom($mdMedia) {
            var _this = this;
            this.$mdMedia = $mdMedia;
            this.link = function (scope, element, attribute) {
                scope.$on('chat-scroll', function () {
                    if (_this.$mdMedia('xs')) {
                        window.scrollTo(0, document.body.scrollHeight);
                    }
                    if (Modernizr.cssscrollbar) {
                        element[0].scrollTop = element[0].scrollHeight;
                        return;
                    }
                    scope["c"].updateScrollbar('scrollTo', 'bottom', { scrollInertia: 0, timeout: 100 });
                });
            };
        }
        ChatScrollButtom.factory = function () {
            var directive = function ($mdMedia) {
                return new ChatScrollButtom($mdMedia);
            };
            directive["$inject"] = ["$mdMedia"];
            return directive;
        };
        return ChatScrollButtom;
    }());
    angular
        .module("app")
        .directive("chatScrollButtom", ChatScrollButtom.factory());
})(app || (app = {}));
var app;
(function (app) {
    "use strict";
    var timeInterval = 900000;
    var VerionChecker = (function () {
        function VerionChecker($http, cacheFactory, $mdToast, resManager, $interval) {
            var _this = this;
            this.$http = $http;
            this.cacheFactory = cacheFactory;
            this.$mdToast = $mdToast;
            this.resManager = resManager;
            this.$interval = $interval;
            if (document.readyState === "complete") {
                $interval(this.checkVersion, timeInterval);
            }
            else {
                window.addEventListener("load", function () {
                    _this.checkVersion();
                    $interval(function () {
                        _this.checkVersion();
                    }, timeInterval);
                }, false);
            }
        }
        VerionChecker.prototype.checkVersion = function () {
            var _this = this;
            this.$http.get("/home/version/").then(function (response) {
                var retVal = response.data;
                if (retVal.success) {
                    if (window["version"] === retVal.payload) {
                        return;
                    }
                    _this.cacheFactory.clearAll();
                    var toast = _this.$mdToast.simple().textContent(_this.resManager.get("spitballUpdate")).action(_this.resManager.get("dialogOk"))
                        .highlightAction(false)
                        .position("top");
                    _this.$mdToast.show(toast).then(function () {
                        window.location.reload(true);
                    });
                }
            });
        };
        VerionChecker.factory = function () {
            var factory = function ($http, cacheFactory, $mdToast, resManager, $interval) {
                return new VerionChecker($http, cacheFactory, $mdToast, resManager, $interval);
            };
            factory["$inject"] = ["$http", "CacheFactory", "$mdToast", "resManager", "$interval"];
            return factory;
        };
        return VerionChecker;
    }());
    angular.module("app").run(VerionChecker.factory());
})(app || (app = {}));
(function () {
    'use strict';
    angular.module('app').run(intercom);
    intercom.$inject = ['userDetailsFactory', '$rootScope', '$mdMenu'];
    function intercom(userDetailsFactory, $rootScope, $mdMenu) {
        var started = false;
        userDetailsFactory.init().then(function (data) {
            start(data);
        });
        function start(data) {
            if (started) {
                return;
            }
            started = true;
            if (data.id) {
                Intercom('boot', {
                    app_id: "njmpgayv",
                    name: data.name,
                    email: data.email,
                    created_at: Math.round(data.createTime.getTime() / 1000),
                    user_id: data.id,
                    user_image: data.image,
                    university_id: data.university.id,
                    university_name: data.university.name,
                    reputation: data.score,
                    language: data.culture,
                    university_country: data.university.country,
                    widget: {
                        activator: '#Intercom'
                    }
                });
                Intercom('onActivatorClick', function () {
                    $mdMenu.hide();
                });
            }
        }
        function stop() {
            started = false;
            Intercom('shutdown');
        }
    }
})();
