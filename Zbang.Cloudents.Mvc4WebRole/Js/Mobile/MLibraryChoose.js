(function (cd, ko, dataContext, $) {
    "use strict";

    if (window.scriptLoaded.isLoaded('mLc')) {
        return;
    }

    var elem = document.getElementById('libraryChoose');
    cd.loadModel('libraryChoose', 'LibraryContext', UniversityChooseViewModel);
   
    function UniversityChooseViewModel() {
        var $libraryChoose = $('#libraryChoose'),
            $uniList = $('#uniList'),
            $countryList = $('#lib_country'),
            currentCountryCode = $countryList.attr('data-country'),
            $uniSearch = $('#uni_search'),
            universityName, universityId;

        function University(data) {
            var that = this;
            data = data || {};

            that.name = data.name;
            that.image = data.image;
            that.id = data.id;
            that.membersCount = data.memberCount;
        }

        $('#mLoading').hide();
        registerEvents();


        function registerEvents() {
            var request2 = true, request = true, INPUT_TEXT = 'input[type=text]:first';


            var term,
                searchUniversity = cd.debounce(function () {
                    if (term === $uniSearch[0].value) {
                        return;
                    }

                    if (Modernizr.input.placeholder) {

                        term = $uniSearch[0].value;

                    } else {
                        if ($uniSearch[0].value === $uniSearch[0].getAttribute('placeholder')) {
                            term = '';
                        } else {
                            term = $uniSearch[0].value;
                        }
                    }

                    if (term.length < 2) {
                        $(uniList).hide();
                        return;
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


            function newUniversity(e) {
                var target = e.target;
                e.target.disabled = true;
                var $addSchoolDialog = $('#addSchool');
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
                    var $addSchoolDialog = $('#addSchool');
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



                //$uniList.on('click', 'li:not(:last)', selectUniversity);
           

            }


            var userNotSelected = true,
                universityId;

            function selectUniversity(e) {
                e.preventDefault();

                if (!userNotSelected) {
                    return;
                }

                userNotSelected = false;

                cd.analytics.setLibrary(universityName);

                cd.pubsub.publish('clear_cache');

                dataContext.updateUniversity({
                    data: [
                       { name: 'UniversityId', value: universityId },
                       { name: 'DepartmentId', value: $('#year').val() },
                       { name: 'GroupNumber', value: $('#group').val() },
                       { name: 'RegisterNumber', value: $('#registration').val() },
                       { name: 'StudentID', value: $('#userIdNumber').val() },
                       { name: 'Code', value: $('#insertCode').val() }
                    ],
                    success: function (d) {
                        if (d.redirect) {
                            window.location.href = d.redirect;
                        }
                        if (d.html) {
                            $(libraryChoose).append(d.html);
                        }

                    },
                    error: function (msg) {
                        cd.notification(msg[0].value[0]);
                    },
                    always: function () {
                        userNotSelected = true;
                    }
                });
            }

            document.getElementById('settingsPanelOpen').onclick = function (e) {
                return false;
            };



            $uniList.on('click', 'li:not(:last)', function (e) {
                universityId = this.getAttribute('data-id');
                universityName = $(this).find('.uniName').text();
                selectUniversity(e);
            });
            $('.newUni').click(newUniversity);
            $uniSearch.keyup(searchUniversity);




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

            $(document).on('keyup', '#group,#registration', function () {
                checkSubmitState();
            });

            $(document).on('change', '#lecturerRadio,#studentRadio', function () {
                checkSubmitState();
            })
                .on('input', '#departmentCodeInput', function () {
                    $('#departmentSubmit1')[0].disabled = !this.value.length;
                })
                .on('submit', '#selectDepartmentForm1', function (e) {
                    e.preventDefault();
                    var code = $('#departmentCodeInput').val();
                    if (!code.length) {
                        return;
                    }

                    dataContext.verifyCode({
                        data: { code: code },
                        success: function (data) {
                            if (data) {
                                $('.departmentChoose').removeClass('codeError');
                                $('.departmentChoose').addClass('step2');
                                return;
                            }
                            $('.departmentChoose').addClass('codeError');

                        }
                    });
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
            $(document).on('keyup', '#userIdNumber', function () {
                if (this.value.length !== 9) {
                    $('#submitRegIdPopup').attr('disabled', 'disabled');
                    return;
                }
                $('#submitRegIdPopup').removeAttr('disabled');

            }).on('click', '#closeRegIdPopup', function () {
                $('#libEnterId').remove();

            }).on('click', '#submitRegIdPopup', function (e) {
                e.preventDefault();
                if ($('#userIdNumber').val().length !== 9) {
                    alert('אנא הכנס 9 ספרות');
                    return;
                }

                selectUniversity(e);
            });

            //#endregion


            //#region code

            $(document).on('keyup', '#insertCode', function () {
                if (this.value === '') {
                    $('#codeSubmit').attr('disabled', 'disabled');
                    return;
                }
                $('#codeSubmit').removeAttr('disabled');

            }).on('click', '#closeNeedCodePopup', function () {
                $('#libEnterCode').remove();

            }).on('click', '#codeSubmit', function (e) {
                e.preventDefault();
                selectUniversity(e);
            });

            //#endregion
        }
      
   
        function appendUniversities(data) {
        $(uniList).show();
        var mappeddata = $.map(data, function (i) { return new University(i); });

        $uniList.find('li:not(:last)').remove();
        cd.appendData($uniList[0], 'universityItemTemplate', mappeddata, 'afterbegin', false);
    }
}
})(cd, ko, cd.data, jQuery);