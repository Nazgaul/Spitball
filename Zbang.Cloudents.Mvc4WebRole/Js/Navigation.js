﻿(function ($, cd, pubsub, dataContext, analytics) {
    if (window.scriptLoaded.isLoaded('nav')) {
        return;
    }

    cd.firstLoad = true;

    var privateLocation = {
        title: document.title,
        url: location.pathname.replace(/[\/\/]+/, '/')
    },
    historyNav;
    cd.historyManager = {
        save: function () {
            var data = {}
            this.remove();
            data.history = historyNav;
            var now = new Date();
            later = now.setHours(now.getHours() + 1);
            data.lastpage = getParameterFromUrl(1);
            data.ttl = later;
            localStorage.setItem('history', JSON.stringify(data));
        },
        get: function () {
            var data = JSON.parse(localStorage.getItem('history')) || { ttl: 0, lastpage: '' };
            var now = new Date();
            if (data.ttl < now.getTime()) {
                this.remove();
                historyNav = [cd.clone(privateLocation)];
                return;
            }

            if (data.lastpage !== getParameterFromUrl(1)) {
                this.remove();
                historyNav = [cd.clone(privateLocation)];
                return;
            }

            historyNav = data.history;
        },
        remove: function () {
            localStorage.removeItem('history');
        }
    }

    cd.historyManager.get();

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

    $(document).on('click', '[data-navigation]', function (e) {
        /// <summary>Handle the application navigation</summary>
        /// <param name="e" type="Event"></param>


        e.preventDefault();
        privateLocation.url = this.pathname;
        if (location.hash) {
            location.hash = '';
        }
        changeHistoryState();
        locationChanged(privateLocation.url, $(this).data('d'));
    });

    $(function () {
        // we want the user with no pop ups if he reload the page/back
        if (location.hash) {
            location.hash = '';
        }
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
        changeHistoryState();


        locationChanged();
    });

    window.onhashchange = function (e) {
        window.scrollTo(0, 1);
        //using to put pop up dialog to with css target
        var hash = location.hash.toLowerCase();
        if (hash.charAt(0) === '#') {
            hash = hash.substr(1);
        }
        pubsub.publish('nav_hash', hash); //mobile
        pubsub.publish('nav_hash_' + hash);
    };
    window.onpopstate = function (e) {
        if (e.state) {
            if (privateLocation.url === removeStartingSlash(location.pathname)) {
                return;
            }
            pubsub.publish('nav', location.pathname);
        }
    };

    function changeHistoryState() {
        cd.firstLoad = false;

        if (window.history && window.history.pushState) {
            if (privateLocation.url && privateLocation.url.charAt(0) !== '/') {
                privateLocation.url = '/' + privateLocation.url;
            }
            if (privateLocation.url && privateLocation.url.slice(-1) !== '/') {
                privateLocation.url = privateLocation.url + '/';
            }
            window.history.pushState(privateLocation.url, '', privateLocation.url);
        }
        historyNav.push(cd.clone(privateLocation));

        //save history to local storage
        cd.historyManager.save();

    }

    function locationChanged(prevLocation, extData) {
        var firstLevel = getParameterFromUrl(0).toLowerCase(),
            secondLevel = getParameterFromUrl(1);
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
            default:
                break;
        }


    }
    function userContext(data) {
        pubsub.publish('user');
    }

    function searchContext(data) {
        pubsub.publish('search');
    }

    function inviteContext(data) {
        pubsub.publish('invite', data);

    }
    function itemContext() {
        pubsub.publish('item', { boxid: getParameterFromUrl(2), id: getParameterFromUrl(4) });
    }
    function boxContext(prevLocation) {
        var secondLevel = getParameterFromUrl(2);
        if (secondLevel.toLowerCase() === 'settings') { //mobile
            return;
        }
        pubsub.publish('box', { id: secondLevel, prevLocation: prevLocation });
    }
    function accountSettingsContext() {
        pubsub.publish('accountSettings_load');
    }
    function dashboardContext() {
        var secondLevel = getParameterFromUrl(1) || '';
        switch (secondLevel.toLowerCase()) {
            case 'search':
                pubsub.publish('dash_search', { query: getParameterFromUrl(2) });
                break;


            default:
                pubsub.publish('dash_boxes');
                break;
        }
    }
    function libraryContext() {
        var secondLevel = getParameterFromUrl(1) || '';
        //, universityExists = $('#LibraryContent').length;
        switch (secondLevel.toLowerCase()) {
            //case 'search':
            //    pubsub.publish('lib_search', { query: getParameterFromUrl(2) });
            //    break;
            case 'choose':
                break;

            default:
                pubsub.publish('lib_nodes', { id: secondLevel, name: getParameterFromUrl(2) });
                break;
        }
    }

    function getParameterFromUrl(index) {
        privateLocation.url = removeStartingSlash(privateLocation.url);
        var pathArray = privateLocation.url.split('/');
        if (pathArray[index]) {
            return decodeURIComponent(pathArray[index]);
        }
        return '';
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
                        data: { boxUid: getParameterFromUrl(2), itemId: getParameterFromUrl(4), uniName: getParameterFromUrl(1) },
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
                        data: { boxid: getParameterFromUrl(1) },
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
                        data: { userId: getParameterFromUrl(1), userName: getParameterFromUrl(2) },
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
                        success: function (html) {
                            main.insertAdjacentHTML('beforeend', html);
                            pubsub.publish('SearchContext', null, function () {
                                searchContext(extData);
                            });
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
        var currentElem = $('#' + getParameterFromUrl(0)),
         elem = $('.page:visible').not(currentElem);
        if (elem.length) {
            elem.fadeOut(200, function () {
                if (!$('.page:visible').length) { // if the page didnt load we need to put the loading icon
                    $('#mLoading').show();
                }
                pubsub.publish(elem[0].id + 'clear');
            });

            $('[data-btn]').removeClass('active');
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

    function showPage(d) {
        if (d.is(':visible')) {
            return;
        }

        $('.page:visible').finish().hide(); // finish - we want the callback sometime both screen a show - not sure how
        //d.show();

        d[0].style.display = 'block';
        d[0].style.opacity = 1;
        analytics.trackPage(privateLocation.url);
        document.getElementById('mLoading').style.display = 'none';
        //$('#mLoading').hide();

        window.scrollTo(0, 0);
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

        if (type === 'box') {
            if (historyNav.length > 0) {
                for (var i = historyNav.length - 1; i >= 0; i--) {
                    if (historyNav[i].url.indexOf('dashboard') > -1 ||
                        historyNav[i].url.indexOf('library') > -1 ||
                        historyNav[i].url.indexOf('user') > -1) {
                        if (historyNav[i].url.charAt(0) !== '/') {
                            historyNav[i].url = '/' + historyNav[i].url;
                        }
                        return historyNav[i];
                    }
                }
            }
        }

        return historyNav[historyNav.length - 2];
    };

  

    cd.getParameterFromUrl = function (i) {
        return getParameterFromUrl(i);
    };

})(jQuery, cd, cd.pubsub, cd.data, cd.analytics);