 import { transformLocation } from "./resources";
import { connectivityModule } from "./connectivity.module"

let currentVertical = 'item';

const getQuestions = (params) => {
    return connectivityModule.http.get("/Question", { params });
}

const getDocument = (params) => {
    return connectivityModule.http.get("search/documents", { params });
}

const getFlashcard = (params) => {
    return connectivityModule.http.get("search/flashcards", { params });
}

const getTutor = (params) => {
    return connectivityModule.http.get("tutor", { params: transformLocation(params) })
}

const getJob = (params) => {
    return connectivityModule.http.get("job", { params: transformLocation(params) })
}

const getBook = (params) => {
    return connectivityModule.http.get("book/search", { params })
}

const getBookDetails = ({ type, isbn13 }) => {
    return connectivityModule.http.get(`book/${type}`, { params: { isbn13 } })
}

const getNextPage = ({ url, vertical }) => {
    currentVertical = vertical;
    return connectivityModule.http.get(url, { baseURL: "" })
}

const autoComplete = (data) => {
    return connectivityModule.http.get("suggest", { params: { sentence: data.term, vertical: data.vertical } })
} 

function QuestionItem(objInit){
    this.id = objInit.id;
    this.subject = objInit.subject;
    this.price = objInit.price;
    this.text = objInit.text;
    this.files = objInit.files;
    this.answers = objInit.answers;
    this.user = objInit.user;
    this.dateTime = objInit.dateTime;
    this.color = !!objInit.color ? objInit.color : undefined;
    this.hasCorrectAnswer = objInit.hasCorrectAnswer;
    this.template = objInit.template;
    this.filesNum = objInit.filesNum;
    this.answersNum = objInit.answersNum;
    this.template = "ask";
    this.filesNum = this.files;
    this.answersNum = this.answers;
    this.watchingNow = 0;
}


let transferResultAsk = response => {
    let res = response.data;
    let itemResult = res.result || [];
    let items = itemResult.map(val => {
            return new QuestionItem(val);
        });
    return { 
        data: items,
        source: res.result.filters,
        filters: res.filters,
        nextPage: res.nextPageLink
    }
};

let transferResultNote = response => {
    let res = response.data;
    let result = res ? res.result : [];
    if (!res) return { data: [] };
    return { source: res.filters, filters: res.filters, data: result.map(val => { return { ...val, template: 'item' } }), nextPage: res.nextPageLink }
};

let transferResultTutor = response => {
    let data = response.data;
    let body = data || {};
    return { data: body.result.map(val => { return { ...val, template: "tutor" } }), nextPage: body.nextPageLink };
};

let transferJob = response => {
    let body = response.data;
    let { result: items, nextPageLink: nextPage, facet: jobType } = body;
    return { jobType, data: items ? items.map(val => { return { ...val, template: "job" } }) : [], nextPage };
};

let transferBook = response => {
    let body = response.data;
    body = body || {};
    let data = body.result.map(val => { return { ...val, template: "book" } });
    return { data, nextPage: body.nextPageLink }
};

let transferBookDetails = response => {
    let body = response.data;
    let prices = body.prices || [];
    return { details: body.details, data: prices.map(val => { return { ...val, template: "book-price" } }) }
};

let transferNextPage = (res) => {
    let { data, nextPage } = transferMap[currentVertical](res);
    return { data, nextPage }
};

const transferMap = {
    ask: (res) => transferResultAsk(res),
    flashcard: (res) => transferResultNote(res),
    note: (res) => transferResultNote(res),
    job: (res) => transferJob(res),
    tutor: (res) => transferResultTutor(res),
    book: (res) => transferBook(res)
}


export default {
    activateFunction: {
        // ask({ source, term=""}) {
        //     return getQuestions({term, source}).then(transferResultAsk);
        // },
        ask(params) {
            console.log(params)
            return getQuestions(params).then(transferResultAsk);
        },
        note({ source, university, course, term="", page, sort }) {
            return getDocument({ source, university, course, query:term, page, sort }).then(transferResultNote);
        },
        flashcard({ source, university, course, term="", page, sort }) {
            return getFlashcard({ source, university, course, query:term, page, sort }).then(transferResultNote);
        },
        tutor({ term="", filter, sort, page, location }) {
            return getTutor({ term, filter, sort, location, page }).then(transferResultTutor);
        },
        job({ term="", filter, sort, jobType: facet, page, location }) {
            return getJob({ term, filter, sort, location, facet: filters, page }).then(transferJob);
        },
        book({ term="", page }) {
            return getBook({ term, page }).then(transferBook);
        },
        bookDetails({ type, isbn13 }) {
            return getBookDetails({ type, isbn13 }).then(transferBookDetails);
        }
    },
    autoComplete:(term)=>{
        return autoComplete(term);
    },

    nextPage:(params)=>{
        return getNextPage(params).then(transferNextPage)
    },

    createQuestionItem:(objInit)=>{
        return new QuestionItem(objInit);
    }
}