import { transformLocation } from "./resources";
import { connectivityModule } from "./connectivity.module"

let currentVertical = 'item';

const getQuestions = (params) => {
    return connectivityModule.http.get("/Question", { params });
}

const getDocument = (params) => {
    return connectivityModule.http.get("/Document", { params });
}


const getTutor = (params) => {
    return connectivityModule.http.get("tutor/search", { params: transformLocation(params) })
}

const getNextPage = ({ url, vertical }) => {
    currentVertical = vertical;
    return connectivityModule.http.get(url, { baseURL: "" })
}

const autoComplete = (data) => {
    return connectivityModule.http.get("suggest", { params: { sentence: data.term, vertical: data.vertical } })
}

function AnswerItem(objInit) {
    this.id = objInit.id;
    this.text = objInit.text;
    this.create = objInit.create;
    this.files = objInit.files;
    this.user = objInit.user;
    this.isRtl = objInit.isRtl;
    this.votes = !!objInit.vote ? objInit.vote.votes : undefined;
    this.upvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "up" ? true : false) : false) : undefined;
    this.downvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "down" ? true : false) : false) : undefined;
}

function createAnswerItem(objInit) {
    return new AnswerItem(objInit)
}

function QuestionItem(objInit) {
    let oneMinute = 60000;
    let oneHour = oneMinute * 60;
    let threshhold = oneHour * 4;
    this.id = objInit.id;
    this.subject = objInit.subject;
    this.text = objInit.text;
    this.files = objInit.files;
    this.answers = objInit.answers !== undefined ? (typeof objInit.answers === "number" ? objInit.answers : objInit.answers.map(createAnswerItem)) : undefined;
    this.user = objInit.user;
    this.dateTime = objInit.dateTime || objInit.create;
    this.hasCorrectAnswer = objInit.hasCorrectAnswer;
    this.correctAnswerId = objInit.correctAnswerId;
    this.course = objInit.course;
    this.template = "ask";
    this.filesNum = this.files;
    this.isRtl = objInit.isRtl;
    this.votes = !!objInit.vote ? objInit.vote.votes : undefined;
    this.upvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "up" ? true : false) : false) : undefined;
    this.downvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "down" ? true : false) : false) : undefined;
    // if the question is younger then 1 minute then watching now will be 0
    //if question is older then threshold, watching now also gonna be 0 other wise random between 0 to 1
    let questionOlderTheOneMinute = (new Date().getTime() - new Date(this.dateTime).getTime()) > oneMinute;
    let questionYoungerThenThreshHold = (new Date().getTime() - new Date(this.dateTime).getTime()) < threshhold;
    this.watchingNow = questionOlderTheOneMinute ? (questionYoungerThenThreshHold ? ((Math.random() * 2) | 0) : 0) : 0; //Todo get value from server
}

function TutorItem(objInit) {
    this.userId = objInit.userId || 12;
    this.name = objInit.name || '';
    this.image = objInit.image;
    this.courses = objInit.courses || '';
    this.price = objInit.price || 50;
    this.score = objInit.score || null;
    this.rating =  objInit.rate ? Number(objInit.rate.toFixed(2)): null;
    this.reviews = objInit.reviewsCount || 0;
    this.template = 'tutor';
    this.bio = objInit.bio || '';

}

function DocumentItem(objInit) {
    this.id = objInit.id || 1;
    this.course = objInit.course;
    this.dateTime = objInit.dateTime;
    this.downloads = objInit.downloads;
    this.professor = objInit.professor;
    this.snippet = objInit.snippet;
    this.source = objInit.source;
    this.title = objInit.title;
    this.type = objInit.type;
    this.university = objInit.university;
    this.url = objInit.url;
    this.user = objInit.user;
    this.views = objInit.views;
    this.template = 'note';
    this.price = objInit.price;
    this.isPurchased = objInit.isPurchased;
    this.votes = !!objInit.vote ? objInit.vote.votes : null;
    this.upvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "up" ? true : false) : false) : null;
    this.downvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "down" ? true : false) : false) : null;
}


function createDocumentItem(objInit) {
    return new DocumentItem(objInit)
}
function createTutorItem(objInit) {
    return new TutorItem(objInit)
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
        data: result.map(createDocumentItem),
        nextPage: res.nextPageLink
    }
};

let transferResultTutor = response => {
    let data = response.data;
    let body = data || {};
    return {
        sort: body.sort,
        filters: body.filters,
        data: body.result.map(createTutorItem),
        nextPage: body.nextPageLink
    };
};


let transferNextPage = (res) => {
    let { data, nextPage } = transferMap[currentVertical](res);
    return { data, nextPage }
};

const transferMap = {
    ask: (res) => transferResultAsk(res),
    note: (res) => transferResultNote(res),
    tutor: (res) => transferResultTutor(res),
};



function FilterItem(ObjInit) {
    this.key = ObjInit.key;
    this.value = ObjInit.value;
}

function FilterChunk(ObjInit) {
    this.id = ObjInit.id;
    this.title = ObjInit.title;
    this.data = [];
    this.dictionaryData = {};
    ObjInit.data.forEach((filterItem) => {
        this.data.push(new FilterItem(filterItem));
        this.dictionaryData[filterItem.key] = filterItem.value
    })
}

function Filters(ObjInit) {
    this.filterChunkList = [];
    this.filterChunkDictionary = [];
    ObjInit.forEach((filterChunk) => {
        let createdFilterChunk = new FilterChunk(filterChunk);
        this.filterChunkList.push(createdFilterChunk);
        this.filterChunkDictionary[createdFilterChunk.id] = createdFilterChunk;
    })
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
        tutor(params) {
            return getTutor(params).then(transferResultTutor);
        },
    },
    autoComplete: (term) => {
        return autoComplete(term);
    },

    nextPage: (params) => {
        return getNextPage(params).then(transferNextPage)
    },

    createQuestionItem: (objInit) => {
        return new QuestionItem(objInit);
    },

    createAnswerItem: (objInit) => {
        return createAnswerItem(objInit);
    },

    createFilters: (objInit) => {
        return new Filters(objInit)
    },
    createTutorItem: (objInit) => {
        return createTutorItem(objInit)
    },

    createDocumentItem: (objInit) => {
        return createDocumentItem(objInit)
    },
}