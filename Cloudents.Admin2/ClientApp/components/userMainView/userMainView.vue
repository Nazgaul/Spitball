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

                    <!--<div v-show="activeUserComponent && userComponentsShow">-->
                    <!--<component :is="activeUserComponent ?  activeUserComponent : ''" :userId="userId"></component>-->
                    <!--</div>-->
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
                            <v-tabs-slider color="yellow"></v-tabs-slider>
                            <v-tab @change="setActiveTab('questions')" href="#userQuestions">User Question</v-tab>

                            <v-tab @change="setActiveTab('answers')" href="#userAnswers">User Answers</v-tab>

                            <v-tab @change="setActiveTab('documents')" href="#userDocuments">User Documents</v-tab>

                            <v-tab-item :key="'1'" :value="'questions'">
                                <v-flex xs12>
                                    <question-item
                                         :updateData="updateData" :questions="questions"></question-item>
                                </v-flex>
                            </v-tab-item>
                            <v-tab-item :key="'2'" :value="'answers'"  v-if="activeTab === 'answers'">
                                <v-flex xs12>
                                    <answer-item :updateData="updateData" :answers="answers"></answer-item>
                                </v-flex>
                            </v-tab-item>
                            <v-tab-item :key="'3'" :value="'documents'" v-if="activeTab === 'documents'">
                                <v-flex xs12>
                                    <document-item :updateData="updateData" :documents="documents"
                                                   :filterVal="searchQuery"></document-item>
                                </v-flex>
                            </v-tab-item>
                        </v-tabs>
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
                suspendedUser: false,
                filters: [
                    {name: 'Accepted', value: 'ok'},
                    {name: 'Pending', value: 'pending'},
                    {name: 'Deleted', value: 'deleted'},
                    {name: 'Flagged', value: 'flagged'}
                ],
                scrollFunc: {
                    questions: {page: 1, getData: this.getUserQuestionsData},
                    answers: {page: 1, getData: this.getUserAnswers},
                    documents: {page: 1, getData: this.getUserDocuments},
                },
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
                activeTab: 'questions',
                searchQuery: 'ok',
                userComponentsShow: false,
                activeUserComponent: '',
                deleteUserQuestions: false
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
            questions() {
                return this.UserQuestions
            },
            answers() {
                return this.UserAnswers
            },
            documents() {
                return this.UserDocuments
            },
            filteredData: function () {
                let self = this;
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

            userId() {
                if (this.userInfo) {
                    return this.userInfo.id.value
                }
            }
        },
        methods: {
            ...mapActions([
                "setTokensDialogState",
                "getUserData",
                "setUserCurrentStatus",
                "getUserQuestions",
                "getUserAnswers",
                "getUserDocuments"
            ]),
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
            updateData(index) {
                // this.userData[`${this.activeTab}`].splice(index, 1);
            },
            setActiveTab(activeTabName) {
                return this.activeTab = activeTabName;
            },
            updateFilter(val) {
                return this.searchQuery = val
            },
            getUserInfoData() {
                let id = this.userIdentifier;
                let self = this;
                self.getUserData(id)
                    .then((data) => {
                        let idPageObj ={
                            id: data.id.value,
                            page:  self.scrollFunc[`${self.activeTab}`].page
                        };
                        self.scrollFunc[`${self.activeTab}`].getData(idPageObj);
                    })
            },
            getUserQuestionsData(id, page) {
                this.getUserQuestions(id, page)
            },
            getUserAnswers(id, page) {
                this.getUserAnswers(id, page)
            },
            getUserDocuments(id, page) {
                this.getUserDocuments(id, page)
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

        },
        created() {

        }
    }

</script>

<style scoped lang="scss">
    .user-page-wrap {
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
            }
        }
    }

</style>