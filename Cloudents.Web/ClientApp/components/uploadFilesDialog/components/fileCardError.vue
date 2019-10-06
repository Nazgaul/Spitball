<template>
    <v-layout align-center justify-space-between row wrap 
    :class="['ufItem-error','mb-3',isMobile? ' py-3':'px-3',{'pl-3':isMobile}]">
        <v-flex xs12 sm7 class="ufItem-error-content">
            <v-icon color="white" class="mr-2 attachClass" v-html="'sbf-attachment'"/>
                <div class="ufItem-error-txt">
                    <span>{{item.name}}</span>
                    <span>{{item.errorText}}</span>
                </div>
        </v-flex>
        <v-flex xs12 sm3 :class="[{'pl-4':isMobile}]">
            <v-btn @click="deleteFile()" :class="['ufItem-error-btn']" color="#d16061" depressed round>
                <!-- <span v-language:inner="'upload_ufItem_error_btn'"/> -->
                <span v-language:inner="'upload_ufItem_error_remove'"/>
            </v-btn>
            <!-- <span @click="deleteFile()" class="ufItem-error-span ml-2" v-language:inner="'upload_ufItem_error_remove'"/> -->
        </v-flex>
    </v-layout>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
export default {
    name: "fileCard",
    props: {
        fileItem: {
            type: Object,
            default: {}
        },
        singleFileIndex: {
            type: Number,
            required: true
        }
    },
    watch: {
        item: {
            deep: true,
            handler(newVal, oldVal) {
                let fileObj = {
                    index: this.singleFileIndex,
                    data: newVal
                };
                this.changeFileByIndex(fileObj);
            }
        }
    },
    computed: {
        ...mapGetters(['getFileData']),
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        item() {
            return this.getFileData[this.singleFileIndex]
        }
    },
    methods: {
        ...mapActions(['changeFileByIndex', 'deleteFileByIndex']),
        deleteFile() {
            this.deleteFileByIndex(this.singleFileIndex)
        }
    },
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
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
        min-width: 150px;
        height: 40px !important;
        text-transform: capitalize !important;
        margin-left: 0;
        margin-right: 0;
    }
    .ufItem-error-btn{
        color: white;
        border: 1px solid white !important;
        font-size: 14px;
        font-weight: 600;
        letter-spacing: -0.26px;
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
        }
        .v-icon{
            transform: rotate(90deg);
            font-size: 20px
        }
    }
}

</style>