const IntercomStatus = !!global.Intercom;
const IntercomSettings = createIntercomSettings();

function IntercomSettingsObj(){
    this.app_id = "njmpgayv";
    this.custom_launcher_selector = '#gH_i_r_intercom';
    this.hide_default_launcher = true;
    this.user_id = null;
    this.user_name = null;
    this.user_email = null;
    this.user_phoneNumber = null;
    this.alignment = global.isRtl ? 'left' : 'right';
    this.is_tutor = false;
}

IntercomSettingsObj.prototype.set = function({id, name, email, phoneNumber, isTutor, alignment}){
    if(IntercomStatus){
        this.custom_launcher_selector = '#gH_i_r_intercom';
        this.user_id = id ? "Sb_" + id : this.user_id;
        this.user_name = name ? name : this.user_name;
        this.user_email = email ? email : this.user_email;
        this.user_phoneNumber = phoneNumber ? phoneNumber : this.user_phoneNumber;
        this.is_tutor = isTutor !== undefined ? isTutor : this.is_tutor;
        this.hide_default_launcher = true;    
        this.alignment = alignment ? alignment : global.isRtl ? 'left' : 'right';
        bootIntercom();
    }
};

IntercomSettingsObj.prototype.reset = function(){
    if(IntercomStatus){
        shutIntercom();
        this.custom_launcher_selector = '#gH_i_r_intercom';
        this.user_id = null;
        this.user_name = null;
        this.user_email = null;
        this.user_phoneNumber = null;
        this.is_tutor = false;
        this.hide_default_launcher = true;    
        this.alignment = global.isRtl ? 'left' : 'right';
        bootIntercom();
    }
};

function createIntercomSettings(){
    return new IntercomSettingsObj();
}

function bootIntercom(){
    if(IntercomStatus){
        global.intercomSettings = {
            custom_launcher_selector : '#gH_i_r_intercom',
            app_id : IntercomSettings.app_id,
            hide_default_launcher: IntercomSettings.hide_default_launcher,
            user_id: IntercomSettings.user_id,
            name: IntercomSettings.user_name,
            email: IntercomSettings.user_email,
            phoneNumber: IntercomSettings.user_phoneNumber,
            alignment: IntercomSettings.alignment,
            language_override: global.lang,
            is_tutor: IntercomSettings.is_tutor
        };
        /* global intercomSettings */
        global.Intercom('boot', {intercomSettings});
    }
}

function shutIntercom(){
    if(IntercomStatus){
        global.Intercom('shutdown');
    }
}





export default{
    IntercomSettings,
    bootIntercom,
    shutIntercom
}