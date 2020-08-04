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
    teachingDates: []
}

const getters = {
    getNumberOfLecture: state => state.numberOfLecture,
    getCourseName: state => state.courseName,
    getFollowerPrice: state => state.followerPrice,
    getSubscriberPrice: state => state.subscribePrice,
    getDescription: state => state.description,
    getCourseVisible: state => state.courseVisible,
    getTeachLecture: state => state.teachingDates,
    getCourseCoverImage: state => state.courseCoverImage,
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
    setCourseName(state, name) {
        state.courseName = name?.text || name
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
    setTextLecture(state, teachObj) {
        this._vm.$set(state.teachingDates, teachObj.index, {
            text: teachObj.text,
            hour: state.teachingDates[teachObj.index]?.hour,
            date: state.teachingDates[teachObj.index]?.date
        })
    },
    setTeachLecture(state, teachObj) {
        this._vm.$set(state.teachingDates, teachObj.index, {
            text: state.teachingDates[teachObj.index]?.text,
            hour: teachObj.hour || state.teachingDates[teachObj.index]?.hour,
            date: teachObj.date || state.teachingDates[teachObj.index]?.date
        })
    },
    removeLecture(state, index) {
        state.numberOfLecture -= 1
        state.teachingDates.splice(index-1, 1)
    },
    setVisibleFile(state, {val, item}) {
        item.visible = val
    },
    setShowCourse(state, val) {
        state.courseVisible = val
    },
    setCourseCoverImage(state, image) {
        state.courseCoverImage = image
    },
    resetCreateCourse(state) {
        state.numberOfLecture = 1,
        state.courseName = '',
        state.followerPrice = 0,
        state.subscribePrice = 0,
        state.description = '',
        state.courseVisible = true,
        state.courseCoverImage = null,
        state.teachingDates = []
    }
}

const actions = {
    getCourseInfo({commit}, id) {
        return courseInstance.get(`${id}/edit`).then(({data}) => {
            commit('setNumberOfLecture', data.studyRooms.length)
            commit('setCourseName', data.name)
            commit('setFollowerPrice', data.price.amount)
            commit('setSubscriberPrice', data.subscriptionPrice.amount)
            commit('setCourseDescription', data.description)
            commit('setCourseCoverImage', data.image)
            data.studyRooms.map((s, i) => {
                commit('setTeachLecture', {
                    index: i,
                    date: this._vm.$moment(s.dateTime).format('YYYY-MM-DD'),
                    hour: this._vm.$moment(s.dateTime).format('HH:mm'),
                })
                commit('setTextLecture', {
                    index: i,
                    text: s.name
                })
            })
            // data.documents.map(s => commit('setTeachLecture', s))
            commit('setShowCourse', data.visible)
        })
    },
    updateCourseInfo({state}, {documents, studyRooms}) {
        let params = {
            name: state.courseName,
            price: state.followerPrice,
            subscriptionPrice: state.subscribePrice,
            description: state.description,
            image: state.courseCoverImage,
            isPublish: state.courseVisible,
            studyRooms,
            documents
        }
        
        return courseInstance.post('', params)
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}