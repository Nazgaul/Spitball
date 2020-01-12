function lazyComponent(path) {
   return () => import(`../components/${path}.vue`);
}
const routesDefaults = {
   banner: lazyComponent('pages/layouts/banner/banner'),
   header: lazyComponent('pages/layouts/header/header'),
   sideMenu: lazyComponent('pages/layouts/sideMenu/sideMenu'),
   footer: lazyComponent('pages/layouts/footer/footer')
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