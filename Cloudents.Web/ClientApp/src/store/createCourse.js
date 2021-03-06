const COURSE_API = 'course';

const state = {
    componentKeyRender: 0,
    numberOfLecture: 0,
    courseName: '',
    followerPrice: 0,
    subscribePrice: 0,
    description: '',
    courseVisible: true,
    showFiles: false,
    courseCoverImage: null,
    teachingDates: [],
    teachingNext:[]
}

const getters = {
    getComponentKey: state => state.componentKeyRender,
    getNumberOfLecture: state => state.numberOfLecture,
    getCourseName: state => state.courseName,
    getFollowerPrice: state => state.followerPrice,
    getSubscriberPrice: state => state.subscribePrice,
    getDescription: state => state.description,
    getCourseVisible: state => state.courseVisible,
    getTeachLecture: state => state.teachingDates,
    getCourseCoverImage: state => state.courseCoverImage,
    getShowFiles: state => state.showFiles,
    getTeachTime: () => {
        const currentTime = new Date();
        let currentHour = currentTime.getHours().toString().padStart(2, '0');
        let currentMinutes = (Math.ceil(currentTime.getMinutes() / 15) * 15);
        if (currentMinutes === 60) {
            currentHour++;
            currentMinutes = 0;
        }
        return `${currentHour}:${currentMinutes.toString().padStart(2,'0')}`
    },
    getTeachingNext: state => state.teachingNext,
}

const mutations = {
    setComponentKey(state) {
        state.componentKeyRender++
    },
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
    setNextSession(state,sessions){
        state.teachingNext = sessions.sort((a,b)=> new Date(a.dateTime) - new Date(b.dateTime)).filter(session=> new Date() < new Date(session.dateTime))[0];
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
    setShowFiles(state, val) {
        state.showFiles = val
    },
    resetCreateCourse(state) {
        state.numberOfLecture = 0,
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
        return this.$axios.get(`${COURSE_API}/${id}/edit`).then(({data}) => {
            commit('setNumberOfLecture', data.studyRooms.length || 1)
            commit('setCourseName', data.name)
            commit('setFollowerPrice', data.price.amount)
            commit('setSubscriberPrice', data.subscriptionPrice?.amount)
            commit('setCourseDescription', data.description)
            commit('setCourseCoverImage', data.image)
            commit('setShowCourse', data.visible)
            commit('setNextSession',data.studyRooms)
            
            if(!data.studyRooms.length) {
                commit('setNumberOfLecture', 0)
            }
            
            let i = 0, studyRooms = data.studyRooms
            for (i = 0; i < studyRooms.length; i++) {
                const elem = studyRooms[i]

                const start = this._vm.$moment(elem.dateTime);
                const remainder = 15 - (start.minute() % 15);
                let hour = remainder % 15 === 0 ? this._vm.$moment(elem.dateTime).format('HH:mm') : this._vm.$moment(start).add(remainder, "minutes").format('HH:mm')

                commit('setTeachLecture', {
                    index: i,
                    date: this._vm.$moment(elem.dateTime).format('YYYY-MM-DD'),
                    hour,
                })
                commit('setTextLecture', {
                    index: i,
                    text: elem.name
                })
            }

            let j = 0, documents = data.documents
            for (j = 0; j < documents.length; j++) {
                const elem = documents[j]
                commit('addFile', {
                    id: elem.id,
                    name: elem.title,
                    visible: elem.visible,
                })
            }
            if(documents.length > 0) {
                commit('setShowFiles', true)
            }
        }).catch(ex => {
            console.error(ex);
        })
    },
    createCourseInfo({state}, {documents, studyRooms}) {
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
        return this.$axios.post(`${COURSE_API}`, params)
    },
    updateCourseInfo({state}, {documents, studyRooms, id}) {
        let params = {
            name: state.courseName,
            price: state.followerPrice,
            subscriptionPrice: state.subscribePrice,
            description: state.description,
            image: state.courseCoverImage.startsWith("https://") ? undefined : state.courseCoverImage,
            isPublish: state.courseVisible,
            studyRooms,
            documents
        }
        return this.$axios.put(`${COURSE_API}/${id}`, params)
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}