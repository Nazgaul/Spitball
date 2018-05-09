import { googleRegistration, emailRegistration, smsRegistration } from './resources';
export default {
    googleRegistration(model) {
        return googleRegistration(model);
    },
    emailRegistration(model) {
        return emailRegistration(model);
    },
    smsRegistration(model) {
        debugger;
        return smsRegistration(model);
    }
}