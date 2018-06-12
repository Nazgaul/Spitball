import accountService from '../../services/accountService';
import {mapGetters} from 'vuex'

export default {
    props:{ },
    data() {
        return {
            activeTab: 1,            
            walletData: {}
        }
    },
    methods: {
        changeActiveTab(tabId) {
            this.activeTab = tabId;
        }        
    },
    computed: {
        ...mapGetters(["accountUser"]),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        }
    },
    created() {
        var self = this;
    }
}