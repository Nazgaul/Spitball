<template>
    <div class="colomn">
        <div v-show="column.thumb" class="columns-thumb">
            <img :src="column.thumb">
        </div>
        <div class="colomn-head">
            <div v-show="index" class="number">{{index}}</div>
            <div class="colomn-title" :class="{'green-text': column.thumb}">{{column.title}}</div>
        </div>
        <div class="colomn-text" :class="{'grey-text': column.thumb}">{{column.text}}</div>
    </div>
</template>

<script>
    export default {
        name: "Column",
        props:['column', 'index']
    }
</script>

<style>

</style>