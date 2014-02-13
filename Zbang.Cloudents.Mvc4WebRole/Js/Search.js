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
            sTabContent = eById('sTabContent'), isLoading = false,
            searchTerm, cPage = 0;
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
            self.universityName = '&nbsp;';
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
            var currentPage = cd.getParameterFromUrl(1)
            getData()
            registerEvents();
        });

        pubsub.subscribe('searchclear', function () {
            clear();
        });

        function getData(term) {
            term = term || {}
            var initData = search.getAttribute('data-data');

            if (term.length) {
                searchTerm = term;
            } else {
                searchTerm = cd.getParameterByName('q');
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
            var loader = renderLoad(sTabContent, !isFirstPage);
            if (isFirstPage) {
                sTabContent.classList.add('sLoading');

            }
            if (isLoading) {
                return;
            }
            isLoading = true;
            sSearchTerm.textContent = searchTerm;
            sTabResults.classList.add('searching');
            dataContext.searchPage({
                data: { q: searchTerm, page: cPage },
                success: function (data) {
                    data = data || {};
                    parseData(data);
                    if (cPage === 0) {
                        setCurrentTab(sTabCourses);
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
                appendData();


                function appendData() {
                    var courses = mapData(Course, data.boxes),
                        materials = mapData(Material, data.items),
                        members = mapData(Member, data.users),
                        otherMaterials = mapData(Material, data.otherItems),
                        toWipe = cPage === 0;


                    appendList(sCourseList, 'sCourseItemTemplate', courses, toWipe);
                    appendList(sMaterialList, 'sMaterialItemTemplate', materials, toWipe);
                    appendList(sMemberList, 'sMemberItemTemplate', members, toWipe);
                    appendList(sOtherMaterialList, 'sMaterialItemTemplate', otherMaterials, toWipe);

                    setNumbersAndText();
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
                        sSearchTerm.textContent = searchTerm;

                        parseNumber(sTabCourses, courses.length);
                        parseNumber(sTabMaterials, materials.length);
                        parseNumber(sTabMembers, members.length);

                        function parseNumber(e, length) {
                            var number = parseInt(e.getAttribute(consts.STABCOUNT), 10);
                            number += length;
                            if (length === 50) {
                                number += '+';
                            }
                            e.setAttribute(consts.STABCOUNT, number);
                        }
                    };
                };
            };
        };

        function registerEvents() {
            //tab switch
            $(tabsContainer).on('click', 'button', function (e) {
                setCurrentTab(this);
            });


            //fetch more data
            $(window).scroll(function () {
                if ($(window).scrollTop() + $(window).height() == $(document).height()) {
                    cPage++;
                    getData(searchTerm);
                }
            });

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
                    list = $('ul[data-list="' + elm.getAttribute('data-type') + '"]');

            if (list.children().length > 0) {
                sTabContent.classList.remove('noResults');
            } else {
                sTabContent.classList.add('noResults');
            }

            $elm.parent().attr('class', 'sTabs').addClass('sTab' + (tabIndex + 1));
            sTabResults.setAttribute('data-resultcount', elm.getAttribute('data-stab-count'));

        }

        function appendList(list, template, dataItems, wipe) {
            cd.appendData(list, template, dataItems, 'beforeend', wipe);
        }

        function clear() {
            sTabCourses.setAttribute(consts.STABCOUNT, 0);
            sTabMaterials.setAttribute(consts.STABCOUNT, 0);
            sTabMembers.setAttribute(consts.STABCOUNT, 0);
            sCourseList.innerHTML = '';
            sMaterialList.innerHTML = '';
            sMemberList.innerHTML = '';
            sOtherMaterialList.innerHTML = '';
            cPage = 0;
            sTabContent.classList.remove('noResults');
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

            loader = element.querySelector('.upLoader') || element.querySelector('.pageAnim');

            return function () {
                if ($(loader).parents('.sTabContent')) {
                    element.removeChild(loader);
                }
            };
        }

    };

})(cd, cd.pubsub, ko, cd.data, jQuery, cd.analytics);