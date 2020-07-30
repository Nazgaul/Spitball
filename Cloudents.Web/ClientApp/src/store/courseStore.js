import axios from 'axios'

const courseInstance = axios.create({
   baseURL:'/api/course'
})

const state = {
   courseDetails:null,
}
const mutations = {
   setCourseEnrolled(state,val){
      state.courseDetails.enrolled = val;
   },
   setCourseDetails(state,courseDetails){
      if(courseDetails?.id){
         state.courseDetails = new CourseDetails(courseDetails)
      }else{
         state.courseDetails = null
      }
      function CourseDetails(objInit){
         this.id = objInit.id; 
         this.name = objInit.name;
         this.image = objInit.image;
         this.enrolled = objInit.enrolled;
         this.full = objInit.full;
         this.description = objInit.description; 
         this.price = {
            amount: objInit.price?.amount,
            currency: objInit.price?.currency
         }
         this.items = objInit.documents.map(item=>{
            return {
               id: item.id,
               title: item.title,
               preview: item.preview,
               documentType: item.documentType,
            }
         })
         this.sessionStarted = objInit.sessionStarted || null;
         this.studyRooms = objInit.studyRooms;
         this.documents = objInit.documents
         this.tutorName = objInit.tutorName; 
         this.tutorImage = objInit.tutorImage;
         this.tutorId = objInit.tutorId; 
         this.tutorCountry = objInit.tutorCountry;
         this.tutorBio = objInit.tutorBio;
         this.startTime = objInit.broadcastTime;
      }
   }
}
const getters = {
   getCourseDetails: state => state.courseDetails,
   getCourseSessions: state => state.courseDetails?.studyRooms || [],
   getIsCourseTutor: (state,getters) => state.courseDetails?.tutorId == getters.getAccountId,
   getCoursePrice: state => state.courseDetails?.price || null,
   getCourseItems: state => state.courseDetails?.items || [],
}
const actions = {
   updateCourseDetails({commit},courseId){
      if(courseId){
         courseInstance.get(`${courseId}`).then(({data})=>{
            commit('setCourseDetails',data)
         })
      }else{
         commit('setCourseDetails',null)
      }
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}
/*
const fakeDocs = [
   {
     "id": 53197,
     "name": "img-(12)",
     "price": 0,
     "views": 5,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53197",
     "url": "/document/dsassa/img-12/53197",
     "type": "Document",
     "date": "2020-07-26T12:53:22.2365244Z",
     "course": "dsassa"
   },
   {
     "id": 53196,
     "name": "img-(12)",
     "price": 0,
     "views": 4,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53196",
     "url": "/document/dsassa/img-12/53196",
     "type": "Document",
     "date": "2020-07-26T12:47:00.5843348Z",
     "course": "dsassa"
   },
   {
     "id": 53156,
     "name": "img-(15)",
     "price": 0,
     "views": 7,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53156",
     "url": "/document/sdfsdfsf/img-15/53156",
     "type": "Document",
     "date": "2020-07-26T12:44:32.0932643Z",
     "course": "sdfsdfsf"
   },
   {
     "id": 53155,
     "name": "img-(12)",
     "price": 0,
     "views": 1,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53155",
     "url": "/document/sdfsdfsf/img-12/53155",
     "type": "Document",
     "date": "2020-07-26T10:48:58.9817841Z",
     "course": "sdfsdfsf"
   },
   {
     "id": 53154,
     "name": "img-(15)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53154",
     "url": "/document/sdfsdfsf/img-15/53154",
     "type": "Document",
     "date": "2020-07-26T10:48:50.492654Z",
     "course": "sdfsdfsf"
   },
   {
     "id": 53153,
     "name": "img-(12)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53153",
     "url": "/document/sdfasdfasdfas/img-12/53153",
     "type": "Document",
     "date": "2020-07-26T10:46:22.1417335Z",
     "course": "sdfasdfasdfas"
   },
   {
     "id": 53152,
     "name": "img-(16)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53152",
     "url": "/document/sdfasdfasdfas/img-16/53152",
     "type": "Document",
     "date": "2020-07-26T10:46:18.4011608Z",
     "course": "sdfasdfasdfas"
   },
   {
     "id": 53087,
     "name": "image (1)",
     "price": 0,
     "views": 20,
     "likes": 0,
     "downloads": 1,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53087",
     "url": "/document/fdgffdgh/image-1/53087",
     "type": "Document",
     "date": "2020-07-23T06:35:23.6691336Z",
     "course": "fdgffdgh"
   },
   {
     "id": 53086,
     "name": "image (1)",
     "price": 0,
     "views": 11,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53086",
     "url": "/document/maor-course/image-1/53086",
     "type": "Document",
     "date": "2020-07-23T06:32:22.8700044Z",
     "course": "maor course"
   },
   {
     "id": 53053,
     "name": "ניהול זמן ומטלות ",
     "price": 0,
     "views": 32,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53053",
     "url": "/document/%D7%98%D7%99%D7%A4%D7%99%D7%9D-%D7%9C%D7%AA%D7%9C%D7%9E%D7%99%D7%93%D7%99%D7%9D-%D7%A1%D7%98%D7%95%D7%93%D7%A0%D7%98%D7%99%D7%9D/%D7%A0%D7%99%D7%94%D7%95%D7%9C-%D7%96%D7%9E%D7%9F-%D7%95%D7%9E%D7%98%D7%9C%D7%95%D7%AA/53053",
     "type": "Document",
     "date": "2020-07-13T13:10:53.9160354Z",
     "course": "טיפים לתלמידים /סטודנטים"
   },
   {
     "id": 53001,
     "name": "Economic problem solving step by step Economic problem solving step by step Economic problem solving step by step Economic problem solving ",
     "price": 0,
     "views": 92,
     "likes": 0,
     "downloads": 1,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53001",
     "url": "/document/economics/economic-problem-solving-step-by-step-economic-problem-solving-step-by-step-econo/53001",
     "type": "Document",
     "date": "2020-07-07T13:14:58.3862576Z",
     "course": "Economics"
   },
   {
     "id": 53000,
     "name": "img (8)",
     "price": 0,
     "views": 24,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/53000",
     "url": "/document/dsassa/img-8/53000",
     "type": "Document",
     "date": "2020-07-07T13:03:46.4858356Z",
     "course": "dsassa"
   },
   {
     "id": 52999,
     "name": "img (12)",
     "price": 0,
     "views": 7,
     "likes": 0,
     "downloads": 1,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52999",
     "url": "/document/asfdasfdfdasfasdafsd/img-12/52999",
     "type": "Document",
     "date": "2020-07-07T13:00:53.6884436Z",
     "course": "asfdasfdfdasfasdafsd"
   },
   {
     "id": 52998,
     "name": "img (16)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52998",
     "url": "/document/sadfasfsf/img-16/52998",
     "type": "Document",
     "date": "2020-07-07T13:00:20.9371782Z",
     "course": "sadfasfsf"
   },
   {
     "id": 52997,
     "name": "img (15)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52997",
     "url": "/document/aas3ds/img-15/52997",
     "type": "Document",
     "date": "2020-07-07T12:59:48.4590594Z",
     "course": "aas3ds"
   },
   {
     "id": 52996,
     "name": "img (16)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52996",
     "url": "/document/asdfasfsafasfsaf/img-16/52996",
     "type": "Document",
     "date": "2020-07-07T12:57:42.3198413Z",
     "course": "asdfasfsafasfsaf"
   },
   {
     "id": 52995,
     "name": "img (15)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52995",
     "url": "/document/sadffsdaafsdasfd/img-15/52995",
     "type": "Document",
     "date": "2020-07-07T12:55:48.8279224Z",
     "course": "sadffsdaafsdasfd"
   },
   {
     "id": 52994,
     "name": "img (14)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52994",
     "url": "/document/ffasdfasdafs/img-14/52994",
     "type": "Document",
     "date": "2020-07-07T12:54:46.5647284Z",
     "course": "ffasdfasdafs"
   },
   {
     "id": 52993,
     "name": "img (16)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52993",
     "url": "/document/sdfasdfasdfas/img-16/52993",
     "type": "Document",
     "date": "2020-07-07T12:53:14.1059416Z",
     "course": "sdfasdfasdfas"
   },
   {
     "id": 52992,
     "name": "img (16)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52992",
     "url": "/document/fasfsdsfs/img-16/52992",
     "type": "Document",
     "date": "2020-07-07T12:52:03.3274257Z",
     "course": "fasfsdsfs"
   },
   {
     "id": 52150,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy (3) - Copy",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52150",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-3-copy/52150",
     "type": "Document",
     "date": "2020-05-27T09:07:24.2175099Z",
     "course": "Economics"
   },
   {
     "id": 52149,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy (2) - Copy - Copy",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52149",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-2-copy-copy/52149",
     "type": "Document",
     "date": "2020-05-27T09:07:24.217048Z",
     "course": "Economics"
   },
   {
     "id": 52148,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy (3)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52148",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-3/52148",
     "type": "Document",
     "date": "2020-05-27T09:07:24.2112236Z",
     "course": "Economics"
   },
   {
     "id": 52147,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy (4) - Copy",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52147",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-4-copy/52147",
     "type": "Document",
     "date": "2020-05-27T09:07:24.2110873Z",
     "course": "Economics"
   },
   {
     "id": 52146,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy - Copy (3)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52146",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-copy-3/52146",
     "type": "Document",
     "date": "2020-05-27T09:07:23.8857421Z",
     "course": "Economics"
   },
   {
     "id": 52145,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy - Copy - Copy (2)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52145",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-copy-copy-2/52145",
     "type": "Document",
     "date": "2020-05-27T09:07:23.8719335Z",
     "course": "Economics"
   },
   {
     "id": 52144,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy - Copy (2)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52144",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-copy-2/52144",
     "type": "Document",
     "date": "2020-05-27T09:07:23.8715822Z",
     "course": "Economics"
   },
   {
     "id": 52142,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy - Copy",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52142",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-copy/52142",
     "type": "Document",
     "date": "2020-05-27T09:07:23.8680831Z",
     "course": "Economics"
   },
   {
     "id": 52143,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy - Copy (2) - Copy",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52143",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-copy-2-copy/52143",
     "type": "Document",
     "date": "2020-05-27T09:07:23.8676132Z",
     "course": "Economics"
   },
   {
     "id": 52141,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy - Copy - Copy",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52141",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-copy-copy/52141",
     "type": "Document",
     "date": "2020-05-27T09:07:23.8633508Z",
     "course": "Economics"
   },
   {
     "id": 52140,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy - Copy - Copy - Copy",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52140",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-copy-copy-copy/52140",
     "type": "Document",
     "date": "2020-05-27T09:07:23.8631025Z",
     "course": "Economics"
   },
   {
     "id": 52139,
     "name": "0 - Copy - Copy - Copy - Copy - Copy (2)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52139",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-2/52139",
     "type": "Document",
     "date": "2020-05-27T09:07:07.0010892Z",
     "course": "Economics"
   },
   {
     "id": 52137,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy (4)",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52137",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy-4/52137",
     "type": "Document",
     "date": "2020-05-27T09:06:52.6941736Z",
     "course": "Economics"
   },
   {
     "id": 52136,
     "name": "0 - Copy - Copy - Copy - Copy - Copy - Copy",
     "price": 0,
     "views": 0,
     "likes": 0,
     "downloads": 0,
     "purchased": 0,
     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/52136",
     "url": "/document/economics/0-copy-copy-copy-copy-copy-copy/52136",
     "type": "Document",
     "date": "2020-05-27T09:06:52.6939352Z",
     "course": "Economics"
   }
 ]

 */