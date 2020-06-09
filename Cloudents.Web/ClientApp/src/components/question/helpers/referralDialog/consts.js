// import { LanguageService } from "../../../../services/language/languageService";
import { i18n } from '../../../../plugins/t-i18n'
function getReferallMessages(type, url) {
    let types = {
        menu: {
                url: url,
                encodedUrl: encodeURIComponent(url),
                title: i18n.t("referralDialog_join_me"),
                text: i18n.t("referralDialog_get_your_homework") + " " + encodeURIComponent(url),
                twitterText: i18n.t("referralDialog_join_me") + " " + i18n.t("referralDialog_get_your_homework_twitter"),
                whatsAppText: i18n.t("referralDialog_join_me") + " " + i18n.t("referralDialog_get_your_homework") + " " + encodeURIComponent(url),
        },
        uploadReffer: {
                url: url,
                encodedUrl: encodeURIComponent(url),
                title: i18n.t("referralDialog_title_document"),
                text: i18n.t("referralDialog_get_document_text") + " " + encodeURIComponent(url),
                twitterText: i18n.t("referralDialog_title_document") + " " + i18n.t("referralDialog_get_document_text"),
                whatsAppText: i18n.t("referralDialog_title_document") + " " + i18n.t("referralDialog_get_document_text") + " " + encodeURIComponent(url),
            }
    };
    return types[type];
}

export  {
    getReferallMessages,
}
