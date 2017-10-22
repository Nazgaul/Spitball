import { search} from './resources';
export default {
    activateFunction :{
        ask: function (params) {
            return new Promise((resolve, reject) => {
                var items = search.getQna(params);
                if (params.page) items.then(({ body }) => { resolve({ items: body }) })
                else {
                    var answer = Promise.resolve({})
                    var video = Promise.resolve({ body: {}})
                    if (params.userText){
                        answer = search.getShortAnswer({term:params.q });
                        video = search.getVideo({ term: params.q });
                }
                    Promise.all([answer, items, video]).then(([short, items, video]) => {
                        resolve({ title: short.body, data: [{ url: video.body.url, template: "video" }, ...items.body.map(val => { return { ...val, template: "item" } })] })
                    })
                }
            })
        },
        note: (params) => {
            console.log(params);
            return new Promise((resolve, reject) => {
                search.getDocument(params).then(({ body }) => resolve({ source: body.facet, data: body.result.map(val => { return { ...val, template: "item" } }) }))
            })
        },
        flashcard: function (params) {
            return new Promise((resolve, reject) => {
                search.getFlashcard(params).then(({ body }) => resolve({ source: body.facet, data: body.result.map(val => Object.assign(val, { template: "item" })) }))
            })
        },
        tutor: function (params) {
            return new Promise((resolve, reject) => {
                search.getTutor(params).then(({ body }) => resolve({ data: body.map(val => { return { ...val, template: "tutor" } }) }));
            })
        },
        job: function (params) {
            return new Promise((resolve, reject) => {
                search.getJob(params).then(({ body }) => resolve({ jobType: body.facet, data: body.result.map(val => { return { ...val, template: "job" } }) }));
            })
        },
        book: function (params) {
            return new Promise((resolve, reject) => {
                search.getBook(params).then(({ body }) => resolve({ data: body.map(val => { return { ...val, template: "book" } }) }));
            })
        },
        bookDetails: function (params) {
            return new Promise((resolve, reject) => {
                search.getBookDetails({ type: params.type, isbn13: params.id}).then(({ body }) => {
                    resolve({ details: body.details, data: body.prices.map(val => { return { ...val, template: "book-price" } }) })
                });
            })
        },
        food: function (params) {
            return new Promise((resolve, reject) => {
                if (params.page) { resolve({ data: [] }); return; }
                search.getFood({ ...params, location: "34.8016837,31.9195509" }).then(({ body }) => resolve({ data: body.item2.map(val => { return { ...val, template: "food" } }) }));
            })
        }
    }
}