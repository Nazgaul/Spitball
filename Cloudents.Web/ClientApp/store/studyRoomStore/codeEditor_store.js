import tutorService from '../../components/tutor/tutorService.js'

const _sendData = (subject,val) => {
    let normalizedData = JSON.stringify({type: subject,data: val});
    tutorService.dataTrack.send(normalizedData);
}

const state = {
    isDarkTheme: true,
    currentLang: {langName: 'C', langMode: 'text/x-c++src', langIcon:'./images/c.png'},
    code: '',
    
};
const getters = {
    getIsDarkTheme:state => state.isDarkTheme,
    getCurrentLang:state => state.currentLang,
    getCode:state => state.code,
};

const mutations = {
    setThemeMode(state,val){
        state.isDarkTheme = val
    },
    setLang(state,lang){
        state.currentLang = lang
    },
    setCode(state,code){
        state.code = code
    }
};

const actions = {
    updateThemeMode({commit},val){
        commit('setThemeMode',val)
    },
    updateLang({getters,commit},lang){
        commit('setLang',lang)
        if(getters['getCurrentRoomState'] === 'active'){
            _sendData('codeEditor_lang',lang)
        }
    },
    updateCode({getters,commit},code){
        commit('setCode',code)
        if(getters['getCurrentRoomState'] === 'active'){
            _sendData('codeEditor_code',code)
        }
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}