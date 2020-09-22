// const IntercomStatus = !!global.Intercom;
// const TwakStatus = global.Tawk_API !== "undefined"

let intercom =  {};
let tawk =  {};
/*
Note you need to implement  start stop showdialog
*/

function ctor() {
    intercom.start = function () {
        // let id = {
        //     user_id: typeof (settings.id) === "number" ? "Sb_" + settings.id : null 
        // }
        // let globalSettings = {
        //     app_id: "njmpgayv",
        //     hide_default_launcher: true,
        //     alignment: global.isRtl ? 'left' : 'right',
        //     language_override: global.lang,
        // };
        // if (global.Intercom) {
        //     global.Intercom('boot', globalSettings);
        // }
    }
    intercom.stop = function () {
        // if (global.Intercom) {
        //     global.Intercom('shutdown');
        // }
    }
    intercom.showDialog = function () {
        // if (global.Intercom) {
        //     global.Intercom('showNewMessage');
        // }
    }

    tawk.start = function (settings) {
        if (global.Tawk_API && global.Tawk_API.setAttributes) {
            global.Tawk_API.setAttributes({
                'id': settings.id,
                email: settings.email,
                name: settings.name
            }, function () { });
        }
    }
    tawk.stop = function () {

    }
    tawk.showDialog = function () {
        if (global.Tawk_API) {
            global.Tawk_API.maximize();
        }
    }

}
ctor();

let services = [intercom, tawk];

function restrartService() {
    stopService();
    startService();
}

function stopService() {
    services.forEach((x) => x.stop());
    // if (IntercomStatus) {
    //     global.Intercom('shutdown');
    // }

}
function showDialog() {
    services.forEach((x) => x.showDialog());
}

function startService(obj) {
    obj = obj || {};

    services.forEach((x) => x.start(obj));
    // if (IntercomStatus) {
    //     let intercomSettings = {


    //         user_id: typeof (id) === "number" ? "Sb_" + obj.id : null,
    //         user_email: obj.email,
    //         is_tutor: obj.isTutor || false,
    //         name: obj.name,
    //         email: obj.email,
    //         phoneNumber: obj.phoneNumber,

    //     };
    //     global.Intercom('boot', intercomSettings);
    // }

}

// function shutIntercom(){
//     if(IntercomStatus){
//         global.Intercom('shutdown');
//     }
// }





export default {
    //IntercomSettings,
    // bootIntercom,
    startService,
    restrartService,
    showDialog
}