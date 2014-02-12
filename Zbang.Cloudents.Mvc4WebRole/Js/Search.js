(function (cd, pubsub, ko, dataContext, $, analytics) {
    if (window.scriptLoaded.isLoaded('searchPage')) {
        return;
    }

    var eById = document.getElementById.bind(document);

    cd.loadModel('search', 'SearchContext', SearchViewModel);

    function SearchViewModel() {
        var search = eById('search');

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
            self.width = 40 / 5  * 5;//data.rate;
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

            if (initData) {
                search.removeAttribute('data-data');
                appendData(JSON.parse(initData));
                pubsub.publish('search_load');
                return;
            }

            dataContext.searchPage({
                data: { q: cd.getParameterByName('q') },
                success: function (data) {
                    data = data || {};
                    appendData(data);
                }
            })



            function appendData(data) {
                var courses = mapData(Course, data.boxes),
                    materials = mapData(Material, data.items),
                    members = mapData(Member, data.users),
                    otherMaterials = mapData(Material, data.otherItems),
                    sCourseList = eById('sCourseList'),
                    sMaterialsList = eById('sMaterialsList'),
                    sOtherMaterialsList = eById('sOtherMaterialsList'),
                    sMembersList = eById('sMembersList');

                cd.appendData(sCourseList, 'sCourseItemTemplate', courses, 'afterbegin', true);
                cd.appendData(sMaterialsList, 'sMaterialItemTemplate', materials, 'afterbegin', true);
                cd.appendData(sMembersList, 'sMemberItemTemplate', members, 'afterbegin', true);
                cd.appendData(sOtherMaterialsList, 'sMaterialItemTemplate', otherMaterials, 'afterbegin', true);

                function mapData(dataType, arr) {
                    if (!arr.length) {
                        return []; // we return empty array to calculate the max items length
                    }
                    return arr.map(function (d) {
                        return new dataType(d);
                    });
                };
            };
        };

        function registerEvents() {

        };

    };

})(cd, cd.pubsub, ko, cd.data, jQuery, cd.analytics);