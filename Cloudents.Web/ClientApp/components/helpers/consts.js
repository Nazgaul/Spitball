import { LanguageService } from '../../services/language/languageService'


export const HOME_MAX_SUGGEST_NUM = 8;
export const VERTICAL_MAX_SUGGEST_NUM = 3;

export const buildInSuggest = {

    note: [
        LanguageService.getValueByKey("buildinsuggest_note_microeconomics"),
        LanguageService.getValueByKey("buildinsuggest_note_pastExams"),
        LanguageService.getValueByKey("buildinsuggest_note_electrical"),

    ],
    flashcard: [
        LanguageService.getValueByKey("buildinsuggest_flashcard_calculus"),
        LanguageService.getValueByKey("buildinsuggest_flashcard_physics_study"),
        LanguageService.getValueByKey("buildinsuggest_flashcard_spanish"),
    ],
    tutor: [
        LanguageService.getValueByKey("buildinsuggest_tutor_biology"),
        LanguageService.getValueByKey("buildinsuggest_tutorphysics_esl"),
        LanguageService.getValueByKey("buildinsuggest_tutor_accounting"),

    ],
    book: [
        "ISBN 1133112285",
        LanguageService.getValueByKey("buildinsuggest_book_biology"),
        LanguageService.getValueByKey("buildinsuggest_book_gregory"),

    ],
    ask: [
        LanguageService.getValueByKey("buildinsuggest_ask_plate"),
        LanguageService.getValueByKey("buildinsuggest_ask_logarithms"),
        LanguageService.getValueByKey("buildinsuggest_ask_triassic"),

    ],
    job: [
        LanguageService.getValueByKey("buildinsuggest_job_ambassador"),
        LanguageService.getValueByKey("buildinsuggest_job_internship"),
        LanguageService.getValueByKey("buildinsuggest_job_design"),

    ]
};


export const SUGGEST_TYPE = {history: "History", buildIn: "BuildIn", autoComplete: "AutoComplete"};