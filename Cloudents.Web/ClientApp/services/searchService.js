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
        sort: res.sort,
        filters: res.filters,
        nextPage: res.nextPageLink
    }
};

let transferResultNote = response => {
    let res = response.data;
    let result = res ? res.result : [];
    if (!res) return { data: [] };
    return {
        sort: res.sort,
        filters: res.filters,
        data: result.map(val => { return { ...val, template: 'item' } }), nextPage: res.nextPageLink }
};

let transferResultTutor = response => {
    let data = response.data;
    let body = data || {};
    return { 
        sort: body.sort,
        filters: body.filters,
        data: body.result.map((val) => {
             return {
                  ...val,
                   template: "tutor" 
                } 
            }),
        nextPage: body.nextPageLink 
        };
};

let transferJob = response => {
    let body = response.data;
    let { result: items, nextPageLink: nextPage } = body;
    return { 
        filters: body.filters,
        data: items ? items.map((val) => {
             return {
                  ...val,
                  template: "job" 
                }}) : [],
            nextPage 
        };
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
            return getQuestions(params).then(transferResultAsk);
        },
        note(params) {
            return getDocument(params).then(transferResultNote);
        },
        flashcard(params) {
            return getFlashcard(params).then(transferResultNote);
        },
        tutor(params) {
            return getTutor(params).then(transferResultTutor);
        },
        job(params) {
            return getJob(params).then(transferJob);
        },
        book(params) {
            return getBook(params).then(transferBook);
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