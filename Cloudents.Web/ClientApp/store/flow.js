﻿import {FLOW} from "./mutation-types";
import * as routes from "./../routeTypes";

const state = {
    node: {}
};

const buildFlashcardRoute = {
    name: routes.flashcardRoute,
    children: [
        {name:routes.tutorRoute,children:[
            {name:routes.notesRoute,children:[
                {name:routes.questionRoute}
            ]}
        ]}
    ]
};
const buildDocumentRoute = {
    name: routes.notesRoute,
    children: [
        {name: routes.flashcardRoute,
            children: [
                {name:routes.tutorRoute,children:[
                    {name:routes.questionRoute}
                ]}
            ]}
    ]
};
const buildQuestionRoute = {
    name: routes.questionRoute,
    children: [
        {name: routes.notesRoute,
            children: [
                {name:routes.flashcardRoute,children:[
                    {name:routes.tutorRoute,children:[
                        {name:routes.bookRoute}]}
                ]}
            ]}
    ]
};
const buildTutorRoute = {
    name: routes.tutorRoute,
    children: [
        {name: routes.flashcardRoute,
            children: [
                {name:routes.notesRoute,children:[
                    {name:routes.bookRoute}
                ]}
            ]}
    ]
};
const optionalRoutes = {
    flashcard: buildFlashcardRoute,
    document: buildDocumentRoute,
    note: buildDocumentRoute,
    tutor: buildTutorRoute,
    job: {
        name: routes.jobRoute
    },
    book: {
        name: routes.bookRoute
    },
    purchase: {
        name: routes.foodRoute
    },
    food: {
        name: routes.foodRoute
    },
    ask: buildQuestionRoute,
    searchOrQuestion:{name:"searchOrQuestion"},
    AddSubjectOrCourse:{name:"AddSubjectOrCourse"}

};
const getters = {
    currenFlow: state => state.node.model ? state.node.model.name : "home",
    flowNode: state => state.node.model
}
const mutations = {
    [FLOW.ADD](state, { vertical,data}) {
        var flow = optionalRoutes[vertical];
        if (flow) {
            state.node = {
                model: flow
            };
        }
    },
    [FLOW.UPDATE_FLOW](state,model){
        state.node={model};
    }
};
const actions = {
    updateFlow(context, flowIndex) {
        return new Promise((resolve, reject) => {resolve(context.commit(FLOW.UPDATE_FLOW,context.state.node.model.children[flowIndex]))});
    }
}

export default {
    state,
    getters,
    actions,
    mutations
};