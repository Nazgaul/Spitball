<template>
    <div class="cashout-table-container">
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <h4>New Courses List</h4>
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
                      :items="newCourseList"
                      class="elevation-1 cash-out-table"
                      disable-initial-sort
                      :search="search">
            <template slot="items" slot-scope="props">
                <td class="text-xs-center">{{ props.item.oldCourse }}</td>
                <td class="text-xs-center">{{ props.item.newCourse }}</td>
               
                <td class="text-xs-center">
                     <span>             
                         <v-btn flat
                                @click="courseMigrate(props.item)">
                             Merge
                         </v-btn>
                     </span>
                </td>

            </template>
        </v-data-table>
    </div>
</template>

<script>
    import { getCourseList, migrateCourses } from './courseMigrationService'

    export default {
        data() {
            return {
                newCourseList: [],
                showLoading: true,
                showNoResult: false,
                editedIndex: -1,

                search: '',
                headers: [
                    { text: 'Old Course', value: 'oldCourse' },
                    { text: 'New Course', value: 'newCourse' },

                ],
            }
        },
        methods: {
            courseMigrate(item) {
                migrateCourses(item).then((resp) => {
                    console.log('got migration resp success')
                },
                    (error) => {
                        console.log(error, 'error migration')
                    }
                )
            }
        },
        created() {
            getCourseList().then((list) => {
                if (list.length === 0) {
                    this.showNoResult = true;
                } else {
                    this.newCourseList = list;
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
    }

</style>