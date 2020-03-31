import { i18n } from '../../plugins/t-i18n'

export const validationRules = {
    required: (value) => !!value || i18n.t("formErrors_required"),
    positiveNumber: (value) => {
        const pattern = /^(?![0.]+$)\d+(\.\d{1,2})?$/gm;
        return pattern.test(value) || i18n.t("formErrors_positive_only");
    },
    maximumChars: (value, maxLength) => {
        return value.length <= maxLength || ` ${maxLength} ${i18n.t("formErrors_max_chars")}`;
    },
    minimumChars: (value, minLength) => {
        let trimmed = value.trim();
        return trimmed.length >= minLength || `${i18n.t("formErrors_min_chars")} ${minLength}`;
    },
    maxVal: (value, max) => {
        return value <= max || `${i18n.t("formErrors_max_number")} ${max}`;
    },
    minVal: (value, max) => {
        return value >= max || `${i18n.t("formErrors_positive_only")} ${max}`;
    },
    notSpaces: (value) => {
        return value.trim().length >= 1 || i18n.t("formErrors_required");
    },
    email: (value) =>{
        let regex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return regex.test(value) || i18n.t("formErrors_email");
    },
    integer: (value) =>{
        return Number.isInteger(+value) || i18n.t("formErrors_integer");
    },
    phone: (value) =>{
        let regex = /^\d{8,13}$/;
        return regex.test(value) || i18n.t("formErrors_phone");
    },
    //this is a validation phone number only for tutorRequest
    phoneValidate: (value) =>{
        let regex = /^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\.0-9]+$/;
        return regex.test(value) || i18n.t("formErrors_phone");
    }
};
