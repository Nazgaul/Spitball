import Vue from 'vue'
export default {
    getDocument(data, cb, errorCb) {
        const resource = Vue.resource('api/search/documents/{?query*}');
        resource.get(
            {
                source: null,
                university: null,
                course: null,
                query: ["war"],
                page: 0,
                sort: "Relevance"
            }).then((response) => {
                cb(response.body);
            });
    },
    getFlashcard(data, cb, errorCb) {
        const resource = Vue.resource('api/search/flashcards/{?query*}');
        resource.get(
            {
                source: null,
                university: null,
                course: null,
                query: ["war"],
                page: 0,
                sort: "Relevance"
            }).then((response) => {
                cb(response.body);
            });
    },
    getQna(data, cb, errorCb) {
        const resource = Vue.resource('api/search/qna/{?query*}');
        resource.get(
            {
                source: null,
                university: null,
                course: null,
                query: ["war"],
                page: 0,
                sort: "Relevance"
            }).then((response) => {
                cb(response.body);
            });
    }
}