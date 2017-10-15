<template>
    <v-toolbar app clipped-left fixed color="#ffdcdc">
        <!--<v-toolbar clipped-left prominent class="white border">-->
        <router-link tag="v-toolbar-title" :to="{name:'home'}" class="logo">
            <picture>
                <source media="(min-width: 641px) and (max-width: 1365px)" srcset="/Images/sb-logo.svg">
                <img src="/Images/spitb-logo.svg" alt="logo">
            </picture>
        </router-link>
        <div class="icon"><component :class="$route.name" class="item" v-bind:is="$route.name+'Header'"></component></div>
        <div class="box-header-search">
            <form @submit.prevent="submit">
                <button type="submit"><img src="/Images/search-icon.svg" alt=""></button>
                <input name="search" type="text" :placeholder="placeholders[$route.name]" v-model="qFilter" @focus="focus">
            </form>
        </div>
        <v-flex :slot="isOptions?'extension':''" v-show="isOptions">
            <search-type class="searchTypes header offset-xs2" :values="names" :model="'searchTypes'" :value="$route.name" @click="changeType"></search-type>
        </v-flex>
        

    </v-toolbar>
    <!--<header>
                    <picture>
                        <source media="(min-width: 641px) and (max-width: 1365px)" srcset="/Images/sb-logo.svg">
                        <img src="/Images/spitb-logo.svg" alt="logo">
                    </picture>
                </router-link>
                <div class="icon"><component :class="$route.name" class="item" v-bind:is="$route.name+'Header'"></component></div>
                <div class="box-header-search">
                    <form @submit.prevent="submit">
                        <button type="submit"><img src="/Images/search-icon.svg" alt=""></button>
                        <input name="search" type="text" :placeholder="placeholders[$route.name]" v-model="qFilter" @focus="focus">
                    </form>
                </div>
            <v-flex :slot="isOptions?'extension':''"  v-show="isOptions">
                <search-type class="searchTypes header offset-xs2" :values="names" :model="'searchTypes'" :value="$route.name" @click="changeType"></search-type>
    </header>-->
    <!--</v-flex>
    </v-toolbar>-->
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
                names: names,
                qFilter: this.userText,
                isOptions: false
            }
        }, computed: {
            ...mapGetters(['userText'])
        },

        props: {
            openOptions: { type: Function }
        },

        mounted: function () {
            this.qFilter = this.userText;
        },

        methods: {
            ...mapActions(['updateSearchText']),
            changeType: function (val) {
                this.updateSearchText();
                this.qFilter = "";
                this.$router.push({ name: val })
                this.updateSearchText();
            },
            submit: function () {
                this.isOptions = false;
                this.updateSearchText(this.qFilter);
                this.$emit('update:overlay', false)
            },
            focus: function () {
                this.isOptions = true
                this.$emit('update:overlay', true)
            }
        }
    }
</script>
<style>
    .toolbar__extension {
        border-top: 1px solid #ddd;
    }
</style>
<style src="./header.less" lang="less" scoped></style>
