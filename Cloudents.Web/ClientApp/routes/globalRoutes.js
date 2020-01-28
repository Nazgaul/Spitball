import { lazyComponent } from "./routesUtils";
import notFound from  "./../components/notFound.vue";

export const globalRoutes = [
    {
        path:'*',
        component: notFound// lazyComponent("notFound")
        // redirect : () => {
        //     debugger;
        //   //  window.location = "/error/notfound?client=true";
        // }
    }
]