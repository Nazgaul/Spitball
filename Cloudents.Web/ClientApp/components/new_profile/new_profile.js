
import profileUserBox from './components/profileUserBox/profileUserBox.vue';
import profileDialogs from './components/profileDialogs/profileDialogs.vue';
import profileUserSticky from './components/profileUserSticky/profileUserSticky.vue';
import profileUserStickyMobile from './components/profileUserSticky/profileUserStickyMobile.vue';
import profileReviewsBox from './components/profileReviewsBox/profileReviewsBox.vue';
import profileEarnMoney from './components/profileEarnMoney/profileEarnMoney.vue';
import profileBecomeTutor from './components/profileBecomeTutor/profileBecomeTutor.vue';
import profileFindTutor from './components/profileFindTutor/profileFindTutor.vue';
import profileItemsBox from './components/profileItemsBox/profileItemsBox.vue';



import analyticsService from '../../services/analytics.service';
import { LanguageService } from "../../services/language/languageService";
import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue'
import storeService from '../../services/store/storeService';
import couponStore from '../../store/couponStore';
import chatService from '../../services/chatService.js';


//old
// import questionCard from "../question/helpers/new-question-card/new-question-card.vue";
// import resultNote from "../results/ResultNote.vue";
// import userBlock from '../helpers/user-block/user-block.vue';
import { mapActions, mapGetters } from 'vuex';
// import uploadDocumentBtn from "../results/helpers/uploadFilesBtn/uploadFilesBtn.vue";
//old
//new



import calendarTab from '../calendar/calendarTab.vue';

