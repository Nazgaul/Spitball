import { LanguageService } from "../../../../../services/language/languageService";

export let documentTypes = [
    // LanguageService.getValueByKey("upload_multiple_files_type_lecture"),
    // LanguageService.getValueByKey("upload_multiple_files_type_textbook"),
    // LanguageService.getValueByKey("upload_multiple_files_type_exam"),
    // LanguageService.getValueByKey("upload_multiple_files_type_default")
    {
        id: 'lecture',
        title:  LanguageService.getValueByKey("upload_multiple_files_type_lecture"),
        // icon:"sbf-lecture-note",
    },
    {
        id: 'textbook',
        title:  LanguageService.getValueByKey("upload_multiple_files_type_textbook"),
        // icon:"sbf-textbook-note"
    },
    {
        id: 'exam',
        title: LanguageService.getValueByKey("upload_multiple_files_type_exam"),
        // icon:"sbf-exam-note"
    },
    {
        id: 'none',
        title:  LanguageService.getValueByKey("upload_multiple_files_type_default")
        // icon:"sbf-document-note",
    },
];

