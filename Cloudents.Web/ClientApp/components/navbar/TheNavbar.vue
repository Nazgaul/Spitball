<template>
    <v-tabs v-model="currentPage" >
        <v-tabs-bar class="cyan" dark >
           <template v-for="tab in verticals">
               <v-tabs-item :href="tab.id" :id="tab.id"
            >
                <span @click.once="$_updateType(tab.id)">{{tab.name}}</span>
            </v-tabs-item>
            <v-tabs-slider color="yellow" :class="`bg-${currentPage}`"></v-tabs-slider></template>
        </v-tabs-bar>
        <!--<v-tabs-items>-->
            <!--<v-tabs-content-->
                    <!--v-for="tab in verticalsIds"-->
                    <!--:key="tab"-->
                    <!--:id="tab" :ref="tab"-->
            <!--&gt;</v-tabs-content>-->
        <!--</v-tabs-items>-->
        <!--<v-tabs-items>-->
            <!--&lt;!&ndash;<v-tabs-content&ndash;&gt;-->
                    <!--&lt;!&ndash;v-for="tab1 in verticalsIds"&ndash;&gt;-->
                    <!--&lt;!&ndash;:key="tab1"&ndash;&gt;-->
                    <!--&lt;!&ndash;:id="tab1"&ndash;&gt;-->
            <!--&lt;!&ndash;&gt;&ndash;&gt;-->
                <!--&lt;!&ndash;{{tab1}}&ndash;&gt;-->
            <!--&lt;!&ndash;</v-tabs-content>&ndash;&gt;-->
        <!--</v-tabs-items>-->
    </v-tabs>
    <!--<div class="sb-menu" v-model="selectedTab" @input="$_updateType($event)">-->
        <!--<v-tabs>-->
            <!--<v-tabs-bar>-->
                <!--&lt;!&ndash;:class="{'list__tile&#45;&#45;active':vertical.id==currentPage}"&ndash;&gt;-->
                <!--<template v-for="verticalw in verticalsIds"><v-tabs-item-->
                              <!--:href="verticalw"-->
                             <!--:class="['bg-'+verticalw]" :ref="verticalw"-->
                             <!--class="ml-1 mr-1 vertical"-->
                             <!--&gt;-->
                        <!--&lt;!&ndash;{{vertical.name}}&ndash;&gt;-->
                    <!--yifat-->
                <!--</v-tabs-item>-->
                <!--<v-tabs-slider :class="'bg-'+verticalw"></v-tabs-slider></template>-->

            <!--</v-tabs-bar>-->
        <!--</v-tabs>&lt;!&ndash;:class="{'selected':vertical.id==currentPage}"&ndash;&gt;-->
        <!--<v-chip v-for="vertical in verticals" :class="'bg-'+vertical.id" :key="vertical.id" @click="$_updateType(vertical.id)">{{vertical.name}}</v-chip>-->
    <!--</div>-->

    <!--<v-navigation-drawer v-model="showNav"  app light clipped persistent enable-resize-watcher :mobile-break-point="960"  class="sb-aside" width="216" >
        <v-list>
                <v-list-tile v-for="vertical in verticals" :key="vertical.id" class="mb-2"  @click="$_updateType(vertical.id)" :class="{'list__tile--active':vertical.id==currentPage}">
                    <v-list-tile-avatar :class="'bg-'+vertical.id" class="vertical-cycle">
                        <component class="item" v-bind:is="vertical.image"></component>
                    </v-list-tile-avatar>
                    <v-list-tile-content class="ml-2">
                        <v-list-tile-title>{{vertical.name}}</v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>
        </v-list>
        </v-navigation-drawer>-->
</template>


<script>
    //import ask from './images/ask.svg';
    //import book from './images/book.svg';
    //import document from './images/document.svg';
    //import flashcard from './images/flashcard.svg';
    //import job from './images/job.svg';
    //import food from './images/food.svg';
    //import tutor from './images/tutor.svg';
    //import setting from './images/setting.svg';
    import { verticalsNavbar as verticals } from '../data.js';
    import { mapMutations } from 'vuex'

    export default {
        //components: {
        //    ask, book, document, flashcard, job, food, tutor, setting
        //},
        data() {
            return {
                verticals: verticals
            }
        },

        methods: {
            ...mapMutations({ 'changeFlow': 'ADD' }),
            $_currentTerm(type) {
                let term = type.includes('food') ? this.$route.meta.foodTerm : type.includes('job') ? this.$route.meta.jobTerm : this.$route.meta.term;
                return term || {};
            },
            $_updateType(result) {
                this.changeFlow({ result });
                this.$router.push({ path: '/' + result, query: { q: this.$_currentTerm(result).term } })
            }
        },
        watch: {
            isOpen(val) { this.showNav = val }
        },
        props: { term: { type: String }, isOpen: { type: Boolean, default: true } },
        computed: {
            showNav: {
                get() { return this.isOpen },
                set(val) { this.$emit('input', val) }
            },
            userText() { return this.term ? this.term : this.$route.query.q },
            currentPage: { get(){
                return this.$route.path.slice(1)},set(val){} }
        }
    };
</script>
<style src="./TheNavbar.less" lang="less"></style>

