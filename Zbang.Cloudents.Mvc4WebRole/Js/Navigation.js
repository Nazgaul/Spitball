(function ($, cd, pubsub, dataContext, analytics) {
    if (window.scriptLoaded.isLoaded('nav')) {
        return;
    }

    var firstLoad = true, waitingForPageLoad = true;

    var privateLocation = {
        title: document.title,
        url: location.pathname.replace(/[\/\/]+/, '/') + location.search
    };
    //historyNav;
    //cd.historyManager = {
    //    save: function () {
    //        var data = {}
    //        this.remove();
    //        data.history = historyNav;
    //        var now = new Date(),
    //        later = now.setHours(now.getHours() + 1);
    //        data.lastpage = location.pathname + location.search; //remove querystring
    //        data.ttl = later;
    //        cd.localStorageWrapper.setItem('history', JSON.stringify(data));
    //    },
    //    get: function () {
    //        var data = JSON.parse(cd.localStorageWrapper.getItem('history')) || { ttl: 0, lastpage: '' };
    //        var now = new Date();
    //        privateLocation.url = this.removeQueryString(privateLocation.url);
    //        if (data.ttl < now.getTime()) {
    //            this.remove();
    //            historyNav = [cd.clone(privateLocation)];
    //            if (history.replaceState) {
    //                history.replaceState(privateLocation.url, '', privateLocation.url);
    //            }
    //            return;
    //        }

    //        if (data.lastpage !== location.pathname + location.search) { //remove querystring) {
    //            this.remove();
    //            historyNav = [cd.clone(privateLocation)];
    //            return;
    //        }

    //        if (!data.history) {
    //            return;
    //        }

    //        historyNav = data.history;

    //        var lastPage = historyNav.pop();
    //        if (lastPage.url === "") {
    //            this.remove();
    //            return;
    //        }
    //        if (privateLocation.url.indexOf('search') === -1) {
    //            privateLocation.url = lastPage.url;
    //        } else {
    //            privateLocation.url = this.removeQueryString(lastPage.url);
    //        }

    //        if (window.history.replaceState) {
    //            history.replaceState(lastPage.url, '', lastPage.url);
    //        }
    //        historyNav.push(lastPage);

    //    },
    //    remove: function () {
    //        cd.localStorageWrapper.removeItem('history');
    //    },
    //    removeQueryString: function (path) {
    //        if (path.indexOf('?') === -1) {
    //            return path;
    //        }

    //        return path.split('?')[0];
    //    }
    //}

    //cd.historyManager.get();

    //HTMLElement.prototype.on = function (event, attr, func) {
    //    /// <summary></summary>
    //    /// <param name="event" type="Object"></param>
    //    /// <param name="attr" type="Object"></param>
    //    /// <param name="func" type="Object"></param>

    //    this.addEventListener(event, function (e) {
    //        /// <summary></summary>
    //        /// <param name="e" type="Event"></param>
    //        var obj = e.target;
    //        do {
    //            obj = obj.parentElement;
    //        } while (obj);

    //    }, false);
    //}

    //replace animation if ie9
    if (!Modernizr.cssanimations) {
        var loader = document.getElementById('mLoading');
        var gifLoader = document.createElement('div');
        gifLoader.className = 'loadingGif';
        gifLoader.id = 'mLoading';
        loader.parentElement.insertBefore(gifLoader, loader);
        loader.parentElement.removeChild(loader);
    }

    $(document).on('click', '[data-navigation]', function (e) {
        /// <summary>Handle the application navigation</summary>
        /// <param name="e" type="Event"></param>
        if (this.getAttribute('data-navigation').toLowerCase() === 'link') {
            return true;
        }

        e.preventDefault();


        if (e.ctrlKey) {
            window.open(this.href, '_blank');
            return;
        }

        privateLocation.url = this.pathname + (this.search || '');
        if (location.hash) {
            location.hash = '';
        }
        //historyNav[historyNav.length - 1].title = document.title === 'Cloudents' ? 'Dashboard' : document.title;
        changeHistoryState();
        locationChanged(privateLocation.url, $(this).data('d'));


    });

    $(function () {
        // we want the user with no pop ups if he reload the page/back
        if (location.hash) {
            location.hash = '';
        }

        //privateLocation.title = document.title === 'Cloudents' ? 'Dashboard' : document.title;;
        locationChanged();
    });
    pubsub.subscribe('nav', function (url) {
        if (url === '/') {
            url = '/dashboard/';
        }
        privateLocation.url = url;
        // mobile user create box redirect doesnt remove pop up
        if (location.hash) {
            location.hash = '';

        }
        //historyNav[historyNav.length - 1].title = document.title === 'Cloudents' ? 'Dashboard' : document.title;
        changeHistoryState();

        locationChanged();
    });

    //window.onhashchange = function () {
    //    try {
    //        window.scrollTo(0, 1);
    //    }
    //    catch (err) {
    //        console.log(err.message);
    //    }
    //    //using to put pop up dialog to with css target
    //    var hash = location.hash.toLowerCase();
    //    if (hash.charAt(0) === '#') {
    //        hash = hash.substr(1);
    //    }
    //    pubsub.publish('nav_hash', hash); //mobile
    //    pubsub.publish('nav_hash_' + hash);
    //};
    //window.onpopstate = function (e) {
    //    if (e.state) {
    //        if (privateLocation.url === removeStartingSlash(location.pathname)) {
    //            return;
    //        }
    //        if (historyNav[historyNav.length - 2] && historyNav[historyNav.length - 2].url) {
    //            pubsub.publish('nav', historyNav[historyNav.length - 2].url);
    //        }
    //    }
    //};

    function changeHistoryState() {
        firstLoad = false;

        if (window.history && window.history.pushState) {
            if (privateLocation.url && privateLocation.url.charAt(0) !== '/') {
                privateLocation.url = '/' + privateLocation.url;
            }
            if (privateLocation.url && privateLocation.url.slice(-1) !== '/') {
                if (privateLocation.url.indexOf('?') === -1) {
                    privateLocation.url = privateLocation.url + '/';
                }
            }
            window.history.pushState(privateLocation.url, '', privateLocation.url);
        }
        var clonedLocation = cd.clone(privateLocation);
        if (clonedLocation.url.lastIndexOf('?') > -1) {
            if (clonedLocation.url.indexOf('search') === -1) {// we want to keep query string for search page
                clonedLocation.url = clonedLocation.url.split('?')[0];
            }
        }

        //historyNav.push(clonedLocation);
        //save history to local storage
        cd.historyManager.save();

    }

    function locationChanged(prevLocation, extData) {
        var firstLevel = cd.getParameterFromUrl(0).toLowerCase(),
            secondLevel = cd.getParameterFromUrl(1);
        if (firstLevel === 'course') {
            firstLevel = 'box';
        }
        if (!changecontext(firstLevel, secondLevel, extData)) {
            return;
        }
        switch (firstLevel) {
            case 'library':
                if (!cd.register()) {
                    location.href = '/account';
                }
                libraryContext();
                break;
            case 'dashboard':
                if (!cd.register()) {
                    location.href = '/account';
                }
                dashboardContext();
                break;
            case 'account':
                accountSettingsContext();
                break;
            case 'box':
            case 'course':
                boxContext(prevLocation);
                break;
            case 'item':
                itemContext();
                break;
            case 'invite':
                if (!cd.register()) {
                    location.href = '/account';
                }
                inviteContext(extData);
                break;
            case 'user':
                if (!cd.register()) {
                    location.href = '/account';
                }
                userContext();
                break;
            case 'search':
                if (!cd.register()) {
                    location.href = '/account';
                }
                searchContext();
                break;
            case 'quiz':
                quizContext();
            default:
                break;
        }

    }
    function userContext() {
        pubsub.publish('user');
    }

    function quizContext() {

        pubsub.publish('quiz', { id: cd.getParameterFromUrl(4) });
    }

    function searchContext() {
        pubsub.publish('search');
    }

    function inviteContext(data) {
        pubsub.publish('invite', data);

    }
    //function itemContext() {
    //    pubsub.publish('item', { boxid: cd.getParameterFromUrl(2), id: cd.getParameterFromUrl(4) });
    //}
    function boxContext(prevLocation) {
        var secondLevel = cd.getParameterFromUrl(2);
        if (secondLevel.toLowerCase() === 'settings') { //mobile
            return;
        }
        pubsub.publish('box', { id: secondLevel, prevLocation: prevLocation });
    }
    function accountSettingsContext() {
        pubsub.publish('accountSettings_load');
    }
    function dashboardContext() {
        //var secondLevel = getParameterFromUrl(1) || '';
        //switch (secondLevel.toLowerCase()) {
        //    case 'search':
        //        pubsub.publish('dash_search', { query: getParameterFromUrl(2) });
        //        break;
        //    default:
        //        pubsub.publish('dash_boxes');
        //        break;
        //}
        showPage($('#dashboard'));
        pubsub.publish('dash_boxes');
    }
    function libraryContext() {
        var secondLevel = cd.getParameterFromUrl(1) || '';
        //, universityExists = $('#LibraryContent').length;
        switch (secondLevel.toLowerCase()) {
            //case 'search':
            //    pubsub.publish('lib_search', { query: getParameterFromUrl(2) });
            //    break;
            case 'choose':
                break;

            default:
                pubsub.publish('lib_nodes', { id: secondLevel, name: cd.getParameterFromUrl(2) });
                break;
        }
    }


    function removeStartingSlash(param) {
        if (param.charAt(0) === '/') {
            return param.substr(1);
        }
        return param;
    }

    function changecontext(elem, secondLvl, extData) {
        var jElem = $('#' + elem);

        if (jElem.is(":visible")) {
            return true;
        }
        if (jElem.length) {
            pubsub.publish('hidePage');
            return true;
        }
        else {
            pubsub.publish('hidePage');
            var main = document.getElementById('main');
            switch (elem) {
                case 'account':
                    dataContext.accountMp({
                        success: function (html) {
                            main.insertAdjacentHTML('beforeend', html);
                            pubsub.publish('AccountContext', null, accountSettingsContext);

                        },
                        error: function () {
                            location.reload();
                        }
                    });
                    break;
                case 'library':
                    if (secondLvl.toLowerCase() === 'choose') {
                        break;
                    }
                    dataContext.libraryMp({
                        data: { LibId: secondLvl },
                        success: function (html) {
                            main.insertAdjacentHTML('beforeend', html);
                            pubsub.publish('LibraryContext', null, libraryContext);
                        },
                        error: function () {
                            location.reload();
                        }
                    });
                    break;
                case 'dashboard':
                    dataContext.dashBoardMp({
                        success: function (html) {
                            //var el = angular.element(html);
                            //var result = cd.$compile(el);
                            //$(main).append(el);
                            //result(cd.$scope.main);

                            main.insertAdjacentHTML('beforeend', html);
                            pubsub.publish('DashboardContext', null, dashboardContext);
                        },
                        error: function () {
                            location.reload();
                        }
                    });
                    break;
                case 'box':
                    dataContext.boxMp({
                        url: '/' + privateLocation.url,
                        //data: { boxUid: getParameterFromUrl(1) },
                        success: function (html) {
                            main.insertAdjacentHTML('beforeend', html);
                            pubsub.publish('BoxContext', null, boxContext);
                        },
                        error: function () {
                            location.reload();
                        }
                    });
                    break;
                case 'item':
                    dataContext.itemMp({
                        data: { boxUid: cd.getParameterFromUrl(2), itemId: cd.getParameterFromUrl(4), uniName: cd.getParameterFromUrl(1) },
                        success: function (html) {
                            main.insertAdjacentHTML('beforeend', html);
                            pubsub.publish('ItemContext', null, itemContext);
                        },
                        error: function () {
                            location.reload();
                        }
                    });
                    break;
                case 'invite':
                    dataContext.inviteMp({
                        data: { boxid: cd.getParameterFromUrl(1) },
                        success: function (html) {
                            main.insertAdjacentHTML('beforeend', html);
                            pubsub.publish('InviteContext', null, function () {
                                inviteContext(extData);
                            });
                        },
                        error: function () {
                            location.reload();
                        }
                    });
                    break;
                case 'user':
                    dataContext.userMp({
                        data: { userId: cd.getParameterFromUrl(1), userName: cd.getParameterFromUrl(2) },
                        success: function (html) {
                            main.insertAdjacentHTML('beforeend', html);
                            pubsub.publish('UserContext', null, function () {
                                userContext(extData);
                            });
                        },
                        error: function () {
                            location.reload();
                        }
                    });
                    break;
                case 'search':
                    dataContext.searchMp({
                        data: { q: cd.getParameterByNameFromString('q', historyNav[historyNav.length - 1].url) },
                        success: function (html) {
                            main.insertAdjacentHTML('beforeend', html);
                            pubsub.publish('SearchContext', null, function () {
                                searchContext(extData);
                            });
                        },
                        error: function (err) {
                            console.log(err);
                        }
                    });
                    break;
                case 'quiz':
                    dataContext.quizMp({
                        data: { boxId: cd.getParameterFromUrl(2), quizId: cd.getParameterFromUrl(4), quizName: cd.getParameterFromUrl(5), universityName: cd.getParameterFromUrl(1), boxName: cd.getParameterFromUrl(3) },
                        success: function (html) {
                            main.insertAdjacentHTML('beforeend', html);
                            pubsub.publish('QuizContext', null, function () {
                                quizContext();
                            });
                        },
                        error: function () {
                            location.reload();
                        }
                    });
                    break;
                default:
                    //some error 
                    break;
            }
            return false;
        }

    }


    pubsub.subscribe('hidePage', function () {
        var currentElem = $('#' + cd.getParameterFromUrl(0)),
         elem = $('.page:visible').not(currentElem);
        if (elem.length) {
            elem.fadeOut(200, function () {
                if (!$('.page:visible').length) { // if the page didnt load we need to put the loading icon
                    $('#mLoading').show();
                }
                pubsub.publish(elem[0].id + 'clear');
            });

            $('[data-btn]').removeClass('active');

            waitingForPageLoad = true;
        }

        pubsub.publish('clearTooltip');
    });
    //pubsub.subscribe('lib_choose_load', function () {
    //    showPage($('#Library'));
    //});
    pubsub.subscribe('lib_load', function () {
        showPage($('#library'));
    });
    pubsub.subscribe('item_load', function () {
        showPage($('#item'));
    });
    pubsub.subscribe('dashboard_load', function () {
        showPage($('#dashboard'));
    });
    pubsub.subscribe('box_load', function () {
        showPage($('#box'));
    });
    pubsub.subscribe('accountSettings_load', function () {
        showPage($('#account'));
    });
    pubsub.subscribe('invite_load', function () {
        showPage($('#invite'));
    });
    pubsub.subscribe('user_load', function () {
        showPage($('#user'));
    });
    pubsub.subscribe('search_load', function () {
        showPage($('#search'));
    });
    pubsub.subscribe('quiz_load', function () {
        showPage($('#quiz'));
    });

    function showPage(d) {

        if (d.is(':visible')) {
            return;
        }
        if (!waitingForPageLoad) {
            return;
        }

        $('.page:visible').finish().hide(); // finish - we want the callback sometime both screen a show - not sure how
        //d.show();

        d[0].style.display = 'block';
        d[0].style.opacity = 1;
        analytics.trackPage(privateLocation.url);
        document.getElementById('mLoading').style.display = 'none';
        //$('#mLoading').hide();

        try {
            window.scrollTo(0, 0);
        }
        catch (err) {
            console.log(err.message);
        }

        cd.updateTimeActions();
        pubsub.publish(d[0].id.toLowerCase() + '_show');
    }

    pubsub.subscribe('scriptLoaded', function (d) {
        switch (d.toLowerCase()) {
            case 'item':
                itemContext();
                break;
        }
    });


    cd.location = function () {
        return location.origin + '/' + privateLocation.url;
    };
    cd.prevLinkData = function (type) {
        var history,
            index,
            found = false;
        if (type === 'box') {
            if (historyNav.length > 0) {
                for (var i = historyNav.length - 1; i >= 0 && !found; i--) {
                    history = cd.clone(historyNav[i]);
                    history.url = history.url.substring(1);
                    history.url = history.url.slice(0, history.url.indexOf('/'));
                    if (history.url.indexOf('dashboard') > -1 ||
                        history.url.indexOf('library') > -1 ||
                        history.url.indexOf('search') > -1 ||
                        history.url.indexOf('user') > -1) {
                        index = i;
                        found = true;

                    }
                }
            }
            if (!found) {
                return;
            }

            if (historyNav[index].url.charAt(0) !== '/') {
                historyNav[index].url = '/' + historyNav[index].url;
            }
            return historyNav[index];
        }
        if (type === 'item') {
            var backItem = historyNav[historyNav.length - 2];
            if (backItem) {
                if (backItem.url.indexOf('search') > -1) {
                    return backItem;
                }

                return false;
            }
            return false;
        }
    };



    //cd.getParameterFromUrl = function (i) {
    //    return getParameterFromUrl(i);
    //};

    cd.setTitle = function (title) {
        if (firstLoad) {
            return;
        }
        historyNav[historyNav.length - 1].title = title;
        document.title = title;
    }

})(jQuery, cd, cd.pubsub, cd.data, cd.analytics);