define('facebook', ['app'], function (app) {

    window.fbAsyncInit = function () {
        FB.init({
            appId: '450314258355338',
            status: true,
            cookie: true,
            xfbml: true,
            oauth: true
        });
        cd.pubsub.publish('fbInit');
    };

    var facebook = app.factory('Facebook',
        ['$document', '$q', '$window',
        function ($document, $q, $window) {
            (function (d) {
                var js, id = 'facebook-jssdk';
                if (d.getElementById(id)) {
                    return;
                }
                js = d.createElement('script');
                js.id = id;
                js.async = true;
                js.src = "//connect.facebook.net/en_US/all.js";
                d.getElementsByTagName('head')[0].appendChild(js);
            }(document));

            return {
                share: function (url, name, caption, description, picture) {
                    var defer = $q.defer();


                    url = url || $window.location.href;
                    FB.ui({
                        method: 'feed',
                        link: url,
                        name: name,
                        caption: caption,
                        description: description,
                        picture: location.origin + (picture || '/images/cloudents-share-FB.png'),
                        display: 'popup'
                    }, function (response) {
                        if (response && response.post_id) {
                            //analytics.trackSocial(url, 'share');
                            //cd.pubsub.publish('addPoints', { type: 'shareFb' });
                            var postId = response.post_id.split('_')[1]; //takes the post id from *user_id*_*post_id*
                            //cd.data.fbRep({                                
                            //    data: { postId: postId }
                            //});
                            defer.resolve();
                            return;
                        }
                        defer.reject();
                    });

                    return defer.promise;
                }
            }
        }
    ]);

    return facebook;
});



