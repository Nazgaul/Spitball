import { university,course } from './resources';
export default {
    getUniversity(term) {
        return university.get({term})
    },
    getCourse(params) {
        return course.get(params)
    },
    createCourse(model) {
        return course.create({name:"yifat"})
    }
}