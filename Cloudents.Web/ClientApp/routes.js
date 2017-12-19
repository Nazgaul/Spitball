﻿const HomePage = () => import("./components/home/home.vue");
const homePageHeader = () => import("./components/home/header.vue");
import * as RouteTypes from "./routeTypes";
const resultContent = () => import("./components/results/Result.vue");
const bookDetails = () => import("./components/details/ResultBookDetails.vue");
const foodDetails = () => import("./components/details/FoodDetails.vue");
const showItem = () => import("./components/preview/Item.vue");
const showFlashcard = () => import("./components/preview/Flashcard.vue");
const notFound = () => import("./components/results/notFound.vue");
const theNavbar = () => import("./components/navbar/TheNavbar.vue");
const moreInfo = () => import("./components/results/MoreInfo.vue");
const personalize=()=>import("./components/settings/ResultPersonalize.vue");
const pageHeader=()=>import("./components/header/header.vue");
const bookTabsMobile=()=>import("./components/details/bookDetailsTabsMobile.vue");
const mobileDetailsFirstLine=()=>import("./components/details/HeaderFirstLineMobile.vue");
import {page, verticalsName,verticalsNavbar,details} from './data'
const $_calcTerm=(name)=>{return (name.includes('food')||name.includes('purchase'))?'foodTerm':name.includes('job')?'jobTerm':'term'};

function dynamicPropsFn(route) {
    let newName=route.path.slice(1);
    let filterOptions=[];
        if(route.query.filter){filterOptions=filterOptions.concat(route.query.filter);
        }else{
            if(route.query.source){
                filterOptions=filterOptions.concat(route.query.source);
            }
            if(route.query.course||
            (newName.includes("flashcard")||newName.includes("note"))
            ){
                let list=[].concat(route.query.course?route.query.course:[]);
                list=list.concat(route.meta.myClasses?route.meta.myClasses:[]);
                filterOptions=filterOptions.concat([...new Set(list.map(i=>Number(i)))]);
            }
            if(route.query.jobType){
                filterOptions=filterOptions.concat(route.query.jobType);
            }
        }
    return {
        name: route.path.slice(1),
        query: route.query,
        filterSelection: filterOptions,
        sort: route.query.sort,
        userText: route.query.q,
        currentTerm: newName.includes("food")
            ? route.meta.foodTerm
            : newName.includes("job")
            ? route.meta.jobTerm
            : route.meta.term,
        params: route.params,
        hasExtra: newName.includes('food'),
        page: page[newName],
        $_calcTerm: $_calcTerm,
        getFacet: route.meta[`${newName}Facet`],
        currentSuggest: verticalsName.filter(i => i !== newName)[(Math.floor(Math.random() * (verticalsName.length - 2)))],
        vertical:route.meta.vertical
    }
}
function dynamicDetailsPropsFn(route) {
    return {
        name: route.name,
        query: route.query,
        filterOptions: [{ title: "Book Type", modelId: "filter", data: details.bookDetails.filter }],
        sort: "price",
        id: route.params.id,
        params: route.params,
        page: page[route.name],
        $_calcTerm: $_calcTerm
    }
}
function moreInfoFn(route){
   return{
       actions:route.path.includes("SubjectOrCourse")?[{name:"edit Subject"},{name:"Select Course"}]:[{name:"edit Search"}]
   };
}
function verticalsLinkFun(route){
    return{
        $_calcTerm:$_calcTerm,
        verticals:verticalsNavbar,
        currentSelection:route.name==="result"?route.path.slice(1):"book"
    }
}
function filterLinkFun(route){
    return{
        verticals:[{id:"all",name:"all"}, ...details.bookDetails.filter],
        callbackFunc:function(res){
            console.log(res);
            this.$root.$children[0].$refs.mainPage.filter=res;
        },
        currentSelection:"all"
    }
}

const resultPage = {  default: resultContent ,verticalList:theNavbar,personalize,verticalListMobile:theNavbar,header:pageHeader};
const bookDetailsPage = { default: bookDetails,verticalList:theNavbar,personalize,verticalListMobile:theNavbar,bookDetailsSecondLineMobile:bookTabsMobile,mobileHeaderFirstLine:mobileDetailsFirstLine,header:pageHeader};
const foodDetailsPage = { default: foodDetails,mobileHeaderFirstLine:mobileDetailsFirstLine,header:pageHeader};
const notFoundPage = { default: notFound };
const resultProps = { default: dynamicPropsFn,verticalList:verticalsLinkFun,verticalListMobile:verticalsLinkFun};

const foodDetailsProps = { default: true,mobileHeaderFirstLine:{name:"Food and Deals"},header:{showSingleLine:true,showMoreOptions:false}};
const bookDetailsProps = { ...resultProps, default: dynamicDetailsPropsFn,verticalListMobile:filterLinkFun,bookDetailsSecondLineMobile:true,mobileHeaderFirstLine:{name:"textbooks"}};
export const routes = [
    {
        path: "/", components: {
            default: HomePage,
            header: homePageHeader
        }, name: "home", meta: {
            
        }
    },

    {
        path: "/result", name: "result", alias: [
            "/" + RouteTypes.questionRoute,
            "/" + RouteTypes.flashcardRoute,
            "/" + RouteTypes.notesRoute,
            "/" + RouteTypes.tutorRoute,
            "/" + RouteTypes.bookRoute,
            "/" + RouteTypes.jobRoute,
            "/" + RouteTypes.foodRoute
        ], components: resultPage, props: resultProps, meta: {
            showHeader: true
            
        }
    },
    {
        path: "/moreInfo", name: "moreInfo", alias: ["/searchOrQuestion", "/AddSubjectOrCourse"], component: moreInfo,
        meta: {
            showHeader: true
           
        }, props: moreInfoFn
    },
    {
        path: "/book/:id",
        name: RouteTypes.bookDetailsRoute,
        components: bookDetailsPage,
        props: bookDetailsProps,
        meta: {
            pageName: "book",
            showHeader: true 
        }
    },
    {
        path: "/food/:id",
        name: RouteTypes.foodDetailsRoute,
        components: foodDetailsPage,
        props: foodDetailsProps,
        meta: {
            showHeader: true,
            showHeaderSingleLine:true
        }
    },
    {
        path: "/not-found", name: "notFound", components: notFoundPage, alias: [

        ], meta: {
            showHeader: true
        }
    },
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