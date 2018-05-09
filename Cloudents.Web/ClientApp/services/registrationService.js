import { googleRegistration, emailRegistration } from './resources';
export default {
    googleRegistration(model) {
        return googleRegistration(model);
    },
    emailRegistration(model) {
        return emailRegistration(model);
    }
}