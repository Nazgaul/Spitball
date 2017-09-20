import Tree from 'tree-model';
import * as types from './mutation-types'
import * as routes from './../routes'

const state = {
    flowTree: new Tree(),
    node: null,
    userText: null

};
function buildFlashcardRoute() {
    return {
        name: routes.flashcardRoute,
        children: [
            buildChatPostRoute()
        ]
    }
}
function buildDocumentRoute() {

    return {
        name: routes.notesRoute,
        children: [
            buildFlashcardRoute()
        ]
    };
}
function buildTutorRoute() {
    return {
        name: routes.tutorRoute,
        children: [
            {
                name: routes.postChatRoute
            }

        ]
    }
}
function buildJobRoute() {
    return {
        name: routes.jobRoute,
        children: [
            {
                name: routes.postChatRoute
            }

        ]
    }
}
function buildBookRoute() {
    return {
        name: routes.bookRoute,
        children: [
            {
                name: routes.postChatRoute
            }

        ]
    }
}
function buildPurchaseRoute() {
    return {
        name: routes.purchaseRoute
    }
}
function buildChatPostRoute() {
    return {
        name: routes.postChatRoute,
        children: [
            {
                name: routes.tutorRoute
            }
        ]
    }
}
function buildAskRoute() {
    return {
        name: routes.buildAsk,
        children: [
            buildChatPostRoute(),
            buildDocumentRoute()
        ]
    }
}

const mutations = {
    [types.ADD](state, payload) {

        if (payload.result === "search") {
            payload.data.searchType = payload.data.searchType || {};
            if (payload.data.searchType.key === "Flashcards") {
                state.node = state.flowTree.parse(buildFlashcardRoute());
                return;
            }
            state.node = state.flowTree.parse(buildDocumentRoute());

        }
        if (payload.result === "tutor") {
            state.node = state.flowTree.parse(buildTutorRoute());
        }
        if (payload.result === "book") {
            state.node = state.flowTree.parse(buildBookRoute());
        }
        if (payload.result === "job") {
            state.node = state.flowTree.parse(buildJobRoute());
        }
        if (payload.result === "purchase") {
            state.node = state.flowTree.parse(buildPurchaseRoute());
        }
        if (payload.result === "chatPost") {
            state.node = state.flowTree.parse(buildChatPostRoute());
        }
        if (payload.result === "question") {
            state.node = state.flowTree.parse(buildAskRoute());
        }

        /* None,
       SearchOrQuestion,
       AddSubjectOrCourse,
       AddSubject,
       Qna,
       PurchaseAskBuy,
        */
    }
};

export default {
    state,
    mutations
}