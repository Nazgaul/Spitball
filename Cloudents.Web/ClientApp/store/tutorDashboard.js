import axios from 'axios'

// const dashboardInstance = axios.create({
//     baseURL: '/'
// })

const state = {
    tutorListActions: []
}

const getters = {
    getTutorListActions: state => state.tutorListActions
}

const mutations = {
    setTutorListActions(state, data) {
        function TutorLink(objInit) {
            this.text = 'lorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsum';
            this.routeName = '/feed';
        }

        for(var i=0;i<6;i++) {
            state.tutorListActions.push(new TutorLink())
        }

        // data.map((link) => {
        //     state.tutorListActions.push(new TutorLink(link))
        // })

    }
}

const actions = {
    updateTutorLinks({commit}) {
        commit('setTutorListActions')
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}