import { LanguageService } from '../../services/language/languageService'


export const HOME_MAX_SUGGEST_NUM = 8;
export const VERTICAL_MAX_SUGGEST_NUM = 3;

export const buildInSuggest = {

    note: [
        LanguageService.getValueByKey("buildinsuggest_note_microeconomics"),
        LanguageService.getValueByKey("buildinsuggest_note_pastExams"),
        LanguageService.getValueByKey("buildinsuggest_note_electrical")

    ],

    tutor: [
        LanguageService.getValueByKey("buildinsuggest_tutor_biology"),
        LanguageService.getValueByKey("buildinsuggest_tutorphysics_esl"),
        LanguageService.getValueByKey("buildinsuggest_tutor_accounting")
    ],

    ask: [
        LanguageService.getValueByKey("buildinsuggest_ask_plate"),
        LanguageService.getValueByKey("buildinsuggest_ask_logarithms"),
        LanguageService.getValueByKey("buildinsuggest_ask_triassic")

    ],
};


export const SUGGEST_TYPE = {history: "History", buildIn: "BuildIn", autoComplete: "AutoComplete"};