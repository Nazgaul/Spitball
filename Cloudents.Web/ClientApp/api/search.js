import {search as resource} from './resources';
export default {
    getShortAnswer(term) {
        return resource.wolfram(
            {
                term: term,
            });
    },
    getDocument(data) {
        return resource.document(
            {
                source: null,
                university: null,
                course: null,
                query: ["war"],
                page: 0,
                sort: "Relevance"
            });
    },
    getFlashcard(data) {
        return resource.flashcard(
            {
                source: null,
                university: null,
                course: null,
                query: ["war"],
                page: 0,
                sort: "Relevance"
            });
    },
    getQna(data) {
        return resource.askData(
            {
                source: null,
                university: null,
                course: null,
                query: ["war"],
                page: 0,
                sort: "Relevance"
            });
    },

    getTutor(term) {
        return resource.tutor(
            {
                term: term
            });
    }
};