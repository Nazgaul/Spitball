
const state = {
    snackbar: {
        value: false,
        name: '',
        params: null
    }
};

const getters = {
    getSnackbar: state => state.snackbar,
};

const mutations = {
    setSnackbar(state, snack) {
        state.snackbar.value = snack?.value || false
        state.snackbar.name = snack?.name || ''
        state.snackbar.params = snack?.params || null
    }
};

const actions = {

};

export default {
    state,
    mutations,
    getters,
    actions
};