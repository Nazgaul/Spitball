import settingsService from './../services/settingsService'

const state = {
    universityId: '',
    myCourses:[]
};


const actions = {
    getUniversities(context, text) {
        return settingsService.getUniversity(text)
    },
    getCorses(context, data) {
        return settingsService.getCourse(data)
    },
    createCourse(context, data) {
        return settingsService.createCourse(data)
    }
}
export default {
    state,
    actions
}