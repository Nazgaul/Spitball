export const staticRoutes = [
    {
        name: "work",
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
        import: () => import("./blog.vue")

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