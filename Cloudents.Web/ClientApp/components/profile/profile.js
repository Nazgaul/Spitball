import questionCard from "../question/helpers/question-card/question-card.vue";

// ﻿import accountService from '../../services/accountService';
import {mapGetters} from 'vuex'

export default {
    components: {questionCard},
    data() {
        return {
            activeTab: 1,
            userData: {}
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
        // this.userData = this.accountUser;
        debugger;
    }
}