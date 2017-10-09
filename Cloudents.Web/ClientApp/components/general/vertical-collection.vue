<template>
    <ul class="scroll">
        <li v-for="(vertical,index) in verticals" :key="vertical.name"
             :class="['box-' + (index+1), {selected: vertical.name === selected}]" >
            <button v-on:click="change(vertical)" type="button">
                <div class="hover"><span><component v-bind:is="vertical.image"></component></span></div>
                {{vertical.name}}
            </button>
        </li>
    </ul>
</template>


<script src="./vertical-collection.js"></script>
<style src="./vertical-collection.less" lang="less" scoped></style>


