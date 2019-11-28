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
    Event: function(objInit,fromSignalR){
        this.type = objInit.type;
        this.unreadMessage = objInit.unreadMessage || objInit.unread;
        this.fromSignalR = fromSignalR || false;
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
            new Chat.Event(objInit,fromSignalR),
            {
                text : objInit.text,
                isDummy : objInit.isDummy || false
            }
        )   
    },
    FileMessage: function(objInit,id,fromSignalR){
        return Object.assign(
            new User.Default(objInit),
            new Chat.Default(objInit,id),
            new Chat.Event(objInit,fromSignalR),
            {
                src :objInit.src,
                href :objInit.href,
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
        this.id = objInit.id; 
    },
    Tutor: function(objInit){
        return Object.assign(
            new User.Default(objInit),
            {                
                courses: objInit.courses || [],
                price: objInit.price || 0,
                discountPrice: objInit.discountPrice,
                country: objInit.country,
                currency: objInit.currency,
                rating:  objInit.rate ? Number(objInit.rate.toFixed(2)): null,
                reviews: objInit.reviewsCount || 0,
                template: 'tutor-result-card',
                bio: objInit.bio || '',
                university: objInit.university || '',
                classes: objInit.classes || 0,
                lessons: objInit.lessons || 0,
                subjects: objInit.subjects || [],
                isTutor: true,
            }
        )  
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