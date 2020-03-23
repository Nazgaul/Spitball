// import { connectivityModule } from "./connectivity.module";
import {LanguageService} from './language/languageService';
import store from '../store/index';
// let cacheControl = `?v=${global.version}&l=${global.lang}`;
const isFrymo = store.getters['isFrymo'];




function websitePrefix(){
    return `${isFrymo ? 'frymo.com':'spitball.co'}`;
}

function urlForEnglishUsers(type){
    let urlByType = {
        about: isFrymo ? `https://help.${websitePrefix()}/en/article/welcome-to-frymo` : `https://help.${websitePrefix()}/en/about-us`,
        feedback: `https://help.${websitePrefix()}/en/article/how-to-contact-us`,
        terms: isFrymo ? `https://help.${websitePrefix()}/en/article/terms` : `https://help.${websitePrefix()}/en/article/terms-of-service`,
        privacy: `https://help.${websitePrefix()}/en/article/privacy-policy`,
        faq: isFrymo ? `https://help.${websitePrefix()}/en/` : `https://help.${websitePrefix()}/en/`,
        contact: `https://help.${websitePrefix()}/en/article/how-to-contact-us`,
    };
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
            he:"https://help.spitball.co/he/"
        } 
    },
    contact:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_contact'),
         url:{
            en: urlForEnglishUsers('contact'),
            he:"https://help.spitball.co/he/contact"
        } 
    },
    blog:{
        title: LanguageService.getValueByKey('tutorListLanding_footer_links_blog'),
         url:{
            en:'https://www.blog.spitball.co/blog/categories/english',
            he:"https://www.blog.spitball.co/blog/categories/hebrew"
        } 
    }
};
const socialMedias = {
    medium:{
        icon:'sbf-social-medium-small',
        svg: '<svg xmlns="http://www.w3.org/2000/svg" width="21" height="18" viewBox="0 0 21 18"><path fill="#FFF" fill-rule="nonzero" d="M7 17.29c0 .444-.238.71-.576.71a.886.886 0 0 1-.393-.104l-5.449-2.76c-.32-.161-.582-.591-.582-.952V.656C0 .3.19.088.46.088c.097 0 .204.025.315.082l.192.098h.002l6.006 3.04c.01.006.017.016.025.024v13.957zM13.293.94l.367-.603a.722.722 0 0 1 .922-.266l6.334 3.207c.002 0 .002.002.002.002l.006.002c.004.002.004.008.008.01.03.025.045.068.023.103L14.646 13.78l-.644 1.059-4.18-8.186 3.471-5.71zM8 11.803V5.28l4.49 8.797-4.039-2.045-.451-.23zm13 5.486c0 .418-.248.656-.613.656-.164 0-.352-.047-.55-.146l-.903-.46-4.04-2.044L21 5.248v12.041z"/></svg>',
        hidden: isFrymo, //change to true if you want it to be hidden
        url:{
            he:'https://medium.com/@spitballstudy',
            en:'https://medium.com/@spitballstudy'
        }

        
    },
    linkedin:{
        icon:'sbf-social-linkedin',
        svg: '<svg xmlns="http://www.w3.org/2000/svg" width="30" height="21"><g fill="none" fill-rule="evenodd"><path fill="#FFF" fill-rule="nonzero" d="M24.923 0H5.077C2.273 0 0 2.299 0 5.133v10.734C0 18.7 2.273 21 5.077 21h19.846C27.727 21 30 18.701 30 15.867V5.133C30 2.3 27.727 0 24.923 0zM16.27 12.833l-4.5 2.334V5.833l9 4.667-4.5 2.333z"/></g></svg>',
        hidden: false, //change to true if you want it to be hidden
        url:{
            he: 'https://linkedin.com/company/spitball',
            en: isFrymo ? 'https://www.linkedin.com/company/frymogo ': 'https://linkedin.com/company/spitball',
        }
       
    },
    facebook:{
        icon:'sbf-social-facebook',
        svg: '<svg xmlns="http://www.w3.org/2000/svg" width="14" height="25"><g fill="none" fill-rule="evenodd"><path fill="#FFF" fill-rule="nonzero" d="M13.473.005L10.114 0c-3.772 0-6.21 2.415-6.21 6.152V8.99H.528A.52.52 0 0 0 0 9.5v4.11c0 .282.237.51.528.51h3.377V24.49c0 .281.236.51.528.51h4.405a.52.52 0 0 0 .528-.51V14.12h3.947a.52.52 0 0 0 .528-.51l.002-4.11a.501.501 0 0 0-.155-.361.538.538 0 0 0-.373-.15h-3.95V6.585c0-1.156.285-1.742 1.845-1.742l2.262-.001a.52.52 0 0 0 .528-.51V.515a.52.52 0 0 0-.527-.51z"/></g></svg>',
        hidden:  false, //change to true if you want it to be hidden
        url:{
            he: 'https://www.facebook.com/spitballstudy/',
            en: isFrymo ? 'https://www.facebook.com/learnfrymo': 'https://www.facebook.com/spitballstudy/',
        }
       
    },
    youtube:{
        icon:'sbf-social-youtube',
        svg: '<svg xmlns="http://www.w3.org/2000/svg" width="30" height="21"><g fill="none" fill-rule="evenodd"><path fill="#FFF" fill-rule="nonzero" d="M24.923 0H5.077C2.273 0 0 2.299 0 5.133v10.734C0 18.7 2.273 21 5.077 21h19.846C27.727 21 30 18.701 30 15.867V5.133C30 2.3 27.727 0 24.923 0zM16.27 12.833l-4.5 2.334V5.833l9 4.667-4.5 2.333z"/></g></svg>',
        hidden: isFrymo , //change to true if you want it to be hidden
        url:{
            he: 'https://www.youtube.com/channel/UCamYabfxHUP3A9EFt1p94Lg/',
            en: 'https://www.youtube.com/channel/UCamYabfxHUP3A9EFt1p94Lg/',
        }
        
    },
    telegram:{
        icon:'sbf-social-telegram',
        svg: '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="22"><g fill="none" fill-rule="evenodd"><path fill="#FFF" fill-rule="nonzero" d="M22.248.244L.934 9.115s-1.008.367-.93 1.042c.08.678.903.987.903.987l5.363 1.915s1.619 5.632 1.938 6.703c.318 1.069.574 1.093.574 1.093.296.137.566-.08.566-.08l3.465-3.35 5.4 4.391c1.46.677 1.993-.732 1.993-.732L24 .78c0-1.352-1.752-.536-1.752-.536zm-3.773 19.207l-5.778-4.698-1.793 1.733.394-3.686L19 5.5 8.502 11.732l-4.656-1.661 17.766-7.395-3.137 16.775z"/></g></svg>',
        hidden: isFrymo , //change to true if you want it to be hidden
        url:{
            he: 'https://t.me/Spitball',
            en: 'https://t.me/Spitball',
        }
        
    },
    twitter:{
        icon:'sbf-social_twitter',
        svg: '<svg xmlns="http://www.w3.org/2000/svg" width="30" height="21"><path fill="#FFF" fill-rule="nonzero" d="M17.015 0c1.367 0 2.56.507 3.578 1.521.058.06.115.09.175.09h.057A10.982 10.982 0 0 0 23.501.598c.03-.03.32-.18.378-.209a5.256 5.256 0 0 1-2.181 2.832c.494-.059.96-.149 1.425-.267.466-.12.93-.299 1.424-.507a8.592 8.592 0 0 1-.727.984 10.845 10.845 0 0 1-1.687 1.61c-.058.059-.087.089-.087.179v.894c-.115 3.545-1.308 6.705-3.547 9.447-1.833 2.177-4.102 3.667-6.835 4.412a15.43 15.43 0 0 1-2.56.478H6.661c-.697 0-1.366-.119-2.036-.268-1.629-.359-3.113-.985-4.538-1.88L0 18.214c.378.031.727.06 1.105.06.931 0 1.891-.119 2.793-.388a9.368 9.368 0 0 0 3.49-1.818c-2.297-.209-3.868-1.401-4.683-3.578.29.06.611.06.902.06.436 0 .902-.06 1.339-.18-1.164-.267-2.124-.894-2.88-1.848C1.31 9.568.962 8.465.962 7.211c.698.389 1.454.627 2.24.627-1.077-.804-1.775-1.819-2.066-3.13-.29-1.311-.088-2.564.552-3.757.088.09.145.15.203.24C4.014 3.723 6.66 5.332 9.86 6.05c.697.149 1.366.238 2.065.298H12.042c.058 0 .058-.03.058-.12a5.467 5.467 0 0 1-.088-1.37c.088-1.461.67-2.654 1.745-3.609.785-.714 1.687-1.133 2.734-1.222.117.003.32-.027.524-.027z"/></svg>',
        hidden: false , //change to true if you want it to be hidden
        url:{
            he: 'https://twitter.com/spitballstudy',
            en: isFrymo ? 'https://twitter.com/frymo_official': 'https://twitter.com/spitballstudy',
        }
    },
    instagram:{
        icon:'sbf-social_instagram',
        svg: '<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="20" height="20"><defs><path id="a" d="M0 0h20v20H0z"/></defs><g fill="none" fill-rule="evenodd"><mask id="b" fill="#fff"><use xlink:href="#a"/></mask><path fill="#FFF" d="M17.715 16.543c-.018.928-.256 1.194-1.191 1.194H3.467c-.917 0-1.173-.266-1.173-1.194 0-2.414.009-4.872-.036-7.303 0-.69.195-.84.83-.823.908.08.926.036.83.938-.354 3.076 1.781 6.092 4.816 6.684 3.14.601 6.273-1.282 7.137-4.297.256-.937.335-1.954.141-2.874-.07-.353.018-.415.327-.415 1.376-.115 1.376-.133 1.376 1.3v6.79zM10.013 6.11c2.082 0 3.873 1.803 3.847 3.89-.018 2.14-1.8 3.9-3.917 3.864-2.1-.036-3.873-1.822-3.82-3.9.044-2.104 1.808-3.854 3.89-3.854zm5.717-3.838c1.985 0 1.985 0 1.985 1.98 0 1.937 0 1.937-1.95 1.937-1.958 0-1.958 0-1.958-1.971 0-1.946 0-1.946 1.923-1.946zM17.353 0H2.655C.91 0 0 .928 0 2.679v14.704C0 19.072.935 20 2.567 20h14.786C19.056 20 20 19.072 20 17.365V2.617C20 .928 19.056 0 17.353 0z" mask="url(#b)"/></g></svg>',
        hidden: !isFrymo , //change to true if you want it to be hidden
        url:{
            //he: 'https://twitter.com/spitballstudy',
            en: 'https://www.instagram.com/frymo_official/' //isFrymo ? 'https://twitter.com/frymo_official': 'https://twitter.com/spitballstudy',
        }
    }   
};

function socialMediaObj(prop){
    let language = global.lang;
    this.icon = socialMedias[prop].icon;
    this.url = socialMedias[prop].url[language];
    this.svg = socialMedias[prop].svg;
}

export default {
    getSatelliteUrlByName(name, lang){
        let language = !!lang ? lang : global.lang;
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

    getSocialMedias(){
        let result = [];
        Object.keys(socialMedias).forEach((prop)=>{
            if(!socialMedias[prop].hidden){
                result.push(new socialMediaObj(prop));
            }
        });
        return result;     
    }
}
