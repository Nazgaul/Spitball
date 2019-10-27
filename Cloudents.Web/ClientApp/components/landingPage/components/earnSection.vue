<template>
    <div class="earnSection">
        <div class="earnImg">
            <div class="earn-title">Earn by teaching what you know best</div>
            <div class="earn-subtitle">Access to the tools you need to create and sell educational content and teach online</div>
            <v-btn depressed class="btn-earn" round color="#4c59ff">Teach on Spitball</v-btn>
        </div>
    </div>
</template>

<script>
export default {

}
</script>

<style lang="less">
.earnSection{
    width: 100%;
    padding: 64px 0;
    margin: 0 auto;
    max-width: calc(~"100% - 546px");
    .earnImg{
        width: 1355px;
        height: 317px;
        background-image: url('./staticCardImgs/earn.png');
        background-repeat: no-repeat;
        background-size: cover; 
        color: #ffffff;
        text-align: center;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        .earn-title{
            text-shadow: 0 2px 15px rgba(0, 0, 0, 0.6);
            font-size: 29px;
            font-weight: bold;
            line-height: 1.34;
            max-width: 714px;
            margin-bottom: 12px;
        }
        .earn-subtitle{
            max-width: 714px;
            font-size: 22px;
            font-weight: normal;
            line-height: 1.55;
            text-align: center;
            margin-bottom: 30px;
        }
        .btn-earn{
            font-size: 18px;
            font-weight: 600;
            font-stretch: normal;
            font-style: normal;
            line-height: 1.33;
            letter-spacing: normal;
            text-align: center;
            color: #ffffff;
            padding: 0 60px;
            height: 52px;
            text-transform: capitalize !important;
            box-shadow: 2px 8.8px 21px 0 rgba(0, 0, 0, 0.42);
        }
    }
}
</style>