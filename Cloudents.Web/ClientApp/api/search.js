import Vue from 'vue';
import VueResource from 'vue-resource';
Vue.use(VueResource)
const basePath = 'api/search';
const searchFunctions= {
    document: { method: 'GET', url: basePath+'/documents/{?query*}' },
    askData: { method: 'GET', url: basePath+'/qna/{?query*}' },
    wolfram: { method: 'GET', url: basePath+'/title/{?query*}' }
}
const resource = Vue.resource('${basePath}/', {}, searchFunctions)
export default {
    getShortAnswer(data, cb, errorCb) {
        return resource.wolfram(
            {
                term: 'president of argentina',
            }).then(({ body }) => Promise.resolve(body))
    },
    getDocument(data, cb, errorCb) {
        return resource.document(
            {
                source: null,
                university: null,
                course: null,
                query: ["war"],
                page: 0,
                sort: "Relevance"
            }).then(({ body }) => Promise.resolve(body));
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
    getQna(data, errorCb) {
        return resource.askData(
            {
                source: null,
                university: null,
                course: null,
                query: ["war"],
                page: 0,
                sort: "Relevance"
            }).then(({ body }) => Promise.resolve(body));
    },

    getTutor(data, cb, errorCb) {
        const resource = Vue.resource('api/tutor/{?query*}');
        resource.get(
            {
                term: "math"
            }).then((response) => {
            cb(response.body);
        });
    }
};