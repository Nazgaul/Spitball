﻿<template>
    <div class="box-search" ref="search">
        <form action="." method="get" @submit.prevent="search" v-scroll="onScroll">
            <v-container>
                <v-layout row>
                    <v-flex class="tx-input">
                        <search-input :placeholder="placeholder" :menu-open="menuOpen"></search-input>
                    </v-flex>
                    <v-flex class="f-submit">
                        <button type="submit">
                            <v-icon class="hidden-md-and-up">sbf-search</v-icon>
                            <span class="hidden-sm-and-down">Search</span>
                        </button>
                    </v-flex>
                </v-layout>
            </v-container>
        </form>
    </div>
</template>
<script>
    import SearchInput from '../helpers/searchInput.vue';

    export default {
        props: ["headerMenu"],
        components:{SearchInput},
        computed: {
            placeholder: function () {
                if (this.$vuetify.breakpoint.smAndUp) {
                    return "Find study documents, textbooks, tutors, jobs, deals and more...";
                }
                return "Study documents, textbooks, tutors …";
            }
        },
        data() {
            return {
                menuOpen:false
            }
        },
        methods: {
            search() {
                if (this.msg) {
                    this.$router.push({ name: "result", query: { q: this.msg } });
                }
            },
            selectos({item,index}) {
                this.msg = item;
                this.$ga.event('Search','Suggest', `#${index+1}_${item}`);
                this.search();
            },
            onScroll(e) {
                this.menuOpen = false;

            },
            //callback for mobile submit mic
            submitMic(val){
                // this.search();
                this.$router.push({ name: "result", query: { q: val } });
            }
        }
    }
</script>
<style lang="less" src="./search.less"></style>