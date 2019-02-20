<template>
    <div class="cashout-table-container">
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <h4>Cashout List</h4>
        <v-layout>
            <v-spacer></v-spacer>
            <v-flex xs4 sm4 md4>
                <v-text-field
                        v-model="minFlags"
                        append-icon="search"
                        label="Minimum Flagged"
                        single-line
                        hide-details
                ></v-text-field>
                <v-btn @click="getActiveList()">Get User list</v-btn>
            </v-flex>
        </v-layout>
        <v-data-table
                :headers="headers"
                :items="activeUsersList"
                :total-items="activeUsersTotal"
                hide-actions
                :pagination.sync="pagination"
                class="elevation-1 cash-out-table"
                disable-initial-sort

        >
            <template slot="items" slot-scope="props">
                <td class="text-xs-center">{{ props.item.userId }}</td>
                <td class="text-xs-center">{{ props.item.country }}</td>

            </template>
        </v-data-table>
        <div class="text-xs-center pt-2">
            <v-pagination v-model="pages" :length="activeUsersTotal /200"></v-pagination>
        </div>
    </div>
</template>
<script>
    import { getActiveUsers } from './activeUsersService'

    export default {
        data() {
            return {
                activeUsersList: [],
                activeUsersTotal: 0,
                page: 0,
                minFlags: null,
                showLoading: true,
                showNoResult: false,
                pages: 0,
                search: '',
                pagination: {},
                headers: [
                    {text: 'User ID', value: 'userId'},
                    {text: 'Country', value: 'country'},
                ],
            }
        },

        watch: {
            pagination: {
                handler() {
                    if (this.minFlags) {
                        this.getActiveList()
                            .then(data => {
                                this.activeUsersList = data.activeUsersList;
                                this.activeUsersTotal = data.total
                            })

                    }

                },
                deep: true
            },
        },
            methods: {
                nextPage() {
                    this.page++
                },

                getActiveList() {
                    let total, rowsPerPage, page, minFlags;
                    minFlags = this.minFlags;
                    rowsPerPage = 200;
                    page = this.page;
                    let self = this;
                    getActiveUsers(minFlags, page)
                        .then((data) => {
                            let activeList = data.flags;
                            if (data.rows !== -1) {
                                total = data.rows;
                            }
                            // activeList = activeList.slice((page - 1) * rowsPerPage, page * rowsPerPage);
                            self.showLoading = false;
                            let returnData = {
                                activeUsersList: activeList,
                                total: total
                            };
                            self.nextPage();
                            return returnData;


                        }, (err) => {
                            console.log(err)
                        })
                }
            },


        }

</script>

<style lang="scss">
    //overwrite vuetify css to narrow the table
    table.v-table tbody td:first-child, table.v-table tbody td:not(:first-child), table.v-table tbody th:first-child, table.v-table tbody th:not(:first-child),
    table.v-table thead td:first-child, table.v-table thead td:not(:first-child), table.v-table thead th:first-child, table.v-table thead th:not(:first-child) {
        padding: 0 4px;
    }

    .cashout-table-container {
        width: 100%;
        max-width: calc(100vw - 325px);

    }
</style>
