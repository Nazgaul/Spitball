import { connectivityModule } from "./connectivity.module"
import {Item} from './Dto/item.js';

const getFeeds = (params) => {
    return connectivityModule.http.get("/feed", { params });
};

const getNextPage = ({ url }) => {
    return connectivityModule.http.get(url, { baseURL: "" });
};


function FirstAnswerItem(objInit) {
    this.date = objInit.dateTime || null;
    this.text = objInit.text || '';
    this.user = objInit.user || {};
}

function createFirstAnswerItem(objInit) {
    return new FirstAnswerItem(objInit);
}

function AnswerItem(objInit) {
    this.id = objInit.id;
    this.text = objInit.text;
    this.create = objInit.create;
    this.user = objInit.user;
}

function createAnswerItem(objInit) {
    return new AnswerItem(objInit);
}

function QuestionItem(objInit) {
    this.id = objInit.id || null;
    this.text = objInit.text || '';
    this.type = objInit.type || 'Question';
    this.dateTime = objInit.dateTime || objInit.create;
    this.course = objInit.course || '';
    this.user = objInit.user || null;    
    this.template = "result-ask";
    this.userId = objInit.userId || objInit.user.id;
    this.user = objInit.user || null;    
    this.firstAnswer = objInit.firstAnswer ? createFirstAnswerItem(objInit.firstAnswer) : null;
    this.answers = objInit.answers !== undefined ? (typeof objInit.answers === "number" ? objInit.answers : objInit.answers.map(createAnswerItem)) : undefined;
}

function createQuestionItem(objInit) {
    return new QuestionItem(objInit);
}

function TutorItem(objInit) {
    this.userId = objInit.userId;
    this.name = objInit.name || '';
    this.image = objInit.image;
    this.courses = objInit.courses || [];
    this.price = objInit.price || 0;
    this.discountPrice = objInit.discountPrice;
    this.country = objInit.country;
    this.currency = objInit.currency;
    this.rating =  objInit.rate ? Number(objInit.rate.toFixed(2)): null;
    this.reviews = objInit.reviewsCount || 0;
    this.template = 'tutor-result-card';
    this.bio = objInit.bio || '';
    this.university = objInit.university || '';
    this.classes = objInit.classes || 0;
    this.lessons = objInit.lessons || 0;
    this.subjects = objInit.subjects || [];
    this.isTutor = true;
}

function createTutorItem(objInit) {
    return new TutorItem(objInit);
}

function createDocumentItem(objInit) {
    return new Item[objInit.documentType](objInit)
}


/* Question Card Result */
let transferResultQuestion = (data) => {
    return (!data) ? [] : createQuestionItem(data);
};

/* Study Document Card Result */
let transferResultDocument = (data) => {    
    return (!data) ? [] : createDocumentItem(data);
};

/* Tutor Card Result */
let transferResultTutor = (data) => {
    return (!data) ? [] : createTutorItem(data);
};

const transferMap = {
    Question: (res) => transferResultQuestion(res),
    Document: (res) => transferResultDocument(res),
    Tutor: (res) => transferResultTutor(res)
};

let transferResult = ({data}) => {
    let documents = data.result.map((doc) => {
        return transferMap[doc.type](doc);
    });

    return {
        filters: data.filters,
        data: documents,
    };
};

let transferNextPage = (res) => {
    return transferResult(res);    
};

export default {
    activateFunction: {
        feed(params) {
            return getFeeds(params).then(transferResult);
        },
    },
    nextPage: (params) => {
        return getNextPage(params).then(transferNextPage);
    },
    createQuestionItem,
    createAnswerItem,
    createTutorItem,
    createDocumentItem,
}