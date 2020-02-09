import {School} from './school.js';
function _createIsTutorState(str){
    if(str && str.toLowerCase() === 'ok')return 'ok';
    else if(str && str.toLowerCase() === 'pending')return 'pending';
    else return null;
}
export const User = {
    Default: function (objInit) {
        this.id = objInit.id || objInit.userId;
        this.name = objInit.name;
        this.image = objInit.image || '';
    },
    TutorDefault: function(objInit){
        this.price = objInit.price || 0;
        this.currency = objInit.currency;
        this.bio = objInit.bio || '';
        this.lessons = objInit.lessons || 0;
        this.discountPrice = objInit.discountPrice;
        this.subjects = objInit.subjects || [];
    },
    Tutor: function (objInit) {
        return Object.assign(
            new User.TutorDefault(objInit),
            {
                contentCount: objInit.contentCount,
                hasCoupon: objInit.hasCoupon,
                rate: objInit.rate || 0,
                reviewCount: objInit.reviewCount || 0,
                firstName: objInit.firstName || '',
                lastName: objInit.lastName || '',
                students: objInit.students || 0,
            }
        )
    },
    TutorItem: function(objInit){
        return Object.assign(
            new User.Default(objInit),
            new User.TutorDefault(objInit),
            {
                courses: objInit.courses || [],
                country: objInit.country,
                rating:  objInit.rate ? Number(objInit.rate.toFixed(2)): null,
                reviews: objInit.reviewsCount || 0,
                template: 'tutor-result-card',
                university: objInit.university || '',
                classes: objInit.classes || 0,
                isTutor: true,
            }
        )
    },
    Account: function(objInit){
        return Object.assign(
            new User.Default(objInit),
            {
                userType: objInit.userType,
                balance: objInit.balance,
                email: objInit.email,
                currencySymbol: objInit.currencySymbol,
                needPayment: objInit.needPayment,
                isTutor: objInit.isTutor && objInit.isTutor.toLowerCase() === 'ok',
                isTutorState: _createIsTutorState(objInit.isTutor),
                university: new School.University(objInit.university),
                courses: objInit.courses.map((course) => new School.Course(course)),
            }
        )
    }
}