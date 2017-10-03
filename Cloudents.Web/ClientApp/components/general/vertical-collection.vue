<template>
    <ul class="scroll">
        <!--v-bind:class="{selected: vertical === selected}"-->
        <li v-for="(vertical,index) in verticals" :key="vertical.name"
             :class="['box-' + (index+1), {selected: vertical === selected}]" >
            <!--<button v-on:click="change(vertical)" type="button">
                <div class="round-icon" :class="'bg-'+vertical.name">
                    <component v-bind:is="vertical.image"></component>
                </div>
                <div class="hover"><span><component v-bind:is="vertical.image"></component></span></div>
                {{vertical.name}}
            </button>-->
            <button v-on:click="change(vertical)" type="button">
                <div class="hover"><span><component v-bind:is="vertical.image"></component></span></div>
                {{vertical.name}}
            </button>
        </li>
        <!--<li class="box-1">
            <a href="#">
                <div class="hover"><span><img src="img/svg/ask-icon.svg" alt=""></span></div>
                Ask
            </a>
        </li>
        <li class="box-2">
            <a href="#">
                <div class="hover"><span><img src="img/svg/class-material-icon.svg" alt=""></span></div>
                Class Material
            </a>
        </li>
        <li class="box-3">
            <a href="#">
                <div class="hover"><span><img src="img/svg/flashcard-icon.svg" alt=""></span></div>
                Flashcards
            </a>
        </li>
        <li class="box-4">
            <a href="#">
                <div class="hover"><span><img src="img/svg/tutor-icons.svg" alt=""></span></div>
                Tutors
            </a>
        </li>
        <li class="box-5">
            <a href="#">
                <div class="hover"><span><img src="img/svg/jobs-icon.svg" alt=""></span></div>
                Jobs
            </a>
        </li>
        <li class="box-6">
            <a href="#">
                <div class="hover"><span><img src="img/svg/textbooks-icon.svg" alt=""></span></div>
                Textbooks
            </a>
        </li>
        <li class="box-7">
            <a href="#">
                <div class="hover"><span><img src="img/svg/food-icon.svg" alt=""></span></div>
                Food
            </a>
        </li>-->
    </ul>
    <!--<div>
        <carousel :scrollPerPage="false" :perPage="7">
            <slide v-for="vertical in verticals" :key="vertical.name" v-bind:class="{selected: vertical === selected}">
                <button v-on:click="change(vertical)" type="button">
                    <div class="round-icon" :class="'bg-'+vertical.name">
                        <component v-bind:is="vertical.image"></component>
                    </div>
                    {{vertical.name}}
                </button>
            </slide>
        </carousel>
    </div>-->
</template>


<script src="./vertical-collection.js"></script>
<style src="./vertical-collection.less" lang="less" scoped></style>


