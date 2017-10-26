<template>
    <div class="ma-4">
        <v-text-field 
                      label="Search" @input="$_search" debounce="500" 
                      class="input-group--focused"
                      single-line></v-text-field>
        <v-list v-if="items.length">
            <template v-for="item in items">
                <div @click="$_selected(item.id)"> <component :is="'search-item-'+type" :item="item" v-model="selectedItems"></component></div>
                <v-divider></v-divider>
            </template>
            
        </v-list><div v-else>
           <div>No Results Found</div>
            <div v-html="emptyText"></div>
        </div>
    </div>
</template>
<script>
    import { emptyStates } from './consts'
    const searchItemUniversity = () => import('./searchItemUniversity.vue');
    const searchItemCourse = () => import('./searchItemCourse.vue');
    export default {
        computed: {
            emptyText: function(){ return emptyStates[this.type] }
        },
        data() {
            return {
                selectedItems:[],
                items: []
            }
        },
        components: { searchItemUniversity, searchItemCourse},
        props: { type: {type:String,required:true},searchApi: { type: String, required: true }, params: { type: Object, default: () => { return {}} } },
        watch: {
            type: function (val) {
                this.items = [];
                this.$_search('');
            } },
        methods: {
            $_search(val) {
                if (!val.length||val.length > 3) {
                    this.$store.dispatch(this.searchApi, { ... this.params, term: val }).then(({ body }) => {
                        console.log(body)
                        this.items = body;
                    })
                }              
            },
            $_selected(val) {
                console.log('selected ' + val)
                this.$emit('selected',val)
            }
        },
        created() {
            this.$_search('')
        }
    }
</script>