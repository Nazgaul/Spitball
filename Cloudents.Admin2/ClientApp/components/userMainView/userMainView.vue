<template>
    <v-layout justify-center class="user-page-wrap" data-app>
        <v-flex xs12 sm12 md12 class="px-2 py-3" style="background: #ffffff;">
            <h1>Welcome to Admin</h1>
            <div class="input-wrap d-flex  justify-end">
                <v-flex xs3>
                    <v-text-field autocomplete solo v-model="userIdentifier"
                                  @keyup.enter.native="getUserInfoData()"
                                  autofocus
                                  clearable
                                  loading="primary"
                                  type="text" class="user-id-input"
                                  placeholder="Insert user identifier..."/>
                </v-flex>
                <v-flex xs1>
                    <v-btn :disabled="!userIdentifier" primary @click="getUserInfoData()">Get User</v-btn>
                </v-flex>
                <v-spacer></v-spacer>
                <v-flex xs4 v-if="userInfo">
                    <v-btn v-if="!userStatusActive && !suspendedUser" :disabled="!userInfo" color="rgb(0, 188, 212)"
                           class="suspend"
                           @click="suspendUser()">
                        Suspend
                    </v-btn>
                    <v-btn v-else :disabled="!userInfo" class="suspend" @click="releaseUser()">UnSuspend</v-btn>

                    <v-btn :disabled="!userInfo" class="grant" @click="openTokensDialog()">Grant Tokens
                    </v-btn>
                </v-flex>
            </div>
            <div class="questions-answers-wrap">
                <div class="filters mb-2">
                    <v-btn v-for="(filter, index) in filters" @click="updateFilter(filter.value)"
                           :color="searchQuery === filter.value ? '#00bcd4' : ''  "
                           :key="'filter_'+index">{{filter.name}}
                    </v-btn>
                </div>
                <v-layout row>
                    <div class="general-info d-flex elevation-2 mb-2" v-if="userInfo">
                        <div class="info-item py-2 px-2" v-for="(infoItem, index) in userInfo" :key="index">
                            <v-flex row class="d-flex align-baseline justify-center">
                                <div class="user-info-label">
                                    <span>{{infoItem.label}}</span>
                                </div>
                                <div class="user-info-value">
                                    <span>{{infoItem.value}}</span>
                                </div>
                                <div class="user-info-button" v-if="infoItem.showButton">
                                    <button @click="userInfoAction(index)">{{infoItem.buttonText}}</button>
                                </div>
                            </v-flex>
                        </div>
                    </div>
                    <div class="tabs-holder">
                        <v-tabs
                                centered
                                color="cyan"
                                dark
                                v-model="activeTab"
                                icons-and-text
                        >
                            <v-tab :href="`#tab-0`">User Question</v-tab>
                            <v-tab :href="`#tab-1`">User Answers</v-tab>
                            <v-tab :href="`#tab-2`">User Documents</v-tab>
                        </v-tabs>
                        <v-tabs-items v-model="activeTab">
                            <v-tab-item :value="`tab-0`">
                                <v-flex xs12>
                                    <question-item
                                             :filterVal="searchQuery" :questions="UserQuestions"
                                    ></question-item>
                                </v-flex>
                            </v-tab-item>
                            <v-tab-item :value="`tab-1`">
                                <v-flex xs12>
                                    <answer-item  :filterVal="searchQuery" :answers="UserAnswers"></answer-item>
                                </v-flex>
                            </v-tab-item>
                            <v-tab-item :value="`tab-2`">
                                <v-flex xs12>
                                    <document-item  :documents="UserDocuments"
                                                   :filterVal="searchQuery"></document-item>
                                </v-flex>
                            </v-tab-item>
                        </v-tabs-items>
                    </div>
                </v-layout>
            </div>
        </v-flex>
        <v-dialog v-model="getTokensDialogState" persistent max-width="600px" v-if="getTokensDialogState">
            <v-card>
                <v-card-title>
                    <span class="headline">Grant Tokens</span>
                </v-card-title>
                <v-card-text>
                    <v-container grid-list-md>
                        <v-layout wrap>
                            <v-flex xs12 sm12 md12>
                                <user-tokens :userId="userId"></user-tokens>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" flat @click="closeTokensDialog()">Close</v-btn>
                    <v-btn color="blue darken-1" flat @click="closeTokensDialog()">Cancel</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <v-progress-circular
                style="position: absolute; top: 300px; left: auto; right: auto;"
                :size="150"
                class="loading-spinner"
                color="#00bcd4"
                v-show="loading"
                indeterminate
        >
            <span>Loading...</span>
        </v-progress-circular>
    </v-layout>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import UserMainService from './userMainService';
    import { suspendUser, releaseUser } from '../user/suspend/suspendUserService';
    import { grantTokens } from '../user/token/tokenUserService';
    import questionItem from './helpers/questionIitem.vue';
    import answerItem from './helpers/answerItem.vue'
    import documentItem from './helpers/documentItem.vue'
    import userTokens from './helpers/sendTokens.vue'
    // import userSuspend from '../user/suspend/suspendUser.vue';
    import userCashout from '../user/cashout/cashoutUser.vue';


    export default {
        name: "userMainView",
        components: {
            questionItem,
            answerItem,
            documentItem,
            userTokens,
            userCashout
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
                },
                loading: false,
                userActions: [
                    {
                        title: "Suspend",
                        action: this.suspendUser,
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
                activeTab: 'tab-0',
                searchQuery: 'ok',
                userComponentsShow: false,
                activeUserComponent: '',
                deleteUserQuestions: false,
                activeTabEnum: {
                    'tab-0': 'questions',
                    'tab-1': 'answers',
                    'tab-2': 'documents',


                }
            }

        },
        computed: {
            ...mapGetters([
                "getTokensDialogState",
                "getUserObj",
                "UserInfo",
                "UserQuestions",
                "UserAnswers",
                "UserDocuments"
            ]),
            userInfo() {
                return this.UserInfo
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
                "getUserData",
                "setUserCurrentStatus",
                "getUserQuestions",
                "getUserAnswers",
                "getUserDocuments",
                "verifyUserPhone"
            ]),
            userInfoAction(actionItem){
                if(actionItem === "phoneNumber"){
                    let userObj = {
                        id: this.userInfo.id.value
                    }
                    this.verifyUserPhone(userObj).then(()=>{
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
            setUserComponent(val) {
                this.userComponentsShow = true;
                return this.activeUserComponent = val
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
                }
            },
            getUserInfoData() {
                let id = this.userIdentifier;
                let self = this;
                self.getUserData(id)
                    .then((data) => {
                        this.userId = data.id.value;
                        this.getDataByTabName()

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
            suspendUser() {
                let idArr = [];
                idArr.push(this.userId);
                suspendUser(idArr, this.deleteUserQuestions).then((email) => {
                    this.$toaster.success(`user got suspended, email is: ${email}`);
                    this.showSuspendedDetails = true;
                    this.suspendedMail = email;
                    this.suspendedUser = true;
                    this.setUserCurrentStatus(true);
                    // this.userData.userInfo.status.value ='suspended'
                }, (err) => {
                    this.$toaster.error(`ERROR: failed to suspend user`);
                    console.log(err)
                }).finally(() => {
                    this.lock = false;
                    this.userIds = null;

                })
            },
            releaseUser() {
                let self = this;
                let idArr = [];
                idArr.push(this.userId);
                releaseUser(idArr).then((email) => {
                    self.$toaster.success(`user got released`);
                    this.suspendedUser = false;
                    this.setUserCurrentStatus(false);
                    // self.userData.userInfo.status.value === 'active'
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

</script>

<style scoped lang="scss">
    .user-page-wrap {
        .item-wrap{
            overflow-y: scroll;
            height: 100%;
            max-height: calc(100vh - 275px);
        }

        .tabs-holder {
            order: 2;
            flex-grow: 1;
        }
        .general-info {
            order: 1;
            flex-direction: column;
            padding: 8px 0;
            margin-right: 8px;
            max-height: 570px;
            height: 570px;
            .info-item:nth-child(even) {
                background-color: #f5f5f5;
            }
            .user-info-label, .user-info-value {
                text-align: left;
            }
            .user-info-label {
                min-width: 125px;
                font-size: 16px;
                font-weight: 500;
                padding-bottom: 8px;
            }
            .user-info-value {
                padding-top: 8px;
                font-size: 16px;
                font-weight: 400;
                text-align: right;
                margin-right: 10px;
            }
            .user-info-button {
                padding-top: 8px;
                font-size: 16px;
                font-weight: 400;
                text-align: right;
                color: #00bcd4;
            }
        }
    }

</style>