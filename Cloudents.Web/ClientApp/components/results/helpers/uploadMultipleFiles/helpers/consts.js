import { LanguageService } from "../../../../../services/language/languageService";

export let documentTypes = [
    LanguageService.getValueByKey("upload_multiple_files_type_lecture"),
    LanguageService.getValueByKey("upload_multiple_files_type_textbook"),
    LanguageService.getValueByKey("upload_multiple_files_type_exam"),
    LanguageService.getValueByKey("upload_multiple_files_type_default")
];

