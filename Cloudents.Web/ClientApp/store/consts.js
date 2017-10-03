import { prefix } from './../components/data'
import search from './../api/search'

export const prefixes = prefix;
export const activateFunction = {
    ask: function (params) {
        return new Promise((resolve, reject) => {
            var items = search.getQna(params);
            if (params.page) items.then(({ body }) => { resolve({ items:body }) })
            else {
                var answer = search.getShortAnswer(params.userText);
                var video = search.getVideo(params.userText);
                Promise.all([answer, items, video]).then(([short, items, video]) => {
                    resolve({ title: short.body, items: items.body, video: video.body.url })
                })
            }
        })
    },
    note: (params) => {
        return new Promise((resolve, reject) => {
            search.getDocument(params).then(({ body }) => resolve({ isEmpty: !Boolean(body.item1.length), data: { items: body.item1, source: body.item2 } }))
        })
    },
    flashcard: function (params) {
        return new Promise((resolve, reject) => {
            search.getFlashcard(params).then(({ body }) => resolve({ isEmpty: !Boolean(body.item1.length), data: { items: body.item1, source: body.item2 } }))
        })
    },
    tutor: function (params) {
        return new Promise((resolve, reject) => {
            search.getTutor(params).then(({ body }) => resolve({ items: body }));
        })
    },
    job: function (params) {
        return new Promise((resolve, reject) => {
            search.getJob(params).then(({ body }) => resolve({ isEmpty: !Boolean(body.length), data: { items: body } }));
        })
    },
    book: function (params) {
        return new Promise((resolve, reject) => {
            search.getBook(params.term, params.page).then(({body}) => resolve({ items: body }));
        })
    },
    purchase: function () {
        return new Promise((resolve, reject) => {
            search.getTutor(state.userText).then(response => resolve({ items: response.item1, sources: response.item2 }));
        })
    }
}