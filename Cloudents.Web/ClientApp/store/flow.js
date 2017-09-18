import Tree from 'tree-model';
import * as types from './mutation-types'

const state = {
    flowTree: new Tree(),
    node: null,
    userText: null

};

const mutations = {
    [types.ADD](state, payload) {
        if (payload.result === "search") {
            payload.data.searchType = payload.data.searchType || {};
            if (payload.data.searchType.key === "Flashcards") {
                state.node = state.flowTree.parse(
                    {
                        name: "flashcard",
                        children: [
                            {
                                name: "post", children: [
                                    {
                                        name: "tutor"
                                    }
                                ]
                            }
                        ]
                    });
                return;
            }

            state.node = state.flowTree.parse({
                name: "document",
                children: [
                    {
                        name: "flashcard",
                        children: [
                            {
                                name: "post",
                                children: [
                                    {
                                        name: "tutor"
                                    }
                                ]
                            }
                        ]
                    }
                ]
            });

        }
    }
};

export default {
    state,
    mutations
}