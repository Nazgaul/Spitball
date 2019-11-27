import { LanguageService } from './language/languageService';

function createLastImageMsg() {
    return `<img src="${require('../components/chat/images/photo-camera-small.png')}" /><span>${LanguageService.getValueByKey('chat_photo')}</span>`;
}

const HomePage = {
    Stats: function(objInit){
        this.documents = objInit.documents;
        this.tutors = objInit.tutors;
        this.students = objInit.students;
        this.reviews = objInit.reviews;
    },
    Review: function(objInit){
        this.text = objInit.text;
        this.userName = objInit.userName;
        this.tutorImage = objInit.tutorImage;
        this.tutorName = objInit.tutorName;
        this.tutorId = objInit.tutorId;
        this.tutorReviews = objInit.tutorReviews;
    }
}
const User = {
    Default: function(objInit){
        this.image = objInit.image;
        this.name = objInit.name;
        this.userId = objInit.userId;
    }  
}
const Tutoring = {
    StudyRoom: function(objInit) {
        return Object.assign(
            new User.Default(objInit),
            new Chat.Default(objInit),
            new Status.Default(objInit)
        )
    }
}
const Chat = {
    Default: function(objInit,id){
        this.dateTime = objInit.dateTime || new Date().toISOString();
        this.conversationId = objInit.conversationId || id;
    },
    Conversation: function(objInit){
        return Object.assign(
            new User.Default(objInit),
            new Chat.Default(objInit),
            {
                online: objInit.online,
                unread: objInit.unread || 0,
                studyRoomId: objInit.studyRoomId,
                lastMessage: objInit.lastMessage || createLastImageMsg(),
            }
        )
    },
    TextMessage: function(objInit,id,fromSignalR){
        return Object.assign(
            new User.Default(objInit),
            new Chat.Default(objInit,id),
            {
                text : objInit.text,
                type : objInit.type,
                fromSignalR : fromSignalR || false,
                unreadMessage : objInit.unreadMessage || objInit.unread,
                isDummy : objInit.isDummy || false
            }
        )   
    },
    FileMessage: function(objInit,id,fromSignalR){
        return Object.assign(
            new User.Default(objInit),
            new Chat.Default(objInit,id),
            {
                src :objInit.src,
                href :objInit.href,
                type :objInit.type,
                fromSignalR :fromSignalR || false,
                unreadMessage :objInit.unreadMessage || objInit.unread
            }
        )
    },
    ActiveConversation: function(objInit){
        return Object.assign(
            new User.Default(objInit),
            new Chat.Default(objInit)
        )
    }
}
const Upload = {
    Default: function(objInit){
        this.id = objInit.id || '';
        this.blobName = objInit.blobName || '';
        this.name = objInit.name || '';
        this.course = objInit.course || '';
        this.price = objInit.price || '';
        this.link  = objInit.link || '';
        this.size  = objInit.bytes || 0;
        this.description = objInit.description || '';
    },
    FileData: function(objInit){
        return Object.assign(
            new Upload.Default(objInit),
            {
                progress: objInit.progress || 100,
                error: objInit.error || false,
                errorText: objInit.errorText || '',
            }
        )
    },
    ServerFormatFileData: function(objInit){
        return Object.assign(
            new Upload.Default(objInit)
        )
    }
}
const Status = {
    Default: function(objInit){
        this.online = objInit.online || false;
        this.id = objInit.id;
    },
    UserStatus: function(objInit){
        return Object.assign(
            new Status.Default(objInit)
        )  
    }
}
const Item = {
    Default: function(objInit){
    },
    Tutor: function(objInit){
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
}

export{
    HomePage,
    User,
    Tutoring,
    Chat,
    Upload,
    Status,
    Item
}

