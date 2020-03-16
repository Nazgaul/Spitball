// const state = {
//     isDarkTheme: true,
//     currentLang: {langName: 'C', langMode: 'text/x-c++src', langIcon:'./images/c.png'},
//     code: '',
    
// };
// const getters = {
//     getIsDarkTheme:state => state.isDarkTheme,
//     getCurrentLang:state => state.currentLang,
//     getCode:state => state.code,
// };

// const mutations = {
//     setThemeMode(state,val){
//         state.isDarkTheme = val;
//     },
//     setLang(state,lang){
//         state.currentLang = lang;
//     },
//     setCode(state,code){
//         state.code = code;
//     }
// };

// const actions = {
//     updateThemeMode({commit},val){
//         commit('setThemeMode',val);
//     },
//     updateLang({getters,commit,dispatch},lang){
//         commit('setLang',lang);
//         if(getters['getCurrentRoomState'] === 'active'){
//             let editorEvent = {
//                 subject: 'codeEditor_lang',
//                 val: lang
//             };
//             dispatch('sendEditorData', editorEvent);
//         }
//     },
//     updateCode({getters,commit, dispatch},code){
//         commit('setCode',code);
//         if(getters['getCurrentRoomState'] === 'active'){
//             let editorEvent = {
//                 subject: 'codeEditor_code',
//                 val: code
//             };
//             dispatch('sendEditorData', editorEvent);
//         }
//     },
//     sendEditorData({dispatch}, {subject, val}){
//         let normalizedData = JSON.stringify({type: subject,data: val});
//         dispatch('sendDataTrack',normalizedData)
//     }
// };
// export default {
//     state,
//     mutations,
//     getters,
//     actions
// }