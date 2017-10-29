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
        defaultFilter: '',
        filters: [],
        actions: []
    }
}
export let emptyStates = {};
export let filtersAction = {};
for (var v in searchObjects) {
    let item = searchObjects[v]
    emptyStates[v] = item.emptyState
    filtersAction[v] = { filters: item.filters, defaultFilter: item.defaultFilter, actions: item.actions }
}