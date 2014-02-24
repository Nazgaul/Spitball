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
        "use strict";
        var currentCountryCode = libraryChoose.getAttribute('data-country');


        function University(data) {
            var that = this;
            data = data || {};
            that.country = data.country;
            that.membersCount = data.memberCount;
            that.name = data.name;
            that.image = data.image;
            that.id = data.uid;
            that.nCode = data.needCode;
        }

        var haveUniversity = libraryChoose.getAttribute('data-haveuniversity');
        if (!haveUniversity) {
            $('.siteHeader').find('a').not('#logOut').click(function () {
                return false;
            });
            $('.siteHeader').find('button').attr('disabled', 'disabled'); //no buttons ??
        }
        var universities = [];
        populateData();

        registerEvents();

        

        function populateData() {

            universities = JSON.parse(libraryChoose.getAttribute('data-data'));
            appendUniversities(universities);
            //sCountry.textContent = $(countryList).find('li[data-value="' + currentCountryCode + '"]').text();
            libraryChoose.removeAttribute('data-data');
        }
       

        function appendUniversities(data) {
            var mappeddata = $.map(data, function (i) { return new University(i); });
            mappeddata.sort(sortArray);
            $('#uniList li:not(:nth-last-child(-n+2))').remove();
            cd.appendData(uniList, 'universityItemTemplate', mappeddata, 'afterbegin', false);
        }

        function sortArray(a, b) {
            if (a.country === currentCountryCode && b.country !== currentCountryCode) {
                return -1;
            }
            if (a.country !== currentCountryCode && b.country === currentCountryCode) {
                return 1;
            }
            if (a.membersCount > b.membersCount) {
                return -1;
            }
            if (a.membersCount < b.membersCount) {
                return 1;
            }
            return 0;
        }


        function registerEvents() {
            //var $countryList = $(countryList),
              var  request2 = true, request = true, INPUT_TEXT = 'input[type=text]:first';

            //cd.menu(sCountry, countryList, function () {
            //    var $innerListItem = $('[data-value="' + currentCountryCode + '"]');
            //    scrollToElement($innerListItem);
            //});
            //$countryList.on('click', 'li', selectCountry);

            $(uniList).on('click', 'li:not(:last)', selectUniversity);
            $('.newUni').click(newUniversity);

            $(uniSearch).keyup(searchUniversity);

            //$(document).keydown(function (e) {
            //    //if (!$countryList.is(':visible')) {
            //    //    return;
            //    //}

            //    //var s = String.fromCharCode(e.keyCode);
            //    //if (/[a-zA-Z]/.test(s))
            //    //    scrollToElement($countryList.find('li:startsWith("' + s + '")').first().focus());
            //});


            //function scrollToElement(elem) {
            //    $countryList.scrollTop($countryList.scrollTop() + elem.position().top - $countryList.height()
            //         - 45);//45 is the margin between the input and the list + the list item size
            //}

            //function selectCountry(e) {
            //    var countryCode = e.target.getAttribute('data-value');

            //    if (currentCountryCode === countryCode) {
            //        return;
            //    }
            //    currentCountryCode = countryCode;

            //    sCountry.textContent = e.target.textContent;
            //    appendUniversities(universities);
            //    //dataContext.university({
            //    //    data: { country: countryCode },
            //    //    success: function (data) {
            //    //        appendUniversities(data);
            //    //        loader();
            //    //    }
            //    //});
            //}

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
                            eById('Name').value = uniSearch.value;

                        }
                    });
                }
                else {
                    $addSchoolDialog.show().find(INPUT_TEXT).focus();
                    eById('Name').value = uniSearch.value;
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

            var userNotSelected = true,
                universityId;
            function selectUniversity(e) {
                var $uni = $(this),
                    name = $uni.find('.uniName').text(),
                    nCode = $uni.attr('data-ncode') === 'true' ? true : false;

                if ($uni.length > 0) {
                    universityId = $uni.attr('data-id');
                }

                if (!userNotSelected) {
                    return;
                }
                userNotSelected = false;

                cd.analytics.setLibrary(name);


                //this can only be Netanya for now
                if (nCode) {
                    cd.analytics.trackEvent('Library Choose', 'Code', universityId);
                    needCodePopUp(universityId);
                    return;
                }


                //var load = cd.renderLoading($(uniList));
                cd.pubsub.publish('clear_cache');
                dataContext.updateUniversity({
                    data: [
                        { name: 'UniversityId', value: universityId },
                        { name: 'DepartmentId', value: $('#year').val() },
                    ],
                    success: function (d) {
                        if (d.redirect) {
                            window.location.href = d.redirect;
                        }
                        if (d.html) {
                            $(libraryChoose).append(d.html);
                        }

                    },
                    error: function () {
                        cd.notification('unspecified error');
                    },
                    always: function () {
                        userNotSelected = true;
                        //load();
                    }
                });

                function needCodePopUp(universityId) {
                    var $libEnterCode = $('#libEnterCode');
                    if (!$libEnterCode.length && request2) {
                        request2 = false;
                        dataContext.universityEnterCode({
                            data: { uid: universityId },
                            success: function (data) {
                                $(libraryChoose).append(data);
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
                                        window.location.href = '/dashboard/';
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
                    $('#uniList').hide();
                    // $('.uniName').not(':last').parents('li').show()
                    return;
                }
                $('#uniList').show();
                cd.analytics.trackEvent('Library Choose', 'Search', term);

                $('.uniName').not(':last').each(function () {
                    var $parent = $(this).parents('li');
                    var lowerText = $(this).text().toLowerCase();
                    var termTrimmed = term.trim();
                    var query;
                    if (currentCountryCode == 'IL') {
                        query = lowerText.indexOf(termTrimmed.toLowerCase()) > -1 || lowerText.indexOf(cd.conversion.convert(termTrimmed)) > -1;
                    } else {
                        query = lowerText.indexOf(termTrimmed.toLowerCase()) > -1;
                    }
                    query ? $parent.show() : $parent.hide();
                });
            }


            //#region department
            $(document).on('change', '#department', function () {
                var $year = $('#year');
                $year.val('-1').trigger('change');
                $year.find('option').not(':first').hide().filter('[data-department= ' + $(this).val() + ']').show();
            })
           .on('change', '#year', function () {
               if ($(this).val() === '-1') {
                   $('#depSubmit').attr('disabled', 'disabled');
               }
               else {
                   $('#depSubmit').removeAttr('disabled');
               }

           })
            .on('click', '.closeDialog', function () {
                $(this).parents('[data-popup]').remove();
            })
            .on('click', '#depSubmit', function (e) {
                selectUniversity(e);
            });
            //#endregion
        }
    }

})(cd, jQuery, cd.data, ko, cd.analytics);