﻿(function (cd, pubsub, ko, dataContext, $, analytics) {
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
            sTabContent = eById('sTabContent'), isLoading = false, isOtherLoading = false, isFirstPage,
            gSearch = eById('g_searchQ'), materialsLoaded = false, otherDataAvailable = true,
            otherMaterialsSplit = document.querySelector('.splitHR'), sOtherMaterialsBtn = eById('sOtherMaterialsBtn'), otherUnisScroll = false,
            searchTerm, cPage = 0, cOtherPage = 0, currentTab = sTabCourses;
        consts = {
            COURSES: 'sTab1',
            MATERIALS: 'sTab2',
            MEMBERS: 'sTab3',
            STABCOUNT: 'data-stab-count',
            BOLDPART: 'sHighlight'
        }

        function Course(data) {
            var self = this;
            self.image = data.image || '/images/emptyState/my_default3.png';
            self.name = cd.highlightSearch(searchTerm, data.name,consts.BOLDPART) || data.name || '';
            self.proffessor = cd.highlightSearch(searchTerm, data.proffessor, consts.BOLDPART) || data.proffessor || '';
            self.courseCode = cd.highlightSearch(searchTerm, data.courseCode, consts.BOLDPART) || data.courseCode || '';
            self.allDetails = data.proffessor && data.courseCode ? 'allDetails' : '';
            self.url = data.url;// + '?r=search&s=courses'
        }

        function Material(data) {
            var self = this;
            self.image = data.image;
            self.name = cd.highlightSearch(searchTerm, data.name, consts.BOLDPART);
            self.boxName = data.boxname;
            self.url = data.url;// + '?r=search&s=materials';
            self.universityName = data.uniName || '';
            self.dotted = data.uniName ? 'dot' : '';
            self.content = cd.highlightSearch(searchTerm, data.content, consts.BOLDPART) || data.content || '';
            self.width = 69 / 5 * data.rate || 0;
            self.views = data.views || '0';
        }

        function Member(data) {
            var self = this;
            self.id = data.id;
            self.name = cd.highlightSearch(searchTerm, data.name, consts.BOLDPART) || data.name || '';
            self.image = data.image;
            self.url = data.url;// + '?r=search&s=members';

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
                searchTerm = gSearch.value || cd.getParameterByName('q');
            }
            if (!cd.firstLoad) {
                cd.setTitle('Search | ' + searchTerm + ' | Cloudents');
            }

            isFirstPage = cPage === 0;

            if (initData) {
                search.removeAttribute('data-data');
                parseData(JSON.parse(initData));
                if (isFirstPage) {
                    setCurrentTab(sTabCourses);
                }
                pubsub.publish('search_load');
                return;
            }



            isFirstPage = cPage === 0;

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
                    pubsub.publish('sInputChanged');
                }

                appendData();


                function appendData() {
                    var courses = mapData(Course, data.boxes),
                        materials = mapData(Material, data.items),
                        members = mapData(Member, data.users),
                        toWipe = cPage === 0, length;


                    appendList(sCourseList, 'sCourseItemTemplate', courses, toWipe);
                    appendList(sMaterialList, 'sMaterialItemTemplate', materials, toWipe);
                    appendList(sMemberList, 'sMemberItemTemplate', members, toWipe);                    

                    //setNumbersAndText();
                    //length = parseNumber(currentTab);
                    //sTabResults.setAttribute('data-resultcount', length && length % 50 === 0 ? length + '+' : length);

                    sSearchTerm.textContent = searchTerm;

                    if (materials.length < 50) {                        
                        materialsLoaded = true;
                        sOtherMaterialsBtn.style.display = 'block';
                        if (isFirstPage && materials.length === 0) {
                           getDataOtherUnis();
                        }
                        
                    }
                    pubsub.publish('search_load');

                   
                    //function setNumbersAndText() {
                    //    var length;

                    //    sSearchTerm.textContent = searchTerm;


                    //    length = parseNumber(sTabCourses);
                    //    applyText(sTabCourses, length, courses.length);

                    //    length = parseNumber(sTabMaterials);
                    //    applyText(sTabMaterials, length, materials.length);

                    //    length = parseNumber(sTabMembers);
                    //    applyText(sTabMembers, length, members.length);

                    //    function applyText(elm, currentLength, length) {
                    //        var type = elm.getAttribute('data-type');
                    //        //if (!currentLength) {
                    //        //    elm.textContent = type + ' (0)';
                    //        //    return;
                    //        //}
                    //        currentLength += length;
                    //        if (length === 50) {
                    //            currentLength += '+';
                    //        }
                    //        elm.textContent = type + ' (' + currentLength + ')';
                    //    }
                    //};
                };
            };
        };

        function getDataOtherUnis() {
            sOtherMaterialsBtn.disabled = true;
            var loader = renderLoad(sTabContent, false);
            if (isOtherLoading) {
                return;
            }
            isOtherLoading = true;

            dataContext.searchOtherUnis({
                data: { q: searchTerm, page : cOtherPage },
                success: function (data) {
                    data = data || {};                    
                    parseData(data);
                },
                always: function () {
                    sOtherMaterialsBtn.disabled = false;
                    isOtherLoading = false;
                    loader();
                }
            });

            function parseData(data) {

                sOtherMaterialsBtn.style.display = 'none';

                if (!data.length) {
                    otherDataAvailable = false;
                    return;
                }
                if (data.length < 50) {
                    otherDataAvailable = false;
                }
                
                otherMaterialsSplit.style.display = 'block';

                var materials = mapData(Material, data);
                
                var toWipe = cOtherPage === 0;

                appendList(sOtherMaterialList, 'sMaterialItemTemplate', materials, toWipe);
            }
        }
        function registerEvents() {
            //tab switch
            $(tabsContainer).on('click', 'button', function (e) {
                if (isLoading) {
                    return;
                }
                setCurrentTab(this);
                //var length = parseNumber(this);
                //sTabResults.setAttribute('data-resultcount', length && length % 50 === 0 ? length + '+' : length);
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
                clear(true);
                getData(term);
            });

            sOtherMaterialsBtn.onclick = function () {
                otherUnisScroll = true;
                getDataOtherUnis();

            }
        };

        function setCurrentTab(elm) {
            $elm = $(elm);
            var tabIndex = $elm.index(),
                $list = $('[data-list="' + elm.textContent + '"]').find('li');

            if ($list.length > 0) {
                sTabContent.classList.remove('noResults');
            } else {
                sTabContent.classList.add('noResults');
            }



            $elm.parent().attr('class', 'sTabs').addClass('sTab' + (tabIndex + 1));
            currentTab = $elm[0];
        }

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

        function appendList(list, template, dataItems, wipe) {
            cd.appendData(list, template, dataItems, 'beforeend', wipe);
        }

        function clear(clearPage) {
            //sTabCourses.textContent = sTabCourses.getAttribute('data-type') + ' (0)';
            //sTabMaterials.textContent = sTabMaterials.getAttribute('data-type') + ' (0)';
            //sTabMembers.textContent = sTabMembers.getAttribute('data-type') + ' (0)';
            sCourseList.innerHTML = '';
            sMaterialList.innerHTML = '';
            sMemberList.innerHTML = '';
            sOtherMaterialList.innerHTML = '';
            if (clearPage) {
                cPage = cOtherPage = 0;
            }
            otherMaterialsSplit.style.display = 'none';
            sOtherMaterialsBtn.style.display = 'none';
            isLoading = isOtherLoading = materialsLoaded = otherUnisScroll = false;
            otherDataAvailable = true;
            sTabContent.classList.remove('noResults');
            $(window).off('scroll', scrollEvent);

        }
        function renderLoad(element, moreContent) {
            var cssLoader, imgLoader, loader;

            if (!element) {
                return function () { };
            }

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
        function isShowMoreAvailable(e) {
            var contentElement;
            switch (e) {
                case sTabCourses:
                     return sCourseList.children.length % 50 === 0                   
                case sTabMaterials:
                    if (!materialsLoaded) {
                        return sOtherMaterialList.children.length % 50 === 0
                    }
                    
                    return sMaterialList.children.length % 50 === 0

                case sTabMembers:
                    return sMemberList.children.length % 50 === 0
                   
                default:
                    return false;
                    break;
            }

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
                    var showMoreAvailable = isShowMoreAvailable(currentTab);

                    if (materialsLoaded && otherUnisScroll && otherDataAvailable) {
                        cOtherPage++;
                        getDataOtherUnis();
                    }

                    if (!showMoreAvailable) {
                            return;
                    }
                    cPage++;
                    getData(searchTerm);
                 

                }
            }
        };


    };

})(cd, cd.pubsub, ko, cd.data, jQuery, cd.analytics);