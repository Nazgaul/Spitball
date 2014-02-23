(function (cd, pubsub, ko, dataContext, $, analytics) {
    if (window.scriptLoaded.isLoaded('searchPage')) {
        return;
    }

    var eById = document.getElementById.bind(document);

    cd.loadModel('search', 'SearchContext', SearchViewModel);

    function SearchViewModel() {
        var search = eById('search'),
            tabsContainer = eById('sTabsMenu'), sSearchTerm = eById('sSearchTerm'),
            sTabResults = eById('sTabResults'), sTabCourses = eById('sTabCourses'),
            sTabMaterials = eById('sTabMaterials'), sTabMembers = eById('sTabMembers'),
            sCourseList = eById('sCourseList'), sMaterialList = eById('sMaterialsList'),
            sOtherMaterialList = eById('sOtherMaterialsList'), sMemberList = eById('sMembersList'),
            sTabContent = eById('sTabContent'), isLoading = false, gSearch = eById('g_searchQ'),
            otherMaterialsSplit = document.querySelector('.splitHR'),
            searchTerm, cPage = 0, currentTab = sTabCourses;
        consts = {
            COURSES: 'sTab1',
            MATERIALS: 'sTab2',
            MEMBERS: 'sTab3',
            STABCOUNT: 'data-stab-count'
        }

        function Course(data) {
            var self = this;
            self.image = data.image;
            self.name = data.name;
            self.proffessor = data.proffessor || '';
            self.courseCode = data.courseCode || '';
            self.allDetails = data.proffessor && data.courseCode ? 'allDetails' : '';
            self.url = data.url + '?r=search&s=courses'
        }

        function Material(data) {
            var self = this;
            self.image = data.image;
            self.name = data.name;
            self.boxName = data.boxname;
            self.url = data.url + '?r=search&s=materials';
            self.universityName = data.universityname;
            self.content = data.content || '';
            self.width = 69 / 5 * data.rate || 0;
            self.views = data.views || '0';
        }

        function Member(data) {
            var self = this;
            self.id = data.id;
            self.name = data.name;
            self.image = data.image;
            self.url = data.url + '?r=search&s=members';

        };


        pubsub.subscribe('search', function () {
            var currentPage = cd.getParameterFromUrl(0);

            getData()
            registerEvents();
        });

        pubsub.subscribe('searchclear', function () {
            clear(true);
        });

        function getData(term) {
            term = term || {}
            var initData = search.getAttribute('data-data');
            
            if (term.length) {
                searchTerm = term;
            } else {
                searchTerm = cd.getParameterByName('q');
            }
            if (!cd.firstLoad) {
                cd.setTitle('Search | ' + searchTerm + ' | Cloudents');
            }



            if (initData) {
                search.removeAttribute('data-data');
                parseData(JSON.parse(initData));
                if (cPage === 0) {
                    setCurrentTab(sTabCourses);
                }
                pubsub.publish('search_load');
                return;
            }



            var isFirstPage = cPage === 0;
            
            if (isLoading) {
                return;
            }
            if (isFirstPage) {
                clear(true);
                sTabContent.classList.add('sLoading');
            }
            var loader = renderLoad(sTabContent, !isFirstPage);
                        
            isLoading = true;
            sSearchTerm.textContent = searchTerm;
            sTabResults.classList.add('searching');
            dataContext.searchPage({
                data: { q: searchTerm, page: cPage },
                success: function (data) {
                    data = data || {};
                    parseData(data);
                    if (isFirstPage) {
                        setCurrentTab(currentTab);
                        return;
                    }
                },
                always: function () {
                    loader();
                    isLoading = false;
                    sTabContent.classList.remove('sLoading');
                    sTabResults.classList.remove('searching');
                }
            })



            function parseData(data) {

                if (gSearch.value === '') {
                    gSearch.value = searchTerm;
                }

                appendData();


                function appendData() {
                    var courses = mapData(Course, data.boxes),
                        materials = mapData(Material, data.items),
                        members = mapData(Member, data.users),
                        otherMaterials = mapData(Material, data.otherItems);

                    toWipe = cPage === 0;


                    appendList(sCourseList, 'sCourseItemTemplate', courses, toWipe);
                    appendList(sMaterialList, 'sMaterialItemTemplate', materials, toWipe);
                    appendList(sMemberList, 'sMemberItemTemplate', members, toWipe);
                    if (otherMaterials.length > 0) {
                        otherMaterialsSplit.style.display = 'block';
                        appendList(sOtherMaterialList, 'sMaterialItemTemplate', otherMaterials, toWipe);
                    } else {
                        otherMaterialsSplit.style.display = 'none';
                    }

                    setNumbersAndText();
                    var length = parseNumber(currentTab);
                    sTabResults.setAttribute('data-resultcount', length && length % 50 === 0 ? length + '+' : length);
                    pubsub.publish('search_load');

                    function mapData(dataType, arr) {
                        if (!arr) {
                            return [];
                        }
                        if (!arr.length) {
                            return []; // we return empty array to calculate the max items length
                        }
                        return arr.map(function (d) {
                            return new dataType(d);
                        });
                    };
                    function setNumbersAndText() {
                        var length;

                        sSearchTerm.textContent = searchTerm;


                        length = parseNumber(sTabCourses);
                        applyText(sTabCourses, length, courses.length);

                        length = parseNumber(sTabMaterials);
                        applyText(sTabMaterials, length, materials.length + otherMaterials.length);

                        length = parseNumber(sTabMembers);
                        applyText(sTabMembers, length, members.length);

                        function applyText(elm, currentLength, length) {
                            currentLength += length;
                            if (length === 50) {
                                currentLength += '+';
                            }
                            elm.textContent = elm.getAttribute('data-type') + ' (' + currentLength + ')';
                        }
                    };
                };
            };
        };

        function registerEvents() {
            //tab switch
            $(tabsContainer).on('click', 'button', function (e) {
                if (isLoading) {
                    return;
                }
                setCurrentTab(this);
                var length = parseNumber(this);
                sTabResults.setAttribute('data-resultcount', length && length % 50 === 0 ? length + '+' : length);
            });


            //fetch more data
            $(window).scroll(scrollEvent);


            $(sTabContent).on('click', '.inviteUserBtn', function (e) {
                nameElement = this.previousElementSibling,
                name = nameElement.textContent,
                imgElement = nameElement.previousElementSibling.querySelector('img'),
                image = imgElement.src,
                id = imgElement.getAttribute('data-uid');

                cd.pubsub.publish('message', { id: '', data: [{ name: name, id: id, userImage: image }] });
            });
            pubsub.subscribe('searchInput', function (term) {
                clear();
                getData(term);
            });
        };

        function setCurrentTab(elm) {
            $elm = $(elm);
            var tabIndex = $elm.index(),
                    list = $('[data-list="' + elm.getAttribute('data-type') + '"]').find('li');

            if (list.length > 0) {
                sTabContent.classList.remove('noResults');
            } else {
                sTabContent.classList.add('noResults');
            }



            $elm.parent().attr('class', 'sTabs').addClass('sTab' + (tabIndex + 1));
            currentTab = $elm[0];
        }

        function appendList(list, template, dataItems, wipe) {
            cd.appendData(list, template, dataItems, 'beforeend', wipe);
        }

        function clear(clearPage) {
            sTabCourses.textContent = 'Courses (0)';
            sTabMaterials.textContent = 'Materials (0)';
            sTabMembers.textContent = 'Members (0)';
            sCourseList.innerHTML = '';
            sMaterialList.innerHTML = '';
            sMemberList.innerHTML = '';
            sOtherMaterialList.innerHTML = '';
            otherMaterialsSplit.style.display = 'none';
            if (clearPage) {
                cPage = 0;
            }
            sTabContent.classList.remove('noResults');
            $(window).off('scroll', scrollEvent);

        }
        function renderLoad(element, moreContent) {
            var cssLoader, imgLoader, loader;

            if (!moreContent) {
                cssLoader = '<div class="smallLoader upLoader"><div class="spinner"></div>';
                imgLoader = '<img class="pageLoaderImg upLoader" src="/images/loader2.gif" />';
            } else {
                cssLoader = '<div class="pageLoader pageAnim"></div>';
                imgLoader = '<img class="pageLoaderImg pageAnim" src="/images/loader1.gif" />';
            }



            if (Modernizr.cssanimations) {
                element.insertAdjacentHTML('beforeend', cssLoader);
            } else {
                element.insertAdjacentHTML('beforeend', imgLoader);
            }
            //try { //show loader
            //    //window.scrollTo(0, document.body.scrollHeight);
            //} catch (err) {
            //    console.log(err.message);
            //}

            $upLoader = $('.upLoader');
            $pageAnim = $('.pageAnim');

            return function () {
                //if ($(loader).parents('.sTabContent')) {
                $upLoader.remove();
                $pageAnim.remove();
                //}
            };
        }
        function parseNumber(e) {

            var number = e.textContent.match(/\(([^)]+)\)/);
            if (number) {
                return parseInt(number[1]);
            } else {
                return 0;
            }
        }
        function scrollEvent() {
            if ($(search).is(':visible')) {
                if (isLoading) {
                    return;
                }

                if ($(window).scrollTop() + $(window).height() == $(document).height()) {
                    var length = parseNumber(currentTab);

                    if (length % 50 !== 0) {
                        return;
                    }
                    cPage++;                    
                    getData(searchTerm);

                }
            }
        };


    };

})(cd, cd.pubsub, ko, cd.data, jQuery, cd.analytics);