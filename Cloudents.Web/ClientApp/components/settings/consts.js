export let searchObjects = {
    course: {
        emptyState: "To add your course tap on the plus sign",
        title: 'Select Courses',
        closeText: "done",
        searchApi: "getCorses",
        defaultFilter:'all',
        filters: [{ id: 'all', name: 'ALL COURSES' }, { id: 'myCourses', name: 'MY COURSES' }],
        actions: [{ id:"add",name:"+",component:'plus-button'}]
    }, university: {
        searchApi:"getUniversities",
        emptyState: "If you would like to add your school send us a note from the feedback link in the settings tab.",
        title: 'Select University',
        closeText: "X",
        click:function(){
            console.log('pojggfg');
            this.dialog=false;},
        defaultFilter: '',
        filters: [],
        actions: []
    }
}
export let settingMenu=[
    {id:"university",name:"Choose university",click:function () {
        this.showDialog=true;this.type="university"
    }},{id:"myCourses",name:"My courses",click:function(){
        this.showDialog=true;this.type="course"
    }},{id:"walkthrough",name:"Walkthrough",click:function(){this.$router.push({name:'walkthrough'})}},{id:"aboutUs",name:"About us",click:function(){this.$router.push({name:'aboutUs'})}}];