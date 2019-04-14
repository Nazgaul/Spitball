const state = {
    becomeTutorObj:{
        image: '',
        firstName: '',
        lastName: '',
        price: 50,
        about: '',
        bio: ''
    },
   tutorDialog: false


};
const getters = {
    becomeTutorData: state => state.becomeTutorObj,
    becomeTutorDialog: state => state.tutorDialog
};

const mutations = {
    changeDialogState(state, val){
        state.tutorDialog = val
    },
    updateImage(state, val) {
        state.becomeTutorObj = {...state.becomeTutorObj, ...val}
    },
    assignFields(state, val){
        state.becomeTutorObj = { ...state.becomeTutorObj, ...val}
    }
};

const actions = {
    updateTutorDialog({commit, state}, val){
        commit('changeDialogState', val)
    },
    setImageUrl({commit, state}, val) {
        commit('updateImage', val)
    },
    updateTutorInfo({commit, state}, val) {
        commit('assignFields', val)
    },
};
export default {
    state,
    mutations,
    getters,
    actions
}