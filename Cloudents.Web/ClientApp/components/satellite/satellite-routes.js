export const staticRoutes = [
    {
        name: "about",
        display: "How Spitball Works",
        import: () => import("./work.vue")

    },
    {
        name: "faq",
        display: "FAQ",
        import: () => import("./faq.vue")

    },
    {
        name: "blog",
        display: "Blog",
        import: ()=>import("./blog.vue"),
        params:(route)=>({university:route.query.uni,path:route.query.path})

    },
    {
        name: "partners",
        display: "Partners",
        import: () => import("./partner.vue")

    },
    {
        name: "reps",
        display: "Reps",
        import: () => import("./reps.vue")

    },
    {
        name: "privacy",
        display: "Privacy",
        import: () => import("./privacy.vue")

    },
    {
        name: "terms",
        display: "Terms",
        import: () => import("./terms.vue")

    },
    {
        name: "contact",
        display: "Contact",
        import: () => import("./contact.vue")

    }
];