import {FLOW} from './mutation-types';
import * as routes from './../routeTypes';

const state = {
    node: {}
};

var buildFlashcardRoute = {
    name: routes.flashcardRoute,
    children: [
        {name:routes.tutorRoute,children:[
            {name:routes.notesRoute,children:[
                {name:routes.questionRoute}
            ]}
        ]}
    ]
};
var buildDocumentRoute = {
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
var buildQuestionRoute = {
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
var buildTutorRoute = {
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
var optionalRoutes = {
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
    [FLOW.ADD](state, { result,data}) {
        var flow;
        if (result === "search") {
            data.searchType = data.searchType || {};
            if (data.searchType.key === "Flashcards") {
                flow = optionalRoutes.flashcard;
            } else {
                flow = optionalRoutes.document;
            }

        } else {
            flow = optionalRoutes[result];
        }
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