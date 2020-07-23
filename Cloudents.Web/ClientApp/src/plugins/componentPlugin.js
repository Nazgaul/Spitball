import { auth_SETTER } from '../store/constants/authConstants';
import { TUTOR_EDIT_PROFILE } from '../components/pages/global/toasterInjection/componentConsts'
import { Profile } from '../routes/routeNames.js';

export default () => {
  return store => {
    store.subscribe((mutation) => {
      if(mutation.payload?.query?.authDialog === auth_SETTER.register) {
        let isLogged = store.getters.getUserLoggedInStatus     
        if (!isLogged){
          store.commit('setComponent', auth_SETTER.register)
        }
      }
      if(mutation.payload?.query?.d && mutation.payload?.name == Profile) {
        store.dispatch('updateCurrentItem',mutation.payload?.query?.d)
      }
      if(mutation.type === 'route/ROUTE_CHANGED') {
        let { name, hash, params } = store.state.route
        if(name === Profile) {
          if(hash === '#tutorEdit') {
            let userId = store.getters.accountUser?.id
            if(params.id == userId) {
              store.commit('addComponent', TUTOR_EDIT_PROFILE)
            }
          }
        }
        if(sessionStorage.getItem('hash') === '#tutorRequest') {
          let tutor = JSON.parse(sessionStorage.getItem('tutorRequest'))
          store.commit('setCourseDescription', tutor.text)
          store.commit('setSelectedCourse', tutor.course)
          store.commit('setCurrentTutorIdFromRegister', tutor.tutorId)

          store.dispatch('updateRequestDialog', true);
          store.dispatch('updateTutorReqStep', 'tutorRequestSuccess')
          sessionStorage.clear()
        }
      }
    })
  }
}