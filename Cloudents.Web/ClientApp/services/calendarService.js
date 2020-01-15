import { connectivityModule } from "./connectivity.module";

const Calendar = {
    Event:function(objInit){
        var date = new Date(objInit);
        this.needToAdd = function() {return date.getHours() >7 && date.getHours() <= 23;};
        this.date = calendarDate(date);
        this.time = calendarTime(date);
    },
    Account:function(objInit){
        this.calendarShared = objInit.calendarShared;
        this.tutorDailyHours =  objInit.tutorDailyHours;
    }
}



function calendarDate(dateTime){
    return dateTime.toISOString().substr(0, 10);
}

function calendarTime(dateTime){
    let hour = dateTime.getHours();
    if(hour < 10) return `0${hour}:00`;
    else return `${hour}:00`;
}

function calendarEvents(objInit){
    let events = [];
    objInit.forEach(e =>{
        let event = new Calendar.Event(e);
        if (event.needToAdd()) {events.push(event);}
    });
    return events;
}
function CalendarHours(day,from,to){
    this.day = day;
    this.from = from;
    this.to = to;
}
function createCalendarHours(objInit){
    let tutorDailyHoursObj = {
        tutorDailyHours:[]
    };
    objInit.forEach(day=>{
        let dayIndex = day.day;
        if(day.timeFrames.length > 2){
            let from = day.timeFrames[0];
            let to = day.timeFrames[1];
            let fromAdditional = day.timeFrames[2];
            let toAdditional = day.timeFrames[3];
            let dayObject = new CalendarHours(dayIndex,from,to);
            let dayObjectAdditional = new CalendarHours(dayIndex,fromAdditional,toAdditional);
            tutorDailyHoursObj.tutorDailyHours.push(dayObject);
            tutorDailyHoursObj.tutorDailyHours.push(dayObjectAdditional);
        }else{
            let from = day.timeFrames[0];
            let to = day.timeFrames[1];
            let dayObject = new CalendarHours(dayIndex,from,to);
            tutorDailyHoursObj.tutorDailyHours.push(dayObject);
        }
    });
    return tutorDailyHoursObj;
}
function signIn(signInData){
    return connectivityModule.http.post(`Tutor/calendar/Access`,signInData).then((response)=>{
        return Promise.resolve(response);
    },(error)=>{
          return Promise.reject(error);
    });
}
function getEvents(params){
    return connectivityModule.http.get(`Tutor/calendar/events`,{params}).then(
        (response)=>{
            return calendarEvents(response.data);
      });
}
function getCalendarsList(){
    return connectivityModule.http.get(`Tutor/calendar/list`).then(
        (response)=>{
            return response;
        });
}
function postCalendarsList(params){
    return connectivityModule.http.post(`Tutor/calendar/list`,params);
}
function postCalendarAvailability(paramsObj){
    let params = createCalendarHours(paramsObj);
    return connectivityModule.http.post(`Tutor/calendar/hours`,params);
}
function addEvent(params){
    return connectivityModule.http.post(`Tutor/calendar/events`,params).then(()=>{
    });
}
function getAccountAvailabilityCalendar(){
    return connectivityModule.http.get(`Account/calendar`).then(({data})=>{
        return new Calendar.Account(data);
    });
}
export default {
    signIn,
    getEvents,
    addEvent,
    getCalendarsList,
    postCalendarsList,
    postCalendarAvailability,
    getAccountAvailabilityCalendar,
}