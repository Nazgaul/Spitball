import { LanguageService } from "../../../../services/language/languageService";



function getReferallMessages(type, url) {
    let types = {
        menu: {
                url: url,
                encodedUrl: encodeURIComponent(url),
                title: LanguageService.getValueByKey("referralDialog_join_me"),
                text: LanguageService.getValueByKey("referralDialog_get_your_homework") + " " + encodeURIComponent(url),
                twitterText: LanguageService.getValueByKey("referralDialog_join_me") + " " + LanguageService.getValueByKey("referralDialog_get_your_homework_twitter"),
                whatsAppText: LanguageService.getValueByKey("referralDialog_join_me") + " " + LanguageService.getValueByKey("referralDialog_get_your_homework") + " " + encodeURIComponent(url),


        },
        uploadReffer: {
                url: url,
                encodedUrl: encodeURIComponent(url),
                title: LanguageService.getValueByKey("referralDialog_join_me"),
                text: LanguageService.getValueByKey("referralDialog_get_your_homework") + " " + encodeURIComponent(url),
                twitterText: LanguageService.getValueByKey("referralDialog_join_me") + " " + LanguageService.getValueByKey("referralDialog_get_your_homework_twitter"),
                whatsAppText: LanguageService.getValueByKey("referralDialog_join_me") + " " + LanguageService.getValueByKey("referralDialog_get_your_homework") + " " + encodeURIComponent(url),
            }


    };
    return types[type]

}



export  {
    getReferallMessages,
}
