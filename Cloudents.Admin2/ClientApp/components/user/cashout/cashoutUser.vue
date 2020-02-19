<template>
    <div class="cashout-table-container">
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <h1 align="center">Cashout List</h1>
        <v-layout>
            <v-spacer></v-spacer>
            <v-flex xs4 sm4 md4>
                <v-text-field
                        v-model="search"
                        append-icon="search"
                        label="Search"
                        single-line
                        hide-details
                ></v-text-field>
            </v-flex>
        </v-layout>

        <v-data-table
                :headers="headers"
                :items="cashOutList"
                class="cash-out-table"
                disable-initial-sort
                :rows-per-page-items="[25, 50, 100,{text: 'All', value:-1}]"
                :search="search"
        >
            <template slot="items" slot-scope="props">
                <td class="text-xs-center">{{ props.item.userId }}</td>
                <td class="text-xs-center">{{ props.item.cashOutPrice }}</td>
                <td class="text-xs-center">{{ props.item.userEmail }}</td>
                <td class="text-xs-center">{{props.item.cashOutTime | dateFromISO}}</td>

                <td class="text-xs-center">{{ props.item.country}}</td>

                <td class="text-xs-center">{{ props.item.referCount }}</td>
                <td class="text-xs-center">{{ props.item.soldDocument }}</td>
                <td class="text-xs-center">{{ props.item.correctAnswer }}</td>
                <td class="text-xs-center">{{ props.item.soldDeletedDocument }}</td>
                <td class="text-xs-center">{{ props.item.deletedCorrectAnswer }}</td>
                <td class="text-xs-center">{{ props.item.cashOut }}</td>
                <td class="text-xs-center">{{ props.item.awardCount }}</td>
                <td class="text-xs-center">{{ props.item.buyCount }}</td>
                <td class="text-xs-center">{{ props.item.declinedReason }}</td>
                <td class="text-xs-center">{{ props.item.approved }}</td>
                <td class="text-xs-center">
                    <span>

                    <!--<span v-if="!props.item.approved && !props.item.declinedReason">-->
                        <v-icon small
                                color="green"
                                class="mr-2"
                                :disabled="!!props.item.approved || !!props.item.declinedReason"
                                @click="editItem(props.item)">
                            call_to_action

                        </v-icon>
                        <!--<v-icon small-->
                                <!--color="green"-->
                                <!--@click="approveCashout(props.item)">-->
                            <!--check-->
                        <!--</v-icon>-->
                    </span>
                </td>

            </template>
        </v-data-table>
        <v-dialog v-model="dialog" max-width="500px">
            <v-card>
                <v-card-title>
                    <span class="headline">Add a reason </span>
                </v-card-title>

                <v-card-text>
                    <v-container grid-list-md>
                        <v-layout wrap>
                            <v-flex xs12 sm12 md12>
                                <v-radio-group v-model="radios">
                                    <v-radio label="Decline" value="decline"></v-radio>
                                    <v-radio label="Approve" value="approve"></v-radio>
                                </v-radio-group>
                                <v-text-field v-show="radios === 'decline'" v-model="editedItem.declinedReason" label="Decline Reason"></v-text-field>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" flat @click="close">Cancel</v-btn>
                    <v-btn color="red darken-1" :disabled="disableDoneBtn" flat @click="done()">
                        Done
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </div>
</template>
<script>
    import { approveCashout, declineCashout, getCashoutList } from './cashoutUserService'

    export default {
        data() {
            return {
                cashOutList: [],
                showLoading: true,
                showNoResult: false,
                editedIndex: -1,
                radios: 'approve',
                editedItem: {
                    declinedReason: '',
                    approved: false,

                },
                defaultItem: {
                    declinedReason: '',
                    approved: false,
                },
                dialog: false,
                search: '',
                headers: [
                    {text: 'User ID', value: 'userId'},
                    {text: 'Price', value: 'cashOutPrice'},
                    {text: 'User Email', value: 'userEmail'},
                    {text: 'Date', value: 'cashOutTime'},
                    {text: 'Country', value: 'country'},
                    {text: 'Referrals', value: 'referCount'},
                    {text: 'Docs sold', value: 'soldDocument'},
                    {text: 'Ans Accept', value: 'correctAnswer'},
                    {text: 'Del Doc', value: 'soldDeletedDocument'},
                    {text: 'Del Answer', value: 'deletedCorrectAnswer'},
                    {text: 'Cashout', value: 'cashOut'},
                    {text: 'Award', value: 'awardCount'},
                    {text: 'Buy Points', value: 'buyCount'},
                    {text: 'Decline reason', value: 'declinedReason'},
                    {text: 'Approved', value: 'approved'},
                    {text: 'Actions', value: 'Actions'},

                ],
            }
        },
        computed: {
            disableDoneBtn() {
                return this.radios === 'decline' &&  !this.editedItem.declinedReason
            }
        },
        methods: {
            editItem(item) {
                this.editedIndex = this.cashOutList.indexOf(item);
                this.editedItem = Object.assign({}, item);
                this.dialog = true;
            },
            done(){
                if(this.radios === 'decline'){
                    this.decline()
                }else {
                    this.approveCashout();
                }
                this.dialog = false;
            },
            decline() {
                if (this.editedIndex > -1) {
                    this.editedItem.approved = null;
                    Object.assign(this.cashOutList[this.editedIndex], this.editedItem)
                }
                declineCashout(this.editedItem.transactionId, this.editedItem.declinedReason)
                    .then((success) => {
                        this.close();
                        this.$toaster.success(`cashout declined`);
                    });

                this.close()
            },
            approveCashout() {
                 let itemToAssign = {};
                 let self = this;
                 if (self.editedIndex > -1) {
                 itemToAssign = {
                    declinedReason: '',
                    approved: true
                    }
                }
                let transactionId = self.editedItem.transactionId;
                approveCashout(transactionId)
                    .then((success) => {
                            Object.assign(self.cashOutList[self.editedIndex], itemToAssign)
                            self.$toaster.success(`Cashout Approved`);

                        },
                        (error) => {
                            self.$toaster.error(`Error cant approve`);

                        }
                    )
            },
            close() {
                this.dialog = false;
                setTimeout(() => {
                    this.editedItem = Object.assign({}, this.defaultItem);
                    this.editedIndex = -1
                }, 300)
            },
        },
        created() {
            getCashoutList().then((list) => {
                if (list.length === 0) {
                    this.showNoResult = true;
                } else {
                    this.cashOutList = list;
                }
                this.showLoading = false;
            }, (err) => {
                console.log(err)
            })
        }
    }
</script>

<style lang="less">
    //overwrite vuetify css to narrow the table
    table.v-table tbody td:first-child, table.v-table tbody td:not(:first-child), table.v-table tbody th:first-child, table.v-table tbody th:not(:first-child), table.v-table thead td:first-child, table.v-table thead td:not(:first-child), table.v-table thead th:first-child, table.v-table thead th:not(:first-child) {
        padding: 0 4px !important;
    }

    /*table.v-table tbody td:first-child, table.v-table tbody td:not(:first-child), table.v-table tbody th:first-child, table.v-table tbody th:not(:first-child),*/
    /*table.v-table thead td:first-child, table.v-table thead td:not(:first-child), table.v-table thead th:first-child, table.v-table thead th:not(:first-child){*/
    /*padding: 0 4px!important;*/
    /*}*/
    .cashout-table-container {
        width: 100%;
        max-width: calc(100vw - 325px);

    }
</style>