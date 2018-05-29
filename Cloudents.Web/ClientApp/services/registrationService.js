import axios from "axios";
import qs from "query-string";



export default {
    googleRegistration: (data) => axios.post("/Register/google", {token: data}),
    emailRegistration: (email,recaptcha) => axios.post("register", {email,captcha: recaptcha}),
    smsRegistration: (data) => axios.post("/Register/sms", {number: data}),
    smsCodeVerification: (data) => axios.post("/Register/sms/verify", {number: data}),
    signIn: (email,key,captcha) => axios.post("login", {email,key, captcha})
}