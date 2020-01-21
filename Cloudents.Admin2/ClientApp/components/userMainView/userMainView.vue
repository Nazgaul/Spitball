<template>
    <v-container fluid>
        <v-form ref="form" lazy-validation  
         @submit.prevent="submitUserData"
          >
            <v-layout row wrap :class="{'column': $vuetify.breakpoint.mdAndDown}">
                <v-flex xs3 style="display: flex;">
                    <v-text-field solo v-model="userIdentifier"
                                  autofocus
                                  type="text"
                                  required
                                  :rules="requiredRules"
                                  placeholder="Insert user identifier..."/>
                    <v-btn color="success" @click="submitUserData()">
                        Get User
                    </v-btn>
                </v-flex>
                
               
                <v-spacer></v-spacer>
                <v-flex xs4 v-if="showActions" >
                    <v-btn v-if="!userStatusActive && !suspendedUser" :disabled="!showActions" color="red lighten-2"
                           @click="showSuspendDialog()">
                        Suspend
                    </v-btn>
                    <v-btn v-else color="teal lighten-1" @click="releaseUser()">UnSuspend</v-btn>

                    <v-btn :disabled="!showActions" color="light-green lighten-2" @click="openTokensDialog()">
                        Grant Tokens
                    </v-btn>
                    
                </v-flex>
                
            </v-layout>
        </v-form>
        <v-layout :class="{'column': $vuetify.breakpoint.mdAndDown}">
            <v-flex md3 v-if="showActions" :class="{'userDetails mb-4': $vuetify.breakpoint.mdAndDown}">
                <v-list dense class="elevation-2">
                    <template v-for="(infoItem, name,index) in userInfo">
                        <v-list-tile :class="[ (index % 2 == 0) ? 'table-odd' : '' ]" :key="index">
                            <v-layout align-center justify-space-between>
                                <div>
                                <span><b>{{infoItem.label}}:</b></span>
                                <span>{{infoItem.value}}</span>

                                </div>
                                <div>
                                    <v-btn small color='info' @click="openNameDialog(userInfo.name.value)" v-if="infoItem.label == 'User Name'">Edit</v-btn>
                                    <v-btn small color='info' @click="openPhoneDialog(userInfo.phoneNumber.value)" v-if="infoItem.label == 'Phone Number'">Edit</v-btn>
                                    <v-btn small color='info' @click="openTutorPriceDialog(userInfo.tutorPrice.value)" v-if="infoItem.label == 'Tutor Price'" class="white--text">Edit</v-btn>
                                    <v-btn small color='info' class="white--text" @click="removePayment(userInfo.id.value)" v-if="infoItem.label == 'Has Payment' && infoItem.value">Delete</v-btn>
                                    <v-btn small color='info' class="white--text" @click="deleteTutor()" v-if="infoItem.label == 'Tutor State'">Delete</v-btn>

                                    <template v-if="infoItem.label == 'Tutor State'">
                                        <v-btn small color='info' class="white--text" @click="suspendTutor()" v-if="infoItem.value === 'ok'">suspend</v-btn>
                                        <v-btn small color='info' class="white--text" @click="unSuspendTutor()" v-if="infoItem.value === 'flagged'">un suspend</v-btn>
                                    </template>

                                    <v-btn small color='info' class="white--text" @click="deleteCalender()" v-if="infoItem.label == 'Has Calendar' && infoItem.value === true">Delete</v-btn>

                                </div>
                            </v-layout>
                        </v-list-tile>
                        <v-divider></v-divider>
                    </template>
                </v-list>
            </v-flex>
            <v-spacer></v-spacer>
            <v-flex xs10 class="ml-2">
                <v-tabs centered color="light-green">
                    <v-tab :to="{name: 'userQuestions', params : {userId: userId} }">Question</v-tab>
                    <v-tab :to="{name: 'userAnswers', params:{userId: userId}}">Answers</v-tab>
                    <v-tab :to="{name: 'userDocuments', params:{userId: userId}}">Documents</v-tab>
                    <v-tab :to="{name: 'userPurchasedDocuments', params:{userId: userId}}">Purchased Documents</v-tab>
                    <v-tab :to="{name: 'userSoldItems', params:{userId: userId}}">Sold Items</v-tab>
                    <v-tab :to="{name: 'userConversations', params:{userId: userId}}">Conversations</v-tab>
                    <v-tab :to="{name: 'userSessions', params:{userId: userId}}">Sessions</v-tab>
                    <v-tab :to="{name: 'userNotes', params:{userId: userId}}">Notes</v-tab>
                </v-tabs>
                <div class="filters mb-2">
                    <v-btn v-for="(filter, index) in filters" @click="updateFilter(filter.value)"
                           :color="filterValue === filter.value ? 'grey lighten-1' : ''  "
                           :key="'filter_'+index">
                        {{filter.name}}
                    </v-btn>
                </div>
                <v-tabs-items>

                    <router-view :userId="userId" :needScroll="needScroll"></router-view>

                    <div class="d-flex justify-center align-center" v-if="loader">
                        <div class="text-xs-center">
                        <v-progress-circular :size="100"
                                            :width="5"
                                             color="primary"
                                             indeterminate>
                            Loading...
                        </v-progress-circular>
                        </div>
                    </div>
                  

                </v-tabs-items>
            </v-flex>
        </v-layout>
        <v-dialog v-model="suspendDialogState" persistent max-width="600px" lazy v-if="suspendDialogState">
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
        <v-dialog v-model="getTokensDialogState" persistent max-width="600px" lazy v-if="getTokensDialogState">
            <v-card>
                <v-card-title>
                    <span class="headline">Grant Tokens</span>
                </v-card-title>
                <v-card-text>
                    <v-container grid-list-md>
                        <v-layout wrap>
                            <v-flex xs12>
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
        <v-dialog v-model="dialogs.name" persistent max-width="600px" lazy v-if="dialogs.name">
            <v-card>
                <v-card-title>
                    <span class="headline">Edit name</span>
                </v-card-title>
                <v-card-text>
                    <v-container grid-list-md>
                        <v-layout wrap>
                            <v-flex xs12>
                                <v-text-field
                                    v-model="newFirstName"
                                    label="First Name"
                                    :placeholder="currentFirstName"
                                ></v-text-field>
                                <v-text-field
                                    v-model="newLastName"
                                    label="Last Name"
                                    :placeholder="currentLastName"
                                ></v-text-field>
                                <v-btn @click="editName()">Send</v-btn>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" flat @click="dialogs.name = false">Close</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <v-dialog v-model="dialogs.phone" persistent max-width="600px" lazy v-if="dialogs.phone">
            <v-card>
                <v-card-title>
                    <span class="headline">Edit phone</span>
                </v-card-title>
                <v-card-text>
                    <v-container grid-list-md>
                        <v-layout wrap>
                            <v-flex xs12 sm12 md12>
                                <v-text-field
                                    v-model="newPhone"
                                    label="Phone"
                                    :placeholder="currentPhone"
                                ></v-text-field>
                                <v-btn @click="editPhone()">Send</v-btn>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" flat @click="dialogs.phone = false">Close</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <v-dialog v-model="dialogs.price" persistent max-width="600px" lazy v-if="dialogs.price">
            <v-card>
                <v-card-title>
                    <span class="headline">Edit price</span>
                </v-card-title>
                <v-card-text>
                    <v-container grid-list-md>
                        <v-layout wrap>
                            <v-flex xs12 sm12 md12>
                                <v-text-field
                                    v-model="newPrice"
                                    label="Tutor Price"
                                    :placeholder="currentPrice"
                                ></v-text-field>
                                <v-btn @click="editPrice()">Send</v-btn>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" flat @click="dialogs.price = false">Close</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-container>
</template>

<script src="./userMainView.js">

</script>

<style lang="less">

.table-odd {
    background: #d2d2d2;
}


</style>
