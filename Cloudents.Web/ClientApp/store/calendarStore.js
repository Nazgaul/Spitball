import calendarService from "../services/calendarService";
import utilitiesService from '../services/utilities/utilitiesService.js'
import {router} from '../main.js';

const state = {
    scope: 'profile https://www.googleapis.com/auth/calendar.readonly',
    calendarEvents: [],
    tutorId: null,
    fromDate: new Date().toISOString(),
    toDate: null,
    needPayment: true,
    showCalendar: false,
}

const mutations ={
    setCalendarEvents(state,events){
        state.calendarEvents = events
    },
    setTutorId(state,tutorId){
        state.tutorId = tutorId
    },
    setToDate(state,toDate){
        state.toDate = toDate
    },
    setNeedPayment(state,val){
        state.needPayment = val
    },
    setShowCalendar(state,val){
        state.showCalendar = val
    }
}

const getters ={
    getCalendarEvents:state => state.calendarEvents,
    getNeedPayment:state => state.needPayment,
    getShowCalendar:state => state.showCalendar,
}

const actions ={
    initCalendar({state,commit,dispatch},tutorId){
        commit('setTutorId',tutorId)
        commit('setToDate',utilitiesService.IsoStringDateWithOffset(60))
       return dispatch('gapiLoad',state.scope).then(()=>{
            let paramsObj = {
                from: state.fromDate,
                to: state.toDate,
                tutorId: state.tutorId
            }
            return calendarService.getEvents(paramsObj).then(response=>{
                commit('setCalendarEvents',response)
                commit('setShowCalendar',true)
                return Promise.resolve(response)
            },err=>{
                commit('setShowCalendar',false)

                return Promise.reject(err)
            })
        })
    },
    signInCalendar({},authResult){
        if (authResult['code']) {
            let serverObj = {code:authResult['code']}
            return calendarService.signIn(serverObj).then(
                (response)=>{
                    return Promise.resolve(response)
                },
                (error)=>{
                    return Promise.reject(error)
                });
        } else {}
    },
    insertEvent({state},{date,time}){
        let from = new Date(`${date} ${time}`).toISOString()
        let isodate = new Date(`${date} ${time}`)
        isodate.setHours(isodate.getHours() + 1);
        let to = isodate.toISOString();
      
        let insertEventObj = {
            from,
            to,
            tutorId: state.tutorId
        }
        
        return calendarService.addEvent(insertEventObj).then(
            (response)=>{
                return Promise.resolve(response)
            },(error)=>{
                return Promise.reject(error)
            }) 
    },
    updateNeedPayment({commit},val){
        commit('setNeedPayment',val)
    },
    updateCalendarStatus({commit,getters,dispatch}){
        let isSharedCalendar = getters.getProfile.user.calendarShared
        if(isSharedCalendar){
            let tutorId = router.history.current.params.id;
           return dispatch('initCalendar',tutorId).then(()=>{
                return Promise.resolve()
            },(err)=>{
                return Promise.reject(err)
            })
        }
    }
}

export default {
    state,
    mutations,
    getters,
    actions
}