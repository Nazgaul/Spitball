<template>
    <div class="cashout-table-container migration-table">
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <h4>New Universities List</h4>
        <h5>
            The table show new universities that been added to the system <br>
            Clicking on "MERGE" will merge the new university into the old university
        </h5>
        <v-layout>
            <v-spacer></v-spacer>
            <v-flex xs4 sm4 md4>
                <v-text-field v-model="search"
                              append-icon="search"
                              label="Search"
                              single-line
                              hide-details></v-text-field>
            </v-flex>
        </v-layout>

        <v-data-table :headers="headers"
                      :items="newUniversityList"
                      class="elevation-1 cash-out-table"
                      disable-initial-sort
                      :search="search">
            <template slot="items" slot-scope="props">
                <td class="text-xs-center">{{ props.item.oldUniversity }}</td>
                <td class="text-xs-center">{{ props.item.newUniversity }}</td>

                <td class="text-xs-center">
                    <span>
                        <v-btn flat
                               :disabled=disableBtn
                               @click="universityMigrate(props.item)">
                            Merge
                        </v-btn>
                    </span>
                </td>

            </template>
        </v-data-table>
    </div>
</template>

<script>
    import { getUniversityList, migrateUniversities } from './universityMigrationService'

    export default {
        data() {
            return {
                newUniversityList: [],
                showLoading: true,
                showNoResult: false,
                editedIndex: -1,
                disableBtn: false,
                search: '',
                headers: [
                    { text: 'Old University', value: 'oldUniversity' },
                    { text: 'New University', value: 'newUniversity' },

                ],
            }
        },
        methods: {
            universityMigrate(item) {
                const index = this.newUniversityList.indexOf(item);
                this.disableBtn = true;
                migrateUniversities(item.newId, item.oldId).then((resp) => {
                    console.log('got migration resp success')
                    this.$toaster.success(`University ${item.newUniversity} merged into ${item.oldUniversity}`);
                    this.newUniversityList.splice(index, 1);
                    this.disableBtn = false;
                },
                    (error) => {
                        console.log(error, 'error migration')
                    }
                )
            }
        },
        created() {
            getUniversityList().then((list) => {
                if (list.length === 0) {
                    this.showNoResult = true;
                } else {
                    this.newUniversityList = list;
                }
                this.showLoading = false;
            }, (err) => {
                console.log(err)
            })
        }
    }

</script>

<style lang="scss">
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
        &.migration-table

    {
        thead tr th

    {
        &:nth-child(odd)

    {
        background-color: green;
    }

    &:nth-child(even) {
        background-color: red;
    }

    }
    }
    }
</style>