<template>
    <div>
        <v-snackbar color="orange" xs12
                    left
                    top v-model="snackbar" :timeout="7000">
            <v-layout row wrap>
                You can tailored the results to you by adding your school and classes.
                <v-btn offset-xs2 xs2 flat color="white" @click.stop="showDialog=true;snackbar=false;">Yes, I want</v-btn>
                <v-btn xs1 flat color="white" @click="snackbar=false">X</v-btn>
            </v-layout>
        </v-snackbar>
        <v-dialog v-model="showDialog" max-width="500">
            <div class="white">
                <v-flex xs12> <h5>{{currentItem.title}}</h5> <v-btn color="primary" flat @click.stop="(!getUniversity?showDialog=false:$_done())">{{currentItem.closeText}}</v-btn></v-flex>
                <search-item :extraItem="extraItem" :searchApi="currentItem.searchApi" :actions.sync="showActions" :actionsCallback="$_updateActions" :type="searchType" :params="params" v-model="selectedItems" :defaultVals="selectedItems">

                </search-item>
            </div>
        </v-dialog>
        <v-dialog v-model="showActions" v-if="showActions" max-width="350" persistent>
            <div class="white">
                <component :is="searchType+'-'+currentAction" @done="$_actionDone"></component>
            </div>
        </v-dialog>
    </div>
</template>
<script>
    import searchItem from './searchItem.vue'
    import courseAdd from './courseAdd.vue'
    import { searchObjects} from './consts'
    import { mapGetters,mapActions } from 'vuex'
    export default {
        data() {
            return { showDialog: false, extraItem:'',snackbar: true, selectedItems: this.myCourses, showActions:false,currentAction:''}
        },


        computed: {
            ...mapGetters(['getUniversity', 'myCourses']),
            currentItem: function () { return searchObjects[this.searchType] },
            searchType: function () { return this.getUniversity ? 'course' : 'university' },
            params: function () { return this.getUniversity ? { universityId: this.getUniversity } : {} }
        },

        props: { show: {} },
        watch: {
            show: function (val) {
                if (val) {
                this.showDialog = true;
                }
            }
        },
        mounted() {
            console.log('created')
            this.selectedItems = this.myCourses
        },
        methods: {
            ...mapActions(['updateMyCourses']),
            $_done() {
                this.updateMyCourses(this.selectedItems).then(() =>
                    this.showDialog=false
                    )
            },
            $_updateActions(data) {
                this.currentAction = data;
                this.showActions = true;
                this.showDialog = false;
            },
            $_actionDone(obj) {
                this.currentAction = '';
                this.showActions = false;
                this.showDialog = true;
                this.extraItem = obj;
            }

        },
        components: {
            searchItem, courseAdd
        }
    }
</script>