import {LanguageService} from '../../services/language/languageService'

export const suggestList={
    tutor:LanguageService.getValueByKey("result_suggestions_tutor"),
    note:LanguageService.getValueByKey("result_suggestions_note"),
    ask:LanguageService.getValueByKey("result_suggestions_ask"),
};