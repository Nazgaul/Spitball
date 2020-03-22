<template>
    <div class="tutor-table-container">
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <h1 align="center">Tutor List</h1>
        <v-layout>
            <v-spacer></v-spacer>
            <v-flex xs3>
                <v-select :items="states"
                          label="state"
                          v-model="state"
                          @change="getTutors(country, state, search)"></v-select>
            </v-flex>   
            <v-flex xs3 offset-xs2>
                <v-select :items="countries"
                          label="country"
                          v-model="country"
                          @change="getTutors(country, state, search)"></v-select>
            </v-flex>
                <v-flex xs4 sm4 md4 offset-xs2>
                    <v-text-field v-model="search"
                                  append-icon="search"
                                  label="Search"
                                  single-line
                                  hide-details
                                  @keyup.enter.native="getTutors(country, state, search)">
                    </v-text-field>
                </v-flex>

</v-layout>

        <v-data-table :headers="headers"
                      :items="tutorList"
                      class="tutor-list-table"
                      disable-initial-sort>
            <template slot="items" slot-scope="props">
            
                <td class="text-xs-center">
                    <router-link :to="{name: `userConversations`, params: {userId: props.item.id}}">
                        {{ props.item.name }}
                    </router-link>
                </td>
                <td class="text-xs-center">
                    <router-link :to="{name: `userConversations`, params: {userId: props.item.id}}">
                        {{ props.item.email }}
                    </router-link>
                </td>
                <td class="text-xs-center">
                    <router-link :to="{name: `userConversations`, params: {userId: props.item.id}}">
                        {{ props.item.phoneNumber }}
                    </router-link>
                </td>
                <td class="text-xs-center">
                    <router-link :to="{name: `userConversations`, params: {userId: props.item.id}}">
                        {{ props.item.country }}
                     </router-link>   
                </td>
                <td class="text-xs-center">
                     <router-link :to="{name: `userConversations`, params: {userId: props.item.id}}">
                        {{ props.item.state }}
                     </router-link>   
                </td>

                 <td class="text-xs-center">
                     <router-link :to="{name: `userConversations`, params: {userId: props.item.id}}">
                        {{ props.item.created }}
                     </router-link>   
                </td>

                

            </template>
        </v-data-table>
    </div>
</template>

<script>
    import { getTutorList } from './tutorListService'
    import searchComponent from '../../helpers/search.vue'

    export default {
        data() {
            return {
                tutorList: [],
                showLoading: true,
                showNoResult: false,
                search: null,
                countries: ["All", "IL", "US", "IN"],
                country: 'All',
                states: ["All", "Pending", "Ok"],
                state: 'All',
                headers: [
                    { align: 'center', text: 'Name', value: 'name' },
                    { align: 'center', text: 'Email', value: 'email' },
                    { align: 'center', text: 'Phone Number', value: 'phoneNumber' },
                    { align: 'center', text: 'Country', value: 'county' },
                    { align: 'center', text: 'State', value: 'state' },
                    { align: 'center', text: 'Created', value: 'created' }
                ]
            }
        },
        methods: {
        getTutors(country, state, search) {
            getTutorList(country, state, this.search).then((list) => {
                if (list.length === 0) {
                    this.showNoResult = true;
                    this.tutorList = [];
                } else {
                    this.tutorList = list;
                    this.showNoResult = false;
                }
                this.showLoading = false;
            }, (err) => {
                console.log(err)
            })          
        }
        },
   
        created() {
            getTutorList('', this.state, this.search).then((list) => {
                if (list.length === 0) {
                    this.showNoResult = true;
                    
                } else {
                    this.tutorList = list;
                    this.showNoResult = false;
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
    .tutor-table-container {
        width: 100%;
        max-width: calc(100vw - 325px);
    }
    .tutor-list-table {

    }

    .user-inputs-container, .select-type-container {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        .user-input-text, .select-type-input

        {
            border: none;
            outline: none;
            border-radius: 25px;
            margin-top: 5px;
            padding: 10px;
            width: 345px;
        }
    }

    
</style>