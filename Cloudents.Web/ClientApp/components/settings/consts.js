export const typesPersonalize = {
    university: "univeristy",
    course: "course"
}
export let searchObjects = {
    course: {
        id: typesPersonalize.course,
        placeholder: 'What class are you taking?',
        closeText: "done",
        searchApi: "getCorses",
        defaultFilter: 'all',
        filters: [{id: 'all', name: 'ALL COURSES'}, {id: 'myCourses', name: 'MY COURSES'}],
        click: function () {
            this.val = "";
            this.$refs.searchText.focus();
        },
        action: "add"
    }, university: {
        id: typesPersonalize.university,
        searchApi: "getUniversities",
        placeholder: 'Where do you go to school?',
        closeText: "X",
        click: function (keep = true) {
            if (!keep) {
                this.$parent.$parent.showDialog = false
            } else {
                this.currentType = "course";
            }
        },
        defaultFilter: '',
        filters: []
    }
};

export let settingMenu = [
    {
        id: typesPersonalize.university,
        name: "Choose university",
        click: function () {
            this.showDialog = true;
            this.type = "";
            this.$nextTick(() => this.type = "university");
            this.keep = true;
            this.isSearch = true;
        }
    },
    {
        id: typesPersonalize.course,
        name: "My courses",
        click: function (universityExist) {
            this.showDialog = true;
            this.type = universityExist ? "course" : "university";
            this.keep = !universityExist ? true : "";
            this.isSearch = true;
        }
    },
    {
        //id: "intercom",
        name: "Feedback",
        click: function () {
            Intercom('showNewMessage', '');
        }
    }
];