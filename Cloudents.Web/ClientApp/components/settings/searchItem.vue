<template>
    <div class="pa-4">
        <v-text-field 
                      label="Search" @input="$_search" debounce="500" 
                      class="input-group--focused"
                      single-line></v-text-field>
        <slot name="options">
            <v-container class="pa-0 mb-3">
                <v-layout row>
                    <radio-list class="search" :values="currentItem.filters"  model="filter" v-model="filter"  :value="currentItem.defaultFilter"></radio-list>
                    <template v-for="act in currentItem.actions">
                        <v-flex @click="actionsCallback(act.id)" v-if="act.component"><component :is="act.component"></component></v-flex>
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
    </div>
</template>
<script>
    const RadioList = () => import('./../helpers/radioList.vue');
    const plusButton = () => import('./svg/plus-button.svg');
    import { emptyStates, filtersAction } from './consts'
    const searchItemUniversity = () => import('./searchItemUniversity.vue');
    const searchItemCourse = () => import('./searchItemCourse.vue');
    import { mapGetters } from 'vuex'

    export default {
        model: {
            prop: 'value',
            event: 'selected'
        },
        computed: {
            ...mapGetters(['myCoursesId']),
            currentItem: function () { return filtersAction[this.type]},
            emptyText: function () { return emptyStates[this.type] },
            filterItems: function () { return this.filter === 'myCourses' ? this.items.filter((i) => this.myCoursesId.length && this.myCoursesId.includes(i.id)):this.items}
        },
        data() {
            return {
                items: [],
                filter: 'all',
                isLoading: true,
                isChanged:false
            }
        },

        components: { searchItemUniversity, searchItemCourse, RadioList, plusButton},
        props: { extraItem: {type:[Object,String]}, actionsCallback: {type:Function},  type: {type:String,required:true},searchApi: { type: String, required: true }, params: { type: Object, default: () => { return {}} } },
        watch: {
            type: function (val) {
                this.items = [];
                this.filteredItems=[]
                this.isChanged = true;
                this.$_search('');
            },
            extraItem: function (val) {
                this.items = [... this.items, val];
            }
        },
        methods: {
            $_search(val) {
                console.log('search bobobobobobo')
                if (!val.length || val.length > 3) {
                    this.isLoading = true;
                    this.$store.dispatch(this.searchApi, { ... this.params, term: val }).then(({ body }) => {
                        this.items = body;
                        this.filteredItems = body;
                        this.isLoading=false
                    })
                }              
            },
            $_selected(val) {
                if (this.isChanged) { this.isChanged = false; return; }
            },

            $_updateFilter(val) {
                this.filteredItems = (val == 'all') ? this.items : this.items.filter((i) => this.myCoursesId.length || this.myCoursesId.includes(i.id))
            }
        },
        mounted() {
            console.log('search mounted')
            this.$_search('')
        }
    }
</script>