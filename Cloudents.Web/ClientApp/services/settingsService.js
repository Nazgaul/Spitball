import { university } from './resources';
export default {
    getUniversity(term) {
        return university.get({term})
    }
}