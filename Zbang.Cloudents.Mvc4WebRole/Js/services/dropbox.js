define(['app'], function(app) {    
    var dropbox = app.factory('Dropbox', 
        ['$document','$q',
        function($document,$q){
            var dropboxLoaded = false;

            init();

            function init() {
                var js = $document[0].createElement('script');
                js.id = "dropboxjs";
                js.setAttribute('data-app-key', 'gppwajedn90rv81');
                js.src = "https://www.dropbox.com/static/api/1/dropins.js";
                $document[0].getElementsByTagName('head')[0].appendChild(js);

                js.onload = function (e) {
                    dropboxLoaded = true;
                }
            }

            return {
                choose: function () {
                    var defer = $q.defer();
                    Dropbox.choose({
                        success: function (files) {
                            defer.resolve(files);
                        },
                        error: function(){
                            defer.reject('dropbox choose error');
                        },
                        linkType: "direct",
                        multiselect: true
                    });

                    return defer.promise;
                }
            }
        }
    ]);

    return dropbox;
});          