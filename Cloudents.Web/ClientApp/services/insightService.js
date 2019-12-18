const EVENT_TYPES = {
    LOG: 'LOG',
    ERROR: 'ERROR',
    TRACK: 'TRACK',
};

function createEventName(type, name){
    return `${type}_${name}`;
}

const track={
    exception: function(exception, properties, measurements, severityLevel){
        /*
        Gaby: use this only in a TRY CATCH expression otherwise use the track Event function
        exception - An Error from a catch clause.
        handledAt - Deprecated. This argument is ignored. Please pass null.
        properties - Map of string to string: Additional data used to filter exceptions in the portal. Defaults to empty.
        measurements - Map of string to number: Metrics associated with this page, displayed in Metrics Explorer on the portal. Defaults to empty.
        severityLevel - Supported values: SeverityLevel.ts
        */
        global.appInsights.trackException(exception, properties, measurements, severityLevel);
    },
    event: function(type = EVENT_TYPES.LOG, name, properties, measurements){
        //type defines if the event is ERROR or TRACK
        let eventName = createEventName(type, name);
        /*  
            Example!
            appInsights.trackEvent("WinGame",{Game: currentGame.name, Difficulty: currentGame.difficulty},{Score: currentGame.score, Opponents: currentGame.opponentCount});
        */
        global.appInsights.trackEvent(eventName, properties, measurements);
    },
};
const authenticate= {
    set: function(userId){
        
        global.appInsights.setAuthenticatedUserContext(userId.toString());
    }
};


export default{
    track,
    EVENT_TYPES,
    authenticate
}