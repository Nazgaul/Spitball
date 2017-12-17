export let searchObjects = {
    course: {
        placeholder: 'What class are you taking?',
        closeText: "done",
        searchApi: "getCorses",
        defaultFilter:'all',
        filters: [{ id: 'all', name: 'ALL COURSES' }, { id: 'myCourses', name: 'MY COURSES' }],
        action: "add"
    }, university: {
        searchApi:"getUniversities",
        placeholder: 'Where do you go to school?',
        closeText: "X",
        click:function(keep=true){
            if(!keep){this.$parent.$parent.showDialog = false}else{
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
        this.type="";
        this.$nextTick(()=>this.type = "university");
        this.keep=false;
        this.isSearch=true;
    }
    },
    {
        id: "myCourses", name: "My courses", click: function (universityExist) {
        this.showDialog=true;
        this.type=universityExist?"course":"university";
        !universityExist?this.keep=true:"";
         this.isSearch=true;

        }
    }
    ];