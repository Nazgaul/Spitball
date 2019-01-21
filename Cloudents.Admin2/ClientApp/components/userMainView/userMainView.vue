<template>
    <v-layout justify-center class="user-page-wrap" data-app>
        <v-flex xs12 sm12 md12 style="background: #ffffff; padding: 24px 24px;">
            <h1 >Welcome to Admin</h1>
            <div class="input-wrap d-flex  justify-end">
                <v-flex xs3>
                    <v-text-field autocomplete solo v-model="userIdentifier" type="text" class="user-id-input"
                                  placeholder="Insert user identifier..."/>
                </v-flex>
                <v-flex xs1>
                    <v-btn :disabled="!userIdentifier" primary @click="getUserData()">Get User</v-btn>
                </v-flex>
                <v-spacer></v-spacer>
                <v-flex xs4>
                    <v-btn v-if="userStatusActive" :disabled="!userData" class="suspend" @click="suspendUser()">
                        Suspend
                    </v-btn>
                    <v-btn v-else :disabled="!userData" class="suspend" @click="releaseUser()">Release</v-btn>

                    <v-btn :disabled="!userData" class="cash" @click="setUserComponent('userCashout')">Pay Cashout
                    </v-btn>
                    <v-btn :disabled="!userData" class="grant" @click="setUserComponent('userTokens')">Grant Tokens
                    </v-btn>
                    <div v-show="activeUserComponent && userComponentsShow">
                        <component :is="activeUserComponent ?  activeUserComponent : ''" :userId="userId"></component>
                    </div>
                </v-flex>
            </div>
            <div class="general-info d-flex elevation-2 mb-2" column v-if="userData && userData.userInfo">
                <div class="info-item" v-for="(infoItem, index) in userData.userInfo" :key="index">
                    <v-flex>
                        <div class="user-info-label">
                            <span>{{infoItem.label}}</span>
                        </div>
                        <div class="user-info-value">
                            <span>{{infoItem.value}}</span>
                        </div>
                    </v-flex>
                </div>

            </div>
            <div class="questions-answers-wrap">
                <div class="filters mb-2">
                    <v-btn v-for="(filter, index) in filters" @click="updateFilter(filter.value)"
                           :color="searchQuery === filter.value ? '#00bcd4' : ''  "
                           :key="'filter_'+index">{{filter.name}}
                    </v-btn>
                </div>
                <div class="tabs-holder">
                    <v-tabs
                            centered
                            color="cyan"
                            dark
                            @change="setActiveTab()"
                            v-model="activeTab"
                            icons-and-text
                    >
                        <v-tabs-slider color="yellow"></v-tabs-slider>

                        <v-tab href="#userQuestions">User Question</v-tab>

                        <v-tab href="#userAnswers">User Answers</v-tab>

                        <v-tab href="#userDocuments">User Documents</v-tab>

                        <v-tab-item :key="'1'" :value="'userQuestions'">
                            <v-flex xs12>
                                <question-item :updateData="updateData" :questions="filteredData"></question-item>
                            </v-flex>
                        </v-tab-item>
                        <v-tab-item :key="'2'" :value="'userAnswers'">
                            <v-flex xs12>
                                <answer-item :updateData="updateData" :answers="filteredData"></answer-item>
                            </v-flex>
                        </v-tab-item>
                        <v-tab-item :key="'3'" :value="'userDocuments'">
                            <v-flex xs12>
                                <document-item :updateData="updateData" :documents="filteredData"></document-item>
                            </v-flex>
                        </v-tab-item>
                    </v-tabs>
                </div>
            </div>
        </v-flex>
    </v-layout>
</template>

<script>
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
                userData: {},
                userIdentifier: '',
                filters: [
                    {name: 'All', value: 'ok'},
                    {name: 'Pending', value: 'pending'},
                    {name: 'Deleted', value: 'deleted'},
                    {name: 'Flagged', value: 'flagged'}
                ],
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
                activeTab: '',
                searchQuery: 'ok',
                userComponentsShow: false,
                activeUserComponent: '',
                deleteUserQuestions: false
            }
        },
        computed: {
            filteredData: function () {
                let self = this;
                if (self.userData && self.userData[`${this.activeTab}`]) {
                    return self.userData[`${this.activeTab}`].filter(function (item) {
                        return item.state.indexOf(self.searchQuery) !== -1
                    })
                }
            },
            userStatusActive() {
                if (this.userData && this.userData.userInfo) {
                    return this.userData.userInfo.status.value === 'active'
                }
            },

            userId() {
                if (this.userData && this.userData.userInfo) {
                    return this.userData.userInfo.id.value
                }
            }
        },
        methods: {
            setUserComponent(val) {
                this.userComponentsShow = true;
                return this.activeUserComponent = val
            },
            updateData(index) {
                this.userData[`${this.activeTab}`].splice(index, 1);
            },
            setActiveTab() {
                console.log(this.activeTab)
            },
            updateFilter(val) {
                return this.searchQuery = val
            },
            getUserData() {
                let id = this.userIdentifier;
                UserMainService.getUserData(id).then((data) => {
                        this.userIdentifier = '';
                        this.userData = data;
                        console.log(data)
                    },
                    (error) => {
                        console.log(error, 'error')
                    }
                )
            },
            suspendUser() {
                let idArr = [];
                idArr.push(this.userId);
                suspendUser(idArr, this.deleteUserQuestions).then((email) => {
                    this.$toaster.success(`user got suspended, email is: ${email}`);
                    this.showSuspendedDetails = true;
                    this.suspendedMail = email;
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
                    // self.userData.userInfo.status.value === 'active'
                }, (err) => {
                    self.$toaster.error(`ERROR: failed to realse user`);
                    console.log(err)
                }).finally(() => {
                    self.lock = false;
                    self.userIds = null;

                })
            },
            sendTokens() {
                if (!this.userId) {
                    this.$toaster.error("you must provide a UserId")
                    return;
                }
                if (!this.tokens) {
                    this.$toaster.error("you must provide tokens")
                    return;
                }
                grantTokens(this.userId, this.tokens, this.tokenType).then(() => {
                    this.$toaster.success(`user id ${this.userId} recived ${this.tokens} tokens`)
                    this.userId = null;
                    this.tokens = null;
                }, (err) => {
                    console.log(err);
                    this.$toaster.error(`Error: couldn't send tokens`)
                })

            }
        },
        created() {

        }
    }

</script>

<style scoped lang="scss">
    .user-page-wrap {
        .general-info {
            padding: 8px 16px;
            .user-info-label, .user-info-value {
                text-align: left;
            }
            .user-info-label {
                min-width: 15%;
                font-size: 16px;
                font-weight: 500;
                border-bottom: 1px solid grey;
                padding-bottom: 8px;
            }
            .user-info-value {
                padding-top: 8px;
                font-size: 16px;
                font-weight: 400;
            }
        }
    }

</style>