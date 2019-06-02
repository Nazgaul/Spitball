import { LanguageService } from "../language/languageService";
export const validationRules = {
    required: (value) => !!value || LanguageService.getValueByKey("formErrors_required"),
    positiveNumber: (value) => {
        const pattern = /^(?![0.]+$)\d+(\.\d{1,2})?$/gm;
        return pattern.test(value) || LanguageService.getValueByKey("formErrors_positive_only");
    },
    maximumChars: (value, maxLength) => {
        return value.length <= maxLength || ` ${maxLength} ${LanguageService.getValueByKey("formErrors_max_chars")}`;
    },
    minimumChars: (value, minLength) => {
        let trimmed = value.trim();
        return trimmed.length >= minLength || `${LanguageService.getValueByKey("formErrors_min_chars")} ${minLength}`;
    },
    maxVal: (value, max) => {
        return value <= max || `${LanguageService.getValueByKey("formErrors_max_number")} ${max}`;
    },
    minVal: (value, max) => {
        return value >= max || `${LanguageService.getValueByKey("formErrors_positive_only")} ${max}`;
    },
    notSpaces: (value) => {
        return value.trim().length >= 1 || `Empty spaces`;
    }
};
