<template>
    <div class="active-users-container">
        <h1 align="center">Active helping users list</h1>
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
            </v-flex>
        </v-layout>
        <v-data-table
                :headers="headers"
                :items="activeUsersList"
                :pagination.sync="pagination"
                :loading="loading"
                class="active-users-table">
            <v-progress-linear slot="progress" color="blue" v-show="loading" indeterminate></v-progress-linear>
            <template slot="items" slot-scope="props">
                <td class="text-xs-left cursor-pointer" @click="openUserView(props.item.userId)">{{ props.item.userId }}</td>
                <td class="text-xs-left">{{ props.item.country }}</td>
                <td class="text-xs-left">{{ props.item.flags }}</td>
            </template>
        </v-data-table>
    </div>
</template>
<script>
    import { getActiveUsers } from './activeUsersService'

    export default {
        data() {
            return {
                activeUsersList: [],
                activeUsersTotal: 0,
                serverPage: 0,
                loading: true,
                minFlags: 1,
                showLoading: true,
                showNoResult: false,
                pages: 0,
                search: '',
                pagination: {},
                headers: [
                    {text: 'User ID', value: 'userId'},
                    {text: 'Country', value: 'country'},
                    {text: 'Flags', value: 'flags'},
                ],
            }
        },

        watch: {
            pagination: {
                handler() {
                    this.getActiveList()
                        .then(data => {
                            this.activeUsersList = data.items;
                            this.activeUsersTotal = data.total;

                        });
                },
                deep: true
            },
            minFlags: {
                handler(newVal) {
                    if (newVal) {
                        this.getActiveList()
                            .then(data => {
                                this.activeUsersList = data.items;
                                this.activeUsersTotal = data.total;

                            });
                    }
                }
            }

        },
        methods: {
            openUserView(userId){
                this.$router.push({name: 'userMainView', params: {userId: userId}})
            },
            nextPage() {
                this.serverPage++
            },
            getActiveList() {
                this.loading= true;
                let total, rowsPerPage, serverPage, minFlags;
                minFlags = this.minFlags;
                rowsPerPage = 200;
                serverPage = this.serverPage;
                let sortBy = '';
                let descending = false;
                let items;
                let currentPage;
                return getActiveUsers(minFlags, serverPage)
                    .then((data) => {
                        items = data.flags;
                        if (data.rows !== -1) {
                            total = data.rows;
                            if (total > 200) {
                                this.nextPage()
                            }
                        }
                        if (rowsPerPage > 0) {
                            if (this.serverPage === 0) {
                                currentPage = 1;
                            } else {
                                currentPage = this.serverPage + 1;
                            }
                            items = items.slice((currentPage - 1) * rowsPerPage, currentPage * rowsPerPage);
                        }
                        this.loading = false;
                        return {items, total};
                    }, (err) => {
                        console.log(err)
                    })
            }
        },


    }

</script>

<style lang="less">
    //overwrite vuetify css to narrow the table
    table.v-table tbody td:first-child, table.v-table tbody td:not(:first-child), table.v-table tbody th:first-child, table.v-table tbody th:not(:first-child),
    table.v-table thead td:first-child, table.v-table thead td:not(:first-child), table.v-table thead th:first-child, table.v-table thead th:not(:first-child) {
        padding: 0 4px;
    }
    .active-users-container {
        width: 100%;
        max-width: calc(100vw - 325px);
        .cursor-pointer{
            cursor: pointer;
        }
    }
</style>
