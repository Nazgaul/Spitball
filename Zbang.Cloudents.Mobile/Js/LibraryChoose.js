(function (cd, $, dataContext, analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('lc')) {
        return;
    }

    var eById = document.getElementById.bind(document),
        libraryChoose = eById('libraryChoose'),
        uniSelect = eById('uni_search'),
        uniList = eById('uniList'),
        uniSearch = eById('uni_search'),
        fbUniList = eById('fbUniList');

    var universityName;

    cd.loadModel('libraryChoose', 'LibraryContext', UniversityChooseViewModel);


    function UniversityChooseViewModel() {
        "use strict";
       // var currentCountryCode = libraryChoose.getAttribute('data-country');

        $('footer').remove(); //hack for now.
        function University(data) {
            var that = this;
            if (data.memberCount) {
                that.membersCount = data.memberCount;
            } else {
                that.friendsCount = data.friends.length;
                that.friends = $.map(data.friends, function (i) { return new Friend(i); });
            }

            that.name = data.name;
            that.image = data.image;
            that.id = data.id;
        }

        function Friend(data) {
            var that = this;
            that.name = data.name;
            that.image = data.image;
        }

        var haveUniversity = libraryChoose.getAttribute('data-haveuniversity');
        if (!haveUniversity) {
            $('.siteHeader').find('a').not('#logOut').click(function () {
                return false;
            });
            $('.siteHeader').find('button').attr('disabled', 'disabled'); //no buttons ??
        }

        var waitForFb = setInterval(function () {
            if (!window.FB) {
                return;
            }            
            clearInterval(waitForFb);
            window.fbAsyncInit();
            var token;
            FB.getLoginStatus(function (response) {
                if (response.status === 'connected') {
                    token = response.authResponse.accessToken;
                    if (!token) {
                        return;
                    }

                    dataContext.getFriendsUnis({
                        data: { authToken: token },
                        success: function (data) {
                            data = data || {};
                            showFriendsUnis(data);
                        }
                    });
                }
            });
        }, 10);


        registerEvents();

        function showFriendsUnis(data) {
            if (!data.length) {
                return;
            }

            var mappeddata = $.map(data, function (i) { return new University(i); }),
                html;

            for (var i = 0, l = mappeddata.length; i < l; i++) {
                var uni = mappeddata[i];
                uni.friendsList = cd.attachTemplateToData('fBFriendTemplate', uni.friends);

                html = cd.attachTemplateToData('universityItemFbTemplate', uni);

                fbUniList.insertAdjacentHTML('beforeend', html);

            }
            $(fbUniList).show();
        }

        function appendUniversities(data) {
            $(uniList).show();
            if (data.length) {                
                $(uniList).removeClass('noResults');
            } else {
                $(uniList).addClass('noResults');   
            }

            var mappeddata = $.map(data, function (i) { return new University(i); });
            $('#uniList li:not(:nth-last-child(-n+2))').remove();
            cd.appendData(uniList, 'universityItemTemplate', mappeddata, 'afterbegin', false);
        }

        function registerEvents() {
           var request = true, INPUT_TEXT = 'input[type=text]:first';

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

              if (term.length < 2) {
                  $(fbUniList).show();
                  $(uniList).hide();
                  return;
              }

              $(fbUniList).hide();

              dataContext.searchUniversity({
                  data: { term: term },
                  success: function (data) {
                      data = data || {};
                      appendUniversities(data);
                  }
              });

              analytics.trackEvent('Library Choose', 'Search', term);
          }, 150);

            $(uniList).on('click', 'li:not(:last)', function (e) {
                universityId = this.getAttribute('data-id');
                universityName = $(this).find('.uniName').text();
                selectUniversity(e);
            });
            $(fbUniList).on('click', 'li', function (e) {
                //var listItem = $(this).parents('li');
                universityId = $(this).find('button').attr('data-id');
                universityName = $(this).find('.uniName').text();
                selectUniversity(e);
            });
            $('.newUni').click(newUniversity);

            $(uniSearch).keyup(searchUniversity);


            function newUniversity(e) {
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
                e.preventDefault();

                if (!userNotSelected) {
                    return;
                }
                userNotSelected = false;

                analytics.setLibrary(universityName);

                //var load = cd.renderLoading($(uniList));
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
                            window.location.href = cd.getParameterByName('returnurl') || d.redirect;
                        }
                        if (d.html) {
                            $(libraryChoose).append(d.html);
                        }

                    },
                    error: function (msg) {
                        if (msg) {
                            cd.notification(msg[0].value[0]);
                        }
                    },
                    always: function () {
                        userNotSelected = true;
                        //load();
                    }
                });                
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

            $(document).on('keyup', '#group,#registration', function () {
                checkSubmitState();
            });

            $(document).on('change', '#lecturerRadio,#studentRadio', function() {
                    checkSubmitState();
                })
                .on('input', '#departmentCodeInput', function() {
                    $('#departmentSubmit1')[0].disabled = !this.value.length;
                })
                .on('submit', '#selectDepartmentForm1', function(e) {
                    e.preventDefault();
                    var code = $('#departmentCodeInput').val();
                    if (!code.length) {
                        return;
                    }

                    dataContext.verifyCode({
                        data: { code: code },
                        success: function(data) {
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
    }

})(cd, jQuery, cd.data, cd.analytics);
