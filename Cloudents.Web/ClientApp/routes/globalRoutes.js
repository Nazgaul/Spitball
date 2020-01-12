export const globalRoutes = [
    {
        path:'*',
        redirect : () => {
            window.location = "/error/notfound?client=true";
        }
    }
]