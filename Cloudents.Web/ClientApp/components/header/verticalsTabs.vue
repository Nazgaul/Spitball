<template>
    <div>
    <v-flex class="line verticals static-card-what-is-hw-question">
        <v-layout row >
            <div class="gap ma-0" v-if="$vuetify.breakpoint.mdAndUp"></div>
            <v-tabs class="verticals-bar" v-model="currentVertical" :value="currentSelection"   :scrollable="true">
                    <v-tab v-for="tab in verticals" :ripple="false" :key="tab.id" :href="tab.id" :id="tab.id"
                                 @click.prevent="$_updateType(tab.id)"
                                 :class="['spitball-text-'+tab.id,tab.id===currentSelection?'v-tabs__item--active':'']"
                                 class="mr-4 vertical">
                        {{tab.name}}
                    </v-tab>
                    <v-tabs-slider :color="`color-${currentVertical}`"></v-tabs-slider>
            </v-tabs>
        </v-layout>
    </v-flex>

    </div>
</template>

<script>
    import {mapActions, mapGetters, mapMutations} from 'vuex'
    import {verticalsNavbar as verticals}  from "../../services/navigation/vertical-navigation/nav";

    export default {
        name: "verticals-tabs",

        props: {currentSelection: {}},
        data() {
            return {
                verticals,
                currentVertical: this.currentSelection
            }
        },
        computed: {
            ...mapGetters(['getVerticalData', 'accountUser']),
            isLogedIn(){
                return this.accountUser
            }
        },
        watch: {
            currentSelection(val) {
                this.currentVertical = val;
            }
        },
        methods: {
            ...mapMutations(['UPDATE_SEARCH_LOADING']),
            ...mapActions(['setCurrentVertical', 'updateLoginDialogState', 'updateUserProfileData', 'updateNewQuestionDialogState']),
            $_updateType(result) {
                this.currentVertical = result;
                this.$ga.event("Vertical_Tab", result);
                let tabs = this.$el.querySelector('.v-tabs__wrapper');
                let currentItem = this.$el.querySelector(`#${result}`);
                if (currentItem) {
                    tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
                }
                this.setCurrentVertical(result);
                let query = {};
                if (this.$route.query.hasOwnProperty("promo")) {
                    query = {promo: this.$route.query.promo}
                }
                let {text = "", course} = this.getVerticalData(result);
                if ((result == 'flashcard' && this.$route.path.includes('note') || result == 'note' && this.$route.path.includes('flashcard')) && this.$route.query.course) {
                    course = this.$route.query.course;
                }
                //if same tab do not do UPDATE_SEARCH_LOADING
                if(this.$route.path !== `/${result}`){
                    this.UPDATE_SEARCH_LOADING(true);
                }
                this.$router.push({path: '/' + result, query: {}});
                // this.$router.push({path: '/' + result, query: {...query, q: text, course}});
            },

            //TODO ABTEST part of ab test if not used delete from here
            goToAskQuestion() {
                // console.log(this.accountUser);
                if (this.accountUser == null) {
                    this.updateLoginDialogState(true);
                    //set user profile
                    this.updateUserProfileData('profileHWH')
                } else {
                    this.updateNewQuestionDialogState(true);
                }
            },
        }
    }
</script>

<style src="./verticalTabs.less" lang="less"></style>