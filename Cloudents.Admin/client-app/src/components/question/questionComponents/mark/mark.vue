<template>
    <div class="mark-table">
        <div class="filter">
            <input type="text" v-model="filter" placeholder="search...">
        </div>
        
        <Question-Detail-Component class="sticky"></Question-Detail-Component>
        <Question-Detail-Component v-for="(question, index) in filteredQuestions" :key="index" :question="question"></Question-Detail-Component>
    </div>
</template>

<script lang="ts" src='./mark.ts'></script>

<style lang="scss" scoped>
.mark-table{
    .filter{
        width:100%;
        display:flex;
        input{
            border:none;
            margin-left:2px;
            margin-right:1px;
            margin-bottom:5px;
            line-height: 25px;
            padding-left: 5px;
            width:100%;
            background-color: #b6b6b6;
            font-weight: bold;
            color:#7c7979;
            outline:none;
        }
    }
    .sticky{
        position:sticky;
        top:0;
        background-color: #97ed82;
        height:50px;
    }
}
    
</style>

