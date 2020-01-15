import calendarService from "../services/calendarService";
import utilitiesService from '../services/utilities/utilitiesService.js'
import {router} from '../main.js';

const state = {
    intervalFirst: 8,
    scope: 'calendar',
    calendarEvents: [],
    tutorId: null,
    fromDate: new Date().toISOString(),
    toDate: null,
    needPayment: true,
    showCalendar: false,
    calendarsList: null,
    selectedCalendarList:[],
    tutorDailyHours:[],
    tutorDailyHoursState:[],
};

const mutations ={
    setCalendarEvents(state,events){
        state.calendarEvents = events;
    },
    setTutorId(state,tutorId){
        state.tutorId = tutorId;
    },
    setToDate(state,toDate){
        state.toDate = toDate;
    },
    setNeedPayment(state,val){
        state.needPayment = val;
    },
    setShowCalendar(state,val){
        state.showCalendar = val;
    },
    setCalendarList(state,list){
        state.calendarsList = list;
    },
    setSelectedCalendarList(state,selectedList){
        state.selectedCalendarList = selectedList;
    },
    setAvailabilityCalendar(state,dayAvailabilityObj){
        if(state.tutorDailyHours.length){
            let self = this;
            let isContain = state.tutorDailyHours.some(
                (dayObj,index)=>{
                    if(dayObj.day === dayAvailabilityObj.day){
                        self.dayIndex = index;
                        return true;
                    }
                });

            if(isContain){
                if(dayAvailabilityObj.timeFrames.length){
                    state.tutorDailyHours.forEach((element) => {
                        if(element.day === dayAvailabilityObj.day){
                            state.tutorDailyHours[self.dayIndex] = dayAvailabilityObj;
                        }
                    });
                }else{
                    state.tutorDailyHours.splice(self.dayIndex,1);
                }
            }else{ 
                if(dayAvailabilityObj.timeFrames.length){
                    state.tutorDailyHours.push(dayAvailabilityObj);
                }
            }
        } else{
            state.tutorDailyHours.push(dayAvailabilityObj);
        }
    }
};

const getters ={
    getCalendarEvents:state => state.calendarEvents,
    getNeedPayment:state => state.needPayment,
    getShowCalendar:state => state.showCalendar,
    getCalendarsList: state => state.calendarsList,
    getSelectedCalendarList: state => state.selectedCalendarList,
    getIntervalFirst: state => state.intervalFirst,
    getCalendarAvailabilityIsValid: state => (state.tutorDailyHours.length),
    getCalendarAvailabilityState: state => state.tutorDailyHoursState,
};

const actions ={
    getCalendarListAction({commit}){
        return calendarService.getCalendarsList().then(response=>{
            commit('setCalendarList',response.data);
            return Promise.resolve(response.data);
        });
    },
    updateStateAvailabilityCalendar({commit},dayAvailability){
        commit('setAvailabilityCalendar',dayAvailability);
    },
    updateStateSelectedCalendarList({commit},selectedCalendarList){
        commit('setSelectedCalendarList',selectedCalendarList);
    },
    updateSelectedCalendarList({state}){
        if(state.selectedCalendarList.length){
            return calendarService.postCalendarsList(state.selectedCalendarList);
        }else{
            return Promise.resolve();
        }
    },
    updateAvailabilityCalendar({state}){
        return calendarService.postCalendarAvailability(state.tutorDailyHours);
    },
    getEvents({commit,getters}){
        let tutorId; 
            if(getters.getProfile){
                tutorId = router.history.current.params.id;
            }else{
                tutorId = getters.accountUser.id;
            }
        commit('setTutorId',tutorId);
        commit('setToDate',utilitiesService.IsoStringDateWithOffset(60));
        let paramsObj = {
            from: state.fromDate,
            to: state.toDate,
            tutorId: state.tutorId
        };
        return calendarService.getEvents(paramsObj).then(response=>{
            commit('setCalendarEvents',response);
            commit('setShowCalendar',true);
            return Promise.resolve(response);
        },err=>{
            commit('setShowCalendar',false);
            return Promise.reject(err);
        });
    },
    initCalendar({state,commit,dispatch},tutorId){
        commit('setTutorId',tutorId);
        commit('setToDate',utilitiesService.IsoStringDateWithOffset(60));
        return dispatch('gapiLoad',state.scope).then(()=>{
            let paramsObj = {
                from: state.fromDate,
                to: state.toDate,
                tutorId: state.tutorId
            };
            return calendarService.getEvents(paramsObj).then(response=>{
                commit('setCalendarEvents',response);
                commit('setShowCalendar',true);
                return Promise.resolve(response);
            },err=>{
                commit('setShowCalendar',false);

                return Promise.reject(err);
            });
        });
    },
    signInCalendar({commit},authResult){
        if (authResult['code']) {
            let serverObj = {code:authResult['code']};
            return calendarService.signIn(serverObj).then(
                ()=>{
                    return calendarService.getCalendarsList().then(response=>{
                        commit('setCalendarList',response.data);
                        return Promise.resolve(response.data);
                    });
                },
                (error)=>{
                    return Promise.reject(error);
                });
        }
    },
    insertEvent({state},{date,time}){
        let from = new Date(`${date} ${time}`).toISOString();
        let isodate = new Date(`${date} ${time}`);
        isodate.setHours(isodate.getHours() + 1);
        let to = isodate.toISOString();
      
        let insertEventObj = {
            from,
            to,
            tutorId: state.tutorId
        };

        return calendarService.addEvent(insertEventObj).then(
            (response)=>{
                return Promise.resolve(response);
            },(error)=>{
                return Promise.reject(error);
            });
    },
    updateNeedPayment({commit},val){
        commit('setNeedPayment',val);
    },
    updateCalendarStatus({state,getters,dispatch}){
        let isSharedCalendar;
        if(getters.getProfile){
           isSharedCalendar = getters.getProfile.user.calendarShared;
        }else{
            calendarService.getAccountAvailabilityCalendar().then(res=>{
                isSharedCalendar = res.calendarShared;
                state.tutorDailyHoursState = res.tutorDailyHours
            })
        }
        setTimeout(() => {
            if(isSharedCalendar){
                let tutorId; 
                if(getters.getProfile){
                    tutorId = router.history.current.params.id;
                }else{
                    tutorId = getters.accountUser.id;
                }
               return dispatch('initCalendar',tutorId).then(()=>{
                    return Promise.resolve();
               },(err)=>{
                    return Promise.reject(err);
               });
            }else{
                dispatch('gapiLoad',state.scope);
            }
        }, 100);
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}