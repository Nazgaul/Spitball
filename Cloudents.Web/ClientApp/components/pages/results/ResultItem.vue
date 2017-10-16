<template>
    <a class="wrapper" target="_blank" :href="item.url">
        <div class="container-box">
            <div class="box-main" :class="[item.image?'box-mask-1':'box-mask-2']">
                <div class="img" :class="[item.image?'border-'+$route.name:'bg-'+$route.name]">
                    <span class="up">
                        <img :src="item.image" alt="">
                    </span>
                </div>
                <div class="box-wrapper">
                    <div class="title" :class="'text-'+$route.name">{{item.title}}</div>
                    <p>{{item.snippet}}</p>
                    <div class="bottom">Source: {{item.source}}</div>
                </div>
            </div>
        </div>
    </a>
</template>
<script>
    export default {
        props: { item: {type:Object,required:true}}
    }
</script>
<style src="./ResultItem.less" lang="less" scoped></style>
