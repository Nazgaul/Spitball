import { getUniversity, course } from './resources';
export default {
    getUniversity({term,location}) {
        return getUniversity({term,location});
    },
    getCourse(params) {
        return course.getCourse(params);
    },
    createCourse(model) {
        return course.createCourse(model);
    }
}