<template>
    <div class="wrapper">
            <div class="container-box">
                <div class="box-main box-food">
                    <div class="img">
                        <span class="up">
                            <img :src="item.image" alt="">
                        </span>
                    </div>
                    <div class="box-wrapper">
                        <div class="title">{{item.name}}</div>
                        <div class="rate">
                            {{item.rating}}
                            <!--<img src="img/rate-full.png" alt="">
                            <img src="img/rate-full.png" alt="">
                            <img src="img/rate-full.png" alt="">
                            <img src="img/rate-full.png" alt="">
                            <img src="img/rate-empty.png" alt="">-->
                        </div>
                        <p>
                            {{item.address}}
                        </p>
                        <div class="bottom" v-if="item.open">Open Now</div>
                    </div>
                    <!--<div class="box-label">
                        <img src="img/svg/hooked-logo-copy.svg" alt="">
                        <span>Discount</span>
                    </div>-->
                </div>
            </div>
        </div>
</template>
<script>
    export default {
        props: { item: {type:Object,required:true}}
    }
</script>
<style src="./ResultFood.less" lang="less" scoped></style>
