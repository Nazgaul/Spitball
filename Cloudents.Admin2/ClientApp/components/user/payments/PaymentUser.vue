<template>
    <div class="cashout-table-container">
      
        <h4>Pending Payments List</h4>
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <v-data-table :headers="headers"
                      :items="paymentRequestsList"
                     :custom-sort="customSort"
                     :pagination.sync="defaultRows"
                      disable-initial-sort>
            <template slot="items" slot-scope="props" >

                <td :class="{ 'no-tutorPayme': !props.item.tutorPayme}" >{{ props.item.tutorId }}</td>
                <td >{{ props.item.tutorName }}</td>
                <td :class="{ 'no-userPayme': !props.item.userPayme }">{{ props.item.userId }}</td>
                <td >{{ props.item.userName }}</td>
                <td>{{ props.item.created }}</td>
                <td>{{ props.item.realDuration }}</td>
                <td :class="{ 'realDurationExitsts': props.item.isRealDurationExitsts }">{{Math.floor(props.item.duration)}}</td>
               
                <td>{{ props.item.price }}</td>
                <td>{{ props.item.totalPrice.toFixed(2) }}</td>
                 <!-- <td>{{ props.item.subsidizing }}</td> -->
                <td >
                    <span style="cursor:pointer"  @click="editItem(props.item)">
                        <v-icon small
                                color="green"
                                class="mr-2"
                               >
                            call_to_action
                        </v-icon>
                        Pay
                    </span>
                </td>

            </template>
        </v-data-table>


        <v-dialog v-model="dialog" max-width="800px" v-if="sessionPayment" persistent>
            <v-card>
                <v-card-text>
                    <v-container grid-list-md>
                        <v-alert v-model="sessionPayment.cantPay" type="error" class="mb-4"> This payment can't be processed because seller is not on payme</v-alert>
                        <v-layout wrap>
                            <v-flex xs6>
                                <v-flex>User Name:  <b>{{sessionPayment.userName}}</b></v-flex>
                                <v-flex>Tutor Name:  <b>{{sessionPayment.tutorName}}</b></v-flex>
                                <v-flex v-if="sessionPayment.cantPay">Tutor Id:  <b>{{sessionPayment.tutorId}}</b></v-flex>
                            </v-flex>
                            <v-flex xs6 class="mb-4">
                                <template v-if="sessionPayment.couponCode">
                                    <v-flex>Coupon Code: {{sessionPayment.couponCode}}</v-flex>
                                    <v-flex>Coupon Type: {{sessionPayment.couponType}}</v-flex>
                                    <v-flex>Coupon Value: {{sessionPayment.couponValue}}</v-flex>
                                </template>
                            </v-flex>

                            <v-flex xs12 sm4>
                               <v-text-field label="Tutor Price Per Hour" v-model="sessionPayment.tutorPricePerHour"></v-text-field>   
                            </v-flex>
                            <v-flex xs12 sm4>
                               <v-text-field label="Student Price Per Hour (depend on coupon)" v-model="sessionPayment.studentPayPerHour"></v-text-field>   
                            </v-flex>
                            <v-flex xs12 sm4>
                               <v-text-field label="Spitball Price Per Hour (depend on coupon)" v-model="sessionPayment.spitballPayPerHour"></v-text-field>   
                            </v-flex>
                            
                            <v-flex xs12 sm3>
                                <v-text-field label="Duration in Minutes" v-model="sessionPayment.duration"></v-text-field>
                            </v-flex>
                            <v-flex xs12 sm3>
                                <v-text-field label="Student Pay Total" v-model="studentPayPerHour"></v-text-field>
                            </v-flex>
                            <v-flex xs12 sm3>
                                <v-text-field label="Spitball Pay Total" v-model="spitballPayPerHour"></v-text-field>
                            </v-flex>
                            <v-flex xs12 sm3>
                                <v-text-field label="Session Total Price" v-model="totalPrice" readonly></v-text-field>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                     <v-btn color="red darken-1" flat @click="decline()">
                        No Pay
                    </v-btn>
                    <v-btn color="blue darken-1" flat @click="close">Cancel</v-btn>
                    <v-btn color="green darken-1" :disabled="editedItem.tutorPayme" flat @click="approve()">
                        Pay
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
</div>
</template>

<script>
import { getPaymentRequests, getUserSessionPayment, approvePayment , subsidizingPrice, declinePayment } from './PaymentUserService';

