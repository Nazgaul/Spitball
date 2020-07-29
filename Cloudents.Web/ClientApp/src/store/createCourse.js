import axios from 'axios'

const state = {
    numberOfLecture: 1,
    courseName: '',
    followerPrice: 0,
    subscribePrice: 0,
    description: '',
    courseVisible: true,
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
    }
}

const actions = {
    updateCourseInfo({commit, state, getters}) {
        // validate if tutor enter documents or studyroom
        if(!getters.getFileData.length || !state.teachingDates.length) {
            return Promise.reject()
        }

        let params = {
            courseName: state.courseName,
            followerPrice: state.followerPrice,
            subscribePrice: state.subscribePrice,
            description: state.description,
            courseVisible: state.courseVisible,
            teachingDates: state.teachingDates,
            files: getters.getFileData.filter(file => !file.error)
        }
        
        console.log(params);
        // axios.post(`api/`, params).then(() => {
        //     commit()
        // })
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}