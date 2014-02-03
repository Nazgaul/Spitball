(function (cd, ko, dataContext, $) {
    "use strict";

    if (window.scriptLoaded.isLoaded('mLc')) {
        return;
    }

    var elem = document.getElementById('libraryChoose');
    if (elem) {
        ko.applyBindings(new UniversityChooseViewModel(), elem);
    }
   // cd.loadModel('libraryChoose', 'LibraryContext', registerKOLibrary);

    //function registerKOLibrary(token) {
    //    var $libraryChoose = $('#libraryChoose');
    //    if (!$libraryChoose.length) {
    //        return;
    //    }
    //    ko.applyBindings(new UniversityChooseViewModel(), $libraryChoose[0]);
    //}

    function UniversityChooseViewModel() {
        var self = this, firstTime = true, $uniSelect = $('#uni_search'),
        $libraryChoose = $('#libraryChoose'),
           $uniList = $('#uniList'),
           page = 0,
           loading = false,
           havemoreData = true;

        self.list = ko.observableArray([]);
        self.country = ko.observable($('#lib_country').data('country'));
        self.countryChange = function (viewmodel, event) {
            self.list([]);
            populateData();
        };
        function University(data) {
            var that = this;
            data = data || {};

            that.name = data.Name;
            that.image = data.Image;
            that.id = data.Uid;
            that.membersCount = data.MemberCount;
            that.nCode = data.NeedCode;
        }


       // cd.pubsub.subscribe('lib_uni', function (e, data) {
            populateData();
            if (firstTime) {
                firstTime = false;
                $('#mLoading').hide();
                registerEvent();
            }
        //});


        function populateData() {

            var initData = $libraryChoose.data('data');
            if (initData) {
                generateModel(initData);
                $libraryChoose.data('data','').removeAttr('data-data');
                return;
            }

            loading = true;
            dataContext.university({
                data: { term: $uniSelect.val(), page: page, country: self.country()},
                success: function (data) {
                    if (!data.length) {
                        havemoreData = false;
                    }
                    generateModel(data);
                    loading = false;
                }
            });
            function generateModel(data) {
               // cd.pubsub.publish('lib_choose_load');                
                self.list.push.apply(self.list, $.map(data, function (i) { return new University(i); }));
            }
        }

        //var request = true;
        //self.newUni = function () {
        //    var $addSchoolDialog = $('#addSchoolDialog');
        //    if (!$addSchoolDialog.length && request) {
        //        request = false;
        //        dataContext.universityPopUp({
        //            success: function (data) {
        //                $libraryChoose.append(data).find('input[type=text]:first').focus();
        //                registerPopEvent();
        //            }
        //        });
        //    }
        //    else {
        //        $addSchoolDialog.show().find('input[type=text]:first').focus();
        //    }
        //};
        registerPopEvent();
        function registerPopEvent() {
            $('#settingsPanelOpen').hide();
            $('#searchMenu').hide();
            
            var $addSchoolDialog = $('#addSchool');
            //$addSchoolDialog.find('.closeDialog,.cancel').click(function () {
            //    if ($addSchoolDialog.find('.requestSent').is(':visible')) {
            //        $addSchoolDialog.find('.addSchool').toggle();
            //    }
            //    cd.resetForm($addSchoolDialog.find('form'));
            //    $addSchoolDialog.hide();

            //});

            $addSchoolDialog.find('form').submit(function (e) {
                e.preventDefault();
                var $form = $(this);
                if (!$form.valid || $form.valid()) {
                    dataContext.newUniversity({
                        data: $form.serializeArray(),
                        success: function () {
                            $addSchoolDialog.find('.addSchoolContent').toggle();
                        },
                        error: function (msg) {
                            cd.resetErrors($form);
                            cd.displayErrors($form, msg);
                        }

                    });
                }
            });
        }
        var request2 = true;
        function needCodePopUp(universityId) {
            var $libEnterCode = $('#libEnterCode');
            if (!$libEnterCode.length && request2) {
                request2 = false;
                dataContext.universityEnterCode({
                    data: { uid: universityId },
                    success: function (data) {
                        $libraryChoose.append(data);
                        registerneedCodePopUpEvent();
                    }
                });
            }
            else {
                $libEnterCode.show().find('.inputText').focus();
            }

            function registerneedCodePopUpEvent() {
                var $libEnterCode = $('#libEnterCode'), codeSubmit = document.getElementById('codeSubmit');

                $libEnterCode.find('form').submit(function (e) {
                    e.preventDefault();
                    var $form = $(this);
                    if (!$form.valid || $form.valid()) {
                        cd.pubsub.publish('clear_cache');
                        dataContext.updateUniversity({
                            data: $form.serializeArray(),
                            success: function () {
                                window.location.href = '/dashboard';
                            },
                            error: function () {
                                cd.notification('unspecified error');
                            }
                        });
                    }
                });
                $('#insertCode').keyup(function () {
                    if (this.value === '') {
                        codeSubmit.setAttribute('disabled', 'disabled');
                        return;
                    }
                    codeSubmit.removeAttribute('disabled');

                });
                $libEnterCode.find('.closeDialog,.cancel').click(function () {
                    $libEnterCode.hide();

                });
            }
        }
        self.uniSelect = function (uni) {
            //this can only be Netanya for now
            if (uni.nCode) {

                needCodePopUp(uni.id);

                return;
            }
            cd.pubsub.publish('clear_cache');
            dataContext.updateUniversity({
                data: { UniversityId: uni.id },
                success: function () {
                    location.href = '/dashboard';
                },
                error: function () {
                    cd.notification('unspecified error');
                }
            });
        };

        function registerEvent() {
            var timer = 0;
            $uniSelect.keydown(function (e) {
                clearTimeout(timer);
                timer = setTimeout(function () {
                    self.list([]);
                    havemoreData = true;
                    page = 0;
                    populateData();
                }, 300);

            });
            $(document).scroll(function () {
                if ($(document).scrollTop() >= $(document).height() - window.innerHeight) {                    
                    if (havemoreData && !loading) {
                        page++;
                        populateData();
                    }

                }
            });
            document.getElementById('settingsPanelOpen').onclick = function (e) {
                return false;

            };
        }
    }
})(cd, ko, cd.data, jQuery);