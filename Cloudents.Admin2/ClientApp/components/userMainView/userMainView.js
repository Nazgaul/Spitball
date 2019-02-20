import { mapGetters, mapActions } from 'vuex';
import { releaseUser } from '../user/suspend/suspendUserService';
import questionItem from './helpers/questionIitem.vue';
import answerItem from './helpers/answerItem.vue';
import documentItem from './helpers/documentItem.vue';
import purchasedDocItem from './helpers/purchasedDocItem.vue';
import userTokens from './helpers/sendTokens.vue';
import userSuspend from '../user/suspend/suspendUser.vue';
import userCashout from '../user/cashout/cashoutUser.vue';


export default {
    name: "userMainView",
    components: {
        questionItem,
        answerItem,
        documentItem,
        purchasedDocItem,
        userTokens,
        userCashout,
        userSuspend
    },
    data() {
        return {
            userIdentifier: '',
            userId: null,
            suspendedUser: false,
            filters: [
                {name: 'Accepted', value: 'ok'},
                {name: 'Pending', value: 'pending'},
                {name: 'Deleted', value: 'deleted'},
                {name: 'Flagged', value: 'flagged'}
            ],
            scrollFunc: {
                questions: {
                    page: 0,
                    getData: this.getUserQuestionsData,
                    scrollLock: false,
                    wasCalled: false
                },
                answers: {
                    page: 0,
                    getData: this.getUserAnswersData,
                    scrollLock: false,
                    wasCalled: false
                },
                documents: {
                    page: 0,
                    getData: this.getUserDocumentsData,
                    scrollLock: false,
                    wasCalled: false
                },
                purchasedDocs: {
                    page: 0,
                    getData: this.getUserPurchasedDocs,
                    scrollLock: false,
                    wasCalled: false
                }
            },
            initScrollObj:{
                questions: {
                    page: 0,
                    getData: this.getUserQuestionsData,
                    scrollLock: false,
                    wasCalled: false
                },
                answers: {
                    page: 0,
                    getData: this.getUserAnswersData,
                    scrollLock: false,
                    wasCalled: false
                },
                documents: {
                    page: 0,
                    getData: this.getUserDocumentsData,
                    scrollLock: false,
                    wasCalled: false
                },
                purchasedDocs: {
                    page: 0,
                    getData: this.getUserPurchasedDocs,
                    scrollLock: false,
                    wasCalled: false
                }
            },
            loading: false,
            userActions: [
                {
                    title: "Suspend",
                    action: this.showSuspendDialog,
                },
                {
                    title: "Suspend",
                    action: this.releaseUser,
                },
                {
                    title: "Pay Cashout",
                    action: this.payCashOut,

                },
                {
                    title: "Grant Tokens",
                    action: this.sendTokens,
                },
            ],
            suspendDialog: false,
            activeTab: 'tab-0',
            searchQuery: 'ok',
            userComponentsShow: false,
            activeUserComponent: '',
            deleteUserQuestions: false,
            activeTabEnum: {
                'tab-0': 'questions',
                'tab-1': 'answers',
                'tab-2': 'documents',
                'tab-3': 'purchasedDocs',


            }
        }
    },
    computed: {
        ...mapGetters([
            "getTokensDialogState",
            "suspendDialogState",
            "getUserObj",
            "UserInfo",
            "UserQuestions",
            "UserAnswers",
            "UserDocuments",
            "UserPurchasedDocuments"
        ]),
        userInfo() {
            return this.UserInfo
        },
        showActions(){
            return Object.keys( this.UserInfo).length !== 0;
        },
        userStatusActive: {
            get() {
                if (this.userInfo && this.userInfo.status) {
                    return this.userInfo.status.value
                }
            },
            set(val) {
                this.suspendedUser = val
            }

        },
    },
    watch: {
        activeTab() {
            this.getDataByTabName();
        }
    },
    methods: {
        ...mapActions([
            "setTokensDialogState",
            "setSuspendDialogState",
            "getUserData",
            "setUserCurrentStatus",
            "getUserQuestions",
            "getUserAnswers",
            "getUserDocuments",
            "verifyUserPhone",
            "getUserPurchasedDocuments",
            "clearUserState"
        ]),
        resetUserData(){
            let strDefault;
            // reinit scrollfunc data and clear store ib new user data requested
            strDefault =JSON.stringify(this.initScrollObj);
            this.scrollFunc = JSON.parse(strDefault);
            this.clearUserState();

        },
        showSuspendDialog(){
            this.setSuspendDialogState(true);
        },
        closeSuspendDialog(){
            this.setSuspendDialogState(false);
        },
        userInfoAction(actionItem){
            if(actionItem === "phoneNumber"){
                let userObj = {
                    id: this.userInfo.id.value
                };
                this.verifyUserPhone(userObj).then((resp)=>{
                    console.log(resp)
                    this.openTokensDialog();
                })
            }
        },
        nextPage() {
            this.scrollFunc[this.activeTabEnum[this.activeTab]].page++
        },
        handleScroll(event) {
            let offset = 2000;
            if (event.target.scrollHeight - offset < event.target.scrollTop) {
                if (!this.scrollFunc[this.activeTabEnum[this.activeTab]].scrollLock) {
                    this.scrollFunc[this.activeTabEnum[this.activeTab]].scrollLock = true;
                    this.scrollFunc[this.activeTabEnum[this.activeTab]].getData(this.userId, this.scrollFunc[this.activeTabEnum[this.activeTab]].page )
                }
            }
        },
        openTokensDialog() {
            this.setTokensDialogState(true);
        },
        closeTokensDialog() {
            this.setTokensDialogState(false);
        },
        setActiveTab(activeTabName) {
            return this.activeTab = activeTabName;
        },
        updateFilter(val) {
            return this.searchQuery = val
        },
        getDataByTabName() {
            if (!this.userId) return;
            if(this.scrollFunc[this.activeTabEnum[this.activeTab]].wasCalled)return;
            if (this.activeTab === "tab-0" ) {
                let page = this.scrollFunc.questions.page;
                this.getUserQuestionsData(this.userId, page)
            } else if (this.activeTab === "tab-1") {
                let page = this.scrollFunc.answers.page;
                this.getUserAnswersData(this.userId, page)
            } else if (this.activeTab === "tab-2") {
                let page = this.scrollFunc.documents.page;
                this.getUserDocumentsData(this.userId, page)
            } else if (this.activeTab === "tab-3") {
                let page = this.scrollFunc.purchasedDocs.page;
                this.getUserPurchasedDocs(this.userId, page)
            }
        },
        getUserInfoData() {
            if(!!this.UserInfo){
                this.resetUserData();
            }
            let id = this.userIdentifier;
            let self = this;
            self.getUserData(id)
                .then((data) => {
                    self.userId =  data.id.value;
                    self.getDataByTabName()

                })
        },
        getUserQuestionsData(id, page) {
            let self = this;
            self.scrollFunc[self.activeTabEnum[self.activeTab]].wasCalled = true;
            self.loading = true;
            self.getUserQuestions({id, page}).then((isComplete) => {
                self.nextPage();
                if(!isComplete){
                    self.scrollFunc[self.activeTabEnum[self.activeTab]].scrollLock = false;
                }else{
                    self.scrollFunc[self.activeTabEnum[self.activeTab]].scrollLock  = true;
                }
                self.loading = false;

            });
        },
        getUserPurchasedDocs(id, page){
            let self = this;
            self.scrollFunc[self.activeTabEnum[self.activeTab]].wasCalled = true;
            self.loading = true;
            self.getUserPurchasedDocuments({id, page}).then((isComplete) => {
                self.nextPage();
                if(!isComplete){
                    self.scrollFunc[self.activeTabEnum[self.activeTab]].scrollLock = false;
                }else{
                    self.scrollFunc[self.activeTabEnum[self.activeTab]].scrollLock  = true;
                }
                self.loading = false;

            });
        },
        getUserAnswersData(id, page) {
            let self = this;
            self.scrollFunc[self.activeTabEnum[self.activeTab]].wasCalled = true;
            self.loading = true;
            self.getUserAnswers({id, page}).then((isComplete) => {
                self.nextPage();
                if(!isComplete){
                    self.scrollFunc[self.activeTabEnum[self.activeTab]].scrollLock = false;
                }else{
                    self.scrollFunc[self.activeTabEnum[self.activeTab]].scrollLock  = true;
                }
                self.loading = false;

            });
        },
        getUserDocumentsData(id, page) {
            let self = this;
            self.scrollFunc[self.activeTabEnum[self.activeTab]].wasCalled = true;
            self.loading = true;
            self.getUserDocuments({id, page}).then((isComplete) => {
                self.nextPage();
                if(!isComplete){
                    self.scrollFunc[self.activeTabEnum[self.activeTab]].scrollLock = false;
                }else{
                    self.scrollFunc[self.activeTabEnum[self.activeTab]].scrollLock  = true;
                }
                self.loading = false;
            });
        },
        //keep here cause there is an option to release from within this component
        releaseUser() {
            let self = this;
            let idArr = [];
            idArr.push(this.userId);
            releaseUser(idArr).then((email) => {
                self.$toaster.success(`user got released`);
                this.suspendedUser = false;
                this.setUserCurrentStatus(false);
            }, (err) => {
                self.$toaster.error(`ERROR: failed to realse user`);
                console.log(err)
            }).finally(() => {
                self.lock = false;
                self.userIds = null;

            })
        },
        attachToScroll(){
            let containerElm = document.querySelector('.item-wrap');
            containerElm.addEventListener('scroll', this.handleScroll)
        }
    },
    created() {
        console.log('hello created');
        this.$nextTick(function () {
            this.attachToScroll();
        })
    },
    beforeDestroy() {
        let containerElm = document.querySelector('.item-wrap');
        if (!containerElm) return;
        containerElm.removeEventListener('scroll', this.handleScroll);
    }
}
