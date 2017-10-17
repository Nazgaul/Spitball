import Tree from 'tree-model';
import * as types from './mutation-types';
import * as routes from './../routeTypes';

const state = {
    flowTree: new Tree(),
    node: null,
    userText: null

};
var buildChatPostRoute = {
    name: routes.postChatRoute,
    children: [
        {
            name: routes.tutorRoute
        }
    ]
};
var buildFlashcardRoute = {
    name: routes.flashcardRoute,
    children: [
        buildChatPostRoute
    ]
};
var buildDocumentRoute = {
    name: routes.notesRoute,
    children: [
        buildFlashcardRoute
    ]
};
var optionalRoutes = {
    post: buildChatPostRoute,
    flashcard: buildFlashcardRoute,
    document: buildDocumentRoute,
    note: buildDocumentRoute,
    tutor: {
        name: routes.tutorRoute,
        children: [
            {
                name: routes.postChatRoute
            }
        ]
    },
    job: {
        name: routes.jobRoute,
        children: [
            {
                name: routes.postChatRoute
            }
        ]
    },
    book: {
        name: routes.bookRoute,
        children: [
            {
                name: routes.postChatRoute
            }
        ]
    },
    purchase: {
        name: routes.foodRoute
    },
    food: {
        name: routes.foodRoute
    },
    ask: {
        name: routes.questionRoute,
        children: [
            buildChatPostRoute,
            buildDocumentRoute
        ]
    }
};
const getters = {
    currenFlow: state => state.node ? state.node.model.name:"home",
}
const mutations = {
    [types.ADD](state, payload) {
        var flow;
        if (payload.result === "search") {        
            payload.data.searchType = payload.data.searchType || {};
            if (payload.data.searchType.key === "Flashcards") {
                flow = optionalRoutes.flashcard;
            } else {
                flow = optionalRoutes.document;
            }

        } else {
            flow = optionalRoutes[ payload.result];
        }
        if (flow) {
            state.node = state.flowTree.parse(flow);
        }
    }
};


export default {
    state,
    getters,
    mutations
};