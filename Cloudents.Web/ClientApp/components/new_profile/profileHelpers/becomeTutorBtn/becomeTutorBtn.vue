<template>
    <div class="tutor-btn-wrap">
        <button class="ct-btn" @click="updateTutorDialog(true)">
            <v-icon class="ct-btn-icon mr-2">sbf-face-icon</v-icon>
            <span class="btn-text" v-language:inner>profile_become_tutor_btn</span>
        </button>
    </div>
</template>

<script>
    import {mapActions} from 'vuex';
    export default {
        name: "becomeTutorBtn",
        methods: {
            ...mapActions(['updateTutorDialog'])
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .tutor-btn-wrap {
        .ct-btn {
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 14px 24px 14px 24px;
            background-color: #ffca54;
            height: 48px;
            border-radius: 24px;
            box-shadow: 0 3px 13px 0 rgba(0, 0, 0, 0.15);
            outline: none;
            min-width: 230px;
            &:hover{
                cursor: pointer;
                color: lighten(#ffca54, 10%);
                .btn-text{
                    color: lighten(@global-purple, 10%);
                }
            }
        }
        .btn-text{
            font-size: 14px;
            font-weight: bold;
            color: @global-purple;
        }
        .ct-btn-icon{
            color: @global-purple;
            font-size: 21px;
        }
    }

</style>