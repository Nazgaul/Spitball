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
        <v-dialog v-model="showDialog">
            <div class="white">
                <div class="title"></div>
                <!--<v-card-text>
                    <v-btn color="primary" dark @click.stop="dialog3 = !dialog3">Open Dialog 3</v-btn>
                    <v-select v-bind:items="select"
                              label="A Select List"
                              item-value="text"></v-select>
                </v-card-text>-->
                <v-flex xs12> <h5>{{title}}</h5> <v-btn color="primary" flat @click.stop="showDialog=false">{{close}}</v-btn></v-flex>
                <search-item :searchApi="searchApi" :type="searchType" @selected="$_selected" :params="params"></search-item>
            </div>
        </v-dialog>
    </div>
</template>
<script>
    import searchItem from './../settings/searchItem.vue'
    import { mapGetters } from 'vuex'
    export default {
        data() {
            return {showDialog:false,snackbar:true}
        },
       
        computed: {
            ...mapGetters(['getUniversity']),
            close: function () {
                return this.getUniversity ? 'done' : 'X'
            },
            title: function () { return this.getUniversity ? 'Select Courses' : 'Select University' },
            searchApi: function () { return this.getUniversity ? 'getCorses' : 'getUniversities' },
            searchType: function () { return this.getUniversity ? 'course' : 'university' },
            params: function () { return this.getUniversity ? { universityId: this.getUniversity } : {} },
            selected: function () { return this.getUniversity ? '' : '$_universitySelected' }
        },

        methods: {
            $_selected(val) {
                if (this.searchType === 'university') this.$_universitySelected(val)
            },
            $_universitySelected(val) {
                console.log('universitySelected')
            }

        },
        components: {
            searchItem
        }
    }
</script>