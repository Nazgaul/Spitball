<template>
    <div class="cashout-table-container">
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <h4>Pending Payments List</h4>

        <v-data-table :headers="headers"
                      :items="paymentRequestsList"
                      class="elevation-1"
                      disable-initial-sort>
            <template slot="items" slot-scope="props">
                <td class="text-xs-center">{{ props.item.tutorId }}</td>
                <td class="text-xs-center">{{ props.item.tutorName }}</td>
                <td class="text-xs-center">{{ props.item.userId }}</td>
                <td class="text-xs-center">{{ props.item.userName }}</td>
                <td class="text-xs-center">{{ props.item.price }}</td>
                <td class="text-xs-center">
                    <span>
                        <v-icon small
                                color="green"
                                class="mr-2"
                                @click="editItem(props.item)">
                            call_to_action
                        </v-icon>
                    </span>
                </td>

            </template>
        </v-data-table>


        <v-dialog v-model="dialog" max-width="800px">
            <v-card>
                <v-card-text>
                    <v-container grid-list-md>
                        <v-layout wrap>
                            <v-flex xs12 style="text-align: left">
                                <span>
                                    Study Room Session Id: <input type="text" v-model="editedItem.studyRoomSessionId">
                                    <br>
                                </span>
                                <span>
                                    User Name: {{editedItem.userName}}
                                    <br>
                                </span>
                                <span>
                                    Payment Key: <input type="text" v-model="editedItem.paymentKey">
                                    <br>
                                </span>
                                <span>
                                    Tutor Name: {{editedItem.tutorName}}
                                    <br>
                                </span>
                                <span>
                                    Seller Key: <input type="text" v-model="editedItem.sellerKey">
                                    <br>
                                </span>
                                <span>
                                    Price: <input type="text" v-model="editedItem.price">
                                    <br>
                                </span>
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
                    { text: 'Price', value: 'price' },
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
                    "price": item.price
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