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

        $('footer').remove(); //hack for now.
        function University(data) {
            var that = this;
            data = data || {};
            that.membersCount = data.memberCount;
            that.name = data.name;
            that.image = data.image;
            that.id = data.id;
        }

        var haveUniversity = libraryChoose.getAttribute('data-haveuniversity');
        if (!haveUniversity) {
            $('.siteHeader').find('a').not('#logOut').click(function () {
                return false;
            });
            $('.siteHeader').find('button').attr('disabled', 'disabled'); //no buttons ??
        }
        var universities = [];

        registerEvents();                       

        function appendUniversities(data) {
            if (data.length) {
                $(uniList).show().removeClass('noResults');
            } else {
                $(uniList).addClass('noResults');
            }

            var mappeddata = $.map(data, function (i) { return new University(i); });        
            $('#uniList li:not(:nth-last-child(-n+2))').remove();
            cd.appendData(uniList, 'universityItemTemplate', mappeddata, 'afterbegin', false);
        }

        function registerEvents() {
             var  request2 = true, request = true, INPUT_TEXT = 'input[type=text]:first';

             var term,
           searchUniversity = cd.debounce(function () {
               if (term === uniSelect.value) {
                   return;
               }

               if (Modernizr.input.placeholder) {

                   term = uniSelect.value;

               } else {
                   if (uniSelect.value === uniSelect.getAttribute('placeholder')) {
                       term = '';
                   } else {
                       term = uniSelect.value;
                   }
               }               

               dataContext.searchUniversity({
                   data: { term: term },
                   success: function (data) {
                       data = data || {};
                       appendUniversities(data);
                       
                   }
               });

               cd.analytics.trackEvent('Library Choose', 'Search', term);
           }, 150);

            $(uniList).on('click', 'li:not(:last)', selectUniversity);
            $('.newUni').click(newUniversity);

            $(uniSearch).keyup(searchUniversity);


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
                        { name: 'GroupNumber', value: $('#group').val() },
                        { name: 'RegisterNumber', value: $('#registration').val() },
                        { name: 'StudentID', value: $('#userIdNumber').val() }
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


      

            //#region department
            $(document).on('change', '#department', function () {
                var $year = $('#year');
                $year.val('-1').trigger('change');
                if (this.selectedIndex === 0) {
                    return;
                }
                $year.find('option').not(':first').hide().filter('[data-department= ' + $(this).val() + ']').show();
            })
           .on('change', '#year', function () {
               checkSubmitState();
           })
            .on('click', '.closeDialog', function () {
                $(this).parents('[data-popup]').remove();
                userNotSelected = true;
            })
            .on('click', '#depSubmit', function (e) {
                selectUniversity(e);
            });

            $(document).on('keyup','#group,#registration', function(){
                checkSubmitState();
            });

            $(document).on('change', '#lecturerRadio,#studentRadio', function () {
                checkSubmitState();
            });


            function checkSubmitState() {
                var studentRadio = document.getElementById('studentRadio');

                if (studentRadio.checked) {
                    var department = document.getElementById('department'),
                        year = document.getElementById('year'),
                        group = document.getElementById('group'),
                        registration = document.getElementById('registration');
                    
                    if (department.selectedIndex && year.selectedIndex && group.value !== '' && registration.value !== '') {
                        $('#depSubmit').removeAttr('disabled');
                    } else {
                        $('#depSubmit').attr('disabled', 'disabled');
                    }
                } else {
                    $('#depSubmit').removeAttr('disabled');

                }                
            }
            //#endregion



            //#region id

            $(document).on('keyup', '#userIdNumber',function () {
                if (this.value === '') {
                    $('#submitRegIdPopup').attr('disabled', 'disabled');
                    return;
                }
                $('#submitRegIdPopup').removeAttr('disabled');

            }).on('click','#closeRegIdPopup', function () {
                $('#libEnterId').remove();

            }).on('click', '#submitRegIdPopup', function (e) {
                e.preventDefault();
                selectUniversity(e);
            });

            //#endregion
        }
    }

})(cd, jQuery, cd.data, ko, cd.analytics);