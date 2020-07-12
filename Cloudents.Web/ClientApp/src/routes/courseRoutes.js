export const courseRoutes = [
   {
      path: "/courses",
      redirect: "/",
      children: [
         {
            path: '*',
            redirect: '/',

         }
      ]
   }
]