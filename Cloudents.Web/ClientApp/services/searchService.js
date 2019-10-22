﻿import { connectivityModule } from "./connectivity.module"

let currentVertical = 'item';

const getFeeds = (params) => {
    return connectivityModule.http.get("/feed", { params });
};

const getTutor = (params) => {
    return connectivityModule.http.get("tutor/search", { params });
};

const getNextPage = ({ url, vertical }) => {
    
    currentVertical = vertical;
    return connectivityModule.http.get(url, { baseURL: "" });
};

const getTutorsByCourse = (courseName) => {
    let path = courseName ? `tutor?course=${encodeURIComponent(courseName)}` : 'tutor';
    return connectivityModule.http.get(path);
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
    this.files = objInit.files;
    this.user = objInit.user;
    this.isRtl = objInit.isRtl;
    // this.votes = !!objInit.vote ? objInit.vote.votes : undefined;
    // this.upvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "up" ? true : false) : false) : undefined;
    // this.downvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "down" ? true : false) : false) : undefined;
}

function createAnswerItem(objInit) {
    return new AnswerItem(objInit);
}

function QuestionItem(objInit) {
    let oneMinute = 60000;
    let oneHour = oneMinute * 60;
    let threshhold = oneHour * 4;
    this.id = objInit.id || null;
    this.text = objInit.text || '';
    this.type = objInit.type || 'Question';
    this.dateTime = objInit.dateTime || objInit.create;
    this.course = objInit.course || '';
    this.template = "ask";
    this.cultureInfo = objInit.cultureInfo || 'en';
    this.isRtl = objInit.isRtl;
    this.userId = objInit.userId || objInit.user.id;
    this.firstAnswer = objInit.firstAnswer ? createFirstAnswerItem(objInit.firstAnswer) : null;
    this.answers = objInit.answers !== undefined ? (typeof objInit.answers === "number" ? objInit.answers : objInit.answers.map(createAnswerItem)) : undefined;
    // if the question is younger then 1 minute then watching now will be 0
    //if question is older then threshold, watching now also gonna be 0 other wise random between 0 to 1
    let questionOlderTheOneMinute = (new Date().getTime() - new Date(this.dateTime).getTime()) > oneMinute;
    let questionYoungerThenThreshHold = (new Date().getTime() - new Date(this.dateTime).getTime()) < threshhold;
    this.watchingNow = questionOlderTheOneMinute ? (questionYoungerThenThreshHold ? ((Math.random() * 2) | 0) : 0) : 0; //Todo get value from server
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
    this.score = objInit.score || null;
    this.rating =  objInit.rate ? Number(objInit.rate.toFixed(2)): null;
    this.reviews = objInit.reviewsCount || 0;
    this.template = 'tutor';
    this.bio = objInit.bio || '';
    this.university = objInit.university || '';
    this.classes = objInit.classes || 0;
    this.courseCount = objInit.courseCount || 0;
    this.lessons = objInit.lessons || 0;
    this.subjects = objInit.subjects || [];
    this.isTutor = true;
}

function createTutorItem(objInit) {
    return new TutorItem(objInit);
}

function DocumentItemUser(objInit){
    this.id = objInit.id;
    this.image = objInit.image || '';
    this.name = objInit.name || '';
    this.score = objInit.score || 0; //TODO remove this
    this.isTutor = objInit.isTutor || false; //TODO remove this
}

function createDocumentItemUser(objInit) {
    return new DocumentItemUser(objInit);
}

function DocumentItem(objInit) {
    this.id = objInit.id || 1;
    this.course = objInit.course;
    this.dateTime = objInit.dateTime;
    this.downloads = objInit.downloads;
    this.purchased = objInit.purchased;
    this.snippet = objInit.snippet;
    // this.source = objInit.source;
    this.title = objInit.title;
    this.university = objInit.university;
    this.url = objInit.url;
    this.user = objInit.user ? createDocumentItemUser(objInit.user) : '';
    this.views = objInit.views;
    this.template = 'note'; //TODO remove this
    this.price = objInit.price;
    this.isPurchased = objInit.isPurchased; //TODO: I never return this    
    this.votes = !!objInit.vote ? objInit.vote.votes : null;
    this.upvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "up" ? true : false) : false) : null;
    this.downvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "down" ? true : false) : false) : null;   
    this.preview = objInit.preview;
    this.type = objInit.type || 'Document';
    this.documentType = objInit.documentType;
    this.itemDuration = objInit.duration;    
}

function createDocumentItem(objInit) {
    return new DocumentItem(objInit);
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
    tutor: (res) => transferResultTutor(res)
};

let transferResult = ({data}) => {
    let documents = data.result.map((doc) => {
        return transferMap[doc.type || 'tutor'](doc);
    })

    return {
        sort: data.sort || '',
        filters: data.filters,
        data: documents,
        nextPage: data.nextPageLink
    };
}

let transferNextPage = (res) => {
    return transferResult(res);    
};

const transferAnswerItem = ({ data }) => {    
    return data.map(createTutorItem);
};


function FilterItem(objInit) {
    this.key = objInit.key;
    this.value = objInit.value;
}

function FilterChunk(objInit) {
    this.id = objInit.id;
    this.title = objInit.title;
    this.data = [];
    this.dictionaryData = {};
    objInit.data.forEach((filterItem) => {
        if(filterItem.key !== null){
            this.data.push(new FilterItem(filterItem));
            this.dictionaryData[filterItem.key] = filterItem.value;
        }
    });
}

function Filters(objInit) {
    this.filterChunkList = [];
    this.filterChunkDictionary = [];
    objInit.forEach((filterChunk) => {
        let createdFilterChunk = new FilterChunk(filterChunk);
        this.filterChunkList.push(createdFilterChunk);
        this.filterChunkDictionary[createdFilterChunk.id] = createdFilterChunk;
    });
}

function createFilters (objInit) {
    return new Filters(objInit);
}

export default {
    activateFunction: {
        feed(params) {
            return getFeeds(params).then(transferResult);
        },
        tutor(params) {          
            return getTutor(params).then(transferResult);
        },
        getTutors(params) {
            return getTutorsByCourse(params).then(transferAnswerItem);
        }
    },
    nextPage: (params) => {
        return getNextPage(params).then(transferNextPage);
    },
    getTutorsByCourse, 
    createQuestionItem,
    createAnswerItem,
    createFilters,
    createTutorItem,
    createDocumentItem,
}