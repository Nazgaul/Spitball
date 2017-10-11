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
                            {{item.rating}} <star-rating :inline="true" :star-size="20" :read-only="true" :show-rating="false" :increment="0.1" :rating="item.rating"></star-rating>
                        </div>
                        <p>
                            {{item.address}}
                        </p>
                        <div class="bottom" v-if="item.open">Open Now</div>
                    </div>
                </div>
            </div>
        </div>
</template>
<script>
    import StarRating from 'vue-star-rating'
    export default {
        props: { item: { type: Object, required: true } },
        components: {StarRating}
    }
</script>
<style src="./ResultFood.less" lang="less" scoped></style>
