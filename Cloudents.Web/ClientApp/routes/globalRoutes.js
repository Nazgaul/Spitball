import notFound from  "./../components/notFound2.vue";

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