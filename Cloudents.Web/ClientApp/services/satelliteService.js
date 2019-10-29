// import { help } from './resources';
import { connectivityModule } from "./connectivity.module";
import {LanguageService} from './language/languageService';
import store from '../store/index';
let cacheControl = `?v=${global.version}&l=${global.lang}`;
const isFrymo = store.getters['isFrymo'];

function websitePrefix(){
    return `${isFrymo ? 'frymo.com':'spitball.co'}`
}

function urlForEnglishUsers(type){
    let urlByType = {
        about: `https://help.${websitePrefix()}/en/about-us`,
        feedback: `https://help.${websitePrefix()}/en/article/how-to-contact-us`,
        terms: isFrymo ? `https://help.${websitePrefix()}/en/article/terms` : `https://help.${websitePrefix()}/en/article/terms-of-service`,
        privacy: isFrymo ? `https://help.${websitePrefix()}/en/policies` : `https://help.${websitePrefix()}/en/article/privacy-policy`,
        faq: isFrymo ? `https://help.${websitePrefix()}/en/faqs` : `https://help.${websitePrefix()}/en/faq`,
        contact: `https://help.${websitePrefix()}/en/article/how-to-contact-us`,
    }
    return urlByType[type];
}

const sattelites = {
    about:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_about'), 
        icon: 'sbf-about',
        url:{
            en: urlForEnglishUsers('about'),
            he:"https://help.spitball.co/he/article/%D7%94%D7%9B%D7%9C-%D7%A2%D7%9C%D7%99%D7%A0%D7%95"
        } 
    },
    feedback:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_feedback'),
        
        url:{
            en: urlForEnglishUsers('feedback'),
            he:"https://help.spitball.co/he/contact"
        } 
    },
    terms: {
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_terms'),
        icon: 'sbf-terms',
        url:{
            en: urlForEnglishUsers('terms'),
            he:"https://help.spitball.co/en/article/terms-of-service"
        } 
    },
    privacy:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_privacy'),
        icon: 'sbf-privacy',
        url:{
            en: urlForEnglishUsers('privacy'),
            he:"https://help.spitball.co/en/article/privacy-policy"
        } 
    },
    faq:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_faq'),
        icon: 'sbf-help',
         url:{
            en: urlForEnglishUsers('faq'),
            he:"https://help.spitball.co/he/%D7%A9%D7%90%D7%9C%D7%95%D7%AA-%D7%A0%D7%A4%D7%95%D7%A6%D7%95%D7%AA"
        } 
    },
    contact:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_contact'),
         url:{
            en: urlForEnglishUsers('contact'),
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

        let satteliteUrl = sattelites[name].url[language];
        if(!!satteliteUrl){
            return satteliteUrl;
        }else{
            //get default english URL's by default
            console.error(`no satellite url for ${language} exists, will bring default EN url instead`);
            return sattelites[name].url['en'];
        }
        
    },
}
