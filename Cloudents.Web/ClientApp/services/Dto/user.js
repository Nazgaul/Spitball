import {School} from './school.js';
function _createIsTutorState(str){
    if(str && str.toLowerCase() === 'ok')return 'ok';
    else if(str && str.toLowerCase() === 'pending')return 'pending';
    else return null;
}
let DummyUni = {id: "80b226ae-94a1-4240-8796-a98200e81a54",
name: "האוניברסיטה הפתוחה",
country: "IL",
image: "https://az32006.vo.msecnd.net/universities/920.jpg",
usersCount: 27962}
let DummyCourses = [{"name":"Economics","students":162,"isTeaching":true},{"name":"Physics","students":103,"isTeaching":true},{"name":"Computer Science 😀","students":72,"isTeaching":true},{"name":"Computer science notes 2","students":72,"isTeaching":true},{"name":"Temp","students":53,"isTeaching":true},{"name":"ג'נוסייד - רצח עם","students":9,"isTeaching":true},{"name":"Something","students":1,"isTeaching":true},{"name":"10664 ג'נוסייד - רצח עם‎‏‏","students":1,"isTeaching":true},{"name":"asdfasf33","students":1,"isTeaching":true}]

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
                balance: objInit.balance,
                email: objInit.email,
                phoneNumber: objInit.phoneNumber,
                currencySymbol: objInit.currencySymbol,
                needPayment: objInit.needPayment,
                universityExists: objInit.universityExists,
                isTutor: objInit.isTutor && objInit.isTutor.toLowerCase() === 'ok',
                isTutorState: _createIsTutorState(objInit.isTutor),

                // university: new School.University(objInit.university),
                // courses: objInit.courses.map((course) => new School.Course(course)),

                university: new School.University(DummyUni),
                courses: DummyCourses.map((course) => new School.Course(course)),
            }
        )
    }
}