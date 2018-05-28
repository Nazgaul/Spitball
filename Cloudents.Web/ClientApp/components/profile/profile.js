import questionCard from "../question/helpers/question-card/question-card.vue";
import userBlock from '../helpers/user-block/user-block.vue';

import accountService from '../../services/accountService';
import {mapGetters} from 'vuex'

export default {
    components: {questionCard, userBlock},
    props:{
        id:{Number}
    },
    data() {
        return {
            activeTab: 1,
            profileData: {}
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
        accountService.getProfile(this.id).then(function (response) {
            self.profileData = response.data;
        })
    }
}