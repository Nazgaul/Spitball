//note if you change the value in here you need to chanem in main.less
export const questionRoute = "ask";
export const flashcardRoute = "flashcard";
export const notesRoute = "note";
export const tutorRoute = "tutor";
export const bookRoute = "book";
export const bookDetailsRoute = "bookDetails";
export const foodRoute = "food";
export const foodDetailsRoute = "foodDetails";
export const jobRoute = "job";



export const staticRoutes = [
{
    name: "work",
    display: "How Spitball Works",
    import: () => import("./components/satellite/faq.vue")

},
{
    name: "faq",
    display: "FAQ",
    import: () => import("./components/satellite/faq.vue")

},
{
    name: "blog",
    display: "Blog",
    import: () => import("./components/satellite/faq.vue")

},
{
    name: "partners",
    display: "Partners",
    import: () => import("./components/satellite/faq.vue")

},
{
    name: "reps",
    display: "Reps",
    import: () => import("./components/satellite/faq.vue")

},
{
    name: "privacy",
    display: "Privacy",
    import: () => import("./components/satellite/faq.vue")

},
{
    name: "terms",
    display: "Terms",
    import: () => import("./components/satellite/faq.vue")

},
{
    name: "contact",
    display: "Contact",
    import: () => import("./components/satellite/faq.vue")

}
];


