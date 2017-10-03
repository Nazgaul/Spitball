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
    getBook(term,page) {
        return resource.book(
            {
                term ,page
            });
    }
};