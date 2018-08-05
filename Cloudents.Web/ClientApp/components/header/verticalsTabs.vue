<template>
    <v-flex class="line verticals">
        <v-layout row>
            <div class="gap ma-0" v-if="$vuetify.breakpoint.mdAndUp"></div>
            <!--vuetify 1.1.1 changed mutation of currentSelection to currentVertical-->
            <v-tabs class="verticals-bar" v-model="currentVertical" :value="currentSelection"  fixed :scrollable="false">

                <!--<v-tabs-bar>-->
                    <v-tab v-for="tab in verticals" :key="tab.id" :href="tab.id" :id="tab.id"
                                 @click.prevent="$_updateType(tab.id)"
                                 :class="['spitball-text-'+tab.id,tab.id===currentSelection?'v-tabs__item--active':'']"
                                 class="mr-4 vertical">
                        {{tab.name}}
                    </v-tab>
                    <v-tabs-slider :color="`color-${currentVertical}`"></v-tabs-slider>
                <!--</v-tabs-bar>-->
            </v-tabs>
        </v-layout>
    </v-flex>
</template>

<script>
    import {mapActions, mapGetters} from 'vuex'
    import {verticalsNavbar as verticals} from '../../data'

    export default {
        name: "verticals-tabs",
        computed: {...mapGetters(['getVerticalData'])},
        props: {currentSelection: {}},
        data() {
            return {
                verticals,
                currentVertical: this.currentSelection
            }
        },
        watch: {
            currentSelection(val) {
                this.currentVertical = val;
            }
        },
        methods: {
            ...mapActions(["setCurrentVertical"]),
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
                this.$router.push({path: '/' + result, query: {...query, q: text, course}});
            }
        }
    }
</script>

<style src="./verticalTabs.less" lang="less"></style>