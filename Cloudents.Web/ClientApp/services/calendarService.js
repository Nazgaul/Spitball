import { connectivityModule } from "./connectivity.module";

function signIn(signInData){
    return connectivityModule.http.post(`Tutor/calendar/Access`,signInData).then((response)=>{
        return Promise.resolve(response)
      },(error)=>{
          return Promise.reject(error)
      });
}
function getEvents(params){
    return connectivityModule.http.get(`Tutor/calendar/events`,{params}).then(
        (response)=>{
            let events = [];
            return calendarEvents(response.data);
            // response.data.forEach(e =>{
            //     if(Object.keys(e).length === 0) return;
            //     events.push(calendarEvent(e))
            // })
            return events
      })
}

function addEvent(params){
    return connectivityModule.http.post(`Tutor/calendar/events`,params).then(response=>{

    })
}

function getCalendarsList(){
    return connectivityModule.http.get(`Tutor/calendar/list`).then(
        (response)=>{
            return response
      })
}
function postCalendarsList(params){
    return connectivityModule.http.post(`Tutor/calendar/list`,params)
}
function calendarDate(dateTime){
    return dateTime.toISOString().substr(0, 10)
}

function calendarTime(dateTime){
    let hour = dateTime.getHours()
    if(hour < 10) return `0${hour}:00`
    else return `${hour}:00`
}

function CalendarEvent(objInit){

    var date = new Date(objInit);
    this.needToAdd = function() {
        return date.getHours() >7 && date.getHours() <= 23;
    };
    
    this.date = calendarDate(date);
    this.time = calendarTime(date);

    

}
function calendarEvents(objInit){
    let events = [];
    objInit.forEach(e =>{
                 //if(Object.keys(e).length === 0) return;
                let event = new CalendarEvent(e);
                if (event.needToAdd()) {
                    events.push(event);
                }
                 //events.push(calendarEvent(e))
             });
    return events;//
}

export default {
    signIn,
    getEvents,
    addEvent,
    getCalendarsList,
    postCalendarsList
}