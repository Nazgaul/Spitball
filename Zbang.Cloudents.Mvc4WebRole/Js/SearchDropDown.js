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
            MINITEMS: 3
        },
        currentValue;



    function Item(data) {
        var self = this;
        self.image = data.image;
        self.name = hightlightSearch(data.name);
        self.boxName = data.boxname;
        self.url = data.url;
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

            if(currentValue === input.value) {
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
            m,indeces = [];

        while (m = reg.exec(name)) {
            indeces.push(m.index);
        } 
        
        for (var i = 0, l = indeces.length; i < l; i++) {
            name = highlight(name, indeces[i], indeces[i] + term.length);
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
            users = mapData(Member, data.users),
            boxList = dropdown.querySelector('.searchList.boxes'),
            itemsList = dropdown.querySelector('.searchList.items'),
            peopleList = dropdown.querySelector('.searchList.people'),
            uniList = dropdown.querySelector('.searchList.university'),
            emptyCategories = getEmptyCategories(boxes.length, items.length, users.length);

        
        appendData(boxList, 'boxesSearchTemplate', boxes);
        appendData(itemsList, 'itemsSearchTemplate', items);
        appendData(peopleList, 'peopleSearchTemplate', users);

        var otherItems = [] //to be deleted
        if (emptyCategories < 2) {
            appendData(uniList, 'peopleSearchTemplate', otherItems);
        }

        function mapData(dataType, arr) {
            if (!arr.length) {
                return []; // we return empty array to get the search result
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

        function appendData(list, template, data) {
            list.previousElementSibling.style.display = data.length ? 'block' : 'none';

            if (!data.length) {
                list.innerHTML = '';
                return;
            }

            if (emptyCategories < 2) {
                data = data.slice(consts.MINITEMS);
            }
            cd.appendData(list, template, data, 'afterbegin', true);
        }
    }

})(cd, cd.pubsub, ko, cd.data, jQuery, cd.analytics);