//new
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
        sbDialog,




        // questionCard,
        // userBlock,
        // resultNote,
        // uploadDocumentBtn,
        calendarTab
    },
    props: {
        id: {
            Number
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
                openUpload: this.openUpload,
                getItems: this.getItems,
                scrollTo: this.scrollToElementId,
            },
            coupon: '',
            couponPlaceholder: LanguageService.getValueByKey('coupon_placeholder'),
            disableApplyBtn: false,
















            isRtl: global.isRtl,
            isEdgeRtl: global.isEdgeRtl,
            loadingContent: false,
            activeTab: 1,
            itemsPerTab: 50,
            answers: {
                isLoading: false,
                isComplete: false,
                page: 1
            },
            questions: {
                isLoading: false,
                isComplete: false,
                page: 1
            },
            documents: {
                isLoading: false,
                isComplete: false,
                page: 1
            },
            purchasedDocuments: {
                isLoading: false,
                isComplete: false,
                page: 1
            },
            calendar: {
                isLoading: false,
                isComplete: false,
                page: 1
            }
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
            'updateTutorDialog',
            'setReturnToUpload',
            'updateDialogState',
            'updateProfileItemsByType',


            
            'updateNewQuestionDialogState',
            'syncProfile',
            'getAnswers',
            'getQuestions',
            // 'getDocuments',
            'resetProfileData',
            'getPurchasedDocuments',
            'setProfileByActiveTab',
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
        this.updateTutorDialog(true)
        },
        goTutorList(){
        this.$router.push({name:'tutorLandingPage'})
        },
        openUpload() {
            let schoolName = this.getSchoolName;
            if (this.accountUser == null) {
              this.updateLoginDialogState(true);
            } else if (!schoolName.length) {
              this.$router.push({ name: "addUniversity" });
              this.setReturnToUpload(true);
            } else if (!this.getSelectedClasses.length) {
              this.$router.push({ name: "addCourse" });
              this.setReturnToUpload(true);
            } else if (schoolName.length > 0 && this.getSelectedClasses.length > 0) {
              this.updateDialogState(true);
              this.setReturnToUpload(false);
            }
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

        






















        changeActiveTab(tabId) {
            this.activeTab = tabId;
        },
        fetchData() {
            // let syncObj = {
            //     id: this.id,
            //     activeTab: this.activeTab
            // };
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
        // getInfoByTab() {
        //     this.loadingContent = true;
        //     this.setProfileByActiveTab(this.activeTab).then(() => {
        //         this.loadingContent = false;
        //     });
        // },
        loadAnswers() {
            if (this.profileData.answers.length < this.itemsPerTab) {
                this.answers.isComplete = true;
                return;
            }
            this.answers.isLoading = true;
            let answersInfo = {
                id: this.id,
                page: this.answers.page
            };
            this.getAnswers(answersInfo).then((hasData) => {
                if (!hasData) {
                    this.answers.isComplete = true;
                }
                this.answers.isLoading = false;
                this.answers.page++;
            }, () => {
                this.answers.isComplete = true;
            });
        },
        loadQuestions() {
            if (this.profileData.questions.length < this.itemsPerTab) {
                this.questions.isComplete = true;
                return;
            }
            this.questions.isLoading = true;
            let questionsInfo = {
                id: this.id,
                page: this.questions.page,
                user: this.profileData.user
            };
            this.getQuestions(questionsInfo).then((hasData) => {
                if (!hasData) {
                    this.questions.isComplete = true;
                }
                this.questions.isLoading = false;
                this.questions.page++;
            }, () => {
                this.questions.isComplete = true;
            });
        },
        // loadDocuments() {
        //     if (this.profileData.documents.length < this.itemsPerTab) {
        //         this.documents.isComplete = true;
        //         return;
        //     }
        //     this.documents.isLoading = true;
        //     let documentsInfo = {
        //         id: this.id,
        //         page: this.documents.page,
        //         user: this.profileData.user
        //     };
        //     this.getDocuments(documentsInfo).then((hasData) => {
        //         if (!hasData) {
        //             this.documents.isComplete = true;
        //         }
        //         this.documents.isLoading = false;
        //         this.documents.page++;
        //     }, () => {
        //         this.documents.isComplete = true;
        //     });
        // },
        loadPurchasedDocuments() {
            if (this.profileData.purchasedDocuments.length < this.itemsPerTab) {
                this.purchasedDocuments.isComplete = true;
                return;
            }
            this.purchasedDocuments.isLoading = true;
            let documentsInfo = {
                id: this.id,
                page: this.purchasedDocuments.page,
                user: this.profileData.user
            };
            this.getPurchasedDocuments(documentsInfo).then((hasData) => {
                    if (!hasData) {
                        this.purchasedDocuments.isComplete = true;
                    }
                    this.purchasedDocuments.isLoading = false;
                    this.purchasedDocuments.page++;
                },
                () => {
                    this.purchasedDocuments.isComplete = true;
                });
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
            'getSchoolName',
            'getSelectedClasses',

        
        
        
        
        
        
        
        
        
        
        "getProfile", "isTutorProfile"]),
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
        showItems(){
            return !!this.getProfile && !!this.uploadedDocuments && !!this.uploadedDocuments.result && this.uploadedDocuments.result.length;
        },
        isTutorPending(){
            return this.isMyProfile && (!!this.accountUser && this.accountUser.isTutorState === "pending")
        },
        showProfileCalendar(){
            // debugger
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














        xsColumn(){
            const xsColumn = {};
            if (this.$vuetify.breakpoint.xsOnly){
                xsColumn.column = true;
            }
            return xsColumn;
        },
        profileData() {
            if (!!this.getProfile) {
                return this.getProfile;
            }
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },

        emptyStateData() {
            let questions = {
                text: LanguageService.getValueByKey("profile_emptyState_questions_text"),
                boldText: LanguageService.getValueByKey("profile_emptyState_questions_btnText"),
                btnText: LanguageService.getValueByKey("profile_emptyState_questions_btnText"),
                btnUrl: () => {
                    if(this.accountUser == null) {
                        this.updateLoginDialogState(true);
                        return;
                    }
                    let obj = {
                        status: true,
                        from: 5
                    };
                    this.updateNewQuestionDialogState(obj);
                }
            };
            let answers = {
                text: LanguageService.getValueByKey("profile_emptyState_answers_text"),
                btnText: LanguageService.getValueByKey("profile_emptyState_answers_btnText"),
                btnUrl: 'ask'
            };
            let documents = {
                text: LanguageService.getValueByKey("profile_emptyState_documents_text"),
                //TODO feel free to remove this after redesign, will not be used, using reusable component instead
                btnText: LanguageService.getValueByKey("profile_emptyState_documents_btnText"),
                btnUrl: 'note'
            };
            if (this.activeTab === 1) {
                return documents;
            } else if (this.activeTab === 2) {
                return answers;
            } else if (this.activeTab === 3) {
                return questions;
            }
        },
        showCalendar(){
            if(!this.getProfile) return;
            let isTutorSharedCalendar = this.getProfile.user.calendarShared;
            if(this.isTutorProfile && (this.isMyProfile || isTutorSharedCalendar)){
                return true;
            }
        },
        questionDocuments() {
            if(this.profileData && this.profileData.questions) {
                return this.profileData.questions;
            }
            return [];
        },
        answerDocuments() {
            if(this.profileData && this.profileData.answers) {
                return this.profileData.answers;
            }
            return [];
        },
        uploadedDocuments() {
            if(this.profileData && this.profileData.documents) {
                return this.profileData.documents;
            }
            return [];
        },
        purchasedsDocuments() {
            if(this.profileData && this.profileData.purchasedDocuments) {
                return this.profileData.purchasedDocuments;
            }
            return [];
        }
    },
    watch: {
        '$route': function(val){







            this.resetProfileData();
            if((val.params.id == this.accountUser.id) && this.accountUser.isTutorState === "pending"){
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
        },
        // activeTab() {









        //     this.getInfoByTab();
        // }
    },
    //reset profile data to prevent glitch in profile loading
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
    },
    mounted() {











        if(this.$route.params && this.$route.params.tab){
            let tabNumber = this.$route.params.tab;
            setTimeout(()=>{
                document.getElementById(`tab-${tabNumber}`).lastChild.click();
            },200);
        }
        if((this.$route.query && this.$route.query.calendar)){
            setTimeout(()=>{
                if(this.getProfile.user.calendarShared){
                    document.getElementById(`tab-5`).lastChild.click();
                }
            },200);
        }
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

