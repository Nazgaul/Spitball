import HomePage from "./components/home/home.vue";
import SectionsPage from "./components/sections/sections.vue";

export const routes = [
    { path: "/", component: HomePage, name: "home" },
    { path: "/result", component: SectionsPage, name: "result" }
];