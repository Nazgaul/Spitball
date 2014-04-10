(function (cd, $, cache, pubsub) {
    if (window.scriptLoaded.isLoaded('dc')) {
        return;
    }
    //this is data definision
    function definitionobj(data) {
        //data.comet = data.comet || false;

        this.url = data.url.slice(-1) === '/' ? data.url : data.url + '/';
        this.type = data.type || 'post';
        this.isJson = data.isJson || false;
        if (!$.isArray(data.cacheExpire)) {
            data.cacheExpire = [data.cacheExpire];
        }
        this.comet = data.comet;
        this.converters = data.converters;

    }
    var dashboard = "/Dashboard/", library = "/Library/", cBOX = "/Box/", share = "/Share/", cAccount = "/Account/", search = '/Search/',
        comment = "/Comment/", cITEM = "/Item/", get = 'get',
    definition = {};
    //#region definition

    definition.dashBoardMp = new definitionobj({ url: dashboard, type: get });
    definition.libraryMp = new definitionobj({ url: library, type: get });
    definition.boxMp = new definitionobj({ url: cBOX, type: get });
    definition.itemMp = new definitionobj({ url: cITEM, type: get });
    definition.inviteMp = new definitionobj({ url: '/invite', type: get });
    definition.userMp = new definitionobj({ url: '/user', type: get });
    definition.accountMp = new definitionobj({ url: cAccount + "Settings", type: get });
    definition.searchMp = new definitionobj({ url: search, type: get });
    definition.quizMp = new definitionobj({ url: '/Quiz/', type: get });

    definition.dashboard = new definitionobj({ url: dashboard + "BoxList", type: get });
    definition.createBox = new definitionobj({ url: dashboard + "Create" });

    definition.searchDD = new definitionobj({ url: search + 'Dropdown', type: get });
    definition.searchPage = new definitionobj({ url: search + 'Data', type: get });
    definition.searchOtherUnis = new definitionobj({ url: search + 'OtherUniversities', type: get });
    //definition.sDashboard = new definitionobj({ url: dashboard + 'Search', type: get });
    // definition.sideNode = new definitionobj({ url: dashboard + "Side", type: get });



    definition.getNotifications = new definitionobj({ url: share + "Notifications", type: get });
    //test
    definition.fbBoxInvite = new definitionobj({ url: share + 'InviteBoxFacebook', isJson: true });
    definition.fbInvite = new definitionobj({ url: share + 'InviteFacebook', isJson: true });
    definition.fbRep = new definitionobj({ url: share + 'Facebook' });

    definition.library = new definitionobj({ url: library + "Nodes", type: get });
    definition.university = new definitionobj({ url: library + "University", type: get });
    definition.universityPopUp = new definitionobj({ url: library + "NewUniversity", type: get });
    definition.universityEnterCode = new definitionobj({ url: library + "InsertCode", type: get });
    definition.newUniversity = new definitionobj({ url: library + "UniversityRequest" });
    definition.depList = new definitionobj({ url: library + "Departments", type: get });

    definition.createDepartment = new definitionobj({ url: library + "Create" });
    definition.createAcademicBox = new definitionobj({ url: library + "CreateBox" });
    definition.deleteNode = new definitionobj({ url: library + "DeleteNode" });
    definition.renameNode = new definitionobj({ url: library + "RenameNode" });

    definition.subscribeBox = new definitionobj({ url: share + "SubscribeToBox" });

    definition.updateUniversity = new definitionobj({ url: cAccount + "UpdateUniversity" });

    definition.getBox = new definitionobj({ url: cBOX + "Data", type: get });
    definition.getItems = new definitionobj({ url: cBOX + "Items", type: get });
    definition.boxComments = new definitionobj({ url: "/Comment", type: get });
    definition.commentAdd = new definitionobj({ url: comment + "Add", isJson: true });
    definition.commentRemove = new definitionobj({ url: comment + "Delete" });

    definition.addAnnotation = new definitionobj({ url: comment + "AddAnnotation", isJson: true });
    definition.replyAnnotation = new definitionobj({ url: comment + "ReplyAnnotation", isJson: true });
    definition.deleteAnnotation = new definitionobj({ url: comment + "DeleteAnnotation", isJson: true });
    definition.getAnnotation = new definitionobj({ url: comment + "Item", type: get });

    definition.removeItem = new definitionobj({ url: cITEM + "Delete" });
    definition.addLink = new definitionobj({ url: "/Upload/Link" });
    definition.dropBox = new definitionobj({ url: "/Upload/DropBox" });

    definition.inviteBox = new definitionobj({ url: share + "InviteBox", isJson: true });
    definition.message = new definitionobj({ url: share + "Message" });
    definition.inviteCloudents = new definitionobj({ url: share + "Invite", isJson: true });

    definition.addTab = new definitionobj({ url: cBOX + "CreateTab" });
    definition.boxMembers = new definitionobj({ url: cBOX + "Members", type: get });
    definition.removeUser = new definitionobj({ url: cBOX + "RemoveUser" });
    definition.removeBox2 = new definitionobj({ url: cBOX + "delete2" });
    definition.addItemToTab = new definitionobj({ url: cBOX + "AddItemToTab", isJson: true });

    definition.getItem = new definitionobj({
        url: cITEM + "Load", /*comet: true,*/ type: get, converters: {
            'text json': function (d) {
                return JSON.parse(d, cd.isoDateReviver);
            }
        }
    });
    definition.badItemPopUp = new definitionobj({ url: cITEM + "Flag" });
    definition.badItemRequest = new definitionobj({ url: cITEM + "FlagRequest" });
    definition.rateItem = new definitionobj({ url: cITEM + "Rate", type: 'post' });
    definition.getItemRate = new definitionobj({ url: cITEM + "Rate", type: get });
    definition.renameItem = new definitionobj({ url: cITEM + "Rename" });
    definition.preview = new definitionobj({ url: cITEM + "Preview", type: get });

    definition.updateBoxInfo = new definitionobj({ url: cBOX + "UpdateInfo" });
    definition.renameTab = new definitionobj({ url: cBOX + "RenameTab", isJson: true });
    definition.deleteTab = new definitionobj({ url: cBOX + "DeleteTab", isJson: true });

    definition.follow = new definitionobj({ url: share + "SubscribeToBox" });

    definition.changeNotification = new definitionobj({ url: cBOX + "ChangeNotification" });

    definition.getNotification = new definitionobj({ url: cBOX + "GetNotification", type: get });

    definition.notificationAsRead = new definitionobj({ url: share + "NotificationAsRead" });
    definition.notificationOld = new definitionobj({ url: share + "NotificationOld" });
    
    definition.firstTimeUpdate = new definitionobj({ url: "/Account/FirstTime" });

    definition.statistics = new definitionobj({ url: "/Home/Statistics", isJson: true, type: 'put' });

    definition.accountSettings = new definitionobj({ url: cAccount + "ChangeProfile" });
    definition.changeLanguage = new definitionobj({ url: cAccount + "ChangeLanguage", isJson: true });
    definition.changePassword = new definitionobj({ url: cAccount + "ChangePassword", isJson: true });
    definition.changeEmail = new definitionobj({ url: cAccount + "ChangeEmail", isJson: true });
    definition.submitCode = new definitionobj({ url: cAccount + "EnterCode" });

    definition.userNotification = new definitionobj({ url: "/User/Notification" });

    definition.addQuestion = new definitionobj({ url: "/QnA/AddQuestion" });
    definition.addAnswer = new definitionobj({ url: "/QnA/AddAnswer" });
    definition.getQuestions = new definitionobj({
        url: "/QnA", type: get, converters: {
            'text json': function (d) {
                return JSON.parse(d, cd.isoDateReviver);
            }
        }
    });
    definition.markAnswer = new definitionobj({ url: "/QnA/MarkAnswer" });
    definition.rateQuestion = new definitionobj({ url: "/QnA/RateQuestion" });
    definition.removefileQnA = new definitionobj({ url: "/QnA/RemoveFile" });
    definition.deleteQuestion = new definitionobj({ url: "/QnA/DeleteQuestion" });
    definition.deleteAnswer = new definitionobj({ url: "/QnA/DeleteAnswer" });
    definition.getFriends = new definitionobj({ url: '/User/Friends', type: get });

    definition.googleFriends = new definitionobj({ url: '/User/GoogleContacts' });

    definition.minProfile = new definitionobj({ url: '/User/MinProfile', type: get });

    definition.getUserpageBoxes = new definitionobj({ url: '/User/Boxes', type: get });
    definition.getUpMembers = new definitionobj({
        url: '/User/AdminFriends', type: get, converters: {
            'text json': function (d) {
                return JSON.parse(d, cd.isoDateReviver);
            }
        }
    });


    definition.newUpdates = new definitionobj({ url: '/User/Updates', type: get });
    definition.deleteUpdates = new definitionobj({ url: '/Box/DeleteUpdates'});

    definition.getUpMemberBoxes = new definitionobj({ url: '/User/AdminBoxes', type: get });
    definition.getUserPageActivity = new definitionobj({ url: '/User/Activity', type: get });
    definition.getUserPageInvites = new definitionobj({ url: '/User/OwnedInvites', type: get });


    definition.quizCreate = new definitionobj({ url: '/Quiz/Create' });
    definition.quizData = new definitionobj({url: '/Quiz/Data',type:get });
    definition.quizHTML = new definitionobj({ url: '/Quiz/CreateQuiz', type:get});
    definition.quizUpdate = new definitionobj({ url: '/Quiz/Update' });
    definition.quizDelete = new definitionobj({ url: '/Quiz/Delete' });
    definition.quizPublish = new definitionobj({ url: '/Quiz/Save' });
    definition.quizSave = new definitionobj({ url: '/Quiz/Delete' });
    definition.quizQCreate = new definitionobj({ url: '/Quiz/CreateQuestion' });
    definition.quizQUpdate = new definitionobj({ url: '/Quiz/UpdateQuestion' });
    definition.quizQDelete = new definitionobj({ url: '/Quiz/DeleteQuestion' });
    definition.quizACreate = new definitionobj({ url: '/Quiz/CreateAnswer' });
    definition.quizAUpdate = new definitionobj({ url: '/Quiz/UpdateAnswer' });
    definition.quizADelete = new definitionobj({ url: '/Quiz/DeleteAnswer' });

    //#endregion definition
    /**
 * Outline our webservices
 * @param {String} action - AJAX action to perform
 * @param {Object} payload - Javascript object containing AJAX method properties
 */
    var dataManager = function (action, payload) {

        var params = definition[action], d = $.Deferred();
        //params.error = params.error || function () { };
        var payload2 = $.extend({}, params, payload);
        var elem = cache.getFromCache(payload2.url, payload2.data);
        //var elem = cacheElement.GetItem(payload.url, payload.data);
        if (elem) {
            d.resolve(elem, true, payload.data);
            OnSuccess(elem, true, payload.data);
            if ($.isFunction(payload2.always)) {
                payload2.always();
            }
            return d.promise();
        }

        var ajaxPromise = AjaxWrapper(payload2, function (data, cache) {

            OnSuccess(data, cache, payload2.data);
        }, payload2.isJson);

        function OnSuccess(data, cacheTime, params) {
            if ($.isFunction(payload2.success)) {
                payload2.success(data, params);
            }

            if (typeof cacheTime === 'boolean') {
                return;
            }
            if (cacheTime) {
                cacheTime = parseInt(cacheTime, 10);// * cd.OneSecond;
            }
            else {
                cacheTime = cd.OneMinute * 5;
            }
            if (payload2.type === 'post') {
                pubsub.publish('clear_cache');
            }
            if (payload2.type === get) {
                pubsub.publish('add_cache', { key: payload2.url, params: payload2.data, value: data, ttl: cacheTime });
            }

        }
        return ajaxPromise;
        //return d.promise();
    };



    /**
     * Fires off the AJAX object with user defined payload information.
     * @param {Object} payload	- AJAX data options to bind to the jQuery object
     */
    var AjaxWrapper = function (options, success, isJson) {
        function buildKey(url, params) {
            params = params || '';
            return url + '_' + JSON.stringify(params);
        }

        var tt = new TrackTiming('ajax', buildKey(options.url, options.data));
        tt.startTime();
        var isjson = isJson || false;
        var ajaxParams = {
            url: options.url,
            type: options.type,
            data: isjson ? JSON.stringify(options.data) : options.data,
            contentType: isjson ? "application/json" : "application/x-www-form-urlencoded",
            headers: { 'X-Requested-With': "XMLHttpRequest" },
            converters: options.converters || $.ajaxSettings.converters,
            statusCode: {

                401: function () {
                    document.location.href = '/account?returnurl=' + encodeURIComponent(cd.location());
                },
                403: error,
                404: error,
                500: function () {
                    if ($.isFunction(options.error)) {
                        //error is alreay trigger. no need to transfer to 500
                        return;
                    }

                    location.href = '/error';
                }

            },
            beforeSend: function () {
                if ($.isFunction(options.beforeSend)) {
                    return options.beforeSend();
                }
            },
            success: function (data, textStatus, jqXHR) {

                var contentType = jqXHR.getResponseHeader('Content-Type') || '';
                var ajaxCache = jqXHR.getResponseHeader('Cd-Cache');
                if (contentType.indexOf("application/json") !== -1) {
                    if (!$.isPlainObject(data)) {
                        if (typeof options.error === 'function') {
                            options.error('not proper response');
                        }
                    }
                    if (data.Success || data.success) {
                        success(data.Payload || data.payload, ajaxCache);
                    } else {
                        if (typeof options.error === 'function') {
                            options.error(data.Payload || data.payload);
                        }
                    }
                }
                if (contentType.indexOf("text/html") !== -1) {
                    success(data, ajaxCache);
                }
                if (contentType.indexOf("application/x-javascript") !== -1) {
                    data();
                }

            },
            error: function () {
                if ($.isFunction(options.error)) {
                    options.error();
                    return;
                }
            },
            complete: function () {
                if ($.isFunction(options.always)) {
                    options.always();
                }
                tt.endTime().send();

            }
        };
        function error() {
            location.href = '/error';
        }
        return $.ajax(ajaxParams);
    };

    cd.data = cd.data || {};
    var d = cd.data;
    for (prop in definition) {
        (function (p) {
            d[p] = function (data) {
                return dataManager(p, data);
            }
        }(prop));
    }

})(window.cd = window.cd || {}, jQuery, cd.cache, cd.pubsub);