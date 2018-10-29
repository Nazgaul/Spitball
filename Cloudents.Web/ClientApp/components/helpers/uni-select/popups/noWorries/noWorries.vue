<template>
    <div class="noWorries-Container">
        <div class="close-btn-container">
            <v-icon>sbf-close</v-icon>
        </div>
        <div class="title-container">
            <span>:)  No Worries</span>
        </div>
        <div class="explain-container">
            <span>You can select school & classes at any time from your user menu.</span>
        </div>
        <div class="img-container"></div>
        <div class="footer-container">
            <span>Select School now</span>
            <span class="clear">Got It!</span>
        </div>
    </div>
</template>

<script>
export default {

}
</script>

<style lang="less">
    .noWorries-Container{
        position: relative;
        text-align: center;
        margin-top: 99px;
        width: 549px;
        height: 444px;
        border-radius: 4px;
        box-shadow: 0 1px 17px 0 rgba(0, 0, 0, 0.39);
        background-color: #ffffff;
        z-index: 1;
        .close-btn-container{
            position: absolute;
            right: 19px;
            top:19px;
            i{
                font-size:13px;
                cursor: pointer;
            }
        }
        .title-container{
            margin-top:32px;
            font-size: 18px;
            font-weight: 600;
            font-style: normal;
            font-stretch: normal;
            line-height: 1.5;
            letter-spacing: normal;
            text-align: center;
            color: rgba(0, 0, 0, 0.87);
        }
        .explain-container{
            margin-top:16px;
            font-size: 16px;
            font-weight: 300;
            font-style: normal;
            font-stretch: normal;
            line-height: 1.38;
            letter-spacing: normal;
            text-align: center;
            font-family: 'Fira Sans', sans-serif;
        }
        .img-container{
            width: 220px;
            height: 220px;
            background-color: red;
            margin: 0 auto;
            margin-top: 27px;
        }
        .footer-container{
            display: flex;
            height: 50px;
            margin-bottom: 20px;
            margin-top: 25px;
            justify-content: center;
            span{
                cursor: pointer;
                padding: 17px;
                width: 162px;
                height: 48px;
                border-radius: 4px;
                background-color: rgba(68, 82, 252, 0.09);
                font-size: 14px;
                margin: 8px;
                font-family: 'Fira Sans', sans-serif;
                &.clear{
                    background-color: transparent;
                    border: solid 1px #979797;
                }
            }
        }
    }
</style>
