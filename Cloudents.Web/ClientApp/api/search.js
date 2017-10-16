import {search as resource} from './resources';
export default {
    getShortAnswer(term) {
        return resource.wolfram(
            {
                term
            });
    },
    getVideo(term) {
        return resource.video(
            {
                term
            });
    },
    getDocument(data) {
        return resource.document(data);
    },
    getFlashcard(data) {
        return resource.flashcard(data);
    },
    getQna(data) {
        return resource.askData(data);
    },

    getTutor(params) {
        return resource.tutor(params);
    },
    getJob(params) {
        return resource.job(params);
    },
    getBook(params) {
        return resource.book(params);
    },
    getBookDetails(params) {
        return resource.bookDetails(params);
    },
    getFood(params) {
        return resource.food({ ...params, location: "34.8016837,31.9195509"});
    }
};