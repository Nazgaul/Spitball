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

                <td :class="{ 'no-pay': props.item.tutorPayme }" >{{ props.item.tutorId }}</td>
                <td >{{ props.item.tutorName }}</td>
                <td >{{ props.item.userId }}</td>
                <td >{{ props.item.userName }}</td>
                <td>{{ props.item.created }}</td>
                <td>{{ props.item.duration }}</td>
               
                <td>{{ props.item.price }}</td>
                <td>{{ props.item.totalPrice }}</td>
                <td >
                    <span  @click="editItem(props.item)">
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


        <v-dialog v-model="dialog" max-width="800px">
            <v-card>
                <v-card-text>
                    <v-container grid-list-md>
                        <v-alert v-model="editedItem.tutorPayme" type="error" class="mb-4"> This payment can't be processed because seller is not on payme</v-alert>
                        <v-layout wrap>
                             <v-flex xs12>
                                 User Name:  <b>{{editedItem.userName}}</b>
                            </v-flex>
                           
                            <v-flex xs12>
                                Tutor Name:  <b>{{editedItem.tutorName}}</b>
                            </v-flex>
                            <v-flex xs12 v-if="editedItem.tutorPayme">
                                Tutor Id:  <b>{{editedItem.tutorId}}</b>
                            </v-flex>

                               <v-flex xs12>
                               
                                 <v-text-field label=" Tutor Price per hour:" v-model="editedItem.tutorPrice"></v-text-field>
                                  
                            </v-flex>
                            <br>
                            <br>
                             <br>
                              <br>
                            
                            
                           <v-flex xs3>
                               <v-text-field label="Duration in minutes" v-model="editedItem.duration"></v-text-field>
                           </v-flex>
                           <v-flex xs3>
                               <v-text-field readonly="" label="Session Total Price" v-model="editedItem.price"></v-text-field>
                           </v-flex>
                         
                           <!-- <v-flex xs4>
                               <v-text-field readonly="" label="Session Price After discount" v-model="editedItem.subsidizing"></v-text-field>
                           </v-flex> -->
                           
                          
                            
                            
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
    import { getPaymentRequests, approvePayment, declinePayment } from './PaymentUserService'

    export default {
       
    computed: {
    durationX() {
      return this.editedItem.duration;
    },
    tutorPriceX() {
        return this.editedItem.tutorPrice;
    }
  },
  watch: {
    durationX() {
    this.editedItem.price = (this.editedItem.tutorPrice*this.editedItem.duration/60).toFixed(2);      
    },
    tutorPriceX() {
         this.editedItem.price = (this.editedItem.tutorPrice*this.editedItem.duration/60).toFixed(2);
    }
  },
        data() {
            return {
                editedIndex: -1,
                defaultRows: { rowsPerPage: 25 },
                dialog: false,
                showLoading: true,
                showNoResult: false,
                paymentRequestsList: [],
                editedItem: { },
                headers: [{ text: 'Tutor Id', value: 'tutorId' },
                    { text: 'Tutor Name', value: 'tutorName' },
                    { text: 'User Id', value: 'userId' },
                    { text: 'User Name', value: 'userName' },
                    { text: 'Date', value: 'created' },
                    { text: 'Duration (min)', value: 'duration' },
                    { text: 'Tutor Price', value: 'price' },
                    { text: 'Lessons Price', value: 'totalPrice' },
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
                this.editedIndex = this.paymentRequestsList.indexOf(item);
                console.log(item);
                this.dialog = true;
                this.editedItem = {
                    "studyRoomSessionId": item.studyRoomSessionId,
                    "userName": item.userName,
                    "tutorName": item.tutorName,
                    tutorId: item.tutorId,
                    tutorPayme: item.tutorPayme,
                    tutorPrice: item.price,
                    duration: item.duration,
                    userId: item.userId,
                    "price": item.totalPrice
                };
            },
            approve() {
                var itemToSubmit = this.editedItem;
                const index = this.editedIndex;
                const item = this.paymentRequestsList[index];
                approvePayment(itemToSubmit).then((resp) => {

                    this.$toaster.success(`User ${item.userName} pay to Tutor ${item.tutorName}`);
                    this.paymentRequestsList.splice(index, 1);
                    this.dialog = false;
                    this.editedItem = {};
                    this.editedIndex = -1;
                },
                    (error) => {
                        debugger;
                        this.$toaster.error(`Error can't approve the payment ${error.response.data}`);
                    }
                )
            },
            decline() {
                 var itemToSubmit = this.editedItem;
                const index = this.editedIndex;
                const item = this.paymentRequestsList[index];
                var result = confirm("Are you sure?");
                if (result) {
                    declinePayment(itemToSubmit).then((resp) => {

                    this.$toaster.success(`Payment removed`);
                    this.paymentRequestsList.splice(index, 1);
                    this.dialog = false;
                    this.editedItem = {};
                    this.editedIndex = -1;
                },
                    (error) => {
                        this.$toaster.error(`Error can't approve the payment`);
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
    .no-pay {
        background: red;
    }
  
</style>