<template>
    <div class="cashout-table-container">
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <h1 align="center">Pending Universities List</h1>
        <v-layout>
            <v-spacer></v-spacer>
            <v-flex xs3>
                <v-select :items="states"
                          label="state"
                          v-model="state"
                          @change="getUniversityList(language, state)"></v-select>
            </v-flex>
            <v-flex xs3 offset-xs2>
                <v-select :items="countries"
                          label="country"
                          v-model="country"
                          @change="getUniversityList(country, state)"></v-select>
            </v-flex>

            <v-flex xs4 sm4 md4 offset-xs2>
                    <v-text-field v-model="search"
                                  append-icon="search"
                                  label="Search"
                                  single-line
                                  hide-details></v-text-field>
                    
                </v-flex>
        </v-layout>

        <v-data-table :headers="headers"
                      :items="newUniversitiesList"
                      class="cash-out-table"
                      disable-initial-sort
                      :search="search">
            <template slot="items" slot-scope="props">
                <td class="text-xs-center">{{ props.item.name }}</td>

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

        <v-dialog persistent v-model="dialog" max-width="500px">
            <v-card>
                <v-card-title>
                    <span v-show="radios === 'merge'" class="headline">{{ editedItem.name }}</span>
                </v-card-title>

                <v-card-text>
                    <v-container grid-list-md>
                        <v-layout wrap>
                            <v-flex xs12>
                                <v-radio-group v-model="radios">
                                    <v-radio label="Rename" value="rename"></v-radio>
                                    <v-radio label="Delete" value="delete"></v-radio>
                                    <v-radio label="Approve" value="approve"></v-radio>
                                    <v-radio label="Merge" value="merge"></v-radio>
                                </v-radio-group>

                                <!--<div class="select-type-container">
        <v-select v-show="radios === 'merge'"
                  class="select-type-input"
                  solo
                  v-model="picked"
                  :items="suggestUniversities"
                  item-text="name"
                  item-value="id"
                  label="Select university"
                  :disabled="disableSelectBtn"></v-select>
    </div>-->
                                <search-Component :context="AdminAPI" :contextCallback="setAdminAPI" :searchValue="picked" :callback="setSearchValue" v-show="radios === 'merge'"> </search-Component>
                                <v-text-field :label="editedItem.name"
                                single-line
                                v-show="radios === 'rename'"
                                v-model ="newName">

                                </v-text-field>
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
    import { getUniversitiesList, /*getSuggestions,*/ migrateUniversities, approve, rename, deleteUniversity } from './universityPendingService'
    import searchComponent from '../../helpers/search.vue'

    export default {
        data() {
            return {
                newUniversitiesList: [],
                //suggestUniversities: [],
                picked: {
                    id: '',
                    name: ''
                },
                AdminAPI: 'University',
                showLoading: true,
                showNoResult: false,
                disableSelectBtn: true,
                editedIndex: -1,
                radios: 'approve',
                search: '',
                countries: ["all","il", "in", "us"],
                country: '',
                states: ["Pending", "Ok"],
                state: 'Pending',
                newName: '',
                editedItem: {
                    course: '',
                },
                newItem: {
                    course: '',
                },
                defaultItem: {
                    course: '',
                },
                dialog: false,
                headers: [
                    { text: 'Pending Universities', value: 'name' },
                    { text: 'Actions', value: 'actions' },

                ],
            }
        },
        computed: {
            disableDoneBtn() {
                return this.radios === 'merge' && !this.picked || this.radios === 'rename' && this.newName.length < 4;
            }
        },
        watch: {
            radios(newVal, oldVal) {
                if (newVal === "rename") {
                    this.newName = this.editedItem.name;
                }
            }
        },
        methods: {
            setSearchValue(searchValue) {
                this.picked = searchValue;
            },
            setAdminAPI(context) {
                this.AdminAPI = context;
            },
            editItem(item) {
                this.editedIndex = this.newUniversitiesList.indexOf(item);
                this.editedItem = item;
                this.dialog = true;
            },
            done() {
                if (this.radios === 'merge') {
                    this.universityMigrate({ "uniToRemove": this.editedItem, "uniToKeep": this.picked })
                } else if (this.radios === 'approve') {
                    this.approve(this.editedItem);
                } else if (this.radios === 'rename') {
                    this.rename(this.editedItem, this.newName);
                } else {
                    this.deleteUniversity(this.editedItem);
                }
                this.dialog = false;
                this.setSearchValue({});
                this.newName = '';
            },
            close() {
                this.dialog = false;
                this.editedItem = this.defaultItem;
                this.editedIndex = -1;
                this.radios = 'approve';
                this.disableSelectBtn = true;
                this.setSearchValue({});
                this.newName = '';
            },
            universityMigrate(item) {
                const index = this.newUniversitiesList.indexOf(item.uniToRemove);
                migrateUniversities(item.uniToRemove.id, item.uniToKeep.id).then((resp) => {
                    console.log('got migration resp success')

                    this.$toaster.success(`University ${item.uniToRemove.name} merged into ${item.uniToKeep.name}`);
                    this.newUniversitiesList.splice(index, 1);
                    this.disableSelectBtn = true;
                },
                    (error) => {
                        this.$toaster.error(`Error can't merge`);
                    }
                )
            },
            approve(item) {
                const index = this.newUniversitiesList.indexOf(item);
                approve(item).then((resp) => {
                    console.log('got migration resp success')
                    this.$toaster.success(`Approved Course ${item.name}`);
                    this.newUniversitiesList.splice(index, 1);
                },
                    (error) => {
                        this.$toaster.error(`Error can't Approve`);
                    }
                )
            },
            rename(university, newName) {
                const index = this.newUniversitiesList.indexOf(university);
                rename(university.id, newName).then((resp) => {
                    console.log('got rename resp success')
                    this.$toaster.success(`Rename Course ${university.name} to ${newName}`);
                    this.newUniversitiesList.splice(index, 1);
                },
                    (error) => {
                        this.$toaster.error(`Error can't Rename`);
                    }
                )
            },
            deleteUniversity(item) {
                const index = this.newUniversitiesList.indexOf(item);
                deleteUniversity(item).then((resp) => {
                    console.log('got migration resp success')
                    this.$toaster.success(`Deleted Course ${item.name}`);
                    this.newUniversitiesList.splice(index, 1);
                },
                    (error) => {
                        this.$toaster.error(`Error can't Delete`);
                    }
                )
            },
            /*getUniversitiesSuggestions(item) {
                getSuggestions(item).then((list) => {
                    if (list.length > 0) {
                        this.suggestUniversities = list;
                        this.disableSelectBtn = false;
                    }
                }, (err) => {
                    console.log(err)
                });
            },*/
            getUniversityList(country, state) {
                getUniversitiesList(country, state).then((list) => {
                    this.newUniversitiesList = [];
                    if (list.length === 0) {
                        this.showNoResult = true;
                    } else {
                        this.newUniversitiesList = list;
                    }
                    this.showLoading = false;
                }, (err) => {
                    console.log(err)
                })
            }
        },
        created() {
            getUniversitiesList('', 'Pending').then((list) => {
                if (list.length === 0) {
                    this.showNoResult = true;
                } else {
                    this.newUniversitiesList = list;
                }
                this.showLoading = false;
            }, (err) => {
                console.log(err)
                })
        },
        components: {
            searchComponent
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

    .cash-out-table {
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