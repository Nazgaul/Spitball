import HomePage from "./components/home/home.vue";
import * as RouteTypes from "./routeTypes";


const SectionsPage = () => import("./components/pages/page.vue");
const resultContent = () => import("./components/pages/results/Result.vue");
const resultPageHeader = () => import("./components/header/header.vue");
const resultPageNavbar = () => import("./components/navbar/navbar.vue");
const bookDetails = () => import("./components/pages/results/ResultBookDetails.vue");
function dynamicPropsFn(route) {
    return {
        name: route.name,
        query: route.query,
        filterOptions: route.query.filter || 'all',
        sort: route.query.sort || 'relevance',
        id: route.params.id,
        userText: route.params.q,
        params: route.params
    }
}
function homePropsFn(route) {
    return {
        metaType: route.meta.searchType,
        metaText: route.meta.userText
    }
}
const resultPage = { navbar: resultPageNavbar, default: resultContent, header: resultPageHeader }
const resultProps = { default: dynamicPropsFn, navbar: (route) => ({ userText: route.query.q }), header: (route) => ({ userText: route.query.q }) }
export const routes = [
    { path: "/", component: HomePage, name: "home", meta: {searchType: 'ask' }, props: homePropsFn },

    { path: "/ask", name: RouteTypes.questionRoute, components: resultPage, props: resultProps},
    { path: "/flashcard", name: RouteTypes.flashcardRoute, components: resultPage, props: resultProps },
    { path: "/note", name: RouteTypes.notesRoute, components: resultPage, props: resultProps},
    { path: "/tutor", name: RouteTypes.tutorRoute, components: resultPage, props: resultProps },
    { path: "/book", name: RouteTypes.bookRoute, components: resultPage, props: resultProps},
    { path: "/food", name: RouteTypes.foodRoute, components: resultPage, meta: { location: true }, props: resultProps },
    { path: "/job", name: RouteTypes.jobRoute, components: resultPage, props: resultProps },
    { path: '/book/:type/:id', name: RouteTypes.bookDetailsRoute, components: bookDetails, props: resultProps },
            { path: "not-found", name: RouteTypes.uploadRoute, components: resultPage },
            { path: "not-found", name: RouteTypes.postRoute, components: resultPage },
            { path: "not-found", name: RouteTypes.createFlashcard, components: resultPage },
            { path: "not-found", name: RouteTypes.chatRoute, components: resultPage },
            { path: "not-found", name: RouteTypes.coursesRoute, components: resultPage },
            { path: "not-found", name: RouteTypes.likesRoute, components: resultPage },
            { path: "not-found", name: RouteTypes.settingsRoute, components: resultPage }


];