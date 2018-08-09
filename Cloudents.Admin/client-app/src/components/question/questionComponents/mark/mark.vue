<template>
    <div class="mark-table">
        <div class="filter">
            <span>Filter:</span>
            <!-- <input type="text" v-model="filter" @change="filterQuestions(filter)"> -->
        </div>
        
        <Question-Detail-Component v-for="(question, index) in 5" :key="index" ></Question-Detail-Component>
    </div>
</template>

<script lang="ts" src='./mark.ts'></script>

<style lang="scss" scoped>
    .filter{
        width:100%;
        display:flex;
    }
</style>

