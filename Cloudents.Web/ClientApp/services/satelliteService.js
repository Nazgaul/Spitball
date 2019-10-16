// import { help } from './resources';
import { connectivityModule } from "./connectivity.module";
import {LanguageService} from './language/languageService';

let cacheControl = `?v=${global.version}&l=${global.lang}`;
const sattelites = {
    about:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_about'), 
        icon: 'sbf-about',
        url:{
            en:"https://help.spitball.co/en/about-us",
            he:"https://help.spitball.co/he/article/%D7%94%D7%9B%D7%9C-%D7%A2%D7%9C%D7%99%D7%A0%D7%95"
        } 
    },
    feedback:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_feedback'),
        
        url:{
            en:"https://help.spitball.co/en/article/how-to-contact-us",
            he:"https://help.spitball.co/he/contact"
        } 
    },
    terms: {
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_terms'),
        icon: 'sbf-terms',
        url:{
            en:"https://help.spitball.co/en/article/terms-of-service",
            he:"https://help.spitball.co/en/article/terms-of-service"
        } 
    },
    privacy:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_privacy'),
        icon: 'sbf-privacy',
        url:{
            en:"https://help.spitball.co/en/article/privacy-policy",
            he:"https://help.spitball.co/en/article/privacy-policy"
        } 
    },
    faq:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_faq'),
        icon: 'sbf-help',
         url:{
            en:"https://help.spitball.co/en/faq",
            he:"https://help.spitball.co/he/%D7%A9%D7%90%D7%9C%D7%95%D7%AA-%D7%A0%D7%A4%D7%95%D7%A6%D7%95%D7%AA"
        } 
    },
    contact:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_contact'),
         url:{
            en:"https://help.spitball.co/en/article/how-to-contact-us",
            he:"https://help.spitball.co/he/contact"
        } 
    },
}
export default {
    getSatelliteUrlByName(name, lang){
        let language = !!lang ? lang : global.lang
        if(!sattelites[name]){
            console.error(`no satellite name ${name} found`);
        }
        console.log(sattelites[name].url[language]);
        return sattelites[name].url[language];
    },
}
