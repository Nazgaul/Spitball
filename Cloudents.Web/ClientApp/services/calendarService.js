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
            let events = []
            response.data.forEach(e =>{
                if(Object.keys(e).length === 0) return;
                events.push(calendarEvent(e))
            })
            return events
      })
}

function addEvent(params){
    return connectivityModule.http.post(`Tutor/calendar/events`,params).then(response=>{

    })
}


function calendarDate(dateTime){
    return new Date(dateTime).toISOString().substr(0, 10)
}

function calendarTime(dateTime){
    let hour = new Date(dateTime).getHours()
    if(hour < 10) return `0${hour}:00`
    else return `${hour}:00`
}

function CalendarEvent(objInit){
    this.date = calendarDate(objInit.from),
    this.time = calendarTime(objInit.from)
}
function calendarEvent(objInit){
    return new CalendarEvent(objInit)
}

export default {
    signIn,
    getEvents,
    addEvent,
}