import { skeletonData } from '../components/results/consts';
import searchService from "./../services/searchService";

const state = {
    items: {},
    itemsSkeleton: skeletonData.tutor,
    dataLoaded: false,
};

const mutations = {
    Tutors_setItems(state, data) {
        state.items = data;
    },
    Tutors_setDataLoaded(state, data) {
        state.dataLoaded = data;
    },
    Tutors_updateItems(state, data) {
        state.items.data = state.items.data.concat(data.data);
        state.items.nextPage = data.nextPage;
    }
};

const getters = {
    Tutors_getItems: function (state, { getIsLoading, getSearchLoading }) {
        if (getIsLoading || getSearchLoading) {
            //return skeleton
            return state.itemsSkeleton;
        } else {
            //return data
            return state.items.data;
        }
    },
    Tutors_getNextPageUrl: function (state) {
        return state.items.nextPage;
    },
    Tutors_isDataLoaded: function (state) {
        return state.dataLoaded;
    }
};

const actions = {
    Tutors_nextPage(context, { url, vertical }) {
        return searchService.nextPage({ url, vertical }).then((data) => {
            context.dispatch('Tutors_updateData', data);            
            return data;
        });
    },
    Tutors_updateDataLoaded({ commit }, data) {
        commit('Tutors_setDataLoaded', data);
    },
    Tutors_fetchingData(context, { name, params, page }) {
        let paramsList = { ...context.state.search, ...params, page };
        //update box terms
        // context.dispatch('updateAITerm', { vertical: name, data: { text: paramsList.term } });
        context.dispatch('Tutors_updateDataLoaded', false);
        //get location if needed
        let verticalItems = context.state.items;
        //when entering a question and going back stay on the same position.
        //can be removed only when question page willo be part of ask question page
        if ((!!verticalItems && !!verticalItems.data && (verticalItems.data.length > 0 && verticalItems.data.length < 150) && !context.getters.getSearchLoading)) {
            let filtersData = !!verticalItems.filters ? searchService.createFilters(verticalItems.filters) : null;
            let sortData = !!verticalItems.sort ? verticalItems.sort : null;
            context.dispatch('updateSort', sortData);
            context.dispatch('updateFilters', filtersData);
            context.dispatch('Tutors_updateDataLoaded', true);
            return verticalItems;
        } else {
            return getData();
        }

        function getData() {
            return searchService.activateFunction[name](paramsList).then((data) => {
                context.dispatch('Tutors_setDataItems', data);
                let sortData = !!data.sort ? data.sort : null;
                context.dispatch('updateSort', sortData);
                let filtersData = !!data.filters ? searchService.createFilters(data.filters) : null;
                context.dispatch('updateFilters', filtersData);
                context.dispatch('Tutors_updateDataLoaded', true);
                return data;
            });
        }
    },
    Tutors_setDataItems({ commit }, data) {
        commit('Tutors_setItems', data);
    },
    Tutors_updateData({ commit }, data) {       
        commit('Tutors_updateItems', data);
    },


};

export default {
    state,
    getters,
    actions,
    mutations
}
