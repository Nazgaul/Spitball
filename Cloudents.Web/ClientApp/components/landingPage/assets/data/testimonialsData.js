import { LanguageService } from "../../../../services/language/languageService";

export const reviews =  [
    [{
        name: LanguageService.getValueByKey("homePage_Testimonial1Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial1Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial1"),
        image:  global.country.toLowerCase() === "il" ? "./helpers/testimonials/images/testimonial1_he.jpg" : './helpers/testimonials/images/testimonial1.jpg',
    },
    {
        name: LanguageService.getValueByKey("homePage_Testimonial2Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial2Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial2"),
        image:  global.country.toLowerCase() === "il" ? "./helpers/testimonials/images/testimonial2_he.jpg" : './helpers/testimonials/images/testimonial2.jpg',
    },
    {
        name: LanguageService.getValueByKey("homePage_Testimonial3Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial3Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial3"),
        image:  global.country.toLowerCase() === "il" ? "./helpers/testimonials/images/testimonial3_he.jpg" : './helpers/testimonials/images/testimonial3.jpg',
    }],
    // [{
    //     name: LanguageService.getValueByKey("homePage_Testimonial4Person"),
    //     title: LanguageService.getValueByKey("homePage_Testimonial4Title"),
    //     text: LanguageService.getValueByKey("homePage_Testimonial4"),
    //     image: './helpers/testimonials/images/testimonial4.jpg'
    // }]
];

export const mobileReviews =  
    [{
        name: LanguageService.getValueByKey("homePage_Testimonial1Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial1Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial1"),
        image:  global.country.toLowerCase() === "il" ? "./helpers/testimonials/images/testimonial1_he.jpg" : './helpers/testimonials/images/testimonial1.jpg',
    },
    {
        name: LanguageService.getValueByKey("homePage_Testimonial2Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial2Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial2"),
        image:  global.country.toLowerCase() === "il" ? "./helpers/testimonials/images/testimonial2_he.jpg" : './helpers/testimonials/images/testimonial2.jpg',
    },
    {
        name: LanguageService.getValueByKey("homePage_Testimonial3Person"),
        title: LanguageService.getValueByKey("homePage_Testimonial3Title"),
        text: LanguageService.getValueByKey("homePage_Testimonial3"),
        image:  global.country.toLowerCase() === "il" ? "./helpers/testimonials/images/testimonial3_he.jpg" : './helpers/testimonials/images/testimonial3.jpg',
    }];
