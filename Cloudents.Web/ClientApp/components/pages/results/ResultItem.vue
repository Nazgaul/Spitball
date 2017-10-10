<template>
        <a :key="item.id" target="_blank" :href="item.url" class="result" :class="'bg-'+$route.name">
            <div class="item-content">
                <div>title: {{item.title}}</div>
                <div>
                    snippet: {{item.snippet}}
                </div>
            </div>
            <div class="source">
                source: {{item.source}}
            </div>            <div class="image"><img :src="item.image" /></div>

        </a>
</template>
<script>
    export default {
        props: { item: {type:Object,required:true}}
    }
</script>
<style scoped>
    .result {
        margin-top: 0.5em;
        border-radius: 4px;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.2);
        padding: 0.5em;
        color: white;
        text-decoration: none;
        display: grid;
        display: -ms-grid;
        grid-template-areas: "image item" "image source";
        grid-template-columns: 30% auto;
        -ms-grid-columns: 30% 1em auto;
        grid-template-rows: calc(176px - 32px) 16px;
        -ms-grid-rows: calc(176px - 32px) 1em auto;
    }

    .image {
        display: flex;
        align-items: center;
        -ms-grid-column: 1;
        -ms-grid-row: 1;
        -ms-grid-row-span: 3;
        grid-area: image;
        height: 100%;
        border: solid 1px blue;
        min-width: 80px;
    }

    img {
        height: 100%;
    }

    .default-image {
        margin: auto;
        transform: scale(1.5);
    }



    .item-content {
        -ms-grid-column: 3;
        -ms-grid-row: 1;
        -ms-grid-row-span: 1;
        grid-area: item;
        font-size: 12px !important;
    }

    .source {
        -ms-grid-column: 3;
        -ms-grid-row: 3;
        -ms-grid-row-span: 1;
        grid-area: source;
        vertical-align: bottom;
    }
</style>