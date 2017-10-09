<template>
    <div class="content">
        <h4><span v-if="isEmpty" class="empty" v-html="page.emptyText"></span>
        <span v-else v-html="page.title"></span>{{dynamicHeader}}</h4>
        <slot name="options">
            <v-container grid-list-md>
                <v-layout row wrap>
                    <v-flex>
                        <radio-list class="text-xs-center" :values="page.filter" @click="$_changeFilter" model="filter" :value="filterOptions"></radio-list>
                    </v-flex>                   
                    <v-flex>
                        <hr v-if="page.sort">
                        <radio-list :values="page.sort" @click="$_updateSort" model="sort" v-model="sort"></radio-list>
                    </v-flex>               
                    <v-flex xs12>
                        <radio-list :values="subFilters" @click="$_changeSubFilter" model="subFilter" :value="subFilter"></radio-list>
                    </v-flex>
                </v-layout>
            </v-container>
        </slot>
        <slot name="firstContent"></slot>
        <slot>

        </slot>
       <slot name="emptyState" v-show="isEmpty"></slot>
    </div>
</template>
<script>
    import { pageMixin } from './mixins'
    export default {
        mixins: [pageMixin],
        //mounted: function(){
        //    this.$refs[this.filter].focus();
        //}
    }
</script>
<style scoped>
    .chip:focus:not(.chip--disabled) {
        background: blue !important;
    }
</style>