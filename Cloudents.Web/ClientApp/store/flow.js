import Tree from 'tree-model';
import * as types from './mutation-types';
import * as routes from './../routes';

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
        name: routes.purchaseRoute
    },
    ask: {
        name: routes.buildAsk,
        children: [
            buildChatPostRoute,
            buildDocumentRoute
        ]
    }
};

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
        console.log(flow);
        if (flow) {
            state.node = state.flowTree.parse(flow);
        }
    }
};

export default {
    state,
    mutations
};