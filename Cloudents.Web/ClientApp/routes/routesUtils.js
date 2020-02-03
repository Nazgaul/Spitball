function lazyComponent(path) {
   return () => import(`../components/${path}.vue`);
}
const routesDefaults = {
   banner: () => import(`../components/pages/layouts/banner/bannerWrapper.vue`),
   header: () => import(`../components/pages/layouts/header/header.vue`) ,
   sideMenu: () => import(`../components/pages/layouts/sideMenu/sideMenu.vue`) ,
   footer: () => import(`../components/pages/layouts/footer/footer.vue`) 
}

function staticComponents(components) {
   let defaultRoutes = {};
   components.forEach(comp => defaultRoutes[comp] = routesDefaults[comp]);
   return defaultRoutes;
}
export{
   lazyComponent,
   staticComponents,
}