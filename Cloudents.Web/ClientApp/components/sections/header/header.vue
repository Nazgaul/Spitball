<template>
    <header>
        <!--this is header
        <input v-model="isOpen" placeholder="is open header" />-->
            <div class="logo">
                <picture>
                    <source media="(min-width: 641px) and (max-width: 1365px)" srcset="/Images/sb-logo.svg" >
                    <!--<img src="/Images/spitball-logo.svg" alt="logo">-->
                    <img src="/Images/spitb-logo.svg" alt="logo">
                </picture>
            </div>
            <div class="section-icon" :class="$route.name" v-if="placeholders[$route.name]">
                <component :class="$route.name" class="icon" v-bind:is="$route.name+'Header'"></component>
            </div>
                <form>
                    <input type="search" id="qfilter" ref='search' v-model="qfilter" :placeholder="placeholders[$route.name]" />
                </form>
            <div id="notification">

            </div>
                <!--<form id="searchArea" v-submit="search()">
            <div id="searchInput" class="color-{{section.name}}">
                <div class="section-icon">
                    <svg class="icon icon-{{section.name}}">
                        <use xlink:href="{{'/App_Themes/Images/icons.svg#icon-'+section.name}}"></use>
                    </svg>
                </div>
                <input id="qFilter" class="speech-input" type="search" v-model="term">
            </div>
            <button id="mic" class="speech-icon" onclick="startDictation(event)">
                <svg class="icon icon-mic">
                    <use xlink:href="/App_Themes/Images/icons.svg#icon-white-mic"></use>
                </svg>
            </button>
            <button id="search" class="search color-{{sec}}">
                <svg class="icon icon-search-icon">
                    <use xlink:href="/App_Themes/Images/icons.svg#icon-search-icon"></use>
                </svg><span class="capitalize Search-text">search</span>
            </button>
        </form>-->
                <!--<div id="notification">
            <svg class="icon icon-path">
                <use xlink:href="/App_Themes/Images/icons.svg#icon-path"></use>
            </svg>
        </div>-->
                <div id="menu" class="hide_641">
                    <span></span>
                    <span></span>
                    <span></span>
                </div>
                <!--<div inline-flex class="sections" v-show="sectionOpen">
            <input id="ask" type="radio" @click="chaveSection('ask')" value="ask" v-model="sec" name="sec"><label v-class="{'color-ask':(sec=='ask')}" for="ask">Ask</label>
            <input id="note" type="radio" @click="chaveSection('note')" value="note" v-model="sec" name="sec"><label v-class="{'color-note':(sec=='note')}" for="note">Note</label>
            <input id="flashcard" type="radio" @click="chaveSection('flashcard')" value="flashcard" v-model="sec" name="sec"><label v-class="{'color-flashcard':(sec=='flashcard')}" for="flashcard">Flashcard</label>
            <input id="tutor" type="radio" @click="chaveSection('tutor')" value="tutor" v-model="sec" name="sec"><label v-class="{'color-tutor':(sec=='tutor')}" for="tutor">Tutor</label>
            <input id="job" type="radio" @click="chaveSection('jobs')" value="jobs" v-model="sec" name="sec"><label v-class="{'color-jobs':(sec=='jobs')}" for="job">Job</label>
            <input id="book" type="radio" @click="chaveSection('book')" value="book" v-model="sec" name="sec"><label v-class="{'color-book':(sec=='book')}" for="book">Book</label>
            <input id="purchase" type="radio" @click="chaveSection('purchase')" value="purchase" v-model="sec" name="sec"><label v-class="{'color-purchase':(sec=='purchase')}" for="purchase">Purchase</label>
            <div class="bunner hide_641"></div>
        </div>-->
    </header>
</template>
<!--<script src="../../general/vertical-collection.js"></script>-->
<script>
    import askHeader from './images/ask.svg'
    import flashcardHeader from './images/flashcard.svg'
    import jobHeader from './images/job.svg'
    import bookHeader from './images/book.svg'
    import notesHeader from './images/document.svg'
    import tutorHeader from './images/tutor.svg'
    import purchaseHeader from './images/purchase.svg'
    import {verticalsPlaceholders as placeholders} from '../../data.js'
    export default {
        props: ["isOpen", "qfilter", "section"],
        components: { askHeader, bookHeader, notesHeader, flashcardHeader, jobHeader, purchaseHeader, tutorHeader },
        data() {
            return {
                placeholders: placeholders
            }
        }
    }
</script>
<style src="./header.less" lang="less" scoped></style>
