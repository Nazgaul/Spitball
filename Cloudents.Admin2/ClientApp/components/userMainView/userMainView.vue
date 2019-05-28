<template>
    <v-layout justify-center class="user-page-wrap">
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
                <v-flex xs4 v-if="showActions">
                    <v-btn v-if="!userStatusActive && !suspendedUser" :disabled="!showActions" color="rgb(0, 188, 212)"
                           class="suspend"
                           @click="showSuspendDialog()">
                        Suspend
                    </v-btn>
                    <v-btn v-else :disabled="!showActions" class="suspend" @click="releaseUser()">UnSuspend</v-btn>

                    <v-btn :disabled="!showActions" class="grant" @click="openTokensDialog()">Grant Tokens
                    </v-btn>
                </v-flex>
            </div>
            <div class="questions-answers-wrap">
                <v-layout row>
                    <div class="general-info d-flex elevation-2 mb-2" v-if="showActions">
                        <div class="info-item px-2" v-for="(infoItem, index) in userInfo" :key="index">
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
                        <v-tabs centered
                                color="cyan"
                                dark
                                icons-and-text>
                            <v-tab :to="{name: 'userQuestions', params : {userId: userId} }">Question</v-tab>
                            <v-tab :to="{name: 'userAnswers', params:{userId: userId}}">Answers</v-tab>
                            <v-tab :to="{name: 'userDocuments', params:{userId: userId}}">Documents</v-tab>
                            <v-tab :to="{name: 'userPurchasedDocuments', params:{userId: userId}}">Purchased Documents</v-tab>
                            <v-tab :to="{name: 'userConversations', params:{userId: userId}}">Conversations</v-tab>
                            <v-tab :to="{name: 'userSessions', params:{userId: userId}}">Sessions</v-tab>
                        </v-tabs>
                        <div class="filters mb-2">
                            <v-btn v-for="(filter, index) in filters" @click="updateFilter(filter.value)"
                                   :color="filterValue === filter.value ? '#00bcd4' : ''  "
                                   :key="'filter_'+index">{{filter.name}}
                            </v-btn>
                        </div>
                        <v-tabs-items>
                            <router-view  :userId="userId"></router-view>
                        </v-tabs-items>
                    </div>
                </v-layout>
            </div>
        </v-flex>
        <v-dialog v-model="suspendDialogState" persistent max-width="600px" v-if="suspendDialogState">
            <v-card>
                <v-card-title>
                    <span class="headline">Suspend User</span>
                </v-card-title>
                <v-card-text>
                    <v-container grid-list-md>
                        <v-layout wrap>
                            <v-flex xs12 sm12 md12>
                                <userSuspend :userIds="userId"></userSuspend>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" flat @click="closeSuspendDialog()">Cancel</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
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

<script src="./userMainView.js">

</script>

<style scoped lang="scss" src="./userMainView.scss">

</style>