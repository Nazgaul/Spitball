import Vue from 'vue';
import VueResource from 'vue-resource';
Vue.use(VueResource)
const basePath = 'api/';
const searchFunctions = {
    document: { method: 'GET', url: basePath + 'search/documents/{?query*}' },
    askData: { method: 'GET', url: basePath + 'search/qna/{?query*}' },
    wolfram: { method: 'GET', url: basePath + 'title/{?query*}' },
    flashcard: { method: 'GET', url: basePath + 'search/flashcards/{?query*}' },
    tutor: { method: 'GET', url: basePath + 'tutor/{?query*}' },
    video: { method: 'GET', url: basePath + 'video/{?query*}' }
}
export const search = Vue.resource(basePath, {}, searchFunctions)