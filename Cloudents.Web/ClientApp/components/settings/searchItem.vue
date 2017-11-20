<template>
    <component :is="(isDialog?'slot':'general-page')" :title="currentItem.title">
    <div class="pa-4" slot="data">


        <v-text-field 
                      label="Search" @input="$_search" v-debounce="500"
                      class="input-group--focused"
                      single-line></v-text-field>
        <slot name="options" >
            <v-container class="pa-0 mb-3">
                <v-layout row>
                    <radio-list class="search" :values="currentItem.filters"  model="filter" v-model="filter"  :value="currentItem.defaultFilter"></radio-list>
                    <template v-for="act in currentItem.actions">
                        <v-flex @click="$_actionsCallback(act.id)" v-if="act.component"><component :is="act.component"></component></v-flex>
                    </template>  
                </v-layout>
            </v-container>
        </slot>
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
        <v-dialog v-model="showActionsDialog">
            <div class="white pa-2" v-for="action in currentItem.actions" :key="action.id">
                <component :is="type+'-'+action.id" v-if="currentAction==action.id" @done="$_actionDone"></component>
            </div>
        </v-dialog>

    </div>
    </component>
</template>
<script>
    import debounce from 'v-debounce'
    const RadioList = () => import('./../helpers/radioList.vue');
    const plusButton = () => import('./svg/plus-button.svg');
    import { emptyStates, filtersAction } from './consts'
    const searchItemUniversity = () => import('./searchItemUniversity.vue');
    const searchItemCourse = () => import('./searchItemCourse.vue');
    import { mapGetters } from 'vuex'
    import CourseAdd from './courseAdd.vue'
    import VDialog from "vuetify/src/components/VDialog/VDialog";

    export default {
        model: {
            prop: 'value',
            event: 'selected'
        },
        directives: {
            debounce
        },
        computed: {
            ...mapGetters(['myCoursesId']),
            showActionsDialog:{
                get(){return this.currentAction},set(val){}},
            isDialog(){
                return this.$attrs.isDialog},
            currentItem: function () { return filtersAction[this.type]},
            emptyText: function () { return emptyStates[this.type] },
            filterItems: function () { return this.filter === 'myCourses' ? this.items.filter((i) => this.myCoursesId.length && this.myCoursesId.includes(i.id)):this.items}
        },
        data() {
            return {
                items: [],
                filter: 'all',
                isLoading: true,
                isChanged:false,
                currentAction:""
            }
        },

        components: {
            VDialog,CourseAdd,
            searchItemUniversity, searchItemCourse, RadioList, plusButton},
        props: { type: {type:String,required:true},searchApi: { type: String, required: true }, selectCallback:{type:Function} },
        watch: {
            type: function (val) {
                this.items = [];
                this.filteredItems=[]
                this.isChanged = true;
                this.$_search('');
            }
        },
        methods: {
            $_actionDone(val){
                this.items = [... this.items, val];
                this.currentAction="";
            },
            $_actionsCallback(action){
                this.currentAction=action;
            },
            $_search(val) {
                if (!val.length || val.length > 1) {
                        this.isLoading = true;
                        this.$store.dispatch(this.searchApi, {term: val}).then(({body}) => {
                            this.items = body;
                            this.filteredItems = body;
                            this.isLoading = false
                        })
                }              
            },
            $_selected(val) {
                if (this.isChanged) { this.isChanged = false; }
                else if(this.selectCallback)this.selectCallback.call(this);
            },

            $_updateFilter(val) {
                this.filteredItems = (val === 'all') ? this.items : this.items.filter((i) => this.myCoursesId.length || this.myCoursesId.includes(i.id))
            }
        },
        mounted() {
            console.log('mount search0');
            this.$_search('')
        }
    }
</script>