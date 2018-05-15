import {
    googleRegistration,
    emailRegistration,
    smsRegistration,
    smsCodeVerification,
    getUserName,
    setUserName,
    getAccountNum
} from './resources';
import axios from "axios";
import qs from "query-string";

var instance = axios.create({
    baseURL : "/api"
});

export default {
    googleRegistration(model) {
        return googleRegistration(model);
    },
    emailRegistration(email,recaptcha) {
        return instance.post("register", qs.stringify( {email,captcha: recaptcha}));
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