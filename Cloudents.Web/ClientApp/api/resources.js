import Vue from 'vue';
import VueResource from 'vue-resource';
Vue.use(VueResource)
const basePath = 'api/';
const searchFunctions = {
    document: { method: 'GET', url: basePath + 'search/documents/{?term*}' },
    askData: { method: 'GET', url: basePath + 'search/qna/{?term*}' },
    wolfram: { method: 'GET', url: basePath + 'title/{?term*}' },
    flashcard: { method: 'GET', url: basePath + 'search/flashcards/{?term*}' },
    tutor: { method: 'GET', url: basePath + 'tutor/{?term*}' },
    video: { method: 'GET', url: basePath + 'video/{?term*}' },
    job: { method: 'GET', url: basePath + 'job/{?term*}' },
    book: { method: 'GET', url: basePath + 'book/search/{?term*}' },
    food: { method: 'GET', url: basePath + 'places/{?term*}' }
}
export const search = Vue.resource(basePath, {}, searchFunctions)