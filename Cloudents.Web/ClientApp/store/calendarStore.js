import calendarService from "../services/calendarService";
import utilitiesService from '../services/utilities/utilitiesService.js'

const state = {
    scope: 'profile https://www.googleapis.com/auth/calendar.readonly',
    calendarEvents: [],
    tutorId: null,
    fromDate: new Date().toISOString(),
    toDate: null,
    needPayment: true,
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
    }
}

const getters ={
    getCalendarEvents:state => state.calendarEvents,
    getNeedPayment:state => state.needPayment,
}

const actions ={
    initCalendar({state,commit,dispatch},tutorId){
        commit('setTutorId',tutorId)
        commit('setToDate',utilitiesService.IsoStringDateWithOffset(60))
        dispatch('gapiLoad',state.scope).then(()=>{
            let paramsObj = {
                from: state.fromDate,
                to: state.toDate,
                tutorId: state.tutorId
            }
            calendarService.getEvents(paramsObj).then(response=>{
                commit('setCalendarEvents',response)
            },err=>{
            })
        })
    },
    signInCalendar({},authResult){
        if (authResult['code']) {
            let serverObj = {code:authResult['code']}
            return calendarService.signIn(serverObj).then(
                (response)=>{
                    return response
                },
                (error)=>{
                    return error
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
        
        return calendarService.addEvent(insertEventObj).then(res=>{
            return res
        })  
    },
    updateNeedPayment({commit},val){
        commit('setNeedPayment',val)
    }
}

export default {
    state,
    mutations,
    getters,
    actions
}