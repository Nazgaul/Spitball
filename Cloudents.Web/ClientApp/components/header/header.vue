<template>
    <v-toolbar app clipped-left fixed color="#ffdcdc">

    </v-toolbar>
    <!--<header>
            <div class="logo">
                <picture>
                    <source media="(min-width: 641px) and (max-width: 1365px)" srcset="/Images/sb-logo.svg" >
                    <img src="/Images/spitb-logo.svg" alt="logo">
                </picture>
            </div>
            <div class="section-icon" :class="$route.name" v-if="placeholders[$route.name]">
                <component :class="$route.name" class="icon" v-bind:is="$route.name+'Header'"></component>
            </div>
                <form  @submit.prevent="">
                    <input type="search" id="qfilter" ref='search' v-model.lazy="qfilter" :placeholder="placeholders[$route.name]" @focus="showOption=true"/>
                </form>
            <div id="notification">

            </div>
                <div id="menu" class="hide_641">
                    <span></span>
                    <span></span>
                    <span></span>
                </div>
        <search-type v-show="showOption" class="searchTypes" :values="names" :model="'searchTypes'" :value="$route.name" @click="changeType"></search-type>
    </header>-->
</template>
<script>
    import askHeader from './images/ask.svg'
    import flashcardHeader from './images/flashcard.svg'
    import jobHeader from './images/job.svg'
    import bookHeader from './images/book.svg'
    import noteHeader from './images/document.svg'
    import tutorHeader from './images/tutor.svg'
    import foodHeader from './images/food.svg'
    import searchTypes from './../helpers/radioList.vue'
    import { mapGetters, mapActions } from 'vuex'
    import { verticalsPlaceholders as placeholders, names } from '../data'
    export default {
        components: { 'search-type': searchTypes, askHeader, bookHeader, noteHeader, flashcardHeader, jobHeader, foodHeader, tutorHeader },
        data() {
            return {
                placeholders: placeholders,
                showOption: false,
                names: names
            }
        }, computed: {
            ...mapGetters(['userText']),
            qfilter: {
                get() {
                    return this.userText;
                },
                set(val) {
                   this.updateSearchText(val);
                }
            }
        },
        methods: {
            ...mapActions(['updateSearchText']),
            changeType: function (val) {
                this.updateSearchText();
                this.$router.push({name:val})
            }
        }
    }
</script>
<style src="./header.less" lang="less" scoped></style>
