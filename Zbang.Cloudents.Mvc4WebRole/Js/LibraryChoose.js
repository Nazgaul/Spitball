(function (cd, $, dataContext, ko, analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('lc')) {
        return;
    }

    var eById = document.getElementById.bind(document),
        libraryChoose = eById('libraryChoose'),
        uniSelect = eById('uni_search'),
        uniList = eById('uniList'),
        sCountry = eById('sCountry'),
        countryList = eById('countryList'),
        uniSearch = eById('uni_search');

    cd.loadModel('libraryChoose', 'LibraryContext', UniversityChooseViewModel);


    function UniversityChooseViewModel() {

        var currentCountryCode = sCountry.getAttribute('data-country');


        function University(data) {
            var that = this;
            data = data || {};
            that.name = data.name;
            that.image = data.image;
            that.id = data.uid;
            that.membersCount = data.memberCount;
            that.nCode = data.needCode;
        }

        var haveUniversity = libraryChoose.getAttribute('data-haveuniversity');
        if (!haveUniversity) {
            $('.siteHeader').find('a').not('#logOut').click(function () {
                return false;
            });
            $('.siteHeader').find('button').attr('disabled', 'disabled'); //no buttons ??
        }

        populateData();

        registerEvents();

        function populateData() {

            var initData = JSON.parse(libraryChoose.getAttribute('data-data'));
            if (initData) {
                appendUniversities(initData);
                sCountry.textContent = $(countryList).find('li[data-value="' + currentCountryCode + '"]').text();
                libraryChoose.removeAttribute('data-data');
                return;
            }


        }


        var request2 = true, INPUT_TEXT = 'input[type=text]:first';


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
        function appendUniversities(data) {
            var mappeddata = $.map(data, function (i) { return new University(i); });

            $('#uniList li:not(:last)').remove();
            cd.appendData(uniList, 'universityItemTemplate', mappeddata, 'afterbegin', false);
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

        function registerEvents() {
            var $countryList = $(countryList);

            cd.menu(sCountry, countryList, function () {
                var $innerListItem = $('[data-value="' + currentCountryCode + '"]');
                scrollToElement($innerListItem);
            });
            $countryList.on('click', 'li', selectCountry);

            //.on('focus', 'li', function () {
            //    $(this).addClass('focus');
            //})
            //.on('blur', 'li', function () {
            //    $(this).removeClass('focus');
            //});
            $(uniList).on('click', 'li:not(:last)', selectUniversity);
            $('.newUni').click(newUniversity);

            $(uniSearch).keyup(searchUniversity);

            $(document).keydown(function (e) {
                if (!$countryList.is(':visible')) {
                    return;
                }

                var s = String.fromCharCode(e.keyCode);
                if (/[a-zA-Z]/.test(s))
                    scrollToElement($countryList.find('li:startsWith("' + s + '")').first().focus());
            });


            function scrollToElement(elem) {
                $countryList.scrollTop($countryList.scrollTop() + elem.position().top - $countryList.height()
                     - 45);//45 is the margin between the input and the list + the list item size
            }

            function selectCountry(e) {
                var countryCode = e.target.getAttribute('data-value');

                if (currentCountryCode == countryCode) {
                    return;
                }
                var loader = cd.renderLoading($(uniList));
                currentCountryCode = countryCode;

                sCountry.textContent = e.target.textContent;

                dataContext.university({
                    data: { country: countryCode },
                    success: function (data) {
                        appendUniversities(data);
                        loader();
                    }
                });
            }

            var request = true, INPUT_TEXT = 'input[type=text]:first';
            function newUniversity(e) {
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

            var userNotSelected = true;
            function selectUniversity(e) {
                var $uni = $(e.target),
                    id = $uni.attr('data-id'),
                    name = $uni.find('uniName').textContent,
                    nCode = $uni.attr('data-ncode') === 'true' ? true : false;

                analytics.setLibrary(name);
                //this can only be Netanya for now
                if (nCode) {
                    analytics.trackEvent('Library Choose', 'Code', id);
                    needCodePopUp(id);

                    return;
                }
                if (!userNotSelected) {
                    return;
                }

                userNotSelected = false;
                var load = cd.renderLoading($(uniList));
                cd.pubsub.publish('clear_cache');
                dataContext.updateUniversity({
                    data: [{ name: 'UniversityId', value: id }],
                    success: function () {
                        window.location.href = '/dashboard/';                          
                    },
                    error: function () {                            
                        cd.notification('unspecified error');                           
                    },
                    always: function(){
                        userNotSelected = true;
                        load();
                    }
                });

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
            }
           

            function searchUniversity() {
                var term;
                if (Modernizr.input.placeholder) {
                    term = uniSelect.value;
                } else {
                    if (uniSelect.value === uniSelect.getAttribute('placeholder')) {
                        term = '';
                    } else {
                        term = uniSelect.value;
                    }
                }
                if (term === '') {
                    $('.uniName').not(':last').parents('li').show()
                    return;
                }

                analytics.trackEvent('Library Choose', 'Search', term);

                $('.uniName').not(':last').each(function(){
                    var $parent = $(this).parents('li');
                    $(this).text().indexOf(term) > -1 ? $parent.show() : $parent.hide();
                });
            }
        }
    }

})(cd, jQuery, cd.data, ko, cd.analytics);