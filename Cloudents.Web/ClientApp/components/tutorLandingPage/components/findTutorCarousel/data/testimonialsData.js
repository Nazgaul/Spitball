import {LanguageService} from '../../../../../services/language/languageService.js'

export const reviews =  [
    [{
        name: LanguageService.getValueByKey("homePage_Testimonial1Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial1Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial1"),
        image:  global.lang.toLowerCase() === "he" ? "./images/testimonial1_he.jpg" : './images/testimonial1.jpg', 
    },
    {
        name: LanguageService.getValueByKey("homePage_Testimonial2Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial2Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial2"),
        image:  global.lang.toLowerCase() === "he" ? "./images/testimonial2_he.jpg" : './images/testimonial2.jpg',
    },
    {
        name: LanguageService.getValueByKey("homePage_Testimonial3Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial3Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial3"),
        image:  global.lang.toLowerCase() === "he" ? "./images/testimonial3_he.jpg" : './images/testimonial3.jpg',
    }]
];

export const mobileReviews =  
    [{
        name: LanguageService.getValueByKey("homePage_Testimonial1Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial1Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial1"),
        image:  global.lang.toLowerCase() === "he" ? "./images/testimonial1_he.jpg" : './images/testimonial1.jpg',
    },
    {
        name: LanguageService.getValueByKey("homePage_Testimonial2Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial2Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial2"),
        image:  global.lang.toLowerCase() === "he" ? "./images/testimonial2_he.jpg" : './images/testimonial2.jpg',
    },
    {
        name: LanguageService.getValueByKey("homePage_Testimonial3Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial3Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial3"),
        image:  global.lang.toLowerCase() === "he" ? "./images/testimonial3_he.jpg" : './images/testimonial3.jpg',
    }];
