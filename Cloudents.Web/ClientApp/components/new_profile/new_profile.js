import { mapActions, mapGetters } from 'vuex';

import analyticsService from '../../services/analytics.service';
import { LanguageService } from "../../services/language/languageService";
import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue'
import storeService from '../../services/store/storeService';
import couponStore from '../../store/couponStore';
import chatService from '../../services/chatService.js';

import profileUserBox from './components/profileUserBox/profileUserBox.vue';
import profileDialogs from './components/profileDialogs/profileDialogs.vue';
import profileUserSticky from './components/profileUserSticky/profileUserSticky.vue';
import profileUserStickyMobile from './components/profileUserSticky/profileUserStickyMobile.vue';
import profileReviewsBox from './components/profileReviewsBox/profileReviewsBox.vue';
import profileEarnMoney from './components/profileEarnMoney/profileEarnMoney.vue';
import profileBecomeTutor from './components/profileBecomeTutor/profileBecomeTutor.vue';
import profileFindTutor from './components/profileFindTutor/profileFindTutor.vue';
import profileItemsBox from './components/profileItemsBox/profileItemsBox.vue';
import profileItemsEmpty from './components/profileItemsEmpty/profileItemsEmpty.vue';
import calendarTab from '../calendar/calendarTab.vue';





// import questionCard from "../question/helpers/new-question-card/new-question-card.vue";
// import resultNote from "../results/ResultNote.vue";
// import userBlock from '../helpers/user-block/user-block.vue';
import shareContent from '../pages/global/shareContent/shareContent.vue';
export default {
    name: "new_profile",
    components: {
        profileUserBox,
        profileDialogs,
        profileUserSticky,
        profileUserStickyMobile,
        profileReviewsBox,
        profileEarnMoney,
        profileBecomeTutor,
        profileFindTutor,
        profileItemsBox,
        profileItemsEmpty,
        calendarTab,
        sbDialog,
        shareContent,
    },
    props: {
        id: {
            // Number
        }
    },
    data() {
        return {
            globalFunctions:{
                openCoupon: this.openCoupon,
                sendMessage: this.sendMessage,
                openCalendar: this.openCalendar,
                closeCalendar: this.closeCalendar,
                openBecomeTutor: this.openBecomeTutor,
                goTutorList: this.goTutorList,
                getItems: this.getItems,
                scrollTo: this.scrollToElementId,
            },
            coupon: '',
            couponPlaceholder: LanguageService.getValueByKey('coupon_placeholder'),
            disableApplyBtn: false,

















            activeTab: 1,
        };
    },
    methods: {
        ...mapActions([
            'updateCouponDialog',
            'updateLoginDialogState',
            'updateCoupon',
            'updateCurrTutor',
            'setTutorRequestAnalyticsOpenedFrom',
            'updateRequestDialog',
            'setActiveConversationObj',
            'openChatInterface',
            'updateProfileItemsByType',


            'syncProfile',
            'resetProfileData',
            'updateToasterParams'
        ]),
        closeCouponDialog() {
            this.coupon = ''
            this.updateCouponDialog(false);
        },
        openCoupon(){
            if(global.isAuth) {
            if(this.accountUser) {          
                if(this.$route.params.id != this.accountUser.id) {
                    this.updateCouponDialog(true)
                    analyticsService.sb_unitedEvent('Tutor_Engagement', 'Click_Redeem_Coupon', `${this.$route.path}`);
                }
            }
            } else {
            this.updateLoginDialogState(true);
            }
        },
        applyCoupon() {
            if(this.isTutor) {
                this.disableApplyBtn = true;
                let tutorId = this.getProfile.user.id;
                let coupon = this.coupon;
                let self = this
                this.updateCoupon({coupon, tutorId}).finally(() => {
                self.coupon = ''
                self.disableApplyBtn = false;
                if(!self.getCouponError) {
                    analyticsService.sb_unitedEvent('Tutor_Engagement', 'Redeem_Coupon_Success', `${this.$route.path}`);
                }
                })
            }
        },
        sendMessage(){
            if(this.isMyProfile) {return}
            if(this.accountUser == null) {
               analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:GUEST`);
               let profile = this.getProfile
               this.updateCurrTutor(profile.user)    
               this.setTutorRequestAnalyticsOpenedFrom({
                  component: 'profileContactBtn',
                  path: this.$route.path
               });
               this.updateRequestDialog(true);
            } else {
               analyticsService.sb_unitedEvent('Request Tutor Submit', 'Send_Chat_Message', `${this.$route.path}`);
               let currentProfile = this.getProfile;
               let conversationObj = {
                  userId: currentProfile.user.id,
                  image: currentProfile.user.image,
                  name: currentProfile.user.name,
                  conversationId: chatService.createConversationId([currentProfile.user.id, this.accountUser.id]),
               }
               let currentConversationObj = chatService.createActiveConversationObj(conversationObj)
               this.setActiveConversationObj(currentConversationObj);
               this.openChatInterface();                    
            }
        },
        openBecomeTutor(){
            this.$router.push({query:{dialog:'becomeTutor'}})
        },
        goTutorList(){
            this.$router.push({name:'tutorLandingPage'})
        },
        getItems(type,params){
            let dataObj = {
                id: this.id,
                type,
                params
            }
            return this.updateProfileItemsByType(dataObj)
        },
        scrollToElementId(elementId){
            document.getElementById(elementId).scrollIntoView({behavior: 'smooth',block: 'start'});
        },
        fetchData() {
            let syncObj = {
                id: this.id,
                type:'documents',
                params:{
                    page: 0,
                    pageSize:this.$vuetify.breakpoint.xsOnly? 3 : 6,
                }
            }
            this.syncProfile(syncObj);
        },
        openCalendar() {
            if(!!this.accountUser) {
                this.activeTab = 5;
            } else {
                this.updateLoginDialogState(true);
                setTimeout(()=>{
                    document.getElementById(`tab-${this.activeTab}`).lastChild.click();
                },200);
            }
        },
        closeCalendar(){
            this.activeTab = null
        }
    },
    computed: {
        ...mapGetters([
            "accountUser",
            'getCouponDialog',
            'getCouponError',
            "getProfile",
        'getBannerParams']),
        isShowCouponDialog(){
            if(this.getCouponDialog){
                setTimeout(() => {
                    document.querySelector('.profile-coupon_input').focus()
                }, 100);
            }
            return this.getCouponDialog;
        },
        showReviewBox(){
            if((!!this.getProfile && this.getProfile.user.isTutor) && (this.getProfile.user.tutorData.rate)){
                return true;
            }else{
                return false
            }
        },
        isTutor(){
            return !!this.getProfile && this.getProfile.user.isTutor
        },
        isMyProfile(){
            return !!this.getProfile && !!this.accountUser && this.accountUser.id == this.getProfile.user.id
        },
        showEarnMoney(){
            return this.isMyProfile && !!this.uploadedDocuments && !!this.uploadedDocuments.result && !this.uploadedDocuments.result.length;
        },
        showItemsEmpty(){
            return !this.isMyProfile && !!this.uploadedDocuments && !!this.uploadedDocuments.result && !this.uploadedDocuments.result.length;
        },
        showItems(){
            return !!this.getProfile;
        },
        isTutorPending(){
            return this.isMyProfile && (!!this.accountUser && this.accountUser.isTutorState === "pending")
        },
        showProfileCalendar(){
            if(this.isMyProfile){
                return (this.isTutor)
            }else{
                if(this.isTutor){
                    return (this.activeTab === 5)
                }else{
                    return false;
                }
            }
        },
        showBecomeTutor(){
            return this.isMyProfile && !this.isTutor && !this.isTutorPending;
        },
        showFindTutor(){
            return (!this.isMyProfile && !this.isTutor)
        },
        profileData() {
            if (!!this.getProfile) {
                return this.getProfile;
            }
        },
        uploadedDocuments() {
            if(this.profileData && this.profileData.documents) {
                return this.profileData.documents;
            }
            return [];
        },
    },
    watch: {
        "$route.params.id": function(val, oldVal){ 
            let old = Number(oldVal,10);
            let newVal = Number(val,10);
            if (newVal !== old) {
                this.resetProfileData();
                if((newVal == this.accountUser.id) && this.accountUser.isTutorState === "pending"){
                    this.updateToasterParams({
                        toasterText: LanguageService.getValueByKey("becomeTutor_already_submitted"),
                        showToaster: true,
                        toasterTimeout: 3600000
                    });
                }else{
                    this.updateToasterParams({
                        showToaster: false
                    }); 
                }
                this.fetchData();
            }
        },
    },
    beforeRouteLeave(to, from, next) {
        this.updateToasterParams({
            showToaster: false
        });
        this.resetProfileData();
        next();
    },
    beforeDestroy(){
        this.closeCouponDialog();
        storeService.unregisterModule(this.$store, 'couponStore');
     },
    created() {
        this.fetchData();
        storeService.registerModule(this.$store, 'couponStore', couponStore);
        if(!!this.$route.query.coupon) {
            setTimeout(() => {
                this.openCoupon();
            },200)
        }
        if(this.$route.params.openCalendar) {
            this.openCalendar();
        }
    },
    mounted() {
        setTimeout(()=>{
            if((this.$route.params && this.$route.params.id) && 
               (this.$route.params.id == this.accountUser.id) && 
               this.accountUser.isTutorState === "pending"){
                this.updateToasterParams({
                    toasterText: LanguageService.getValueByKey("becomeTutor_already_submitted"),
                    showToaster: true,
                    toasterTimeout: 3600000
                });
            }
        },200);
    }
}