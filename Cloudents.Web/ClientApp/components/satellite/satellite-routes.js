export const staticRoutes = [
    {
        name: "work",
        display: "How Spitball Works",
        import: () => import("./faq.vue")

    },
    {
        name: "faq",
        display: "FAQ",
        import: () => import("./faq.vue")

    },
    {
        name: "blog",
        display: "Blog",
        import: () => import("./faq.vue")

    },
    {
        name: "partners",
        display: "Partners",
        import: () => import("./faq.vue")

    },
    {
        name: "reps",
        display: "Reps",
        import: () => import("./faq.vue")

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
        import: () => import("./faq.vue")

    }
];