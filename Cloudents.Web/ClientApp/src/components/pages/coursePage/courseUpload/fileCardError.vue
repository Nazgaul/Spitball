<template>
    <v-layout align-center justify-space-between wrap :class="['ufItem-error','mb-4',isMobile? ' py-3':'px-4',{'ps-4':isMobile}]" v-if="item.error">
        <v-flex xs12 sm7 class="ufItem-error-content">
                <div class="ufItem-error-txt">
                    <span>{{item.name}}</span>
                    <span class="ufItem-error-txt-content">{{item.errorText}}</span>
                </div>
        </v-flex>
        <v-flex xs12 sm3 :class="[{'ps-4':isMobile}]">
            <v-btn @click="deleteFile()" :class="['ufItem-error-btn']" color="#d16061" depressed rounded>
                <span v-t="'upload_ufItem_error_remove'"/>
            </v-btn>
        </v-flex>
    </v-layout>
</template>

<script>
export default {
    name: "fileCardError",
    props: {
        singleFileIndex: {
            type: Number,
            required: true
        }
    },
    computed: {
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        item() {
            return this.$store.getters.getFileData[this.singleFileIndex]
        }
    },
    methods: {
        deleteFile() {
            this.$store.commit('deleteFileByIndex', this.singleFileIndex)
        }
    }
}
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
.ufItem-error{
    background-color: #d16061;
    @media (max-width: @screen-xs) {
        height:unset;
    }
    height: 60px;
    box-shadow: 0 1px 1px 0 rgba(0, 0, 0, 0.14) !important;
    border-radius: 4px;
    color: white;
    .v-btn{
        height: 40px !important;
        text-transform: capitalize !important;
        margin-left: 0;
        margin-right: 0;
    }
    .ufItem-error-btn{
        color: white;
        border: 1px solid white !important;
        font-size: 14px;
        letter-spacing: 0.5px;
        min-width: 150px !important; //vuetify
    }
    .ufItem-error-span{
        font-size: 14px;
        vertical-align: middle;
        font-weight: bold;
        cursor: pointer;
    }
    .ufItem-error-content{
        display: inline-flex;
        align-items: flex-start;
        .ufItem-error-txt{
            display: flex;
            flex-direction: column;
            .ufItem-error-txt-content{
                font-size: 12px;
                line-height: 1.6;
                font-weight: normal;
            }
        }
        .v-icon{
            transform: rotate(90deg);
            font-size: 20px
        }
    }
}

</style>