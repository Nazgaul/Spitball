import Vue from 'vue';
import VueResource from 'vue-resource';
Vue.use(VueResource)
const basePath = 'api/';
const searchFunctions = {
    getDocument: { method: 'GET', url: basePath + 'search/documents/{?term*}' },
    getQna: { method: 'GET', url: basePath + 'ask/{?term*}' },
    getShortAnswer: { method: 'GET', url: basePath + 'title/{?term*}' },
    getFlashcard: { method: 'GET', url: basePath + 'search/flashcards/{?term*}' },
    getTutor: { method: 'GET', url: basePath + 'tutor/{?term*}' },
    getVideo: { method: 'GET', url: basePath + 'video/{?term*}' },
    getJob: { method: 'GET', url: basePath + 'job/{?term*}' },
    getBook: { method: 'GET', url: basePath + 'book/search/{?term*}' },
    getBookDetails: { method: 'GET', url: basePath + 'book/{type}/{?term*}' },
    getFood: { method: 'GET', url: basePath + 'places/{?term*}' }
}

const courseFunctions = {
    get: { method: 'GET'},
    create: { method: 'POST'}
}
export const university = Vue.resource(basePath + 'university/{?term*}');
export const search = Vue.resource(basePath, {}, searchFunctions);
export const course = Vue.resource(basePath + 'course', {}, courseFunctions);
export const flashcard = Vue.resource(basePath + 'flashcard');