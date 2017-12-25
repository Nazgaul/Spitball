import { app, router } from "./main";

router.push("/");
router.onReady(() => {
  //const matchedComponents = router.getMatchedComponents()
  //console.log(matchedComponents);
  //router.options.routes.forEach(route => {
  //  console.log(route);
  //});
  //router.beforeResolve((to, from, next) => {
  //  const matched = router.getMatchedComponents(to)
  //  console.log(matched);
  //});
    app.$mount("#app");
})