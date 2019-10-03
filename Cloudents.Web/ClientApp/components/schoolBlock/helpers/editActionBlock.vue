<template>
    <div class="edit-action-block" >
        <div class="edit-action d-flex align-baseline">
            <v-icon class="edit-icon mr-2">sbf-edit-icon</v-icon>
            <span  class="edit-class mr-2" v-language:inner>schoolBlock_edit_classes</span>

        </div>
    </div>
</template>

<script>
    export default {
        name: "editActionBlock"
    }
</script>

<style lang="less">
    @import "../../../styles/mixin.less";
    @backgroundGrey: #ececec;

    .edit-action-block {
        background-color: @color-white;
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: center;
        padding: 8px;
        cursor: pointer;
        @media(max-width: @screen-xs){
            box-shadow: 0 0 11px 0 rgba(0, 0, 0, 0.43);
            z-index: 9;
        }
        .edit-action {
            cursor: pointer;
            z-index: 1;
        }
        .edit-icon {
            font-size: 12px;
        }
        .edit-class {
            
            font-size: 13px;
            letter-spacing: -0.3px;
            color: @colorBlackNew;
        }
    }

</style>