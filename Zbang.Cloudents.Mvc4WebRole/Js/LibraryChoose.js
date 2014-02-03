(function (cd, $, dataContext, ko, analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('lc')) {
        return;
    }

    cd.loadModel('libraryChoose', 'LibraryContext', registerKOLibrary);

    function registerKOLibrary(token) {
        var $libraryChoose = $('#libraryChoose');
        if (!$libraryChoose.length) {
            return;
        }
        ko.applyBindings(new UniversityChooseViewModel(), $libraryChoose[0]);
    }

    function UniversityChooseViewModel() {

        var self = this,
            //firstTime = true,
            $uniSelect = $('#uni_search'),
         $libraryChoose = $('#libraryChoose'),
            $uniList = $('#uniList'),
            page = 0,

            loading = false,
            havemoreData = true;

        self.list = ko.observableArray([]);
        self.country = ko.observable($('#sCountry').data('country'));

        var haveUniversity = $libraryChoose.data('haveuniversity');
        if (!haveUniversity) {
            $('.siteHeader').find('a').not('#logOut').click(function () {
                return false;
            });
            $('.siteHeader').find('button').attr('disabled', 'disabled');
        }

        function University(data) {
            var that = this;
            data = data || {};

            that.name = data.Name;
            that.image = data.Image;
            that.id = data.Uid;
            that.membersCount = data.MemberCount;
            that.nCode = data.NeedCode;
        }


        //cd.pubsub.subscribe('lib_uni', function (e, data) {
        populateData();
        //if (firstTime) {
        //    firstTime = false;
        registerEvent();
        //}
        //});


        function populateData() {

            var initData = $libraryChoose.data('data');
            if (initData) {
                generateModel(initData);
                $libraryChoose.removeAttr('data-data').data('data', '');
                return;
            }

            loading = true;
            var term;
            if (Modernizr.input.placeholder) {
                term = $uniSelect.val();
            } else {
                if ($uniSelect.val() === $uniSelect.attr('placeholder')){
                    term = '';
                } else {
                    term = $uniSelect.val();
                }
            }
            dataContext.university({
                data: { term: term, page: page, country: self.country() },
                success: function (data) {
                    if (page === 0) {
                        self.list([]);
                    }
                    if (!data.length) {
                        havemoreData = false;
                    }
                    generateModel(data);
                    loading = false;
                }
            });
            function generateModel(data, page) {
                var mappeddata = $.map(data, function (i) { return new University(i); });
                if (page === 0) {
                    self.list(mappeddata);
                    return;
                }
                //cd.pubsub.publish('lib_choose_load');
                self.list.push.apply(self.list, mappeddata);

            }

        }


        var request = true, request2 = true, INPUT_TEXT = 'input[type=text]:first';
        self.newUni = function () {
            var $addSchoolDialog = $('#addSchoolDialog');
            if (!$addSchoolDialog.length && request) {
                request = false;
                dataContext.universityPopUp({
                    success: function (data) {
                        $libraryChoose.append(data).find(INPUT_TEXT).focus();
                        registerPopEvent();
                    }
                });
            }
            else {
                $addSchoolDialog.show().find(INPUT_TEXT).focus();
            }
        };

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
                $libEnterCode.show().find(INPUT_TEXT).focus();
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
                            error: function (msg) {
                                cd.displayErrors($form, msg);
                                //cd.notification(msg);
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

        function registerPopEvent() {
            var $addSchoolDialog = $('#addSchoolDialog');
            $addSchoolDialog.find('.closeDialog,.cancel').click(function () {
                if ($addSchoolDialog.find('.requestSent').is(':visible')) {
                    $addSchoolDialog.find('.addSchool').toggle();
                }
                cd.resetForm($addSchoolDialog.find('form'));
                $addSchoolDialog.hide();

            });

            $addSchoolDialog.find('form').submit(function (e) {
                e.preventDefault();
                var $form = $(this);
                if (!$form.valid || $form.valid()) {
                    dataContext.newUniversity({
                        data: $form.serializeArray(),
                        success: function () {
                            $addSchoolDialog.find('.addSchool').toggle();
                        },
                        error: function (msg) {
                            cd.resetErrors($form);
                            cd.displayErrors($form, msg);
                        }

                    });
                }
            });
        }
        var userNotSelected = true;
        self.uniSelect = function (uni) {
            analytics.setLibrary(uni.name);
            //this can only be Netanya for now
            if (uni.nCode) {
                analytics.trackEvent('Library Choose', 'Code', uni.id);
                needCodePopUp(uni.id);

                return;
            }
            if (userNotSelected) {
                userNotSelected = false;
                var load = cd.renderLoading($('#uniList'));
                cd.pubsub.publish('clear_cache');
                dataContext.updateUniversity({
                    data: [{ name: 'UniversityId', value: uni.id }],
                    success: function () {
                        window.location.href = '/dashboard';
                        load();
                    },
                    error: function () {
                        userNotSelected = true;
                        cd.notification('unspecified error');
                        load();
                    }
                });
            }
        };

        function registerEvent() {
            //cd.loader.registerFacebook();
            //$('#logOut').click(function (e) {
            //    cd.userLogout(e);
            //});
            var timer = 0, $sCountry = $('#sCountry'), $countryList = $('#countryList'), countries = $countryList.find('li');
            $uniSelect.keydown(function (e) {
                clearTimeout(timer);
                timer = setTimeout(function () {

                    havemoreData = true;
                    page = 0;
                    populateData();
                }, 300);

            });
            $uniList.scroll(function () {
                if ($uniList.scrollTop() >= $uniList[0].scrollHeight - $uniList.height() - 100) {
                    if (havemoreData && !loading) {
                        page++;
                        populateData();
                    }

                }
            });

            $sCountry.text($('[data-value="' + self.country() + '"]:first').text());
            cd.menu($sCountry, $countryList, function () {
                var $innerListItem = $('[data-value="' + self.country() + '"]');
                scrollToElement($innerListItem);
            });
            $countryList.on('click', 'li', function (e) {
                self.country(e.target.getAttribute('data-value'));
                $sCountry.text(e.target.textContent);
                havemoreData = true;
                page = 0;
                populateData();

            });
            countries.focus(function () {
                $(this).addClass('focus');
            })
            .blur(function () {
                $(this).removeClass('focus');
            })
            .mouseover(function (e) {
                countries.blur();
                $(this).focus();

            });

            $(document).keydown(function (e) {

                if (!$countryList.is(':visible')) {
                    return;
                }

                var s = String.fromCharCode(e.keyCode);
                if (/[a-zA-Z]/.test(s))
                    scrollToElement($countryList.find('li:startsWith("' + s + '")').first().focus());

            });
            function scrollToElement(elem) {
                $countryList.scrollTop($countryList.scrollTop() + elem.position().top
                        - $countryList.height());
            }


        }
    }

})(cd, jQuery, cd.data, ko, cd.analytics);