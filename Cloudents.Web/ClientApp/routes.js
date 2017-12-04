import HomePage from "./components/home/home.vue";
import * as RouteTypes from "./routeTypes";
const resultContent = () => import("./components/results/Result.vue");
const bookDetails = () => import("./components/results/ResultBookDetails.vue");
const showItem = () => import("./components/preview/Item.vue");
const showFlashcard = () => import("./components/preview/Flashcard.vue");
const notFound = () => import("./components/results/notFound.vue");
//const searchItem = () => import("./components/settings/searchItem.vue");
const moreInfo = () => import("./components/results/MoreInfo.vue");
function dynamicPropsFn(route) {
    let newName=route.path.slice(1);
    let filterOptions=[];
        if(route.query.filter){filterOptions=filterOptions.concat(route.query.filter);
        }else{
            if(route.query.source){
                filterOptions=filterOptions.concat(route.query.source);
            }
            if(route.query.course){
                filterOptions=filterOptions.concat(route.query.course);
            }
            if(route.query.jobType){
                filterOptions=filterOptions.concat(route.query.jobType);
            }
        }
    console.log(filterOptions);
    return {
        name: route.path.slice(1),
        query: route.query,
        filterSelection: filterOptions,
        sort: route.query.sort,
        userText: route.params.q,
        currentTerm:newName.includes('food')?route.meta.foodTerm:newName.includes('job')?route.meta.jobTerm:route.meta.term,
        params: route.params,
        hasExtra: route.path.slice(1).includes('food')
    }
}
function dynamicDetailsPropsFn(route) {
    return {
        name: route.name,
        query: route.query,
        filterOptions: 'all',
        sort: 'price',
        id: route.params.id,
        params: route.params
    }
}
function moreInfoFn(route){
   return{
       actions:route.path.includes('SubjectOrCourse')?[{name:"edit Subject"},{name:"Select Course"}]:[{name:"edit Search"}]
   }
}

const resultPage = {  default: resultContent };
const bookDetailsPage = { default: bookDetails };
const notFoundPage = { default: notFound };
const resultProps = { default: dynamicPropsFn};
const bookDetailsProps = { ...resultProps, default: dynamicDetailsPropsFn };
export const routes = [
    {
        path: "/", component: HomePage, name: "home", meta: {
            showHeader: false
            
        }
    },

    {
        path: "/result", name: "result", alias: [
            '/' + RouteTypes.questionRoute,
            '/' + RouteTypes.flashcardRoute,
            '/' + RouteTypes.notesRoute,
            '/' + RouteTypes.tutorRoute,
            '/' + RouteTypes.bookRoute,
            '/' + RouteTypes.jobRoute,
            '/' + RouteTypes.foodRoute
        ], components: resultPage, props: resultProps, meta: {
            showHeader: true
            
        }
    },
    {
        path: "/moreInfo", name: "moreInfo", alias: ['/searchOrQuestion', '/AddSubjectOrCourse'], component: moreInfo,
        meta: {
            showHeader: true
           
        }, props: moreInfoFn
    },
    {
        path: "/book/:type/:id",
        name: RouteTypes.bookDetailsRoute,
        components: bookDetailsPage,
        props: bookDetailsProps,
        meta: {
            pageName: 'book',
            showHeader: true 
        }
    },
    {
        path: "/not-found", name: "notFound", components: notFoundPage, alias: [
            //'/' + RouteTypes.postRoute,
            '/' + RouteTypes.uploadRoute,
            //'/' + RouteTypes.chatRoute,         
            //'/' + RouteTypes.createFlashcard,
            //'/' + RouteTypes.coursesRoute,
            //'/' + RouteTypes.likesRoute
        ], meta: {
            showHeader: true
        }
    },
    //{ path: "/" + RouteTypes.settingsRoute, name: RouteTypes.settingsRoute, component: settings, props: { searchApi: 'getUniversities', type: 'university' }, meta: { pageName: RouteTypes.settingsRoute, showHeader: true, showSidebar: true }},
    //{ path: "/university", name: RouteTypes.universityRoute, component: searchItem, props: { searchApi: 'getUniversities', type: 'university',selectCallback:function(){this.$router.go(-1)} }, meta: { pageName: RouteTypes.settingsRoute, showHeader: true, showSidebar: true }},
    //{ path: "/myCourses", name: RouteTypes.myCoursesRoute, component: searchItem, props: { searchApi: 'getCorses', type: 'course' }, meta: { pageName: RouteTypes.coursesRoute, showHeader: true, showSidebar: true }},
    //{
    //    path: "/walkthrough", name: "walkthrough", component: walkthrough, meta: {
    //        pageName: RouteTypes.settingsRoute, showHeader: true,
    //        //showSidebar: true,
    //        isStatic: true
    //    }
    //},
    //{
    //    path: "/aboutUs", name: "aboutUs", component: aboutUs, meta: {
    //        pageName: RouteTypes.settingsRoute, showHeader: true,
    //        //showSidebar: true,
    //        isStatic: true
    //    }
    //},
    {
        path: "/item/:university/:courseId/:courseName/:id/:itemName", name: "item", component: showItem, props: true, meta: {
            pageName: RouteTypes.notesRoute,
            showHeader: true
        }
    },
    {
        path: "/flashcard/:university/:courseId/:courseName/:id/:itemName", name: "flashcard", component: showFlashcard, props: true, meta: {
            pageName: RouteTypes.flashcardRoute, showHeader: true
        }
    }
];