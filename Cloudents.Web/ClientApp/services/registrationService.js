import { googleRegistration, emailRegistration, smsRegistration, smsCodeVerification, getUserName, setUserName, getAccountNum } from './resources';
export default {
    googleRegistration(model) {
        return googleRegistration(model);
    },
    emailRegistration(email,recaptch) {
        return emailRegistration(email,recaptch);
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
    },
    getAccountNum() {
        return getAccountNum();
    }
}