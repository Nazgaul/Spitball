import {search as resource} from './resources';
export default {
    getShortAnswer(term) {
        return resource.wolfram(
            {
                term: term
            });
    },
    getVideo(term) {
        return resource.video(
            {
                term: term
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

    getTutor(term) {
        return resource.tutor(
            {
                term: term
            });
    },
    getJob(term) {
        return resource.job(
            {
                term: term
            });
    }
};