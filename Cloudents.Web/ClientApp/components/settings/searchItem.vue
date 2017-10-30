<template>
    <div class="pa-4">
        <v-text-field 
                      label="Search" @input="$_search" debounce="500" 
                      class="input-group--focused" v-model="term"
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
                    <div @click="$_selected({id:item.id,name:item.name})"> <component :is="'search-item-'+type" :item="item" v-model="selectedItems"></component></div>
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
            ...mapGetters(['myCourses']),
            currentItem: function () { return filtersAction[this.type]},
            emptyText: function () { return emptyStates[this.type] },
            filterItems: function () { return this.filter === 'myCourses' ? this.items.filter((i) => this.myCourses.length && this.myCourses.includes(i.id)):this.items}
        },
        data() {
            return {
                selectedItems: this.myCourses || [],
                items: [],
                term: '',
                filter: 'all',
                isLoading: true,
                isChanged:false
            }
        },

        components: { searchItemUniversity, searchItemCourse, RadioList, plusButton},
        props: { extraItem: {type:[Object,String]}, actionsCallback: {type:Function}, defaultVals: {type:[Array,String]}, type: {type:String,required:true},searchApi: { type: String, required: true }, params: { type: Object, default: () => { return {}} } },
        watch: {
            type: function (val) {
                this.items = [];
                this.filteredItems=[]
                this.term = '';
                console.log(this.selectedItems);
                this.isChanged = true;
                this.$_search('');
            },
            extraItem: function (val) {
                this.items = [... this.items, val];
                this.selectedItems = [... this.selectedItems, val]
                this.$emit('selected', this.selectedItems)
            }
        },
        methods: {
            $_search(val) {
                if (!val.length || val.length > 3) {
                    this.isLoading = true;
                    this.$store.dispatch(this.searchApi, { ... this.params, term: val }).then(({ body }) => {
                        console.log(body)
                        this.items = body;
                        this.filteredItems = body;
                        this.isLoading=false
                    })
                }              
            },
            $_selected(val) {
                if (this.isChanged) { this.isChanged = false; return; }
                this.selectedItems.find((item) => val.id == item.id) ? this.selectedItems = this.selectedItems.filter(e=>e.id!=val.id) : this.selectedItems.push(val)
                    console.log('selected ' + val)
                    this.$emit('selected', this.selectedItems)
            },

            $_updateFilter(val) {
                this.filteredItems = (val == 'all') ? this.items : this.items.filter((i) => this.myCourses.length || this.myCourses.includes(i.id))
            }
        },
        mounted() {
            this.selectedItems = this.defaultVals ? this.defaultVals : this.myCourses;
            this.$emit('selected', [])
            this.$_search('')
        }
    }
</script>