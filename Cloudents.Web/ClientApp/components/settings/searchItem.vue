<template>
    <component is="slot">
        <!--class="white"  -->
        <!--class="input-group--focused"-->
        <v-dialog v-model="dialog" max-width="500px" content-class="dialog-choose" v-if="currentItem">
            <div class="d-header">
                <v-layout row>
                    <v-flex>
                        <button type="button" @click="dialog=false">
                            <close-button></close-button>
                        </button>
                    </v-flex>
                    <v-flex>
                        <h5>{{currentItem.title}}</h5>
                    </v-flex>
                </v-layout>
                <v-text-field label="Search" @input="$_search" ref="searchText" v-debounce="500"
                              single-line></v-text-field>
                <v-container class="pa-0 mb-3" v-if="currentItem.filters">
                    <v-layout row>
                        <radio-list class="search" :values="currentItem.filters" model="filter" v-model="filter" :value="currentItem.defaultFilter"></radio-list>
                        <template v-for="act in currentItem.actions">
                            <v-flex @click="$_actionsCallback(act.id)" v-if="act.component"><component :is="act.component"></component></v-flex>
                        </template>
                    </v-layout>
                </v-container>
            </div>
            <div class="loader" v-if="isLoading">
                <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
            </div>
            <div class="d-result" v-else>
                <v-list v-if="items.length">
                    <template v-for="(item,index) in filterItems">
                        <div @click="$_clickItemCallback(keep)">
                            <component :is="'search-item-'+type" :item="item"></component>
                        </div>
                        <v-divider v-if="index < filterItems.length-1"></v-divider>
                    </template>
                </v-list><div v-else>
                    <div>No Results Found</div>
                    <div v-html="emptyText"></div>
                </div>
            </div>
        </v-dialog>
        <v-dialog v-model="showActionsDialog" v-if="currentItem" persistent>
            <div class="white pa-2" v-for="action in currentItem.actions" :key="action.id">
                <component :is="type+'-'+action.id" v-if="currentAction==action.id" @done="$_actionDone"></component>
            </div>
        </v-dialog>
    </component>
</template>
<script>
    import debounce from 'v-debounce'
    const RadioList = () => import('./../helpers/radioList.vue');
    const plusButton = () => import('./svg/plus-button.svg');
    const closeButton = () => import('./svg/close-icon.svg');
    import { searchObjects } from './consts'
    const searchItemUniversity = () => import('./searchItemUniversity.vue');
    const searchItemCourse = () => import('./searchItemCourse.vue');
    import { mapGetters } from 'vuex'
    import CourseAdd from './courseAdd.vue'
    import VDialog from "vuetify/src/components/VDialog/VDialog";

    export default {
        model: {
            prop: 'value',
            event: 'change'
        },
        directives: {
            debounce
        },
        watch:{
            type(val){
               this.$refs.searchText?this.$refs.searchText.inputValue='':this.$_search('')
            }
        },
        computed: {
            dialog: {
                get() {
                    return this.value;
                }, set(val) {
                    this.$emit('change', val)
                }
            },
            ...mapGetters(['myCoursesId']),
            showActionsDialog:{
                get(){return this.currentAction},set(val){}},
            currentItem: function () {
                    return searchObjects[this.type]
                },
                emptyText: function () { return this.currentItem.emptyState },
                filterItems: function () { return this.filter === 'myCourses' ? this.items.filter((i) => this.myCoursesId.length && this.myCoursesId.includes(i.id)) : this.items }
            },
            data() {
                return {
                    items: [],
                    filter: 'all',
                    isLoading: true,
                    isChanged: false,
                    currentAction: ""
                }
            },

            components: {
                CourseAdd, VDialog,
                searchItemUniversity, searchItemCourse, RadioList, plusButton, closeButton
            },
            props: { type: { type: String, required: true }, value: { type: Boolean },keep:{type:Boolean} },
            methods: {
                $_clickItemCallback(keep) {
                    this.currentItem.click ? this.currentItem.click.call(this,keep) : ''
                },
                $_actionDone(val) {
                    this.items = [... this.items, val];
                    this.currentAction = "";
                },
                $_actionsCallback(action) {
                    this.currentAction = action;
                },
                $_search(val) {
                    if (!val.length || val.length > 1) {
                        this.isLoading = true;
                        this.$store.dispatch(this.currentItem.searchApi, { term: val }).then(({ body }) => {
                            this.items = body;
                            this.filteredItems = body;
                            this.isLoading = false
                        })
                    }
                },

                $_updateFilter(val) {
                    this.filteredItems = (val === 'all') ? this.items : this.items.filter((i) => this.myCoursesId.length || this.myCoursesId.includes(i.id))
                }
            }
        }
</script>
<style src="./searchItem.less" lang="less"></style>



<!--<v-card>
                <v-card-title>
                    <v-flex xs12 class="headline">{{currentItem.title}}</v-flex>
                    <v-flex xs12>
                        <v-text-field label="Search" @input="$_search" v-debounce="500"
                                      class="input-group--focused"
                                      single-line></v-text-field>
                    </v-flex>
                    <v-container class="pa-0 mb-3" v-if="currentItem.filters">
                        <v-layout row>
                            <radio-list class="search" :values="currentItem.filters" model="filter" v-model="filter" :value="currentItem.defaultFilter"></radio-list>
                            <template v-for="act in currentItem.actions">
                                <v-flex @click="$_actionsCallback(act.id)" v-if="act.component"><component :is="act.component"></component></v-flex>
                            </template>
                        </v-layout>
                    </v-container>
                </v-card-title>
                <v-divider></v-divider>
                <v-card-text style="height: 300px;">
                    <div class="loader" v-if="isLoading">
                        <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
                    </div>
                    <div v-else>
                        <v-list v-if="items.length">
                            <template v-for="(item,index) in filterItems">
                                <div @click="$_selected({id:item.id,name:item.name})"> <component :is="'search-item-'+type" :item="item"></component></div>
                                <v-divider v-if="index < filterItems.length-1"></v-divider>
                            </template>
                        </v-list><div v-else>
                            <div>No Results Found</div>
                            <div v-html="emptyText"></div>
                        </div>
                    </div>
                </v-card-text>
                <v-divider></v-divider>
                <v-card-actions>
                    <v-btn color="blue darken-1" flat @click.native="dialog = false">Close</v-btn>
                    <v-btn color="blue darken-1" flat @click.native="dialog = false">Save</v-btn>
                </v-card-actions>
            </v-card>-->