export default {
    computed: {
        durationX() {
            return this.editedItem.duration;
        },
        tutorPriceX() {
            return this.editedItem.tutorPrice;
        },
        studentPayPerHour: {
            get() {
                let session = this.sessionPayment;
                if(session.studentPayPerHour) {
                    return ((session.studentPayPerHour * session.duration) / 60).toFixed(2);
                }
                return 0;
            },
            set(val) {
                this.sessionPayment.studentPayPerHour = (( val / this.sessionPayment.duration) * 60).toFixed(2);
            }
        },
        spitballPayPerHour: {
            get() {
                let session = this.sessionPayment;
                if(session.spitballPayPerHour) {
                    return ((session.spitballPayPerHour * session.duration) / 60).toFixed(2);
                }
                return 0;
            },
            set(val) {
                this.sessionPayment.spitballPayPerHour = (( val / this.sessionPayment.duration) * 60).toFixed(2);
            }
        },
        totalPrice() {
            let session = this.sessionPayment;
            return Number(this.spitballPayPerHour) + Number(this.studentPayPerHour);
        }
    },
    watch: {
        durationX() {
            this.editedItem.price = (this.editedItem.tutorPrice*this.editedItem.duration/60).toFixed(2);
            this.editedItem.subsidizing = (subsidizingPrice(this.editedItem.tutorPrice)*this.editedItem.duration/60).toFixed(2);
        },
        tutorPriceX() {
            this.editedItem.price = (this.editedItem.tutorPrice*this.editedItem.duration/60).toFixed(2);
            this.editedItem.subsidizing = (subsidizingPrice(this.editedItem.tutorPrice)*this.editedItem.duration/60).toFixed(2);
        }
    },
    data() {
        return {
            sessionPayment: null,
            editedIndex: -1,
            defaultRows: { rowsPerPage: 25 },
            dialog: false,
            showLoading: true,
            showNoResult: false,
            paymentRequestsList: [],
            editedItem: { },
            headers: [
                { text: 'Tutor Id', value: 'tutorId' },
                { text: 'Tutor Name', value: 'tutorName' },
                { text: 'User Id', value: 'userId' },
                { text: 'User Name', value: 'userName' },
                { text: 'Date', value: 'created' },
                { text: 'RealDuration (min)', value : 'realDuration' },
                { text: 'Duration (min)', value: 'duration' },
                { text: 'Tutor Price', value: 'price' },
                { text: 'Lessons Price', value: 'totalPrice' },
                // { text: 'Price After Subsidizing', value: 'subsidizing' },
                { text: 'Approve', value: '' }
            ]
        }
    },
    methods: {
        customSort(items, index, isDesc) {
            items.sort((a, b) => {
                if (index === "created") {
                    if (!isDesc) {
                        a = new Date(a.created);
                        b = new Date(b.created);
                        return a>b ? -1 : a<b ? 1 : 0;
                    } else {
                        a = new Date(a.created);
                        b = new Date(b.created);
                        return a>b ? 1 : a<b ? -1 : 0;
                    }
                } else {
                if (!isDesc) {
                    return a[index] < b[index] ? -1 : 1;
                } else {
                    return b[index] < a[index] ? -1 : 1;
                }
                }
        });
        return items;
        },
        editItem(item) {
            this.dialog = true;
            let params = {
                sessionId: item.studyRoomSessionId,
                userId: item.userId
            }
            this.editedIndex = this.paymentRequestsList.indexOf(item);
            getUserSessionPayment(params).then(session => {
                this.sessionPayment = session;
            })
        },
        approve() {
            let item = this.sessionPayment;
            let itemObj = {
                studentPay : Number(this.studentPayPerHour),
                spitballPay: Number(this.spitballPayPerHour),
                userId: item.userId,
                tutorId: item.tutorId,
                StudyRoomSessionId: item.studyRoomSessionId,
                adminDuration: item.duration
            }

            approvePayment(itemObj).then((resp) => {

                this.$toaster.success(`User ${item.userName} pay to Tutor ${item.tutorName}`);
                this.paymentRequestsList.splice(this.editedIndex, 1);
                this.dialog = false;
                this.editedItem = {};
                this.editedIndex = -1;
            },
                (error) => {
                    this.$toaster.error(`Error can't approve the payment ${error.response.data}`);
                }
            )
        },
        decline() {
              let item = this.sessionPayment;
              let itemObj = {
                 studyRoomSessionId: item.studyRoomSessionId,
                 userId: item.userId
              }
            //     var itemToSubmit = this.editedItem;
            // const index = this.editedIndex;
            // const item = this.paymentRequestsList[index];
            var result = confirm("Are you sure?");
            if (result) {
                declinePayment(itemObj).then((resp) => {

                this.$toaster.success(`Payment removed`);
                 this.paymentRequestsList.splice(this.editedIndex, 1);
                this.dialog = false;
                this.editedItem = {};
                this.editedIndex = -1;
            },
                (error) => {
                    this.$toaster.error(`Error can't decline the payment`);
                }
            )
                
            }
        },
        close() {
            this.dialog = false;
            this.editedItem = {};
            this.editedIndex = -1;
        }
    },
    created() {
        getPaymentRequests().then((list) => {
            if (list.length === 0) {
                this.showNoResult = true;
            } else {
                this.paymentRequestsList = list;
            }
            this.showLoading = false;
        }, (err) => {
            console.log(err)
        })
    }
}
</script>


<style>
    /*.elevation-1 {
        width: 100%;
        text-align: center
    }*/
    input {
    width: 300px;
    }
    .no-tutorPayme {
        background: red;
    }
    .no-userPayme {
        background: purple;
    }
    
    .realDurationExitsts {
        background: green;
    }
  
</style>