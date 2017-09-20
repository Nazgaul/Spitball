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
        var flow;
        if (payload.result === "search") {


            payload.data.searchType = payload.data.searchType || {};
            if (payload.data.searchType.key === "Flashcards") {
                flow = buildFlashcardRoute();
                //state.node = state.flowTree.parse(buildFlashcardRoute());
                //return;
            } else {
                flow = buildDocumentRoute();
            }

        }
        if (payload.result === "tutor") {
            flow = buildTutorRoute();
            //state.node = state.flowTree.parse(buildTutorRoute());
        }
        if (payload.result === "book") {
            flow = buildBookRoute();
           // state.node = state.flowTree.parse(buildBookRoute());
        }
        if (payload.result === "job") {
            flow = buildJobRoute();
           // state.node = state.flowTree.parse(buildJobRoute());
        }
        if (payload.result === "purchase") {
            flow = buildPurchaseRoute();
           // state.node = state.flowTree.parse(buildPurchaseRoute());
        }
        if (payload.result === "chatPost") {
            flow = buildChatPostRoute();
           // state.node = state.flowTree.parse(buildChatPostRoute());
        }
        if (payload.result === "question") {
            flow = buildAskRoute();
          //  state.node = state.flowTree.parse(buildAskRoute());
        }
        if (flow) {
            state.node = state.flowTree.parse(flow);
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