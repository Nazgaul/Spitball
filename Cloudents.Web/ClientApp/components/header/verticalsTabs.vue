<template>
    <v-flex class="line verticals">
        <v-layout row>
            <div class="gap ma-0" v-if="$vuetify.breakpoint.mdAndUp"></div>
            <v-tabs class="verticals-bar" :value="currentSelection" :scrollable="false">
                <v-tabs-bar>
                    <v-tabs-item v-for="tab in verticals" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_updateType(tab.id)" :class="['spitball-text-'+tab.id,tab.id===currentSelection?'tabs__item--active':'']"
                                 class="mr-4 vertical">
                        {{tab.name}}
                    </v-tabs-item>
                    <v-tabs-slider :color="`color-${currentSelection}`"></v-tabs-slider>
                </v-tabs-bar>
            </v-tabs>
        </v-layout>
    </v-flex>
</template>

<script>
    import {mapActions,mapGetters} from 'vuex'
    import {verticalsNavbar as verticals} from '../../data'
    export default {
        name: "verticals-tabs",
        computed:{...mapGetters(['getVerticalData'])},
        props:{currentSelection:{}},
        data(){return {verticals}},
        methods: {
            ...mapActions(["setCurrentVertical"]),
            $_updateType(result) {
                this.$ga.event("Vertical_Tab",result);
                let tabs = this.$el.querySelector('.tabs__wrapper');
                let currentItem = this.$el.querySelector(`#${result}`);
                if (currentItem) {
                    tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
                }
                this.setCurrentVertical(result);
                let {text="",course}=this.getVerticalData(result);
                // this.getAIDataForVertical(result).then(({text="",course})=>{
                    this.$router.push({ path: '/' + result, query: { q: text,course } });
                // })
            }
        }
    }
</script>

<style src="./verticalTabs.less" lang="less"></style>