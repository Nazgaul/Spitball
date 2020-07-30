import axios from 'axios'

const courseInstance = axios.create({
    baseURL: '/api/course'
 })

const state = {
    numberOfLecture: 1,
    courseName: '',
    followerPrice: 0,
    subscribePrice: 0,
    description: '',
    courseVisible: true,
    courseCoverImage: null,
    teachingDates: [] // courseTeaching.vue
}

const getters = {
    getNumberOfLecture: state => state.numberOfLecture,
    getCourseName: state => state.courseName,
    getFollowerPrice: state => state.followerPrice,
    getSubscriberPrice: state => state.subscribePrice,
    getDescription: state => state.description,
    getCourseVisible: state => state.courseVisible,
    getTeachLecture: state => state.teachingDates,
    getTeachTime: () => {
        const currentTime = new Date();
        let currentHour = currentTime.getHours().toString().padStart(2, '0');
        let currentMinutes = (Math.ceil(currentTime.getMinutes() / 15) * 15);
        if (currentMinutes === 60) {
            currentHour++;
            currentMinutes = 0;
        }
        return `${currentHour}:${currentMinutes.toString().padStart(2,'0')}`
    }
}

const mutations = {
    setNumberOfLecture(state, num) {
        state.numberOfLecture = num;
    },
    removeLecture(state, index) {
        state.numberOfLecture -= 1;
        state.teachingDates.splice(index-1, 1)
    },
    setCourseName(state, name) {
        state.courseName = name
    },
    setFollowerPrice(state, price) {
        state.followerPrice = price
    },
    setSubscriberPrice(state, price) {
        state.subscribePrice = price
    },
    setCourseDescription(state, description) {
        state.description = description
    },
    setTeachLecture(state, teachObj) {
        this._vm.$set(state.teachingDates, teachObj.index, {
            text: teachObj.text || state.teachingDates[teachObj.index]?.text,
            hour: teachObj.hour ||  state.teachingDates[teachObj.index]?.hour,
            date: teachObj.date ||  state.teachingDates[teachObj.index]?.date
        })
    },
    setVisibleFile(state, {val, item}) {
        item.visible = val
    },
    setShowCourse(state, val) {
        state.courseVisible = val
    },
    setCourseCoverImage(state, image) {
        state.courseCoverImage = image
    }
}

const actions = {
    updateCourseInfo({commit, state, getters}) {
        // validate if tutor enter documents or studyroom
        if(!getters.getFileData.length && !state.teachingDates.length) {
            return Promise.reject('Error, must include documents or studyroom')
        }

        let studyRooms = state.teachingDates.filter(studyRoom => {
            let userChooseDate =  this._vm.$moment(`${studyRoom.date}T${studyRoom.hour}:00`);         
            let isToday = userChooseDate.isSame(this._vm.$moment(), 'day');
            if(isToday) {
               let isValidDateToday = userChooseDate.isAfter(this._vm.$moment().format())
                if(!isValidDateToday) {
                    return Promise.reject('Error, date created')
                } 
            }
            return {
                name: studyRoom.text,
                date: userChooseDate
            }
        })

        let documents = getters.getFileData.filter(file => {
            if(!file.error) {
                return {
                    blobName: file.blobName,
                    name: file.name,
                    visible: file.visible || false
                }
            }
        })

        let params = {
            name: state.courseName,
            price: state.followerPrice,
            subscriptionPrice: state.subscribePrice,
            description: state.description,
            image: state.courseCoverImage,
            isPublish: state.courseVisible,
            studyRooms: studyRooms,
            documents: documents
        }
        
        console.log(params);
        
        return courseInstance.post('', params).then(res => {
            console.log(res);
        }).catch(ex => {
            console.error(ex);
        })
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}