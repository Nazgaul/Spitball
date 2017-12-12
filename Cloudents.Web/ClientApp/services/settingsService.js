import { getUniversity,course } from './resources';
export default {
    getUniversity(term) {
        return getUniversity({term});
    },
    getCourse(params) {
        return course.getCourse(params);
    },
    createCourse(model) {
        return course.createCourse(model);
    }
}