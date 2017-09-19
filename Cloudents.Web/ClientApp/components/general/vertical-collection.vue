<template>
    <div>
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
    </div>
</template>


<script src="./vertical-collection.js"></script>
<style src="./vertical-collection.less" lang="less" scoped></style>


