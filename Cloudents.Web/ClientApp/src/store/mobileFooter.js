const getters = {
    getMobileFooterState: (state, val, {route}) => {
        return !!route.meta && route.meta.hasOwnProperty('showMobileFooter');
    }
};

export default {
    getters
}