﻿import { prefix } from './../components/data'
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
                    resolve({ title: short.body, data: [{ url:video.body.url, template: "video" },...items.body.map(val => { return { ...val, template: "item" } })] })
                })
            }
        })
    },
    note: (params) => {
        return new Promise((resolve, reject) => {
            search.getDocument(params).then(({ body }) => resolve({ isEmpty: !Boolean(body.item1.length), source: body.item2, data: body.item1.map(val => {return { ...val, template:"item"}}) }))
        })
    },
    flashcard: function (params) {
        return new Promise((resolve, reject) => {
            search.getFlashcard(params).then(({ body }) => resolve({ isEmpty: !Boolean(body.item1.length), source: body.item2, data: body.item1.map(val => Object.assign(val, { template: "item" })) }))
        })
    },
    tutor: function (params) {
        return new Promise((resolve, reject) => {
            search.getTutor(params).then(({ body }) => resolve({ data: body.map(val => { return {...val,template:"tutor"}}) }));
        })
    },
    job: function (params) {
        return new Promise((resolve, reject) => {
            search.getJob(params).then(({ body }) => resolve({ isEmpty: !Boolean(body.length), data: body.map(val => { return { ...val, template: "job" } }) }));
        })
    },
    book: function (params) {
        return new Promise((resolve, reject) => {
            search.getBook(params).then(({ body }) => resolve({ data: body.map(val => { return {...val,template:"book"}}) }) );
        })
    },
    food: function (params) {
        return new Promise((resolve, reject) => {
            search.getFood(params).then(({ body }) => resolve({isEmpty:!body.length,data: body.map(val => { return { ...val, template: "food" } })}));
        })
    }
}