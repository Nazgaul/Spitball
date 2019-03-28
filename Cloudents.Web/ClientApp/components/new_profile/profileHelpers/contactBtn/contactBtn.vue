<template>
    <div class="contact-btn-wrap">
        <button class="ct-btn">
            <v-icon class="ct-btn-icon mr-2">sbf-comment-icon</v-icon>
            <span class="btn-text text-uppercase" v-language:inner>profile_tutor_contact_btn</span>
        </button>
    </div>
</template>

<script>
    export default {
        name: "contactBtn"
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .contact-btn-wrap {
        .ct-btn {
            display: flex;
            align-items: center;
            justify-content: center;
            /*padding: 14px 42px 14px 34px;*/
            padding: 14px 60px 14px 50px;
            background-color: #ffca54;
            height: 48px;
            border-radius: 24px;
            box-shadow: 0 3px 13px 0 rgba(0, 0, 0, 0.15);
            outline: none;
            &:hover{
                cursor: pointer;
                color: lighten(#ffca54, 10%);
                .btn-text{
                    color: lighten(@profileTextColor, 10%);
                }
            }
        }
        .btn-text{
            font-size: 16px;
            font-weight: bold;
            color: @profileTextColor;
        }
        .ct-btn-icon{
            color: @profileTextColor;
            font-size: 16px;
        }
    }

</style>