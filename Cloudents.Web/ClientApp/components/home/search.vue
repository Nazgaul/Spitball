﻿
<template>
    <div class="box-search" ref="search">
        <v-container>
            <v-layout row>
                <v-flex class="tx-input">
                    <search-input :placeholder="placeholder" :hide-on-scroll="$vuetify.breakpoint.smAndDown">
                        <!--<component :is="props.type" :class="props.classIcon" slot="stripImage" slot-scope="props"></component>-->
                        <button class="search-button" slot="searchBtn" slot-scope="props" @click="props.search">
                            <v-icon class="hidden-md-and-up">sbf-search</v-icon>
                            <span class="hidden-sm-and-down">Search</span>
                        </button>
                    </search-input>
                </v-flex>

                <v-flex class="f-submit">

                </v-flex>
            </v-layout>
        </v-container>
    </div>
</template>
<script>
    import SearchInput from '../helpers/searchInput.vue';

    export default {
        // props: ["headerMenu"],
        components: {SearchInput},
        computed: {
            placeholder: function () {
                if (this.$vuetify.breakpoint.smAndUp) {
                    return "Find study documents, textbooks, tutors, jobs, deals and more...";
                }
                return "Study documents, textbooks, tutors …";
            }
        }
    }
</script>
<style lang="less" src="./search.less"></style>