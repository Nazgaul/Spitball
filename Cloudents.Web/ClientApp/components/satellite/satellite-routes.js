import {LanguageService} from '../../services/language/languageService'

export const staticRoutes = [
    {
        name: "about",
        display: LanguageService.getValueByKey("satellite_routes_about"),
        import: () => import("./work.vue")

    },
    {
        name: "faq",
        display: LanguageService.getValueByKey("satellite_routes_faq"),
        import: () => import("./faq.vue"),
        params: (route) => ({ id: route.query.id })

    },
    //{
    //    name: "faqSelect",
    //    path: "/faq/:id",
    //    import: () => import("./faq.vue"),
    //    params:(route) => ({id: route.query.id})

    //},
    {
        name: "blog",
        display: LanguageService.getValueByKey("satellite_routes_blog")
        // import: ()=>import("./blog.vue"),
        // params:(route)=>({university:route.query.uni,path:route.query.path})

    },
    {
        name: "partners",
        display: LanguageService.getValueByKey("satellite_routes_partners"),
        import: () => import("./partner.vue")

    },
    {
        name: "reps",
        display: LanguageService.getValueByKey("satellite_routes_reps"),
        import: () => import("./reps.vue")

    },
    {
        name: "privacy",
        display: LanguageService.getValueByKey("satellite_routes_privacy"),
        import: () => import("./privacy.vue")

    },
    {
        name: "terms",
        display: LanguageService.getValueByKey("satellite_routes_terms"),
        import: () => import("./terms.vue")

    },
    {
        name: "contact",
        display: LanguageService.getValueByKey("satellite_routes_contact"),
        import: () => import("./contact.vue")

    },
    {
        name: "studentFaq",
        display: LanguageService.getValueByKey("satellite_routes_studentFaq"),
        import: () => import("./student/studentFaq.vue")

    },
    {
        name: "tutorFaq",
        display: LanguageService.getValueByKey("satellite_routes_tutorFaq"),
        import: () => import("./tutor/tutorFaq.vue")

    },

];