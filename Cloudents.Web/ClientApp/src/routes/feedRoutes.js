export const feedRoutes = [
    {
        path: "/feed",
        name: 'feed',
        redirect: "/",
        children: [
            {
                path: '*',
                redirect: '/',
            }
        ]
    },
]