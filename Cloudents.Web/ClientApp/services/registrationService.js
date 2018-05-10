import { googleRegistration, emailRegistration, smsRegistration, smsCodeVerification, getUserName, setUserName } from './resources';
export default {
    googleRegistration(model) {
        return googleRegistration(model);
    },
    emailRegistration(model) {
        return emailRegistration(model);
    },
    smsRegistration(model) {
        return smsRegistration(model);
    },
    smsCodeVerification(model) {
        return smsCodeVerification(model);
    },
    getUserName() {
        return getUserName();
    },
    setUserName(model) {
        return setUserName(model);
    }
}