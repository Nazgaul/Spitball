(function (cd, ko, dataContext, $) {
    "use strict";

    if (window.scriptLoaded.isLoaded('mLc')) {
        return;
    }

    var elem = document.getElementById('libraryChoose');
    cd.loadModel('libraryChoose', 'LibraryContext', UniversityChooseViewModel);

    //function registerKOLibrary(token) {
    //    var $libraryChoose = $('#libraryChoose');
    //    if (!$libraryChoose.length) {
    //        return;
    //    }
    //    ko.applyBindings(new UniversityChooseViewModel(), $libraryChoose[0]);
    //}

    function UniversityChooseViewModel() {
        var $libraryChoose = $('#libraryChoose'),
            $uniList = $('#uniList'),
            $countryList = $('#lib_country'),
            currentCountryCode = $countryList.attr('data-country'),
            $uniSearch = $('#uni_search');

        function University(data) {
            var that = this;
            data = data || {};

            that.name = data.name;
            that.image = data.image;
            that.id = data.uid;
            that.membersCount = data.memberCount;
            that.nCode = data.needCode;
        }

        populateData();
        $('#mLoading').hide();
        registerEvent();


        function populateData() {

            var initData = JSON.parse($libraryChoose.attr('data-data'));
            if (!initData) {
                return
            }

            $countryList.find('option[value="' + currentCountryCode + '"]').prop('selected', true);
            appendUniversities(initData);
            $libraryChoose.removeAttr('data-data');

        }
       
        function registerEvent() {
            var request2 = true, request = true, INPUT_TEXT = 'input[type=text]:first';


            $uniList.on('click', 'li:not(:last)', selectUniversity);
            $countryList.on('change', selectCountry);
            $('.newUni').click(newUniversity);
            $uniSearch.keyup(searchUniversity);

            function selectCountry(e) {
                var countryCode = e.target.value;

                if (currentCountryCode === countryCode) {
                    return;
                }

                currentCountryCode = countryCode;

                dataContext.university({
                    data: { country: countryCode },
                    success: function (data) {
                        appendUniversities(data);
                    }
                });
            }

            var userNotSelected = true;
            function selectUniversity(e) {
                var $uni = $(this),
                    id = $uni.attr('data-id'),
                    name = $uni.find('.uniName').text(),
                    nCode = $uni.attr('data-ncode') === 'true' ? true : false;

                if (!userNotSelected) {
                    return;
                }
                userNotSelected = false;

                cd.analytics.setLibrary(name);


                if (nCode) {
                    needCodePopUp(id);
                    return;
                }

                cd.pubsub.publish('clear_cache');
                dataContext.updateUniversity({
                    data: { UniversityId: id },
                    success: function () {
                        location.href = '/dashboard/';
                    },
                    error: function () {
                        cd.notification('unspecified error');
                    },
                    always: function () {
                        userNotSelected = true;
                    }
                });

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
            }
            function newUniversity() {
                var target = e.target;
                e.target.disabled = true;
                var $addSchoolDialog = $('#addSchoolDialog');
                if (!$addSchoolDialog.length && request) {
                    request = false;
                    dataContext.universityPopUp({
                        success: function (data) {
                            $(libraryChoose).append(data).find(INPUT_TEXT).focus();
                            registerPopEvent();
                        }
                    });
                }
                else {
                    $addSchoolDialog.show().find(INPUT_TEXT).focus();
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
            }
        
            function searchUniversity() {
                var term;
                if (Modernizr.input.placeholder) {
                    term = $uniSearch.val();
                } else {
                    if ($uniSearch.val() === $uniSearch.attr('placeholder')) {
                        term = '';
                    } else {
                        term = $uniSearch.val();
                    }
                }
                if (term === '') {
                    $('.uniName').not(':last').parents('li').show()
                    return;
                }

                cd.analytics.trackEvent('Library Choose', 'Search', term);

                $('.uniName').not(':last').each(function () {
                    var $parent = $(this).parents('li');
                    $(this).text().indexOf(term) > -1 ? $parent.show() : $parent.hide();
                });
            }
        }
    
        document.getElementById('settingsPanelOpen').onclick = function (e) {
            return false;
        };
   
    function appendUniversities(data) {
        var mappeddata = $.map(data, function (i) { return new University(i); });

        $uniList.find('li:not(:last)').remove();
        cd.appendData($uniList[0], 'universityItemTemplate', mappeddata, 'afterbegin', false);
    }
}
})(cd, ko, cd.data, jQuery);