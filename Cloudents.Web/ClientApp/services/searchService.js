import { search } from './resources';
let location = null;
export default {
    activateFunction :{
        ask: function (params) {
            return new Promise((resolve, reject) => {
                var items = search.getQna(params);
                if (params.page) items.then(({ body }) => {
                    resolve({ data: body.map(val => { return { ...val, template: "item" } }) })
                })
                else {
                    var answer = Promise.resolve({})
                    var video = Promise.resolve({ body: {}})
                    if (params.q){
                        answer = search.getShortAnswer({term:params.q });
                        video = search.getVideo({ term: params.q });
                }
                    Promise.all([answer, items, video]).then(([short, items, video]) => {
                        let list = items.body.map(val => { return { ...val, template: "item" } })
                        video.body.url ? list = [{ url: video.body.url, template: "video" }, ...list]:''
                        resolve({ title: short.body, data: list })
                    })
                }
            })
        },
        note: (params) => {
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
                search.getBook(params).then(({ body }) => {
                    let data = body ? body.map(val => { return { ...val, template: "book" } }):[]
                    resolve({ data })
                });
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
                if (params.page) {
                    console.log("pageeee")
                    search.getFood({ nextPageToken: params.page/*, location: "34.8016837,31.9195509"*/ }).then(({ body }) => resolve({ token: body.token, data: body.data.map(val => { return { ...val, template: "food" } }) }));
                }
                if (!location) {
                    if (navigator.geolocation) {
                        navigator.geolocation.getCurrentPosition(({ coords }) => {
                            location = coords.latitude + ',' + coords.longitude;
                            params = { ...params, location }
                            search.getFood(params).then(({ body }) => resolve({ token: body.token, data: body.data.map(val => { return { ...val, template: "food" } }) }));
                        });
                    } else {
                        search.getFood(params).then(({ body }) => resolve({ token: body.token, data: body.data.map(val => { return { ...val, template: "food" } }) }));
                    }
                } else {
                    params = { ...params, location }
                    search.getFood(params).then(({ body }) => resolve({ token: body.token, data: body.data.map(val => { return { ...val, template: "food" } }) }));
                }
            })
        }
    }
}