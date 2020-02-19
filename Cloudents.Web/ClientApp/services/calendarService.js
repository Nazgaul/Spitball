import { connectivityModule } from "./connectivity.module";
function _dayStringToNumber(dayString){
    if(typeof dayString === "number"){
        return dayString;
    }else{
        const Days = ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'];
        return Days.indexOf(dayString);
    }
}
function _calendarDate(dateTime){
    return dateTime.toISOString().substr(0, 10);
}
function _calendarTime(dateTime){
    let hour;
    (typeof dateTime === 'number')? hour = dateTime : hour = dateTime.getHours();
    if(hour < 10) return `0${hour}:00`;
    else return `${hour}:00`;
}
function _formatDayRead(UTC){

    let UtcSplited = UTC.split(":");
    let currentDate = new Date()
    let timeFormated = new Date(Date.UTC(currentDate.getFullYear(), currentDate.getMonth() , currentDate.getDay(), UtcSplited[0],UtcSplited[1],UtcSplited[2],0))
    return _calendarTime(timeFormated.getHours())
}
const Calendar = {
    Event:function(objInit){
        var date = new Date(objInit);
        this.needToAdd = function() {return date.getHours() >7 && date.getHours() <= 23;};
        this.date = _calendarDate(date);
        this.time = _calendarTime(date);
    },
    Day:function(objInit){
        this.day = _dayStringToNumber(objInit.day);
        this.from = objInit.from;
        this.to = objInit.to;
    },
    DayRead:function(objInit){
        this.day = _dayStringToNumber(objInit.day);
        this.from = _formatDayRead(objInit.from);
        this.to = _formatDayRead(objInit.to);
    },
    Account:function(objInit){
        this.calendarShared = objInit.calendarShared;
        this.tutorDailyHours = objInit.tutorDailyHours.map(day=> new Calendar.DayRead(day));
    }
}

function calendarEvents(objInit){
    let events = [];
    objInit.forEach(e =>{
        let event = new Calendar.Event(e);
        if (event.needToAdd()) {events.push(event);}
    });
    return events;
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
            let dayObject = new Calendar.Day({day:dayIndex,from,to});
            let dayObjectAdditional = new Calendar.Day({day:dayIndex,from:fromAdditional,to:toAdditional});
            tutorDailyHoursObj.tutorDailyHours.push(dayObject);
            tutorDailyHoursObj.tutorDailyHours.push(dayObjectAdditional);
        }else{
            let from = day.timeFrames[0];
            let to = day.timeFrames[1];
            let dayObject = new Calendar.Day({day:dayIndex,from,to});
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