(function (cd, pubsub, ko, dataContext, $, analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('searchdd')) {
        return;
    }

    var eById = document.getElementById.bind(document),
        form = eById('g_search'),
        input = eById('g_searchQ'),
        dropdown = document.querySelector('.searchDD'),
        consts = {
            MAXITEMS: 6,
            MINITEMS: 3,
            BOLDSTRINGLENTH: 30
        },
        currentValue, maxCategoryItems, maxOtherItems;



    function Item(data) {
        var self = this;
        self.image = data.image;
        self.name = hightlightSearch(data.name);
        self.boxName = data.boxname;
        self.url = data.url;
        self.universityName = '';
    }

    function OtherItem(data) {
        var that = this;
        Item.call(that, data);
        that.universityName = data.universityname;

    }
    function Box(data) {
        var self = this;
        self.image = data.image;
        self.name = hightlightSearch(data.name);
        self.proffessor = data.proffessor || '';
        self.courseCode = data.courseCode || '';
        self.allDetails = data.proffessor && data.courseCode ? 'allDetails' : '';
        self.url = data.url;
    }
    function Member(data) {
        var self = this;
        self.image = data.image;
        self.name = hightlightSearch(data.name);
        self.url = data.url;
    }

    registerEvents();


    function registerEvents() {

        form.onsubmit = function (e) {
            e.preventDefault();
            if (!input.value) {
                return;
            }
            pubsub.publish('nav', '/search/?q=' + input.value);
        };

        var search = cd.debounce(function (e) {

            if (!input.value.length) {
                dropdown.style.display = 'none';
                return;
            }

            if (currentValue === input.value) {
                return;
            }
            currentValue = input.value;

            dataContext.searchDD({
                data: { q: currentValue },
                success: function (data) {
                    parseData(data);
                },
                error: function () {
                }
            });
        }, 200);

        input.onkeyup = search;
    }

    function hightlightSearch(name) {
        var term = input.value,
            reg = new RegExp(term, 'gmi'),
            m, indeces = [];

        while (m = reg.exec(name)) {
            indeces.push(m.index);
        }

        for (var i = 0, l = indeces.length; i < l; i++) {

            name = highlight(name, indeces[i] + i * consts.BOLDSTRINGLENTH, indeces[i] + term.length + i * consts.BOLDSTRINGLENTH);
        }

        return name;

        function highlight(str, start, end) {
            var text = '<span class="boldPart">' + str.substring(start, end) + '</span>';

            return str.substring(0, start) + text + str.substring(end);
        }
    }

    function parseData(data) {
        dropdown.style.display = 'block';
        var boxes = mapData(Box, data.boxes),
            items = mapData(Item, data.items),
            otherItems = mapData(OtherItem, data.otherItems),
            users = mapData(Member, data.users),
            boxList = dropdown.querySelector('.searchList.boxes'),
            itemsList = dropdown.querySelector('.searchList.items'),
            peopleList = dropdown.querySelector('.searchList.people'),
            uniList = dropdown.querySelector('.searchList.university'),
            emptyCategories = getEmptyCategories(boxes.length, items.length, users.length);


        maxCategoryItems = 0;
        maxOtherItems = 0;
        switch (emptyCategories) {
            case 0:
                maxCategoryItems = consts.MINITEMS;
                break;
            case 3:
                maxOtherItems = consts.MAXITEMS;
                break;
            default:
                maxCategoryItems = consts.MINITEMS;
                maxOtherItems = consts.MINITEMS;
                break;
        }
        appendData(boxList, 'boxesSearchTemplate', boxes, maxCategoryItems);
        appendData(itemsList, 'itemsSearchTemplate', items, maxCategoryItems);
        appendData(peopleList, 'peopleSearchTemplate', users, maxCategoryItems);
        appendData(uniList, 'itemsSearchTemplate', otherItems, maxOtherItems);


        function mapData(dataType, arr) {
            if (!arr.length) {
                return []; // we return empty array to calculate the max items length
            }
            return arr.map(function (d) {
                return new dataType(d);
            });
        }

        function getEmptyCategories(bL, iL, uL) {
            var counter = 0;
            if (bL === 0) {
                counter++;
            }
            if (iL === 0) {
                counter++;
            }
            if (uL === 0) {
                counter++;
            }

            return counter;
        }

        function appendData(list, template, dataItems, maxItems) {
            
            if(dataItems.length && maxItems) {
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
        }
    }

})(cd, cd.pubsub, ko, cd.data, jQuery, cd.analytics);