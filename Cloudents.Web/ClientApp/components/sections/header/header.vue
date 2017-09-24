<template>
    <header>
            <div class="logo">
                <picture>
                    <source media="(min-width: 641px) and (max-width: 1365px)" srcset="/Images/sb-logo.svg" >
                    <img src="/Images/spitb-logo.svg" alt="logo">
                </picture>
            </div>
            <div class="section-icon" :class="$route.name" v-if="placeholders[$route.name]">
                <component :class="$route.name" class="icon" v-bind:is="$route.name+'Header'"></component>
            </div>
                <form  @submit.prevent="submit">
                    <input type="search" id="qfilter" ref='search' v-model="$store.getters.userText" :placeholder="placeholders[$route.name]" @focus="showOption=true"/>
                </form>
            <div id="notification">

            </div>
                <div id="menu" class="hide_641">
                    <span></span>
                    <span></span>
                    <span></span>
                </div>
        <search-type v-show="showOption" class="searchTypes" :values="names" :model="'searchTypes'" :changeCallback="changeType"></search-type>
    </header>
</template>
<script>
    import search from "./../../../api/search"
    import askHeader from './images/ask.svg'
    import flashcardHeader from './images/flashcard.svg'
    import jobHeader from './images/job.svg'
    import bookHeader from './images/book.svg'
    import notesHeader from './images/document.svg'
    import tutorHeader from './images/tutor.svg'
    import purchaseHeader from './images/purchase.svg'
    import searchTypes from './../helpers/radioList.vue'
    import { mapGetters } from 'vuex'
    import { verticalsPlaceholders as placeholders, names} from '../../data.js'
    export default {
        props: ["isOpen", "section"],
        components: { 'search-type': searchTypes, askHeader, bookHeader, notesHeader, flashcardHeader, jobHeader, purchaseHeader, tutorHeader },
        data() {
            return {
                placeholders: placeholders,
                showOption: false,
                names: names,
                qfilter: this.$store.getters.userText
            }
        },
        methods: {
            changeType: function (e,v,s) {
                this.$router.push({name:e.target.value})
            },
            submit: function () {
                console.log('boooo');
                search.getDocument(null, (response) => {
                    console.log(response);
                    this.$emit('update:result', response)
                })
            }
        }
    }
</script>
<style src="./header.less" lang="less" scoped></style>
