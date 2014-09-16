//#region google
(function (cd, dataContext) {
    if (window.scriptLoaded.isLoaded('sc')) {
        return;
    }


    var clientId = '616796621727-o9vr11gtr5p9v2t18co7f7kjuu0plnum.apps.googleusercontent.com',
    apiKey = 'AIzaSyBqnR38dm9S2E-eQWRj-cTgup2kGA7lmlg',
    scopes = ['https://www.google.com/m8/feeds/contacts/default/full', 'https://www.googleapis.com/auth/drive.readonly'],
    access_token, contacts = [],/* isImmediate = true,*/ loaded = false;

    function checkAuth(isImmediate) {

        if (window.gapi !== undefined && window.gapi.client !== undefined) {
            gapi.auth.authorize({ client_id: clientId, scope: scopes, immediate: isImmediate }, function (authResult) {
                handleResult(authResult, isImmediate);
            });
        }
    }

    cd.pubsub.subscribe('gRegister', register);
    function register(isImmediate) {
        if (loaded) {

            checkAuth(isImmediate);
            return;
        }
        loaded = true;

        if (document.getElementById('jsGoogleContact')) {
            checkAuth(isImmediate);
            return;
        }

        var js = document.createElement('script');
        js.id = "jsGoogleContact";
        js.src = " https://apis.google.com/js/client.js";
        document.getElementsByTagName('head')[0].appendChild(js);
        var interval = window.setInterval(function () {
            if (window.gapi !== undefined && window.gapi.client !== undefined) {
                window.clearInterval(interval);
                gapi.client.setApiKey(apiKey);
                checkAuth(isImmediate);
            }
        }, 50);
    }

    cd.pubsub.subscribe('gGetContacts', getContacts);

    function getContacts() {
        if (contacts.length) { //no need to get contacts again
            cd.pubsub.publish('gContacts', contacts);
            return;
        }
        if (!access_token) {
            cd.pubsub.publish('gContacts', null);
            return;
        }
        //register
        //auth
        dataContext.googleFriends({
            data: { token: access_token },
            dataType: 'json',
            success: function (data) {
                var feed = JSON.parse(data).feed;
                for (var i = 0 ; i < feed.entry.length; i++) {
                    var contact = {}, entry = feed.entry[i];
                    if (entry.gd$email) {
                        contact.id = entry.gd$email[0].address;

                        if (entry.title.$t !== '') {
                            contact.name = cd.escapeHtmlChars(entry.title.$t);
                        } else {
                            contact.name = entry.gd$email[0].address;
                        }

                        contact.defaultImage = '/Images/user-gmail-pic.jpg';
                        if (entry.link[0].gd$etag) {
                            contact.userImage = decodeURIComponent(entry.link[0].href) + '&access_token=' + access_token;
                        } else {
                            contact.userImage = 'null';
                        }

                        contacts.push(contact);
                    }
                }

                contacts.sort(function (a, b) {
                    return cd.sortMembersByName(a.name, b.name);
                });
                cd.pubsub.publish('gContacts', contacts);
                cd.google.connected = true;
            },
            error: function () {
                //console.log(data);
            }
        });
    }

    function handleResult(authResult, isImmediate) {
        if (authResult && !authResult.error) {
            access_token = gapi.auth.getToken().access_token;
            cd.google.connected = true;
            cd.pubsub.publish('gAuthSuccess', isImmediate);
            return;
        }

        //   isImmediate = false;
        cd.pubsub.publish('gAuthFail');

    }

    cd.google = {
        register: register,
        connected: false
    };
}(cd, cd.data));
//#endregion

//#region facebook
(function (cd) {
    cd.loader.registerFacebook();
    var access_token, contacts = [];

    //cd.pubsub.subscribe('fbLogin', login);
    function login() { // we use as function instead of pubsub because ie block pop up
        FB.login(function (response) {
            if (response.status !== 'connected') {
                cd.pubsub.publish('fbLoginFail', response.authResponse);
                return;
            }
            if (!response.authResponse.accessToken) {
                cd.pubsub.publish('fbLoginFail', response.authResponse);
                return;
            }
            access_token = response.authResponse.accessToken;
            cd.pubsub.publish('fbLoggedIn', response.authResponse);
        });
    }
    cd.pubsub.subscribe('fbGetStatus', loginStatus);
    function loginStatus() {
        FB.getLoginStatus(function (response) {
            cd.pubsub.publish('fbStatus', response.status);
        });
    }

    cd.pubsub.subscribe('fbGetContacts', getContacts);
    function getContacts() {
        if (contacts.length) { //no need to get contacts again
            cd.pubsub.publish('fbContacts', contacts);
            return;
        }


        var friend;
        FB.api('/me/friends?fields=id,first_name,middle_name,last_name,gender,username,picture.height(64).width(64)', function (response) {
            for (var i = 0, l = response.data.length; i < l; i++) {
                if (!response.data) {
                    return;
                }
                friend = response.data[i];
                contacts.push({
                    id: friend.id,
                    firstname: cd.escapeHtmlChars(friend.first_name),
                    middlename: cd.escapeHtmlChars(friend.middle_name),
                    lastname: cd.escapeHtmlChars(friend.last_name),
                    name: friend.first_name + ' ' + (friend.middle_name ? friend.middle_name + ' ' : '') + friend.last_name,
                    userImage: 'null',
                    username: friend.username,
                    defaultImage: friend.picture.data.url,
                    gender: friend.gender === 'male' ? 1 : 0
                });
            }

            contacts.sort(function (a, b) {
                return cd.sortMembersByName(a.firstname + ' ' + (a.middlename ? a.middlename + ' ' : '') + a.lastname, b.firstname + ' ' + (b.middlename ? b.middlename + ' ' : '') + b.lastname);
            });

            cd.pubsub.publish('fbContacts', contacts);

            cd.facebook.connected = true;
        });
    }
    cd.facebook = {
        login: login,
        connected: false
    };
}(cd, cd.data));


//#endregion