<template>
    <div class="shared-document-container">
        <v-layout>
            <v-flex>
                <div class="iframe-container">
                    <iframe v-if="sharedDocUrl" :src="sharedDocUrl" frameborder="0"></iframe>
                </div>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex';
    export default {
        name: "sharedDocument",
        data() {
            return {
            }
        },
        props: {
            roomId: {
                required: false,
                type: String
            }
        },
        computed: {
            ...mapGetters(['getStudyRoomData']),
            sharedDocUrl(){
                if(this.getStudyRoomData && this.getStudyRoomData.onlineDocument){
                     return this.getStudyRoomData.onlineDocument
                }else{
                    return false
                }
            }
        },
    }
</script>

<style scoped lang="less">
    .shared-document-container {
        .iframe-container {
            position: relative;
            width: ~"calc(100% - 322px)"; //minus chat& vide width
            height: ~"calc(100vh - 106px)"; // minus toop toolbar menu height
            margin-top: 2px;
        }
        .iframe-container iframe {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            width: 100%;
            height: 100%;
            border: 0;
        }

        /* 4x3 Aspect Ratio */
        .iframe-container-4x3 {
            padding-top: 75%;
        }
    }

</style>