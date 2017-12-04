export let searchObjects = {
    course: {
        emptyState: "To add your course tap on the plus sign",
        placeholder: 'Select courses',
        closeText: "done",
        searchApi: "getCorses",
        defaultFilter:'all',
        filters: [{ id: 'all', name: 'ALL COURSES' }, { id: 'myCourses', name: 'MY COURSES' }],
        action: "add"
    }, university: {
        searchApi:"getUniversities",
        emptyState: "If you would like to add your school send us a note from the feedback link in the settings tab.",
        placeholder: 'Select school',
        closeText: "X",
        click:function(keep=true){
            // console.log("huii")
            // this.dialog = false;
            if(!keep){this.dialog = false}else{
                this.currentType="course";
            }
        },
        defaultFilter: '',
        filters: []
    }
};
export let settingMenu=[
    {id:"university",name:"Choose university",click:function () {
        this.showDialog = true;
        this.type = "university";
        this.keep=false;
    }
    },
    {
        id: "myCourses", name: "My courses", click: function (universityExist) {
        this.showDialog=true;
        this.type=universityExist?"course":"university";
        !universityExist?this.keep=true:"";
        }
    }
    ];