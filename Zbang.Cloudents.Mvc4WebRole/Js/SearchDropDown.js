(function (cd, pubsub, ko, dataContext, $, analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('searchdd')) {
        return;
    }

    var eById = document.getElementById.bind(document),
        form = eById('g_search'),
        input = eById('g_searchQ'),
        dropdown = document.querySelector('.searchDD'),
        showAll = document.querySelector('.dashLibSearch .showAll'),
        boxList = dropdown.querySelector('.searchList.boxes'),
        itemsList = dropdown.querySelector('.searchList.items'),
        peopleList = dropdown.querySelector('.searchList.people'),
        uniList = dropdown.querySelector('.searchList.university'),
        showAllText = showAll.querySelector('q'),
        searchDropdownBtn = eById('searchDropDownBtn'),
        isLoading = false, formSubmitted = false,
        lastInput,
        consts = {
            MAXITEMS: 6,
            MINITEMS: 3,
            BOLDSTRINGLENTH: 30,
            REF: '?r=searchdd'
        },
        currentValue, maxCategoryItems, maxOtherItems;



    function Item(data) {
        var self = this;
        self.image = data.image;
        self.name = cd.highlightSearch(input.value, data.name) || data.name || '';
        self.boxName = data.boxname;
        self.url = data.url + consts.REF;
        self.universityName = '';
    }

    function OtherItem(data) {
        var that = this;
        Item.call(that, data);
        that.universityName = data.universityname;

    }
    function Box(data) {
        var self = this;
        self.image = data.image || '/images/emptyState/my_default3.png';
        self.name = cd.highlightSearch(input.value, data.name) || data.name || '';
        self.proffessor = cd.highlightSearch(input.value, data.proffessor) || data.proffessor || '';
        self.courseCode = cd.highlightSearch(input.value, data.courseCode) || data.courseCode || '';
        self.allDetails = data.proffessor && data.courseCode ? 'allDetails' : '';
        self.url = data.url + consts.REF;
    }
    function Member(data) {
        var self = this;
        self.image = data.image;
        self.name = cd.highlightSearch(input.value, data.name) || data.name || '';
        self.url = data.url + consts.REF;
    }

    registerEvents();


    function registerEvents() {



        var search = cd.debounce(function (e) {

            if (!input.value.length) {
                currentValue = '';
                hide(true);
                return;
            }

            if (currentValue === input.value) {
                return;
            }
            if (isLoading) {
                return;
            }

            var keyCode = e.keyCode || e.which;
            if (keyCode === 13) {

                input.focusout();
            }

            currentValue = input.value;
            isLoading = true;
            dataContext.searchDD({
                data: { q: currentValue },
                success: function (data) {
                    data = data || {};
                    if (!input.value.length) { //we need this because of debounce function
                        return;
                    }
                    parseData(data);
                },
                error: function () {
                },
                always: function () {
                    isLoading = false;
                    formSubmitted = false;
                }
            });
        }, 100);

        input.onkeyup = search;

        dropdown.onmouseover = function (e) {
            this.classList.add('hover');
        };
        dropdown.onmouseout = function (e) {
            this.classList.remove('hover');
        };
        dropdown.onclick = function (e) {
            if (e.target.nodeName === 'H3') {
                input.focus();
                return;
            }
            input.value = currentValue = '';
            cd.historyManager.remove();
            hide();
        };


        showAll.onclick = function (e) {
            var isSearchPage = cd.getParameterFromUrl(0).toLowerCase() === 'search';
            if (isSearchPage) {
                e.preventDefault();
                pubsub.publish('searchInput', input.value);
                return;
            }
        };



        form.onsubmit = function (e) {
            e.preventDefault();
            if (!input.value) {
                return;
            }
            if (lastInput === input.value) {
                return;
            }
            lastInput = input.value;
            currentValue = '';

            hide();
            cd.historyManager.remove();
            formSubmitted = true;
            pubsub.publish('nav', '/search/?q=' + input.value + '&r=searchdd');

        };



        input.onfocus = function (e) {
            e.stopPropagation();
            formSubmitted = false;
            if (input.value.length > 0) {
                show();
            }

        };

        input.onfocusout = function (e) {
            if (dropdown.classList.contains('hover')) {
                return;
            }
            hide();
        };

        pubsub.subscribe('searchclear', function () {
            input.value = '';
        });

    };

    function itemsExists() {
        return boxList.children.length > 0 || itemsList.children.length > 0 || peopleList.children.length > 0 || uniList.children.length > 0;
    }


    function hide(blur) {
        if (!blur) {
            input.blur();
        }
        dropdown.style.display = 'none';
    };
    function show() {
        if (!formSubmitted) {
            if (document.activeElement === input) {
                if (itemsExists()) {
                    dropdown.style.display = 'block';
                }

            }
        }

    };

    function parseData(data) {

        var boxes = mapData(Box, data.boxes),
            items = mapData(Item, data.items),
            otherItems = mapData(OtherItem, data.otherItems),
            users = mapData(Member, data.users),
            emptyCategories = (boxes.length === 0) + (items.length === 0) + (users.length === 0),
            emptyOtherItems = (otherItems.length === 0);


        dropdown.classList.remove('noResults');


        if (emptyCategories === 3 && emptyOtherItems) {
            dropdown.classList.add('noResults');
            searchDropdownBtn.disabled = true;
            return;
        }

        searchDropdownBtn.disabled = false;
        showAll.href = '/search/?q=' + input.value + '&r=searchdd';
        showAllText.textContent = input.value;

        maxCategoryItems = 0;
        maxOtherItems = 0;


        switch (emptyCategories) {
            case 0:
                maxCategoryItems = consts.MINITEMS;
                break;
            case 2:
                maxCategoryItems = consts.MAXITEMS;
                maxOtherItems = consts.MINITEMS;
                break;
            case 3:
                maxOtherItems = consts.MAXITEMS;
                break;
            default:
                maxCategoryItems = consts.MINITEMS;
                maxOtherItems = consts.MINITEMS;
                break;
        };


        appendData(boxList, 'boxesSearchTemplate', boxes, maxCategoryItems);
        appendData(itemsList, 'itemsSearchTemplate', items, maxCategoryItems);
        appendData(peopleList, 'peopleSearchTemplate', users, maxCategoryItems);
        appendData(uniList, 'itemsSearchTemplate', otherItems, maxOtherItems);

        show();

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

        function appendData(list, template, dataItems, maxItems) {

            if (dataItems.length && maxItems) {
                list.previousElementSibling.style.display = 'block';
            } else {
                list.previousElementSibling.style.display = 'none';
            }

            if (!dataItems.length) {
                list.innerHTML = '';
                return;
            }

            dataItems = dataItems.slice(0, maxItems);

            cd.appendData(list, template, dataItems, 'afterbegin', true);
        };
    };
})(cd, cd.pubsub, ko, cd.data, jQuery, cd.analytics);