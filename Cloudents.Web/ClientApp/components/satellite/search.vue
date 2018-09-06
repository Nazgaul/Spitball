﻿
<template>
    <div class="box-search" ref="search">
        <v-container>
            <v-layout row>
                <v-flex class="tx-input">
                    <search-input :placeholder="placeholder" :hide-on-scroll="$vuetify.breakpoint.smAndDown">
                        <button class="search-button" slot="searchBtn" slot-scope="props" @click="props.search">
                            <v-icon class="hidden-md-and-up">sbf-search</v-icon>
                            <span class="hidden-sm-and-down" v-language:inner>search_Search</span>
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
    import { LanguageService } from '../../services/language/languageService'
    export default {
        components: {SearchInput},
        computed: {
            placeholder: function () {
                if (this.$vuetify.breakpoint.smAndUp) {
                    return  LanguageService.getValueByKey("seacrh_searchText_find");
                }
                return LanguageService.getValueByKey("seacrh_searchText_find_short");
            }
        }
    }
</script>
<style lang="less" src="./search.less"></style>