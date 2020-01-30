import { connectivityModule } from "./connectivity.module"

const getFeeds = (params) => {
    return connectivityModule.http.get("/feed", { params });
};

const getTutor = (params) => {
    return connectivityModule.http.get("tutor/search", { params });
};

const getNextPage = ({ url }) => {
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

function DocumentItemUser(objInit){
    this.id = objInit.id;
    this.image = objInit.image || '';
    this.name = objInit.name || '';
}

function createDocumentItemUser(objInit) {
    return new DocumentItemUser(objInit);
}

function DocumentItem(objInit) {
    this.id = objInit.id || null; 
    this.course = objInit.course; 
    this.dateTime = objInit.dateTime; 
    this.downloads = objInit.downloads; 
    this.purchased = objInit.purchased; 
    this.snippet = objInit.snippet;
    this.title = objInit.title;
    this.university = objInit.university;
    this.url = objInit.url;
    this.user = objInit.user ? createDocumentItemUser(objInit.user) : '';
    this.views = objInit.views;
    this.template = 'result-note';
    this.price = objInit.price;
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
    Tutor: (res) => transferResultTutor(res)
};

let transferResult = ({data}) => {
    let documents = data.result.map((doc) => {
        return transferMap[doc.type](doc);
    });

    return {
        sort: data.sort || '',
        filters: data.filters,
        data: documents,
        nextPage: data.nextPageLink
    };
};

let transferNextPage = (res) => {
    return transferResult(res);    
};

const transferAnswerItem = ({ data }) => {    
    return data.map(createTutorItem);
};

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
    createTutorItem,
    createDocumentItem,
}