<template>
    <div class="cashout-table-container">
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <h1 align="center">Pending Courses List</h1>
        <v-layout>
            <v-spacer></v-spacer>
            <v-flex xs3>
                <v-select :items="states"
                          label="state"
                          v-model="state"
                          @change="getCourseList(language, state, search)"></v-select>
            </v-flex>   
            <v-flex xs3 offset-xs2>
                <v-select :items="languages"
                          label="language"
                          v-model="language"
                          @change="getCourseList(language, state, search)"></v-select>
            </v-flex>
                <v-flex xs4 sm4 md4 offset-xs2>
                    <v-text-field v-model="search"
                                  append-icon="search"
                                  label="Search"
                                  single-line
                                  hide-details
                                  @keyup.enter.native="getCourseList(language, state, search)">
                    </v-text-field>
                </v-flex>

</v-layout>

        <v-data-table :headers="headers"
                      :items="newCourseList"
                      class="cash-out-table"
                      disable-initial-sort>
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
                    <span v-show="radios === 'merge' || radios === 'rename'" class="headline">{{ editedItem.name }}</span>
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
                                <!--<v-text-field v-show="radios === 'merge'" v-model="newItem.course" label="New course"></v-text-field>-->

                                <template v-if="radios === 'approve'">
                                    <div class="select-type-container">
                                        <v-select
                                            class="select-type-input"
                                            v-model="subject"
                                            :items="subjects"
                                            :loading="isLoading"
                                            label="Select subjects"
                                        ></v-select>
                                        <v-select 
                                            class="select-type-input"
                                            v-model="schoolType"
                                            :items="schoolTypes"
                                            label="Select school type"
                                        ></v-select>
                                    </div>
                                </template>

                                <search-Component :context="adminAPI" 
                                                  :contextCallback="setadminAPI" 
                                                  :searchValue.name="picked" 
                                                  :callback="setSearchValue"
                                                  v-show="radios === 'merge'">
                                </search-Component>
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
    import { getCourseList, getSubjects, migrateCourses, approve, rename, deleteCourse } from './coursesPendingService'
    import searchComponent from '../../helpers/search.vue'

    export default {
        data() {
            return {
                isLoading: true,
                newCourseList: [],
                subjects: [],
                subject: '',
                picked: {
                    id: '',
                    name: ''
                },
                adminAPI: 'Course',
                showLoading: true,
                showNoResult: false,
                editedIndex: -1,
                radios: 'delete',
                search: null,
                languages: ["All", "He", "En"],
                language: 'All',
                states: ["Pending", "Ok"],
                state: 'Pending',
                newName: '',
                editedItem: {
                    course: '',
                },
                defaultItem: {
                    course: '',
                },
                dialog: false,
                headers: [
                    { text: 'Pending Courses', value: 'name' },
                    { text: 'Actions', value: 'actions' },
                ],
                schoolTypes: ['University', 'Highschool'],
                schoolType: '',
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
                this.picked.name = searchValue.name;
                this.picked.id = searchValue.name;
            },
            setadminAPI(context) {
                this.adminAPI = context;
            },
            editItem(item) {
                this.editedIndex = this.newCourseList.indexOf(item);
                this.editedItem = item;
                this.dialog = true;
            },
            done() {
                if (this.radios === 'merge') {
                    this.courseMigrate({ "newCourse": this.editedItem.name, "oldCourse": this.picked.name })
                } else if (this.radios === 'approve') {
                    this.approve({ "name": this.editedItem, "subject": this.subject});
                } else if (this.radios === 'rename') {
                    this.rename(this.editedItem, this.newName);
                }
                else {
                    this.deleteCourse(this.editedItem);
                }
                this.dialog = false;
                this.setSearchValue({ "name": '' });
                this.newName = '';
            },
            close() {
                this.dialog = false;
                this.editedItem = this.defaultItem;
                this.editedIndex = -1;
                this.radios = 'approve';
                this.setSearchValue({ "name": '' });
                this.newName = '';
            },
            courseMigrate(item) {
                const index = this.newCourseList.indexOf(item);
                migrateCourses(item.newCourse, item.oldCourse).then((resp) => {
                    console.log('got migration resp success')

                    this.$toaster.success(`Course ${item.newCourse} merged into ${item.oldCourse}`);
                    this.newCourseList.splice(index, 1);
                },
                    (error) => {
                        this.$toaster.error(`Error can't merge`);
                    }
                )
            },
            approve(item) {
                const index = this.newCourseList.indexOf(item.name);
                let schoolType = this.schoolType;
                approve({course: item, schoolType}).then((resp) => {
                    console.log('got migration resp success')
                    this.$toaster.success(`Approved Course ${item.name.name}`);
                    this.newCourseList.splice(index, 1);
                },
                    (error) => {
                        this.$toaster.error(`Error can't Approve`);
                    }
                )
            },
            rename(course, newName) {
                const index = this.newCourseList.indexOf(course);
                rename(course.name, newName).then((resp) => {
                    console.log('got rename resp success')
                    this.$toaster.success(`Rename Course ${course.name} to ${newName}`);
                    this.newCourseList.splice(index, 1);
                },
                    (error) => {
                        this.$toaster.error(`Error can't Rename`);
                    }
                )
            },
            deleteCourse(item) {
                const index = this.newCourseList.indexOf(item);
                deleteCourse(item).then((resp) => {
                    console.log('got migration resp success')
                    this.$toaster.success(`Deleted Course ${item.name}`);
                    this.newCourseList.splice(index, 1);
                },
                    (error) => {
                        this.$toaster.error(`Error can't Delete`);
                    }
                )
            },
   
            getCourseList(language, state, filter) {
                getCourseList(language, state, filter).then((list) => {
                    this.newCourseList = [];
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
        },
        created() {
            getCourseList('', 'Pending', this.search).then((list) => {
                if (list.length === 0) {
                    this.showNoResult = true;
                } else {
                    this.newCourseList = list;
                }
                this.showLoading = false;
            }, (err) => {
                console.log(err)
            })
                .then(getSubjects().then((list) => {
                    if (list.length > 0) {
                        this.subjects = list;
                        this.isLoading = false;
                    }
                }, (err) => {
                    console.log(err)
                    })
                )
           
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