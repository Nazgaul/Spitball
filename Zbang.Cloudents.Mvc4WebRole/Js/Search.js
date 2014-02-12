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
             sCourseList = eById('sCourseList'),sMaterialList = eById('sMaterialsList'),
            sOtherMaterialList = eById('sOtherMaterialsList'),sMemberList = eById('sMembersList'),
        sTabContent = eById('sTabContent'),
        searchTerm,
        consts = {
            COURSES: 'sTab1',
            MATERIALS: 'sTab2',
            MEMBERS: 'sTab3',
            STABCOUNT:'data-stab-count'
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
        self.views = data.views || '';
    }

    function Member(data) {
        var self = this;
        self.name = data.name;
        self.image = data.image;
        self.url = data.url + '?r=search&s=members';

    };


    pubsub.subscribe('search', function () {

        getData()
        registerEvents();
    });

    function getData() {

        var initData = search.getAttribute('data-data');

        searchTerm = cd.getParameterByName('q');


        if (initData) {
            search.removeAttribute('data-data');
            parseData(JSON.parse(initData));
            setCurrentTab(sTabCourses);
            pubsub.publish('search_load');
            return;
        }


        dataContext.searchPage({
            data: { q: searchTerm },
            success: function (data) {
                data = data || {};
                parseData(data);
                setCurrentTab(sTabCourses);
                pubsub.publish('search_load');
            }
        })



        function parseData(data) {
            appendData();


            function appendData() {
                var courses = mapData(Course, data.boxes),
                    materials = mapData(Material, data.items),
                    members = mapData(Member, data.users),
                    otherMaterials = mapData(Material, data.otherItems);


                appendList(sCourseList, 'sCourseItemTemplate', courses,true);
                appendList(sMaterialList, 'sMaterialItemTemplate', materials, true);
                appendList(sMemberList, 'sMemberItemTemplate', members, true);
                appendList(sOtherMaterialList, 'sMaterialItemTemplate', otherMaterials, true);

                setNumbersAndText();

                function mapData(dataType, arr) {
                    if (!arr.length) {
                        return []; // we return empty array to calculate the max items length
                    }
                    return arr.map(function (d) {
                        return new dataType(d);
                    });
                };
                function setNumbersAndText() {
                    sSearchTerm.textContent = searchTerm;
                    
                    
                    sTabCourses.setAttribute(consts.STABCOUNT, courses.length > 50 ? courses.length + '+' : courses.length);
                    sTabMaterials.setAttribute(consts.STABCOUNT, materials.length);
                    sTabMembers.setAttribute(consts.STABCOUNT, members.length);

                    function parseNumber(e,length) {
                        var number = parseInt(e.getAttribute(consts.STABCONTENT), 10);
                        number += length;
                        if (length === 50) {
                            number += '+';
                        }
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

    function appendList(list, template, dataItems,wipe) {
        cd.appendData(list, template, dataItems, 'beforeend', wipe);
    }
};

})(cd, cd.pubsub, ko, cd.data, jQuery, cd.analytics);