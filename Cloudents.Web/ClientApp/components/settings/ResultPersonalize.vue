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
        <search-item  :type="searchType" v-model="showDialog">
        </search-item>
    </div>
</template>
<script>
    import searchItem from './searchItem.vue'
    import courseAdd from './courseAdd.vue'
    import { searchObjects} from './consts'
    import { mapGetters } from 'vuex'
    export default {
        data() {
            return { showDialog: false, snackbar: true}
        },


        computed: {
            ...mapGetters(['getUniversity']),
            searchType: function () { return this.getUniversity ? 'course' : 'university' }
        },

        props: { show: {} },
        watch: {
            show: function (val) {
                if (val) {
                this.showDialog = true;
                }
            }
        },

        components: {
            searchItem, courseAdd
        }
    }
</script>