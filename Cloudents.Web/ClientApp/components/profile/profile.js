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
        },
        answers(){
            return this.profileData.answer?this.profileData.answer.map(i=> {return {...i, user:this.profileData.user}}):[]
        },
        questions(){
            return this.profileData.ask?this.profileData.ask.map(i=> {return {...i, user:this.profileData.user}}):[]
        },
        isMyProfile() {
            return this.accountUser && this.accountUser.id && this.profileData ? this.profileData.user.id == this.accountUser.id : false;
        }
    },
    created() {
        var self = this;
        accountService.getProfile(this.id).then(function (response) {
            self.profileData = response.data;
        })
    }
}