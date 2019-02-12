<template>
    <div>
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <h4>Cashout List</h4>
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
                class="elevation-1"
                :rows-per-page-items="[25, 50, 100,{text: 'All', value:-1}]"
                :search="search"
        >
            <template slot="items" slot-scope="props">
                <td class="text-xs-center">{{ props.item.userId }}</td>
                <td class="text-xs-center">{{ props.item.cashOutPrice }}</td>
                <td class="text-xs-center">{{ props.item.userEmail }}</td>
                <td class="text-xs-center">{{new Date(props.item.cashOutTime).toUTCString()}}</td>
                <td class="text-xs-center">{{ props.item.userQueryRatio }}</td>
                <td class="text-xs-center" :class="{'suspect': props.item.isSuspect}">
                    {{props.item.isSuspect ? "Yes" : "--"}}
                </td>
                <td class="text-xs-center">{{ props.item.isIsrael ? "Yes" : "--" }}</td>
                <td class="text-xs-center">{{ props.item.declinedReason }}</td>
                <td class="text-xs-center">{{ props.item.approved }}</td>
                <td class="text-xs-center">
                    <span v-if="!props.item.approved && !props.item.declinedReason">
                          <v-icon
                                  small
                                  color="red"
                                  class="mr-2"
                                  @click="editItem(props.item)"
                          >delete

                    </v-icon>
                    <v-icon
                            small
                            color="green"
                            @click="approveCashout(props.item)"
                    >
                        check
                    </v-icon>
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
                                <v-text-field v-model="editedItem.declinedReason" label="Decline Reason"></v-text-field>
                            </v-flex>
                            <!--<v-flex xs12 sm6 md4>-->
                            <!--<v-text-field v-model="editedItem.calories" label="Calories"></v-text-field>-->
                            <!--</v-flex>-->
                            <!--<v-flex xs12 sm6 md4>-->
                            <!--<v-text-field v-model="editedItem.fat" label="Fat (g)"></v-text-field>-->
                            <!--</v-flex>-->
                            <!--<v-flex xs12 sm6 md4>-->
                            <!--<v-text-field v-model="editedItem.carbs" label="Carbs (g)"></v-text-field>-->
                            <!--</v-flex>-->
                            <!--<v-flex xs12 sm6 md4>-->
                            <!--<v-text-field v-model="editedItem.protein" label="Protein (g)"></v-text-field>-->
                            <!--</v-flex>-->
                        </v-layout>
                    </v-container>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" flat @click="close">Cancel</v-btn>
                    <v-btn color="red darken-1" :disabled="!editedItem.declinedReason" flat @click="decline()">
                        Decline
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </div>
</template>
<script>
    import { getCashoutList, declineCashout, approveCashout, createCashoutItem } from './cashoutUserService'

    export default {
        data() {
            return {
                cashOutList: [],
                showLoading: true,
                showNoResult: false,
                editedIndex: -1,
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
                    {text: 'Cashout Price', value: 'cashoutPrice'},
                    {text: 'User Email', value: 'userEmail'},
                    {text: 'Date of cash out', value: 'cashDate'},
                    {text: 'User Query Ratio', value: 'userQueryRatio'},
                    {text: 'Is Suspect', value: 'isSuspect'},
                    {text: 'Is From Israel', value: 'isIsrael'},
                    {text: 'Decline reason', value: 'declinedReason'},
                    {text: 'Is Approved', value: 'approved'},
                    {text: 'Actions', value: 'Actions'},

                ],
            }
        },
        methods: {
            editItem(item) {
                this.editedIndex = this.cashOutList.indexOf(item);
                this.editedItem = Object.assign({}, item);
                this.dialog = true;
            },
            decline() {
                if (this.editedIndex > -1) {
                    Object.assign(this.cashOutList[this.editedIndex], this.editedItem)
                }
                declineCashout(this.editedItem.transactionId, this.editedItem.declinedReason)
                    .then((success) => {
                        this.close();
                        this.$toaster.success(`cashout declined`);
                    });

                this.close()
            },
            approveCashout(item) {
                let transactionId = item.transactionId;
                approveCashout(transactionId)
                    .then((success) => {
                            item.approved = true;
                            this.$toaster.success(`Cashout Approved`);

                        },
                        (error) => {
                            this.$toaster.error(`Error cant approve`);

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

<style lang="scss" scoped>
    .cashout-table-container {
        .cashout-table {
            margin: 0 auto;
            text-align: center;
            vertical-align: middle;
            width: 90%;
            td {
                border: 2px solid #b6b6b6;
                border-radius: 18px;
                &.suspect {
                    background-color: #ff9b9b;
                    font-weight: 600;
                }
            }
            th {
                border: 2px solid #b6b6b6;
                border-radius: 18px;
                background-color: #b6b6b6;
            }
        }

    }
</style>
