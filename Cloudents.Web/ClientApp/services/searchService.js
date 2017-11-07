import { search } from './resources';
let location = null;
const previewMap = {
    item(id) {
        return {
            //blob: ["https://zboxstorage.blob.core.windows.net/zboxcahce/850de13a-001a-4284-9fa8-68d97dcd81adV6_0_.docx.svg?sv=2016-05-31&sr=b&sig=3Ymn%2BX5WFN%2BuBA3kaxImosiQQhdSeK0QmZNcItQQUX8%3D&st=2017-11-07T09%3A19%3A47Z&se=2017-11-07T09%3A40%3A47Z&sp=r", "https://zboxstorage.blob.core.windows.net/zboxcahce/850de13a-001a-4284-9fa8-68d97dcd81adV6_1_.docx.svg?sv=2016-05-31&sr=b&sig=zhbfiY%2Fi6Z%2F6i2995OevC1CXsJU9jTx7eaekzl8m9pA%3D&st=2017-11-07T09%3A19%3A47Z&se=2017-11-07T09%3A40%3A47Z&sp=r"]
//            blob: ["https://zboxstorage.blob.core.windows.net/zboxcahce/c0a06374-acc2-4815-89cc-8f5bb11cda4cV3_.cpp.html?sv=2016-05-31&sr=b&sig=4EowLDbARoM5%2F3CiSJe1YQwHVhnKEdbn%2BadeIuE%2BsPc%3D&st=2017-11-07T09%3A24%3A23Z&se=2017-11-07T09%3A55%3A23Z&sp=r"
//],
//            blob: ["https://www.spitball.co/item/%D7%94%D7%9E%D7%A1%D7%9C%D7%95%D7%9C-%D7%94%D7%90%D7%A7%D7%93%D7%9E%D7%99-%D7%94%D7%9E%D7%9B%D7%9C%D7%9C%D7%94-%D7%9C%D7%9E%D7%A0%D7%94%D7%9C/72742/%D7%90%D7%A0%D7%92%D7%9C%D7%99%D7%AA-%D7%9E%D7%AA%D7%A7%D7%93%D7%9E%D7%99%D7%9D/175573/%D7%9E-%D7%A0%D7%95%D7%A9%D7%90-410011-%D7%90%D7%A0%D7%92%D7%9C%D7%99%D7%AA-%D7%9E%D7%AA%D7%A7%D7%93%D7%9E%D7%99%D7%9D-%D7%9E%D7%95%D7%A2%D7%93-1-2.pdf/"
//],
            type: 'lijionk',
            blob: ["https://zboxstorage.blob.core.windows.net/zboxcahce/eaa2ec96-4b61-49cf-97da-6e1aaf45e9c3V4_0_.pdf.jpg?sv=2016-05-31&sr=b&sig=tpGJH2NVDgIR8j5xoC%2BG2UUk7%2F75nyB5OwKW4S9%2FliI%3D&st=2017-11-07T09%3A16%3A16Z&se=2017-11-07T09%3A37%3A16Z&sp=r", "https://zboxstorage.blob.core.windows.net/zboxcahce/eaa2ec96-4b61-49cf-97da-6e1aaf45e9c3V4_1_.pdf.jpg?sv=2016-05-31&sr=b&sig=XWjY5PbgnL%2B5FF3vf3tqQ3BzH4tWj%2FJQHh9g8eSZmM4%3D&st=2017-11-07T09%3A16%3A16Z&se=2017-11-07T09%3A37%3A16Z&sp=r", "https://zboxstorage.blob.core.windows.net/zboxcahce/eaa2ec96-4b61-49cf-97da-6e1aaf45e9c3V4_2_.pdf.jpg?sv=2016-05-31&sr=b&sig=u2C39%2B1E25zM9uZ363TzZhdpX5UUSddMMnpy0FMF7UA%3D&st=2017-11-07T09%3A16%3A16Z&se=2017-11-07T09%3A37%3A16Z&sp=r"],
            author: "Jamie Schneider",
            name:"Item name",
            date: "2014-01-03T14:02:12Z",

        }
    },
    flashcard(id) {
        return { name: 'flashcard' }
    },
    quiz(id) {
        return { name: 'quiz' }
    }
}
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
    },
    getPreview(data) {
        let previewFunc = previewMap[data.type] ? previewMap[data.type] : previewMap.item;

        return previewFunc(data.id);
    }
}