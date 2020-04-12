import calendarService from "../services/calendarService";
import utilitiesService from '../services/utilities/utilitiesService.js'
import paymentService from '../services/payment/paymentService.js';
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
    isCalendarShared:null,
};

const mutations = {
    setResetCalendar(state){
        state.calendarEvents = [];
        state.needPayment = true;
        state.showCalendar = false;
        state.intervalFirst = 8;
        state.isCalendarShared = null;
        state.tutorId = null;
        state.fromDate = new Date().toISOString();
        state.toDate = null;
    },
    setIsCalendarShared(state,val){
        state.isCalendarShared = val;
    },
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
    getNeedPayment:state => paymentService.getIsCalendarNeedPayment() && state.needPayment,
    getShowCalendar:state => state.showCalendar,
    getCalendarsList: state => state.calendarsList,
    getSelectedCalendarList: state => state.selectedCalendarList,
    getIntervalFirst: state => state.intervalFirst,
    getCalendarAvailabilityIsValid: state => (state.tutorDailyHours.length),
    getCalendarAvailabilityState: state => state.tutorDailyHoursState,
    getIsCalendarShared:state => state.isCalendarShared,
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
        let dateTime = new Date(`${date}T${time}`);
        //let hour = +time.split(':')[0];

        //let from = new Date(dateTime.setHours(hour));
        let fromISO = dateTime.toISOString();

       //let to = new Date(dateTime.setHours(hour + 1))
       // let toISO = to.toISOString();

        let insertEventObj = {
            from: fromISO,
         //   to: toISO,
            tutorId: state.tutorId
        };

        return calendarService.addEvent(insertEventObj);
    },
    updateNeedPayment({commit},val){
        commit('setNeedPayment',val);
    },
    updateCalendarStatus({state,getters,dispatch}){
        let isSharedCalendar = getters.getProfile.user.calendarShared;


            if(isSharedCalendar){
                let tutorId = router.history.current.params.id;
               return dispatch('initCalendar',tutorId).then(()=>{
                    return Promise.resolve();
               },(err)=>{
                    return Promise.reject(err);
               });
            }else{
                dispatch('gapiLoad',state.scope);
            }
    },
    updateCalendarStatusDashboard({dispatch,commit,state}){
        return calendarService.getAccountAvailabilityCalendar().then(res=>{
            commit('setIsCalendarShared',res.calendarShared)
            if(!res.calendarShared){
                return Promise.resolve(false);
            }else{
                return dispatch('getCalendarListAction').then(()=>{
                    state.tutorDailyHoursState = res.tutorDailyHours
                    return Promise.resolve(true);
                })
            }
        })
    },
    resetCalendar({commit}){
        commit('setResetCalendar')
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}