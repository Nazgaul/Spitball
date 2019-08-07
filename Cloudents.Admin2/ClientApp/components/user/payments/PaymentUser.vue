<template>
    <div class="cashout-table-container">
      
        <h4>Pending Payments List</h4>
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <v-data-table :headers="headers"
                      :items="paymentRequestsList"
                     
                      disable-initial-sort>
            <template slot="items" slot-scope="props">
                <td >{{ props.item.tutorId }}</td>
                <td >{{ props.item.tutorName }}</td>
                <td >{{ props.item.userId }}</td>
                <td >{{ props.item.userName }}</td>
                <td>{{ props.item.created }}</td>
                <td>{{ props.item.duration }}</td>
               
                <td>{{ props.item.price }}</td>
                 <td>{{ props.item.subsidizing }}</td>
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
                        <v-layout wrap>
                            <v-flex xs6>
                                 Study Room Session Id:
                            </v-flex>
                            <v-flex xs6>
                                {{editedItem.studyRoomSessionId}}
                                
                            </v-flex>
                             <v-flex xs6>
                                 User Name:
                            </v-flex>
                            <v-flex xs6>
                                {{editedItem.userName}}
                            </v-flex>
                              <v-flex xs6>
                                Tutor Name:
                            </v-flex>
                            <v-flex xs6>
                                {{editedItem.tutorName}}
                            </v-flex>
                            <v-flex xs12>
                                     <v-text-field
            label="Payment Key" v-model="editedItem.paymentKey"
          ></v-text-field>
                                
                            </v-flex>
                            
                           
                            <v-flex xs12>
                                  <v-text-field
            label="Seller Key" v-model="editedItem.sellerKe"
          ></v-text-field>
                               
                            </v-flex>
                           
                            <v-flex xs12>
                                  <v-text-field
            label="Price" v-model="editedItem.price"
          ></v-text-field>
                        
                            </v-flex>
                            
                        </v-layout>
                    </v-container>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" flat @click="close">Cancel</v-btn>
                    <v-btn color="red darken-1" flat @click="approve()">
                        Done
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
</div>
</template>

<script>
    import { getPaymentRequests, approvePayment } from './PaymentUserService'

    export default {
        data() {
            return {
                editedIndex: -1,
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
                    { text: 'Lesson Price', value: 'price' },
                    { text: 'Price After Subsidizing', value: 'subsidizing' },

                    { text: 'Approve', value: '' }
                ]
            }
        },
        methods: {
            editItem(item) {
                this.editedIndex = this.paymentRequestsList.indexOf(item);
                this.dialog = true;
                this.editedItem = {
                    "studyRoomSessionId": item.studyRoomSessionId,
                    "userName": item.userName,
                    "paymentKey": item.paymentKey,
                    "tutorName": item.tutorName,
                    "sellerKey": item.sellerKey,
                    "price": item.subsidizing
                };
            },
            approve() {
                var itemToSubmit = this.editedItem;
                const index = this.editedIndex;
                const item = this.paymentRequestsList[index];
                approvePayment(itemToSubmit).then((resp) => {
                    console.log('got payment resp success')

                    this.$toaster.success(`User ${item.userName} pay to Tutor ${item.tutorName}`);
                    this.paymentRequestsList.splice(index, 1);
                    this.dialog = false;
                    this.editedItem = {};
                    this.editedIndex = -1;
                },
                    (error) => {
                        this.$toaster.error(`Error can't approve the payment`);
                    }
                )
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
  
</style>