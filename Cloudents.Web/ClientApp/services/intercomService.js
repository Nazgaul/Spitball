// const IntercomStatus = !!global.Intercom;
// const TwakStatus = global.Tawk_API !== "undefined"

let intercom =  {};
let tawk =  {};
/*
Note you need to implement  start stop showdialog
*/

function ctor() {
    intercom.start = function (settings) {
        let id = {
            user_id: typeof (settings.id) === "number" ? "Sb_" + settings.id : null 
        }
        let globalSettings = {
            app_id: "njmpgayv",
            hide_default_launcher: true,
            alignment: global.isRtl ? 'left' : 'right',
            language_override: global.lang,
        };
        if (global.Intercom) {
            global.Intercom('boot', { ...globalSettings, ...settings ,...id});
        }
    }
    intercom.stop = function () {
        if (global.Intercom) {
            global.Intercom('shutdown');
        }
    }
    intercom.showDialog = function () {
        if (global.Intercom) {
            global.Intercom('showNewMessage');
        }
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
//const IntercomSettings = new IntercomSettingsObj();


// function IntercomSettingsObj() {
//     this.user_id = null;
//     this.user_name = null;
//     this.user_email = null;
//     this.user_phoneNumber = null;
//     this.is_tutor = false;
// }

// IntercomSettingsObj.prototype.set = function ({ id, name, email, phoneNumber, isTutor }) {
//     if (IntercomStatus) {
//         this.user_id = id ? "Sb_" + id : this.user_id;
//         this.user_name = name ? name : this.user_name;
//         this.user_email = email ? email : this.user_email;
//         this.user_phoneNumber = phoneNumber ? phoneNumber : this.user_phoneNumber;
//         this.is_tutor = isTutor !== undefined ? isTutor : this.is_tutor;
//         startIntercom();
//     }
// };

// IntercomSettingsObj.prototype.reset = function(){
//     if(IntercomStatus){
//        // shutIntercom();
//         //this.custom_launcher_selector = '#gH_i_r_intercom';
//         this.user_id = null;
//         this.user_name = null;
//         this.user_email = null;
//         this.user_phoneNumber = null;
//         this.is_tutor = false;
//         //this.hide_default_launcher = true;    
//         //this.alignment = global.isRtl ? 'left' : 'right';
//         startIntercom();
//     }
// };

// function createIntercomSettings(){
//     return new IntercomSettingsObj();
//}
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